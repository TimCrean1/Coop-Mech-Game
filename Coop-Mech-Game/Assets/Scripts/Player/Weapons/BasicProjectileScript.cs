using UnityEngine;
using UnityEngine.Animations.Rigging;

public class BasicProjectileScript : BaseProjectile
{
    [SerializeField] private float projectileSpeed = 50f;
    [SerializeField] private float dropOffRate = 5f;
    public override void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        rb.AddForce(transform.forward * projectileSpeed, ForceMode.Impulse);
        rb.AddForce(Vector3.down * dropOffRate * Time.deltaTime, ForceMode.Acceleration);
    }
}
