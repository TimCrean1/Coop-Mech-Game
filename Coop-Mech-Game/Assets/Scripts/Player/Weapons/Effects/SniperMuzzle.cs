using UnityEngine;

public class SniperMuzzle : WeaponMuzzle
{
    public override void SendFireEvent(RaycastHit hit)
    {
        bulletEffect.SetVector3("Bullet_LineEnd", hit.point);
        bulletEffect.SetVector3("Bullet_LineStart", transform.position);

        bulletEffect.SendEvent("OnFire");
    }
}
