using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponCannon : BaseWeapon
{

    public override void Fire(float mouseDistance)
    {
        base.Fire(mouseDistance);
    }

    protected override void FireRpc()
    {
        base.FireRpc();
    }

    protected override void AdjustDistanceBasedStats(float mouseDistance)
    {
        Debug.Log(mouseDistance + " " + currentFireRate);
        currentFireRate = baseFireRate * 1-mouseDistance;
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

    protected override void ChangeAmmoText()
    {
        base.ChangeAmmoText();
    }



#if UNITY_EDITOR
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
    }
#endif
}
