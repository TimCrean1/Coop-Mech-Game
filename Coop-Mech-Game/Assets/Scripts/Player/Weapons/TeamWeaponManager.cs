using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TeamWeaponManager : MonoBehaviour
{
    [SerializeField] private List<BaseWeapon> weaponsList = new List<BaseWeapon>();

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

    public void FireWeapons()
    {
        foreach (BaseWeapon weapon in weaponsList) { weapon.Fire(); }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.aquamarine;
        Gizmos.DrawSphere(hit.point, 0.25f);
    }
#endif
}
