using UnityEngine;
using UnityEngine.VFX;

public class WeaponMuzzle : MonoBehaviour
{
    [SerializeField] private VisualEffect bulletEffect;
    [SerializeField] private VisualEffect fireEffect;

    public void SendFireEvent()
    {
        //Debug.Log("Sending event");
        if (bulletEffect)
        {
            bulletEffect.SendEvent("OnFire");
        }

        if(fireEffect)
        {
            fireEffect.SendEvent("OnFire");
        }
    }
}
