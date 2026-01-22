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

    private float cooldownTime = 1.0f;
    private int ammoCount;

    private bool canFire = true;

    protected virtual IEnumerator FireRoutine(float fireRate) //public because this will be called by input handler
    {
        if (canFire)
        {
            //pick projectile from team array
            BaseProjectile proj = teamProjectilePool.GetNextProjectile(this);
            proj.transform.position = muzzle.transform.position;
            proj.transform.rotation = muzzle.transform.rotation;

            //activate projectile which will "fire" it
            proj.gameObject.SetActive(true);
        }

        BuildCooldown();
        yield return new WaitForSeconds(fireRate);
    }

    protected virtual void BuildCooldown()
    {
        ammoCount -= 1;
        if(ammoCount <= 0)
        {
            ActivateCooldown();
        }
    }

    public virtual IEnumerator ActivateCooldown() //this is used for reloading but maybe also from damage effects
    {
        canFire = false;

        yield return new WaitForSeconds(cooldownTime);

        ammoCount = ammo;
        canFire = true;
    }

}
