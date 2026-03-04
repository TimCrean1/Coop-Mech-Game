using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponShotgun : BaseWeapon
{
    List<RaycastHit> hits = new List<RaycastHit>();

    protected override void FireRpc()
    {
        hits = VectorExtensions.MultipleRaycastInCone(Muzzle.position, Muzzle.forward, Muzzle.up, 8, 30f);
        foreach(var hit in hits)
        {
            if (hit.collider.gameObject.CompareTag("TeamOne"))
            {
                GameManager.Instance.DamageTeamRpc(1, currentDamage);
            }
            else if (hit.collider.gameObject.CompareTag("TeamTwo"))
            {
                GameManager.Instance.DamageTeamRpc(2, currentDamage);
            }
        }
    }

    public override void Fire(float mouseDistance)
    {
        base.Fire(mouseDistance);
    }
    
    protected override void AdjustDistanceBasedStats(float mouseDistance)
    {
        currentKnockback = baseKnockbackForce * mouseDistance;
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
