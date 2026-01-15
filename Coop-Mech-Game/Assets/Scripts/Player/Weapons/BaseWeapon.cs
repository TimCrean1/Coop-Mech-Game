using UnityEngine;
using System.Collections;

public abstract class BaseWeapon : MonoBehaviour
{
    private float fireRate = 1.0f;
    private float cooldownTime = 1.0f;
    private int ammoCount;

    private bool canFire = true;

    private Transform muzzle;

    /// <summary>
    /// 
    /// This class should be pretty weapon type agnostic, so ammoCount is the universal cooldown counter
    /// ActivateCooldown is reloading or overheating etc.
    /// 
    /// </summary>

    public virtual IEnumerator FireRoutine() //public because this will be called by input handler
    {
        if (canFire)
        {
            //pick projectile from team array

            //proj.transform.position = muzzle.transform.position
            //proj.transform.rotation = muzzle.transform.rotation

            //activate projectile which will "fire" it
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

        canFire = true;
    }

}
