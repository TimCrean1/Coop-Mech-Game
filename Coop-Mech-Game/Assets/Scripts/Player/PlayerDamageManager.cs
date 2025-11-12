using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerDamageManager : MonoBehaviour
{
    [Header("Object References")]
    //[SerializeField] private Collider notcapsuleCollider;
    [SerializeField] private CombatSFXManager combatSFXManager;
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [Header("Collision Variables")]
    [SerializeField] private LayerMask collisionLayer;

    public void DamageTaken()
    {
        impulseSource.DefaultVelocity = new Vector3(Random.Range(-1f, 1f), impulseSource.DefaultVelocity.y, impulseSource.DefaultVelocity.z);
        impulseSource.GenerateImpulse();
        combatSFXManager.PlayHeavyDamageSound();
    }
}
