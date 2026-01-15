using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public abstract class BaseProjectile : MonoBehaviour
{
    // protected float projectileSpeed;
    // protected float dropOffRate;
    protected Rigidbody rb;
    protected Collider col;
    public abstract void OnCollisionEnter(Collision collision);
    // public abstract void OnTriggerEnter();

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

}