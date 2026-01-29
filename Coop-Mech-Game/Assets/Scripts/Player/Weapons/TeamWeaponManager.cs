using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TeamWeaponManager : MonoBehaviour
{
    [SerializeField] private List<BaseWeapon> weaponsList = new List<BaseWeapon>();

    private RaycastHit hit;
    private Ray screenRay;

    public void SetScreenRay(Ray ray)
    {
        screenRay = ray;
        Physics.Raycast(screenRay, out hit);
        UpdateWeaponTarget();
    }

    private void UpdateWeaponTarget()
    {
        for (int  i = 0; i < weaponsList.Count; i++)
        {
            Vector3 direction = hit.GetDirectionFromRaycastHit(weaponsList[i].Muzzle.position);
            weaponsList[i].SetMuzzleRotation(direction);
        }
    }

    public void FireWeapons()
    {
        foreach (BaseWeapon weapon in weaponsList) { weapon.Fire(); }
    }
}
