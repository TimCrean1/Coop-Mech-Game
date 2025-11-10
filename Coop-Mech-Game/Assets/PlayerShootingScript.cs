using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShootingScript : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject bulletInstantiationPosition;
    [SerializeField] private CombatSFXManager combatSFXManager;
    
    [Header("Shooting Variables")]
    [SerializeField][Range(0, 10f)] private float shootingCooldown;
    private float shootingTimer;
    [SerializeField][Range(0, 360)] private float xShootDirection;
    [SerializeField][Range(0, 360)] private float yShootDirection;
    [SerializeField][Range(0, 360)] private float zShootDirection;
    private Quaternion rotation;

    private void FixedUpdate()
    {
        rotation = Quaternion.Euler(xShootDirection, yShootDirection, zShootDirection);
        if (shootingTimer > 0)
        {
            shootingTimer -= Time.fixedDeltaTime;
        }
    }

    public void Shoot()
    {
        if (shootingTimer > 0) return;
        Instantiate(bulletPrefab, bulletInstantiationPosition.transform.position, rotation);
        combatSFXManager.PlayShootSound();
        shootingTimer = shootingCooldown;
    }
}