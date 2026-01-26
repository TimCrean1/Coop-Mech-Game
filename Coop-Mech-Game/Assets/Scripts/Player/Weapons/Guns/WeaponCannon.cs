using UnityEngine;
using System.Collections;

public class WeaponCannon : BaseWeapon
{

    public override void Fire()
    {
        base.Fire();
        Debug.Log("WeaponCannon Fire()");
    }
    protected override IEnumerator FireRoutine(float fr)
    {
        fr = FireRate;
        return base.FireRoutine(fr);
    }
}
