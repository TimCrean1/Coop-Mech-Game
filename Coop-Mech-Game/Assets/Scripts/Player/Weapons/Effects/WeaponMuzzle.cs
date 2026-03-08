using UnityEngine;
using UnityEngine.VFX;

public class WeaponMuzzle : MonoBehaviour
{
    [SerializeField] private VisualEffect bulletEffect;
    [SerializeField] private VisualEffect fireEffect;

    public void SendFireEvent(RaycastHit hit)
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
