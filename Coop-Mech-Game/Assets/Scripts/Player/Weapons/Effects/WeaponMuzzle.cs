using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public abstract class WeaponMuzzle : MonoBehaviour
{
    [SerializeField] protected VisualEffect bulletEffect;

    public abstract void SendFireEvent(RaycastHit hit);
    public virtual void SendFireEventList(List<RaycastHit> hitList)
    {
        //no default behaviour
    }
}
