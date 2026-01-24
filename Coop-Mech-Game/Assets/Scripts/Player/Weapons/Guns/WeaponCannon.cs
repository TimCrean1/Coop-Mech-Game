using UnityEngine;
using System.Collections;

public class WeaponCannon : BaseWeapon
{

    protected override IEnumerator FireRoutine(float fr)
    {
        fr = FireRate;
        return base.FireRoutine(fr);
    }
}
