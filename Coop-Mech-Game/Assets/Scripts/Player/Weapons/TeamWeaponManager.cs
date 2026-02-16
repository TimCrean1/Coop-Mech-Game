using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;


public class TeamWeaponManager : MonoBehaviour
{
    [SerializeField] private List<BaseWeapon> weaponsList = new List<BaseWeapon>();
    [SerializeField] private bool _enableStaggeredFire = true;
    [SerializeField] private float staggeredFireTime = 0.25f;
    [SerializeField] private CinemachineImpulseSource shootingImpulseSource;


    public bool EnableStaggeredFire { get { return _enableStaggeredFire; } }

    private RaycastHit hit;
    private Ray screenRay;
    private Vector3 rotDir;

    public void SetScreenRay(Ray ray)
    {
        screenRay = ray;
        UpdateWeaponTarget();
    }

    private void UpdateWeaponTarget()
    {
        Physics.Raycast(screenRay, out hit);

        for (int  i = 0; i < weaponsList.Count; i++)
        {
            //Vector3 direction = hit.GetDirectionFromRaycastHit(weaponsList[i].Muzzle.position);
            rotDir = hit.GetDirectionFromRaycastHit(weaponsList[i].Muzzle.position);

            weaponsList[i].SetMuzzleRotation(hit, rotDir);
        }
    }
    public void FireWeapons(float input)
    {
        // Check if the first weapon can fire
        if (weaponsList[0].CanWeaponFire)
        {
            if (_enableStaggeredFire)
            {
                // Fire weapons one after another with a delay
                StartCoroutine(WeaponFireRoutine(input));
                return;
            }
            else
            {
                if (input == 0.25) //Player 1 shooting
                {
                    foreach (BaseWeapon weapon in weaponsList) if (weapon.owningPlayer == 1)
                    {
                        weapon.FireRpc();
                    }
                }
                else if (input == 0.75) //Player 2 shooting
                {
                    foreach (BaseWeapon weapon in weaponsList) if (weapon.owningPlayer == 2)
                    {
                        weapon.FireRpc();
                    }
                }
                else if (input == 1) //Both players shooting
                {
                    // Fire all weapons simultaneously
                    foreach (BaseWeapon weapon in weaponsList)
                    {
                        weapon.FireRpc();
                    }
                }
            }
            // Generate camera impulse effect after firing
            shootingImpulseSource.GenerateImpulse();
        }
    }

    private IEnumerator WeaponFireRoutine(float input)
    {
        if (input == 0.25) //Player 1 shooting
            {
                foreach (BaseWeapon weapon in weaponsList) if (weapon.owningPlayer == 1)
                {
                    weapon.FireRpc();
                    shootingImpulseSource.GenerateImpulse();
                    yield return new WaitForSeconds(staggeredFireTime);
                }
            }
            else if (input == 0.75) //Player 2 shooting
            {
                foreach (BaseWeapon weapon in weaponsList) if (weapon.owningPlayer == 2)
                {
                    weapon.FireRpc();
                    shootingImpulseSource.GenerateImpulse();
                    yield return new WaitForSeconds(staggeredFireTime);
                }
            }
            else if (input == 1) //Both players shooting
            {
                // Fire all weapons simultaneously
                foreach (BaseWeapon weapon in weaponsList)
                {
                    weapon.FireRpc();
                    shootingImpulseSource.GenerateImpulse();
                    yield return new WaitForSeconds(staggeredFireTime);
                }
            }
        // foreach (BaseWeapon weapon in weaponsList) 
        // {
        //     weapon.Fire();
        //     shootingImpulseSource.GenerateImpulse();
        //     yield return new WaitForSeconds(staggeredFireTime);
        // }
        yield return null;
    }

    public void SetTeamWeaponsFireMode(TeamFireModes fireMode)
    {
        switch (fireMode)
        {
            case TeamFireModes.Simultaneous:
                _enableStaggeredFire = false;
                break;
            case TeamFireModes.Staggered:
                _enableStaggeredFire = true;
                break;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.aquamarine;
        Gizmos.DrawSphere(hit.point, 0.25f);
    }
#endif
}

public enum TeamFireModes
{
    Simultaneous,
    Staggered
}