using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Matchmaker.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;
using static UnityEngine.Rendering.DebugUI.Table;

public enum WeaponType
{
    Autocannon,
    Shotgun,
    Sniper
}

public abstract class BaseWeapon : NetworkBehaviour
{
    /// <summary>
    /// 
    /// This class should be pretty weapon type agnostic, so ammoCount is the universal cooldown counter
    /// ActivateCooldown is reloading or overheating etc.
    /// 
    /// </summary>

    [Header("Object/Component References")]
    // [SerializeField] private TeamProjectilePool teamProjectilePool;
    [SerializeField] private Transform muzzle;
    [SerializeField] public MechScreen ammoCountScreen;
    [SerializeField] public SingleComboScript comboManager;

    [Header("Weapon Stats")]
    [SerializeField] private WeaponType weaponType;
    public float owningPlayer = 0; //Set to 1 for player, Set to 2 for player 2
    [SerializeField] private int ammo = 30;
    [Tooltip("Tick this true if the raycast method used for the weapon returns an array of hit points")]
    [SerializeField] private bool isMultiShot = false;
    [SerializeField] protected float baseFireRate = 1f;
    [SerializeField] protected float cooldownTime = 1.0f;
    [SerializeField] protected float damage = 50;
    [SerializeField] protected float baseKnockbackForce = 1;
    [SerializeField] [Range(1,5)] private float damageMultiplier = 2.5f;
    [SerializeField] private Vector3 maxRotationAxes = Vector3.zero;

    [Header("Current Weapon Stats")]
    [SerializeField] protected float currentDamage;
    [SerializeField] protected float currentFireRate;
    [SerializeField] protected float currentKnockback;

