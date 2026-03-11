using UnityEngine;

public class CannonMuzzle : WeaponMuzzle
{
    public override void SendFireEvent(RaycastHit hit)
    {
        //Debug.Log("Sending event");
        if (bulletEffect)
        {
            bulletEffect.SetVector3("Bullet_LineEnd", hit.point);
            bulletEffect.SetVector3("Bullet_LineStart", transform.position);
            bulletEffect.SetVector3("Hit_PointNormal", hit.normal);

            bulletEffect.SendEvent("OnFire");
        }
    }
}
