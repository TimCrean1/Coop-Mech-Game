using UnityEngine;
using System.Collections;

public class WeaponCannon : BaseWeapon
{

    public override void Fire()
    {
        base.Fire();
        Debug.Log("WeaponCannon Fire()");
    }
}
