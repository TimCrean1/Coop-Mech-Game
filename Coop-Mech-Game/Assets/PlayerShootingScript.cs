using UnityEngine;

public class PlayerShootingScript : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletInstantiationPosition;
    [SerializeField] private CombatSFXManager combatSFXManager;
    [SerializeField] private Animator playerAnimator;

    [Header("Shooting Variables")]
    [SerializeField, Range(0f, 10f)] private float shootingCooldown = 0.2f;
    private float shootingTimer = 0f;

    [Header("Manual Rotation Override (Optional)")]
    [SerializeField, Range(0, 360)] private float xShootDirection = 0;
    [SerializeField, Range(0, 360)] private float yShootDirection = 0;
    [SerializeField, Range(0, 360)] private float zShootDirection = 0;
    private Quaternion manualRotation;

    private void Update()
    {
        // cooldown timer
        if (shootingTimer > 0)
            shootingTimer -= Time.deltaTime;

        // update manual rotation if sliders are used
        manualRotation = Quaternion.Euler(xShootDirection, yShootDirection, zShootDirection);
    }

    public void Shoot()
    {
        if (shootingTimer > 0) return;

        // if rotation override is being used, apply it.
        // Otherwise, just use current transform forward
        Quaternion spawnRot = (xShootDirection != 0 || yShootDirection != 0 || zShootDirection != 0)
                                ? manualRotation
                                : bulletInstantiationPosition.rotation;

        Instantiate(bulletPrefab, bulletInstantiationPosition.position, spawnRot);

        if (combatSFXManager != null)
            combatSFXManager.PlayShootSound();

        shootingTimer = shootingCooldown;

        playerAnimator.SetTrigger("Shoot");
    }
}