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
    private Vector3 collisionNormal;

    private Vector3 targetPos = Vector3.zero;
    private Quaternion targetRot = Quaternion.identity;

    public virtual void PrepFire(Vector3 tpos, Quaternion trot)
    {
        targetPos = tpos;
        targetRot = trot;
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        Debug.Log("We hit this thing" + collision.gameObject);
        //check what the projectile hit
        hitPlayer = collision.gameObject.CompareTag("Player");
        if (hitPlayer)
        {
            //send event to it's health manager
        }

        collisionNormal = collision.GetContact(0).normal;

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

        //Debug.Log("Awake in BaseProj");

    }

    protected virtual void OnEnable()
    {
        transform.position = targetPos;
        transform.rotation = targetRot;
        Debug.Log("target position: " + targetPos + " target rotation " + targetRot.eulerAngles + " rb pos: " + transform.position + " rb rot: " + transform.rotation.eulerAngles);
        //rb.AddForce(transform.forward *  baseProjectileSpeed, ForceMode.Impulse);
        // do nothing
    }

    protected virtual void OnHit(bool didHitPlayer)
    {
        //BaseEffect fx = teamProjectilePool.GetNextEffect(didHitPlayer);
        //fx.PrepPlay(collisionNormal);

        //if (fx == null) 
        //{ 
        //    Debug.LogError("No valid effect found, bool status fx == null: " + fx == null); 
        //    return; 
        //}
        //else 
        //{ 
        //    fx.gameObject.transform.position = transform.position;
        //    fx.gameObject.SetActive(true); 
        //}
    }
}