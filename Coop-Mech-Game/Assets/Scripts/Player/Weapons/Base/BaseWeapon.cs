using UnityEngine;
using System.Collections;

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
    [SerializeField] private int ammo = 10;
    [SerializeField] private float baseFireRate = 1f;

    private float cooldownTime = 1.0f;
    private int ammoCount;

    private bool canFire = true;

    public float FireRate { get { return baseFireRate; } }

    public virtual void Fire()
    {
        StartCoroutine(FireRoutine(baseFireRate));
        Debug.Log("Fire input received");
    }

    protected virtual IEnumerator FireRoutine(float fireRate) //public because this will be called by input handler
    {
        if (canFire)
        {
            //pick projectile from team array
            BaseProjectile proj = teamProjectilePool.GetNextProjectile(this);
            proj.PrepFire(muzzle.position, muzzle.rotation);

            //proj.transform.position = muzzle.transform.position;
            //proj.transform.rotation = muzzle.transform.rotation;

            //activate projectile which will "fire" it
            proj.gameObject.SetActive(true);
        }

        BuildCooldown();
        yield return new WaitForSeconds(fireRate);
        Debug.Log("Fire complete!");
    }

    protected virtual void BuildCooldown()
    {
        ammoCount -= 1;
        Debug.Log("Ammo: " + ammoCount);
        if (ammoCount <= 0)
        {
            ActivateCooldown();
        }
    }

    public virtual IEnumerator ActivateCooldown() //this is used for reloading but maybe also from damage effects
    {
        canFire = false;

        Debug.Log("cooldown start");

        yield return new WaitForSeconds(cooldownTime);

        ammoCount = ammo;
        canFire = true;
        Debug.Log("cooldown end");
    }

}
