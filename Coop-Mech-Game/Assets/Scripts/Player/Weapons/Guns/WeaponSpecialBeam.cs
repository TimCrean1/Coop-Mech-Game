using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;

public class WeaponSpecialBeam : BaseWeapon
{
    [Header("Special Weapon Variables")]
    [SerializeField] private float weaponFireTime = 5f;

    private bool _isSpecialReady = true;
    private float count = 0f;

    public override void Fire(float mouseDistance)
    {
        if (IsOwner)
        {
            if (_isSpecialReady == false) return;

            AdjustDistanceBasedStats(mouseDistance); //set damage on initial fire to encourage coordination

            //if (audioSource != null && weaponAudioClip != null)
            //{
            //    audioSource.PlayOneShot(weaponAudioClip);
            //}

            FireRpc();

            StartContinuousFireClientRpc();

            //FireEventMethodClientRpc();
        }

        ChangeAmmoText();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void StartContinuousFireClientRpc()
    {
        StartCoroutine(ContinuousFireRoutine());
    }

    private IEnumerator ContinuousFireRoutine()
    {
        _isSpecialReady = false;

        while (count < weaponFireTime)
        {
            count += Time.deltaTime;

            if (audioSource != null && weaponAudioClip != null)
            {
                audioSource.PlayOneShot(weaponAudioClip);
            }

            Physics.Raycast(Muzzle.position, Muzzle.forward, out hit);
            GameObject other = hit.collider.gameObject;

            if (IsHost || IsServer) //TODO: ask nando which is correct
            {
                if (other.CompareTag("TeamOne"))
                {
                    GameManager.Instance.DamageTeamRpc(1, currentDamage, hit.transform.position);
                }
                else if (other.CompareTag("TeamTwo"))
                {
                    GameManager.Instance.DamageTeamRpc(2, currentDamage, hit.transform.position);
                }
                else if (other.CompareTag("Target"))
                {
                    //other.GetComponent<KillhouseEnemy>().Deactivate();
                    // other.GetComponent<KillhouseEnemy>().DeactivateRpc();
                    other.GetComponent<KillhouseEnemy>().Deactivate();
                }
            }

            FireEventMethodClientRpc();

            yield return new WaitForSeconds(FireRate);
        }

        yield return null;

        _isSpecialReady = false;
        StartCoroutine(SpecialCooldownRoutine());
    }

    private IEnumerator SpecialCooldownRoutine()
    {
        yield return new WaitForSeconds(cooldownTime);
        _isSpecialReady = true;

        //TODO: figure out a way to set ready material on console (direct reference?)
    }

    protected override void AdjustDistanceBasedStats(float mouseDistance)
    {
        currentDamage *= damageMultiplier;
    }

}
