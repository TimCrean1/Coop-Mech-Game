using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public abstract class BaseProjectile : MonoBehaviour
{
     protected float baseProjectileSpeed = 50f;
    // protected float dropOffRate;
    protected TeamProjectilePool teamProjectilePool;
    protected Collider col;
    protected Rigidbody rb;
    protected Transform muzzle;
    //protected BaseProjectileEffect expEffect;
    protected virtual void OnCollisionEnter(Collision collision)
    {
        //check what the projectile hit
        if (collision.gameObject.CompareTag("Player"))
        {
            //send event to it's health manager
        }

        this.gameObject.SetActive(false);
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //send event to it's health manager
            this.gameObject.SetActive(false);
        }

    }

    protected virtual void Start()
    {
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
    }

    protected virtual void OnEnable()
    {
        rb.AddForce(transform.forward *  baseProjectileSpeed, ForceMode.Impulse);
    }

    protected virtual void OnHit()
    {
        BaseEffect fx = teamProjectilePool.GetNextEffect();
        if (fx == null) { Debug.LogError("No effect found!"); }
        else 
        { 
            fx.gameObject.transform.position = transform.position;
            fx.gameObject.SetActive(true); 
        }
    }
}