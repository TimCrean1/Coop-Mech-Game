using UnityEngine;

public class CannonProjectile : BaseProjectile
{
    [SerializeField] private float projectileSpeed = 50f;
    [SerializeField] private float damage = 10f;

    protected override void Awake()
    {
        base.Awake();

        Debug.Log("Awake in CannonProj");
        //baseProjectileSpeed = projectileSpeed;
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }

    protected override void OnHit(bool didHitPlayer)
    {
        base.OnHit(didHitPlayer);
    }

    protected override void OnEnable()
    {
        //base.OnEnable();

        Debug.Log("OnEnable in CannonProj");
        Debug.Log("rb may be null, status: " + (rb == null));

        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
            Debug.Log("rb should no longer be null (i.e bool false): " + (rb == null));
        }


        if (gameObject.activeSelf)
        {
            rb.AddForce(transform.forward * projectileSpeed, ForceMode.Impulse);
        }
    }

}
