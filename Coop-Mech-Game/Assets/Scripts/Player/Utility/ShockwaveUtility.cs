
using UnityEngine;
using Unity.Netcode;


/// <summary>
/// Handles the Shockwave utility, which knocks back enemy players within a radius if the player is airborne.
/// </summary>
public class ShockwaveUtility : BaseUtility
{
    [Header("Shockwave Utility Stats")]
    [SerializeField] private float shockwaveRadius = 5f; // The radius of the shockwave effect
    [SerializeField] private float shockwaveKnockbackForce = 10f; // The force applied to enemies

    [Header("Shockwave Conditions")]
    [SerializeField] private float minDistanceFromGround = 1f; // Minimum distance from ground to activate

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayerMask; // LayerMask for ground detection

    /// <summary>
    /// Activates the shockwave utility, applying knockback to enemies in range if conditions are met.
    /// </summary>
    [Rpc(SendTo.ClientsAndHost)]
    public override void ActivateUtilityRpc()
    {
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

        // Get the owning character's movement component
        CharacterMovement owningCharacter = utilityManager.GetCharacterMovement();

        if (owningCharacter == null)
        {
            Debug.LogError("ShockwaveUtility: Owning character is NULL.");
            return;
        }

        // Force the owning character to return to the ground
        owningCharacter.ReturnToGround();

        // Find all colliders within the shockwave radius on the PlayerExterior layer
        Collider[] hitColliders = Physics.OverlapSphere(
            transform.position,
            shockwaveRadius,
            LayerMask.GetMask("PlayerExterior")
        );

        // Iterate through all hit colliders
        foreach (Collider hitCollider in hitColliders)
        {
            // Ignore self
            if (hitCollider.gameObject == gameObject)
                continue;

            // Determine if the hit object is an enemy based on team tags
            bool isEnemy =
                (CompareTag("TeamOne") && hitCollider.CompareTag("TeamTwo")) ||
                (CompareTag("TeamTwo") && hitCollider.CompareTag("TeamOne"));

            if (!isEnemy)
                continue;

            // Get the CharacterMovement component from the hit object
            CharacterMovement targetMovement =
                hitCollider.GetComponentInParent<CharacterMovement>();

            if (targetMovement != null)
            {
                // Apply knockback to the enemy
                targetMovement.ApplyKnockback(
                    transform.position,
                    shockwaveKnockbackForce
                );
                Debug.Log($"ShockwaveUtility: Applied knockback to {hitCollider.gameObject.name}.");
            }
            else
            {
                Debug.LogWarning($"ShockwaveUtility: No CharacterMovement found on {hitCollider.gameObject.name}.");
            }
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
        bool nearGround = Physics.Raycast(
            transform.position,
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
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, shockwaveRadius);
    }
}