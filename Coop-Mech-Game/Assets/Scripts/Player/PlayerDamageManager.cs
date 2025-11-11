using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerDamageManager : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private CapsuleCollider capsuleCollider;
    [SerializeField] private CombatSFXManager combatSFXManager;
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [Header("Collision Variables")]
    [SerializeField] private LayerMask collisionLayer;

    void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & collisionLayer.value) != 0)
        {
            impulseSource.DefaultVelocity = new Vector3(Random.Range(-1f, 1f), impulseSource.DefaultVelocity.y, impulseSource.DefaultVelocity.z);
            impulseSource.GenerateImpulse();
            combatSFXManager.PlayHeavyDamageSound();
        }
    }
}
