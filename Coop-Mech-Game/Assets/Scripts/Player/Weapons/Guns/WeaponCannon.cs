using UnityEngine;
using System.Collections;

public class WeaponCannon : BaseWeapon
{

    public override void Fire()
    {
        base.Fire();
        Debug.Log("WeaponCannon Fire()");
    }

    protected override IEnumerator FireRateRoutine(float fireRate)
    {
        return base.FireRateRoutine(fireRate);
    }

    protected override void BuildCooldown()
    {
        base.BuildCooldown();
    }

    protected override IEnumerator ActivateCooldown()
    {
        return base.ActivateCooldown();
    }

    public override void SetMuzzleRotationAtHit(RaycastHit rayHit)
    {
        base.SetMuzzleRotationAtHit(rayHit);
    }

#if UNITY_EDITOR
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
    }
#endif
}
