using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponShotgun : BaseWeapon
{
    [Header("Shotgun Variables")]
    [SerializeField] private int numPellets = 8;
    [SerializeField] private float spreadHalfAngle = 30f;
    private List<RaycastHit> hits = new List<RaycastHit>();

    protected override void FireRpc()
    {
        hits.Clear();
        Debug.Log(Muzzle == null ? "Muzzle is NULL" : "Muzzle is not null");
        hits = VectorExtensions.MultipleRaycastInCone(Muzzle.position, Muzzle.forward, Muzzle.up, numPellets, spreadHalfAngle);
        Debug.Log(hits == null ? "Hits is null weapon shotgun" : $"Hits is not null, hit count: {hits.Count}");

        GetHitDataList(hits);

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
        if (IsOwner)
        {
            if (!CanWeaponFire) return;
            Debug.Log("Fire() in weapon shotgun");

            AdjustDistanceBasedStats(mouseDistance);
            FireRpc();
            FireEventMethodClientRpc();
        }

        ChangeAmmoText();
    }

    protected override void GetHitDataList(List<RaycastHit> hits)
    {
        Debug.Log("calling GetHitDataList in weapon shotgun");
        base.GetHitDataList(hits);
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
