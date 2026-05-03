using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSniper : BaseWeapon
{
    [Header("Sniper Variables")]
    [SerializeField] private int maxBounceCount = 2;
    [SerializeField] private float maxCastDistance = 300f;
    [SerializeField] private float castBounceMult = 0.5f;
    private List<RaycastHit> weaponHits = new List<RaycastHit>();

    protected override void FireRpc()
    {
        weaponHits = VectorExtensions.SequentialRaycast(Muzzle.transform.position, Muzzle.transform.forward, maxCastDistance, maxBounceCount, castBounceMult);
        hit = weaponHits[0];

        foreach (var hit in weaponHits)
        {
            if (hit.collider.gameObject.CompareTag("TeamOne"))
            {
                GameManager.Instance.DamageTeamRpc(1, currentDamage, transform.position);
            }
            else if (hit.collider.gameObject.CompareTag("TeamTwo"))
            {
                GameManager.Instance.DamageTeamRpc(2, currentDamage,transform.position);
            }
        }

        Debug.Log("FireRpc in WeaponSniper");
        
        BuildCooldown();
    }

    public override void Fire(float mouseDistance)
    {
        base.Fire(mouseDistance);
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
