using UnityEngine;
using Unity.Netcode;

public class ShockwaveUtility : BaseUtility
{
    [Header("Shockwave Utility Stats")]
    [SerializeField] private float shockwaveRadius = 5f;
    [SerializeField] private float shockwaveKnockbackForce = 10f;

    [Header("Shockwave Conditions")]
    [SerializeField] private float minDistanceFromGround = 1f;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayerMask;

    [Rpc(SendTo.ClientsAndHost)]
    public override void ActivateUtilityRpc()
    {
        if (!canActivateUtility)
            return;

        if (!UtilityConditionsMet())
            return;

        CharacterMovement owningCharacter = utilityManager.GetCharacterMovement();

        if (owningCharacter == null)
        {
            Debug.LogError("ShockwaveUtility: Owning character is NULL.");
            return;
        }

        owningCharacter.ReturnToGround();

        Collider[] hitColliders = Physics.OverlapSphere(
            transform.position,
            shockwaveRadius,
            LayerMask.GetMask("PlayerExterior")
        );

        foreach (Collider hitCollider in hitColliders)
        {
            // Ignore self
            if (hitCollider.gameObject == gameObject)
                continue;

            bool isEnemy =
                (CompareTag("TeamOne") && hitCollider.CompareTag("TeamTwo")) ||
                (CompareTag("TeamTwo") && hitCollider.CompareTag("TeamOne"));

            if (!isEnemy)
                continue;

            CharacterMovement targetMovement =
                hitCollider.GetComponentInParent<CharacterMovement>();

            if (targetMovement != null)
            {
                targetMovement.ApplyKnockback(
                    transform.position,
                    shockwaveKnockbackForce
                );
            }
        }

        PlayUtilitySound();

        // StartCoroutine(UtilityCooldown(utilityCooldownTime));
    }

    protected override bool UtilityConditionsMet()
    {
        bool nearGround = Physics.Raycast(
            transform.position,
            Vector3.down,
            minDistanceFromGround,
            groundLayerMask
        );

        return !nearGround;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, shockwaveRadius);
    }
}