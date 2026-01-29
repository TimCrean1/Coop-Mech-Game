using UnityEngine;
using System.Collections;
using UnityEngine.VFX;

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


    public virtual void Fire() //public because this will be called by weapon manager
    {
        if (canFire)
        {
            Debug.Log("Fire input received");

            Physics.Raycast(muzzle.position, muzzle.forward, out RaycastHit hit);
            if (fireEffect) { fireEffect.SendEvent("OnFire"); }

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

    public void SetMuzzleRotation(Vector3 rot)
    {
        muzzle.rotation = Quaternion.LookRotation(rot);
    }

}
