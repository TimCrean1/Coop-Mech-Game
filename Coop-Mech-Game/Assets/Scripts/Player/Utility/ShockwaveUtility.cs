
using UnityEngine;
using Unity.Netcode;


/// <summary>
/// Handles the Shockwave utility, which knocks back enemy players within a radius if the player is airborne.
/// </summary>
public class ShockwaveUtility : BaseUtility
{
    [Header("Shockwave Utility Stats")]
    [SerializeField] private float shockwaveRadius = 100f; // The radius of the shockwave effect
    [SerializeField] private float shockwaveKnockbackForce = 50f; // The force applied to enemies

    [Header("Shockwave Conditions")]
    [SerializeField] private float minDistanceFromGround = 1f; // Minimum distance from ground to activate

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayerMask; // LayerMask for ground detection
    [SerializeField] private CharacterMovement owningCharacter; // Reference to the owning character's movement component
    
    private void FixedUpdate()
    {
        // Only update position if owningCharacter is assigned
        if (owningCharacter != null)
        {
            gameObject.transform.position = owningCharacter.transform.position;
        }
    }

    /// <summary>
    /// Activates the shockwave utility, applying knockback to enemies in range if conditions are met.
    /// </summary>
    [Rpc(SendTo.ClientsAndHost)]
    public override void ActivateUtilityRpc()
    {
        owningCharacter = utilityManager.GetCharacterMovement();

        // Check if the utility can be activated
        if (!canActivateUtility)
        {
            Debug.LogWarning("ShockwaveUtility: Cannot activate utility - canActivateUtility is false.");
            return;
        }

        // Check if utility conditions are met (e.g., player is airborne)
        if (!UtilityConditionsMet())
        {
            Debug.LogWarning("ShockwaveUtility: Utility conditions not met - player may be too close to the ground.");
            return;
        }

        if (owningCharacter == null)
        {
            Debug.LogError("ShockwaveUtility: Owning character is NULL.");
            return;
        }

        // Force the owning character to return to the ground
        owningCharacter.ReturnToGround();

        // Find all colliders within the shockwave radius on the PlayerExterior layer
        Collider[] hitColliders = Physics.OverlapSphere(
            owningCharacter.transform.position,
            shockwaveRadius,
            LayerMask.GetMask("PlayerExterior")
        );

        if (hitColliders.Length == 0)
        {
            Debug.Log("ShockwaveUtility: No player related objects hit by shockwave.");
        }
        else
        {
            Debug.Log($"ShockwaveUtility: {hitColliders.Length} player related object(s) hit by shockwave.");
        }

        // Iterate through all hit colliders
        foreach (Collider hitCollider in hitColliders)
        {
            CharacterMovement targetMovement =
                hitCollider.GetComponentInParent<CharacterMovement>();

            // Ignore self
            if (targetMovement == owningCharacter)
                continue;

            // Skip invalid targets
            if (targetMovement == null)
                continue;

            // Determine if the hit object is an enemy based on team tags
            bool isEnemy =
                (owningCharacter.CompareTag("TeamOne") && targetMovement.CompareTag("TeamTwo")) ||
                (owningCharacter.CompareTag("TeamTwo") && targetMovement.CompareTag("TeamOne"));

            if (!isEnemy)
                continue;

            // Apply knockback to the enemy
            targetMovement.ApplyKnockback(
                owningCharacter.transform.position,
                shockwaveKnockbackForce
            );

            Debug.Log($"ShockwaveUtility: Applied knockback to {targetMovement.gameObject.name}.");
        }

        // Play the utility sound effect
        PlayUtilitySound();

        // Uncomment if you want to start the cooldown coroutine
        // StartCoroutine(UtilityCooldown(utilityCooldownTime));
    }

    /// <summary>
    /// Checks if the utility can be activated (e.g., player is not too close to the ground).
    /// </summary>
    /// <returns>True if conditions are met, false otherwise.</returns>
    protected override bool UtilityConditionsMet()
    {
        // Raycast down to check if the player is near the ground
        if (owningCharacter == null)
        {
            return false;
        }

        bool nearGround = Physics.Raycast(
            owningCharacter.transform.position,
            Vector3.down,
            minDistanceFromGround,
            groundLayerMask
        );

        if (nearGround)
        {
            Debug.Log("ShockwaveUtility: Player is too close to the ground to activate shockwave.");
        }

        // Return true if not near the ground
        return !nearGround;
    }

    /// <summary>
    /// Draws the shockwave radius in the editor for visualization.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (owningCharacter != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(owningCharacter.transform.position, shockwaveRadius);
        }
    }
}