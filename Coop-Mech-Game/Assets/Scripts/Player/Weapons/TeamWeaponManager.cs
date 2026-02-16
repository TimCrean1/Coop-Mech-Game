using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;


public class TeamWeaponManager : MonoBehaviour
{
    [SerializeField] private List<BaseWeapon> P1WeaponsList = new List<BaseWeapon>();
    [SerializeField] private List<BaseWeapon> P2WeaponsList = new List<BaseWeapon>();

    //[SerializeField] private bool _enableStaggeredFire = true;
    //[SerializeField] private float staggeredFireTime = 0.25f;
    [SerializeField] private CinemachineImpulseSource shootingImpulseSource;

    //public bool EnableStaggeredFire { get { return _enableStaggeredFire; } }

    private int _p1EquippedWeapon = 0;
    private int _p2EquippedWeapon = 0;

    public int P1EquippedWeapon { get {  return _p1EquippedWeapon; } }
    public int P2EquippedWeapon { get {  return _p2EquippedWeapon; } }

    private RaycastHit hit;
    private Ray screenRay;
    private Vector3 rotDir;

    public void SetScreenRay(Ray ray)
    {
        screenRay = ray;
        UpdateWeaponTarget();
    }

    public void ChangeEquippedWeapon(int player, int weaponIdx)
    {
        switch (player)
        {
            case 1:
                _p1EquippedWeapon = weaponIdx; break;
            case 2:
                _p2EquippedWeapon= weaponIdx; break;
        }
    }

    public void UpgradeWeapon(int player, int weaponIdx)
    {

    }

    public void AppendWeaponToList(int player, int weaponIdx)
    {

    }

    public void RemoveWeaponFromList(int player, int weaponIdx)
    {

    }

    private void UpdateWeaponTarget()
    {
        Physics.Raycast(screenRay, out hit);

        for (int  i = 0; i < P1WeaponsList.Count; i++)
        {
            //Vector3 direction = hit.GetDirectionFromRaycastHit(weaponsList[i].Muzzle.position);
            rotDir = hit.GetDirectionFromRaycastHit(P1WeaponsList[i].Muzzle.position);

            P1WeaponsList[i].SetMuzzleRotation(hit, rotDir);
        }
    }
    public void FireWeapons(float input)
    {
        if(input == 0.25f) //P1 fire
        {
            if (P1WeaponsList[_p1EquippedWeapon].CanWeaponFire) { P1WeaponsList[_p1EquippedWeapon].Fire(); }
            shootingImpulseSource.GenerateImpulse();
        }
        else if(input == 0.75f) //P2 fire
        {
            if (P2WeaponsList[_p2EquippedWeapon].CanWeaponFire) { P2WeaponsList[_p2EquippedWeapon].Fire(); }
            shootingImpulseSource.GenerateImpulse();
        }
        else if(input == 1f) //Both P1 & P2 fire
        {
            if (P1WeaponsList[_p1EquippedWeapon].CanWeaponFire) { P1WeaponsList[_p1EquippedWeapon].Fire(); }
            else if (P2WeaponsList[_p2EquippedWeapon].CanWeaponFire) { P2WeaponsList[_p2EquippedWeapon].Fire(); }
            shootingImpulseSource.GenerateImpulse();
        }

        // Check if the first weapon can fire
        //if (P1WeaponsList[0].CanWeaponFire)
        //{
        //    if (_enableStaggeredFire)
        //    {
        //        // Fire weapons one after another with a delay
        //        StartCoroutine(WeaponFireRoutine(input));
        //        return;
        //    }
        //    else
        //    {
        //        if (input == 0.25) //Player 1 shooting
        //        {
        //            foreach (BaseWeapon weapon in P1WeaponsList) if (weapon.owningPlayer == 1)
        //            {
        //                weapon.Fire();
        //            }
        //        }
        //        else if (input == 0.75) //Player 2 shooting
        //        {
        //            foreach (BaseWeapon weapon in P1WeaponsList) if (weapon.owningPlayer == 2)
        //            {
        //                weapon.Fire();
        //            }
        //        }
        //        else if (input == 1) //Both players shooting
        //        {
        //            // Fire all weapons simultaneously
        //            foreach (BaseWeapon weapon in P1WeaponsList)
        //            {
        //                weapon.Fire();
        //            }
        //        }
        //    }
        //    // Generate camera impulse effect after firing
        //    shootingImpulseSource.GenerateImpulse();
        //}
    }

    //private IEnumerator WeaponFireRoutine(float input)
    //{
    //    if (input == 0.25) //Player 1 shooting
    //        {
    //            foreach (BaseWeapon weapon in P1WeaponsList) if (weapon.owningPlayer == 1)
    //            {
    //                weapon.Fire();
    //                shootingImpulseSource.GenerateImpulse();
    //                yield return new WaitForSeconds(staggeredFireTime);
    //            }
    //        }
    //        else if (input == 0.75) //Player 2 shooting
    //        {
    //            foreach (BaseWeapon weapon in P1WeaponsList) if (weapon.owningPlayer == 2)
    //            {
    //                weapon.Fire();
    //                shootingImpulseSource.GenerateImpulse();
    //                yield return new WaitForSeconds(staggeredFireTime);
    //            }
    //        }
    //        else if (input == 1) //Both players shooting
    //        {
    //            // Fire all weapons simultaneously
    //            foreach (BaseWeapon weapon in P1WeaponsList)
    //            {
    //                weapon.Fire();
    //                shootingImpulseSource.GenerateImpulse();
    //                yield return new WaitForSeconds(staggeredFireTime);
    //            }
    //        }
        
    //    yield return null;
    //}

    //public void SetTeamWeaponsFireMode(TeamFireModes fireMode)
    //{
    //    switch (fireMode)
    //    {
    //        case TeamFireModes.Simultaneous:
    //            _enableStaggeredFire = false;
    //            break;
    //        case TeamFireModes.Staggered:
    //            _enableStaggeredFire = true;
    //            break;
    //    }
    //}

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