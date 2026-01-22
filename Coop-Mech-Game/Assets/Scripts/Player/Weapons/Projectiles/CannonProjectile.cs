using UnityEngine;

public class CannonProjectile : BaseProjectile
{
    [SerializeField] private float projectileSpeed = 50f;
    [SerializeField] private float damage = 10f;

    private void Start()
    {
        base.Start();

        baseProjectileSpeed = projectileSpeed;
    }

}