    [Header("READ ONLY")]
    [Tooltip("READY ONLY")]
    //[SerializeField] private int ammoCount;
    [SerializeField] private NetworkVariable<int> ammoCount = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);
    private bool canFire = true;
    private bool isCooldownOn = false;
    protected RaycastHit hit;
    private WeaponMuzzle muzzleComp;
    protected List<RaycastHit> hits = new List<RaycastHit>();

    public float FireRate { get { return baseFireRate; } }
    public Transform Muzzle { get { return muzzle; } }
    public float AmmoCount {  get { return ammoCount.Value; } }
    public bool CanWeaponFire { get { return canFire; } }
    public bool IsMultiShotWeapon {  get { return isMultiShot; } }

   
    private void Start()
    {
        //ammoCount.Value = ammo;
        muzzleComp = muzzle.GetComponent<WeaponMuzzle>();
        GameManager.Instance.OnRoundEnd.AddListener(ResetGunAttributes);
        currentDamage = damage;
        currentFireRate = baseFireRate;

        if(hits == null) { hits = new List<RaycastHit>(); }
    }
    public override void OnNetworkSpawn()
    {
        //set ammo in OnNetworkSpawn because its a networkvariable
        //also set as an rpc so the client can set their ammo too?
        if (ammoCountScreen != null)
            ChangeAmmoText();
        if (!IsServer) { return; }
        SetAmmoRpc(ammo);
        if (ammoCountScreen != null)
            ChangeAmmoText();
    }

    private void ResetGunAttributes()
    {
        // reset everything to do with weapons in this function
        if(!IsServer) { return; }
        ResetAmmoRpc();
        StartCoroutine(CooldownRotuine());
        
    }

    [Rpc(SendTo.Server)]
    private void SetAmmoRpc(int ammo)
    {
        if (!IsServer) { return; }
        //ammoCount.Value = 0;
        ammoCount.Value = ammoCount.Value + ammo;
    }

    [Rpc(SendTo.Server)]
    private void ResetAmmoRpc()
    {
        if (!IsServer) { return; }
        //ammoCount.Value = 0;
        ammoCount.Value = 0;
    }

    public virtual void Fire(float mouseDistance) //public because this will be called by weapon manager
    {
        
       
        if (IsOwner) {
            if (!canFire) return;
            AdjustDistanceBasedStats(mouseDistance);
            FireRpc();
            FireEventMethodClientRpc();

        }

        ChangeAmmoText();


    }

    protected abstract void AdjustDistanceBasedStats(float mouseDistance);

    [Rpc(SendTo.NotServer)]
    protected virtual void FireEventMethodClientRpc()
    {
        if (muzzleComp && IsMultiShotWeapon == false) { muzzleComp.SendFireEvent(hit); }
        else if(muzzleComp && IsMultiShotWeapon == true) { muzzleComp.SendFireEventList(hits); }
    }
    

    protected virtual void ChangeAmmoText()
    {
        ammoCountScreen.ChangeText(ammoCount.Value.ToString(), false);
    }

    [Rpc(SendTo.Server)]
    protected virtual void FireRpc()
    {
       
        //Debug.Log("FireServerRpc");
        // Do raycast on server
        Physics.Raycast(muzzle.position, muzzle.forward, out hit);
        GameObject other = hit.collider.gameObject;
        //Debug.Log("Hit layer: " + hit.collider.gameObject.layer + " hit tag: " + hit.collider.gameObject.tag);

        // if (comboManager.GetIsComboFull())
        // {
        //     currentDamage = damage * damageMultiplier;
        //     comboManager.UseMaxPointsRpc();
        // }
        // else
        // {
        //     currentDamage = damage;
        // }

        if (other.CompareTag("TeamOne"))
        {
            GameManager.Instance.DamageTeamRpc(1, currentDamage);
        }
        else if (other.CompareTag("TeamTwo"))
        {
            GameManager.Instance.DamageTeamRpc(2, currentDamage);
        }
        else if (other.CompareTag("Target"))
        {
            other.GetComponent<KillhouseEnemy>().Deactivate();
            other.GetComponent<KillhouseEnemy>().DeactivateRpc();
        }

        ///this is real bad and should be in WeaponShotgun class anyways
        //if (weaponType == WeaponType.Shotgun)
        //{
        //    CharacterMovement characterMovement = other.GetComponent<CharacterMovement>();
        //    if (!characterMovement.GetIsBeingKnockedBack())
        //    {
        //        Vector3 c = characterMovement.transform.position;
        //        characterMovement.ApplyKnockback(c.GetDirectionFromVectors(transform.position), currentKnockback);
        //    }
        //}

        
        BuildCooldown();
    }
    protected virtual void BuildCooldown()
    {
        SetAmmoRpc(-1);
        //ammoCount.Value = ammoCount.Value - 1;
        canFire = false;
        //Debug.Log("Ammo: " + ammoCount);
        if (ammoCount.Value <= 0)
        {
            ActivateCooldown();
        }
        else
        {
            StartCoroutine(FireRateRoutine(currentFireRate));
        }
    }

    protected virtual void ActivateCooldown()
    {
        if(isCooldownOn == false)
        {
            isCooldownOn = true;
            StartCoroutine(CooldownRotuine());
        }
    }

    protected virtual IEnumerator FireRateRoutine(float fireRate)
    {
        yield return new WaitForSeconds(fireRate);
        canFire = true;
    }

    public void Reload()
    {
        canFire = false;
        ActivateCooldown();
    }

    protected virtual IEnumerator CooldownRotuine() //this is used for reloading but maybe also from damage effects
    {
        //Debug.Log("cooldown start");
        yield return new WaitForSeconds(cooldownTime * 0.25f);

        ammoCountScreen.ChangeText("-..", false);

        yield return new WaitForSeconds(cooldownTime*0.25f);

        ammoCountScreen.ChangeText("--.", false);

        yield return new WaitForSeconds(cooldownTime * 0.25f);

        ammoCountScreen.ChangeText("---",false);

        yield return new WaitForSeconds(cooldownTime * 0.25f);

        if (IsServer)
        {
            SetAmmoRpc(ammo);
        }
        //ammoCount.Value = ammo;
        canFire = true;
        isCooldownOn = false;
        ammoCountScreen.ChangeText(ammoCount.Value.ToString(), false);
        //Debug.Log("cooldown end");
    }

    public virtual void SetMuzzleRotation(RaycastHit rayHit, Vector3 rotDir) //rayHit is used for debug
    {
        hit = rayHit;
        //clamp rotDir to be within (10,10,10) and negatives
        muzzle.transform.forward = rotDir;

        //Debug.Log(this + " weapon muzzle forward is: " + muzzle.transform.forward);
    }

#if UNITY_EDITOR
    protected virtual void OnDrawGizmos()
    {   
        Gizmos.color = Color.indianRed;
        Gizmos.DrawSphere(hit.point, 0.5f);

        Gizmos.DrawRay(muzzle.position, muzzle.transform.forward);

        if (hit.collider != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(muzzle.position, hit.point);
        }
        else
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(muzzle.position, muzzle.position + muzzle.forward * 100f);
        }
    }
#endif

}