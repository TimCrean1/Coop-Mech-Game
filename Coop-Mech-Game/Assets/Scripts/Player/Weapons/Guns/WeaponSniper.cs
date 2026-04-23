using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSniper : BaseWeapon
{
    [Header("Sniper Variables")]
    [SerializeField] private int maxBounceCount = 2;
    [SerializeField] private float maxCastDistance = 300f;
    [SerializeField] private float castBounceMult = 0.5f;

    protected override void FireRpc()
    {
        float t1dmg = 0f;
        float t2dmg = 0f;

        foreach (var hit in hits)
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

        if (t1dmg > 0f) { GameManager.Instance.DamageTeamRpc(1, t1dmg); }
        if (t2dmg > 0f) { GameManager.Instance.DamageTeamRpc(2, t2dmg); }

        BuildCooldown();
    }

    public override void Fire(float mouseDistance)
    {
        //base.Fire(mouseDistance);

        if (IsOwner)
        {
            if (!CanWeaponFire) return;
            Debug.Log("Fire() in weapon sniper");

            AdjustDistanceBasedStats(mouseDistance);

            SequentialRaycastClientRpc(maxBounceCount, maxCastDistance, castBounceMult);
            FireRpc();
            FireEventMethodClientRpc();
        }

        ChangeAmmoText();
    }
    
    protected override void AdjustDistanceBasedStats(float mouseDistance)
    {
        currentDamage = damage * mouseDistance;
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
