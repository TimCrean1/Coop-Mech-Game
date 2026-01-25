using UnityEngine;

public class CannonProjectile : BaseProjectile
{
    [SerializeField] private float projectileSpeed = 50f;
    [SerializeField] private float damage = 10f;

    protected override void Start()
    {
        base.Start();

        Debug.Log("Start in CannonProj");
        //baseProjectileSpeed = projectileSpeed;
    }

    protected override void OnEnable()
    {
        //base.OnEnable();

        Debug.Log("OnEnable in CannonProj");

        if (gameObject.activeSelf)
        {
            rb.AddForce(transform.forward * projectileSpeed, ForceMode.Impulse);
        }
    }

}
