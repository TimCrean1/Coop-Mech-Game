using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponShotgun : BaseWeapon
{
    [Header("Shotgun Variables")]
    [SerializeField] private int numPellets = 8;
    [SerializeField] private float spreadHalfAngle = 30f;

    protected override void FireRpc()
    {
        float t1dmg = 0f;
        float t2dmg = 0f;

        foreach(var hit in hits) //hits is a protected list in the base class
        {
            // Debug.Log(hit);
            if (hit.collider.gameObject.CompareTag("TeamOne"))
            {
                t1dmg += currentDamage;
            }
            else if (hit.collider.gameObject.CompareTag("TeamTwo"))
            {
                t2dmg += currentDamage;
            }
        }

        GameManager.Instance.DamageTeamRpc(1, t1dmg);
        GameManager.Instance.DamageTeamRpc(2, t2dmg);

        BuildCooldown();
    }

    public override void Fire(float mouseDistance)
    {
        hits = VectorExtensions.MultipleRaycastInCone(Muzzle.position, Muzzle.forward, Muzzle.up, numPellets, spreadHalfAngle);

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
