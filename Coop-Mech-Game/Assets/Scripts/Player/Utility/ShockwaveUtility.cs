using UnityEngine;

public class ShockwaveUtility : BaseUtility
{
    [Header("Shockwave Utility Stats")]
    [SerializeField] private float shockwaveRadius = 5f;
    [SerializeField] private float shockwaveDamage = 50f;
    [SerializeField] private float shockwaveKnockbackForce = 10f;
    [Header("Shockwave Conditions")]
    [SerializeField] private float minDistanceFromGround = 3f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (utilityManager == null)
        {
            Debug.LogError("UtilityManager reference is not set in the inspector.");
        }
    }

    public override void ActivateUtilityRpc()
    {
        if (canActivateUtility && UtilityConditionsMet())
        {
            utilityManager.GetCharacterMovement().ReturnToGround();
            // Activate the shockwave utility
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, shockwaveRadius, layerMask: LayerMask.GetMask("PlayerExterior"));
            foreach (Collider hitCollider in hitColliders)
            {
                if (this.gameObject.CompareTag("TeamOne"))
                {
                    if (hitCollider.CompareTag("TeamTwo"))
                    {
                        hitCollider.GetComponent<CharacterMovement>().ApplyKnockback(transform.position, shockwaveKnockbackForce);
                    }
                }
                else if (this.gameObject.CompareTag("TeamTwo"))
                {
                    if (hitCollider.CompareTag("TeamOne"))
                    {
                        hitCollider.GetComponent<CharacterMovement>().ApplyKnockback(transform.position, shockwaveKnockbackForce);
                    }
                }
            }
            StartCoroutine(UtilityCooldown(utilityCooldownTime));
        }
    }

    protected override bool UtilityConditionsMet()
    {
        // Check if the player is on the ground and not too close to it
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo, minDistanceFromGround))
        {
            return false; // Player is too close to the ground, utility cannot be activated
        }
        return true;
    }
}
