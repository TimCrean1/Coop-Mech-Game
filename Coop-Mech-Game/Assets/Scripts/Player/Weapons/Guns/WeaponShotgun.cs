using UnityEngine;
using System.Collections;

public class WeaponShotgun : BaseWeapon
{
    public override void Fire(float mouseDistance)
    {
        base.Fire(mouseDistance);
    }
    
    protected override void AdjustDistanceBasedStats(float mouseDistance)
    {
        // currentKnockback = ?
    }

    protected override IEnumerator FireRateRoutine(float fireRate)
    {
        return base.FireRateRoutine(fireRate);
    }

    protected override void BuildCooldown()
    {
        base.BuildCooldown();
    }

    protected override void ActivateCooldown()
    {
        base.ActivateCooldown();
    }

    protected override IEnumerator CooldownRotuine()
    {
        return base.CooldownRotuine();
    }

    public override void SetMuzzleRotation(RaycastHit rayHit, Vector3 rotDir)
    {
        base.SetMuzzleRotation(rayHit, rotDir);
    }

#if UNITY_EDITOR
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
    }
#endif
}
