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

    [SerializeField] private TeamProjectilePool teamProjectilePool;
    [SerializeField] private Transform muzzle;
    [SerializeField] private VisualEffect fireEffect;
    [SerializeField] private int ammo = 10;
    [SerializeField] private float baseFireRate = 1f;

    private float cooldownTime = 1.0f;
    private int ammoCount;

    private bool canFire = true;

    public float FireRate { get { return baseFireRate; } }
    public Transform Muzzle { get { return muzzle; } }

    private RaycastHit hit;
    private Vector3 rotDir;

    public virtual void Fire() //public because this will be called by weapon manager
    {
        if (canFire)
        {
            Debug.Log("Fire input received");

            Physics.Raycast(muzzle.position, muzzle.forward, out hit);
            if (fireEffect) { fireEffect.SendEvent("OnFire"); }

            if (hit.collider.CompareTag("Player"))
            {
                // get team-specific info and send to wherever we're handling the health of the teams
            }

            canFire = false;
            BuildCooldown();
        }
    }
    protected virtual void BuildCooldown()
    {
        ammoCount -= 1;
        Debug.Log("Ammo: " + ammoCount);
        if (ammoCount <= 0)
        {
            ActivateCooldown();
        }
        else
        {
            StartCoroutine(FireRateRoutine(baseFireRate));
        }
    }

    protected virtual IEnumerator FireRateRoutine(float fireRate)
    {
        yield return new WaitForSeconds(fireRate);
        canFire = true;
    }

    protected virtual IEnumerator ActivateCooldown() //this is used for reloading but maybe also from damage effects
    {
        Debug.Log("cooldown start");

        yield return new WaitForSeconds(cooldownTime);

        ammoCount = ammo;
        canFire = true;
        Debug.Log("cooldown end");
    }

    public virtual void SetMuzzleRotationAtHit(RaycastHit rayHit)
    {

        hit = rayHit;
        rotDir = hit.GetDirectionFromRaycastHit(muzzle.position);
        muzzle.transform.forward = rotDir;
    }

#if UNITY_EDITOR
    protected virtual void OnDrawGizmos()
    {   
        Gizmos.color = Color.indianRed;
        Gizmos.DrawSphere(hit.point, 0.5f);

        Gizmos.DrawRay(muzzle.position, rotDir);
    }
#endif

}
