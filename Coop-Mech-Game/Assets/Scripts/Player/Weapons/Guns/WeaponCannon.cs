using UnityEngine;
using System.Collections;

public class WeaponCannon : BaseWeapon
{
    [SerializeField] private float fireRate = 1f;

    protected override IEnumerator FireRoutine(float fr)
    {
        fr = fireRate;
        return base.FireRoutine(fr);
    }
}
