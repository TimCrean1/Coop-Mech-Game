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
    private bool hitPlayer;

    public virtual void PrepFire(Vector3 targetPos, Quaternion targetRot)
    {
        rb.position = targetPos;
        rb.rotation = targetRot;
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        //check what the projectile hit
        hitPlayer = collision.gameObject.CompareTag("Player");
        if (hitPlayer)
        {
            //send event to it's health manager
        }

        OnHit(hitPlayer);

        this.gameObject.SetActive(false);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        //check what the projectile hit
        hitPlayer = other.gameObject.CompareTag("Player");
        if (hitPlayer)
        {
            //send event to it's health manager
        }

        OnHit(hitPlayer);

        this.gameObject.SetActive(false);

    }

    protected virtual void Awake()
    {
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();

        Debug.Log("Awake in BaseProj");

    }

    protected virtual void OnEnable()
    {
        //rb.AddForce(transform.forward *  baseProjectileSpeed, ForceMode.Impulse);
        // do nothing
    }

    protected virtual void OnHit(bool didHitPlayer)
    {
        BaseEffect fx = teamProjectilePool.GetNextEffect(didHitPlayer);


        if (fx == null) { Debug.LogError("No effect found!"); }
        else 
        { 
            fx.gameObject.transform.position = transform.position;
            fx.gameObject.SetActive(true); 
        }
    }
}