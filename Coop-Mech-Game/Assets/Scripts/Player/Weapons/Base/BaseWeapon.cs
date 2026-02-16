using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
using static UnityEngine.Rendering.DebugUI.Table;

public abstract class BaseWeapon : MonoBehaviour
{
    /// <summary>
    /// 
    /// This class should be pretty weapon type agnostic, so ammoCount is the universal cooldown counter
    /// ActivateCooldown is reloading or overheating etc.
    /// 
    /// </summary>

    [Header("Object/Component References")]
    [SerializeField] private TeamProjectilePool teamProjectilePool;
    [SerializeField] private Transform muzzle;
    [SerializeField] private MechScreen ammoCountScreen;

    [Header("Weapon Stats")]
    public float owningPlayer = 0; //Set to 1 for player, Set to 2 for player 2
    [SerializeField] private int ammo = 10;
    [SerializeField] private float baseFireRate = 1f;
    [SerializeField] private float cooldownTime = 1.0f;
    [SerializeField] private float damage = 50;
    [SerializeField] private Vector3 maxRotationAxes = Vector3.zero;

    [Header("READ ONLY")]
    [Tooltip("READY ONLY")]
    [SerializeField] private int ammoCount;

    private bool canFire = true;
    private bool isCooldownOn = false;
    private RaycastHit hit;
    private WeaponMuzzle muzzleComp;

    public float FireRate { get { return baseFireRate; } }
    public Transform Muzzle { get { return muzzle; } }
    public float AmmoCount {  get { return ammoCount; } }
    public bool CanWeaponFire { get { return canFire; } }


    private void Start()
    {
        ammoCount = ammo;
        muzzleComp = muzzle.GetComponent<WeaponMuzzle>();
    }

    public virtual void Fire() //public because this will be called by weapon manager
    {
        //Debug.Log("BaseWeapon Fire() " + canFire);

        if (canFire)
        {
            //Debug.Log("Fire input received");

            Physics.Raycast(muzzle.position, muzzle.forward, out hit);
            if (muzzleComp) { muzzleComp.SendFireEvent(); }

            if (hit.collider.gameObject.CompareTag("TeamOne"))
            {
                // get team-specific info and send to wherever we're handling the health of the teams
                GameManager.Instance.DamageTeamRpc(1, damage);
            }
            else if (hit.collider.gameObject.CompareTag("TeamTwo"))
            {
                GameManager.Instance.DamageTeamRpc(2, damage);
            }
            else if (hit.collider.gameObject.CompareTag("Target"))
            {
                Debug.Log("Hit!");
                hit.collider.gameObject.SetActive(false);
            }

            canFire = false;
            BuildCooldown();
        }
        //else if (ammoCount <= 0)
        //{
        //    ActivateCooldown();
        //}
    }
    protected virtual void BuildCooldown()
    {
        ammoCount = ammoCount - 1;
        ammoCountScreen.ChangeText(AmmoCount.ToString(), false);
        //Debug.Log("Ammo: " + ammoCount);
        if (ammoCount <= 0)
        {
            ActivateCooldown();
        }
        else
        {
            StartCoroutine(FireRateRoutine(baseFireRate));
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

    protected virtual IEnumerator CooldownRotuine() //this is used for reloading but maybe also from damage effects
    {
        //Debug.Log("cooldown start");

        yield return new WaitForSeconds(cooldownTime);

        float reloadTimer = Time.deltaTime;
        if (reloadTimer <= cooldownTime/3){ammoCountScreen.ChangeText("-..",false);}
        else if (reloadTimer <= cooldownTime * (2/3)){ammoCountScreen.ChangeText("--.",false);}
        else {ammoCountScreen.ChangeText("---",false);}

        ammoCount = ammo;
        canFire = true;
        isCooldownOn = false;
        ammoCountScreen.ChangeText(ammoCount.ToString(), false);
        //Debug.Log("cooldown end");
    }

    public virtual void SetMuzzleRotation(RaycastHit rayHit, Vector3 rotDir) //rayHit is used for debug
    {
        hit = rayHit;
        //clamp rotDir to be within (10,10,10) and negatives
        muzzle.transform.forward = rotDir;

        Debug.Log(this + " weapon muzzle forward is: " + muzzle.transform.forward);
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