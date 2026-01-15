using UnityEngine;

public abstract class BaseProjectile
{
    private Collider collider;
    private float projectileSpeed;
    private float dropOffRate;

    public abstract void OnCollisionEnter();
    public abstract void OnTriggerEnter();
    
}