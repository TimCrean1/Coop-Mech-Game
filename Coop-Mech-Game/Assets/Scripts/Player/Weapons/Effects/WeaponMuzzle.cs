using UnityEngine;
using UnityEngine.VFX;

public abstract class WeaponMuzzle : MonoBehaviour
{
    [SerializeField] protected VisualEffect bulletEffect;

    public abstract void SendFireEvent(RaycastHit hit);
}
