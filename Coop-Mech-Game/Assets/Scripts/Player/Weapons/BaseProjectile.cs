using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public abstract class BaseProjectile : MonoBehaviour
{
     //protected float projectileSpeed = 50f;
    // protected float dropOffRate;
    protected Rigidbody rb;
    protected Collider col;
    protected Transform muzzle;
    protected virtual void OnCollisionEnter(Collision collision)
    {
        //check what the projectile hit
        //if it's an enemy send event to it's health manager
        //enable particle effect that plays when OnEnable is received
    }
    // public abstract void OnTriggerEnter();

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    protected virtual void OnEnable()
    {
        //position/rotation init is handled in the Fire() function of the weapon
        rb.AddForce(transform.forward * projectileSpeed, ForceMode.Impulse);
    }

}