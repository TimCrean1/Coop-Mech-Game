using UnityEngine;

public class ShotgunMuzzle : WeaponMuzzle
{
    public override void SendFireEvent(RaycastHit hit)
    {
        bulletEffect.SendEvent("OnFire");
    }
}
