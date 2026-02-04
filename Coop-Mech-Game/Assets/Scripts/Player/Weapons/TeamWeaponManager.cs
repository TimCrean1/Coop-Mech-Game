using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TeamWeaponManager : MonoBehaviour
{
    [SerializeField] private List<BaseWeapon> weaponsList = new List<BaseWeapon>();
    [SerializeField] private bool _enableStaggeredFire = true;
    [SerializeField] private float staggeredFireTime = 0.25f;

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

    public void FireWeapons()
    {
        if (weaponsList[0].CanWeaponFire)
        {
            if(_enableStaggeredFire)
            {
                StartCoroutine(WeaponFireRoutine());
            }
            else
            {
                foreach (BaseWeapon weapon in weaponsList) { weapon.Fire(); }
            }
        }
    }

    private IEnumerator WeaponFireRoutine()
    {
        foreach (BaseWeapon weapon in weaponsList) 
        { 
            weapon.Fire();
            yield return new WaitForSeconds(staggeredFireTime);
        }
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
