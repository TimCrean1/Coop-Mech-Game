using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;

public class WeaponShotgun : BaseWeapon
{
    [Header("Shotgun Variables")]
    [SerializeField] private int numPellets = 8;
    [SerializeField] private float spreadHalfAngle = 18f;
    [SerializeField] private float maxRange = 70f;
   
    //private List<RaycastHit> hits = new List<RaycastHit>();

    protected override void FireRpc()
    {
        float t1dmg = 0f;
        float t2dmg = 0f;

        foreach(var hit in hits)
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

        if(t1dmg > 0f) { GameManager.Instance.DamageTeamRpc(1, t1dmg, transform.position); }
        if(t2dmg > 0f) { GameManager.Instance.DamageTeamRpc(2, t2dmg, transform.position); }

        BuildCooldown();
    }

    public override void Fire(float mouseDistance)
    {
        if (IsOwner)
        {
            if (!CanWeaponFire) return;
            //Debug.Log("Fire() in weapon shotgun");

            AdjustDistanceBasedStats(mouseDistance);

            RaycastInConeClientRpc(numPellets, maxRange, spreadHalfAngle);
            FireRpc();
            FireEventMethodClientRpc();
        }

        ChangeAmmoText();
    }
    [Rpc(SendTo.ClientsAndHost)]
    protected override void FireEventMethodClientRpc()
    {
        if (muzzleComp && isMultiShot == false)
        {
            Debug.Log("hit pos is:" + hit.point); muzzleComp.SendFireEvent(hit);
            gunAnimator.SetTrigger("shot");
            ammoAnimator.SetTrigger("shot");

        }

        if (audioSource != null && weaponAudioClip != null)
        {
            audioSource.PlayOneShot(weaponAudioClip);
        }
        if (muzzleComp && isMultiShot == false) { Debug.Log("hit pos is:" + hit.point); muzzleComp.SendFireEvent(hit); }
        else if (muzzleComp && isMultiShot == true) { Debug.Log("BaseWeapon: hits list count is: " + hits.Count); muzzleComp.SendFireEventList(hits); }
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
