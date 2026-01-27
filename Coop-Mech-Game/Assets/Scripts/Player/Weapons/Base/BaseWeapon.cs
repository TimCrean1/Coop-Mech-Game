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

    private Vector3 rot;

    public virtual void Fire() //public because this will be called by input handler
    {
        if (canFire)
        {
            StartCoroutine(FireRoutine(baseFireRate));
            Debug.Log("Fire input received");
        }
    }

    protected virtual IEnumerator FireRoutine(float fireRate) 
    {
        if (canFire)
        {
            canFire = false;
            //pick projectile from team array
            GameObject proj = teamProjectilePool.GetNextProjectile(this);
            BaseProjectile ee = proj.GetComponent<BaseProjectile>();
            
            ee.PrepFire(muzzle.position, muzzle.rotation);

            proj.gameObject.SetActive(true);
            //proj.transform.position = muzzle.transform.position;
            //proj.transform.rotation = muzzle.transform.rotation;

            //activate projectile which will "fire" it


            BuildCooldown();
            yield return new WaitForSeconds(fireRate);
        }

        canFire = true;
        Debug.Log("Fire complete!");
    }

    public virtual void SetAverageCursorPos(Vector3 pos)
    {
        rot = pos;
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
