using Unity.Netcode;
using UnityEngine;
using UnityEngine.VFX;

public class SmokeGrenadeUtility : BaseUtility
{
    [Header("Smoke Settings")]
    [SerializeField] private VisualEffect visualEffect;
    [SerializeField] private float castDistance = 10f;
    [Range(0f, 1f)]
    [SerializeField] private float nandoChance = 0.05f;

    private CharacterMovement owningCharacter;
    private RaycastHit hit;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        TryInitialize();
    }

    private void TryInitialize()
    {
        // Try UtilityManager first (preferred)
        if (utilityManager != null)
        {
            owningCharacter = utilityManager.GetCharacterMovement();
        }

        // Fallback (VERY important for safety)
        if (owningCharacter == null)
        {
            owningCharacter = GetComponentInParent<CharacterMovement>();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public override void ActivateUtilityRpc()
    {
        TryInitialize();

        if (owningCharacter == null)
        {
            Debug.LogError("SmokeGrenadeUtility: owningCharacter is NULL.");
            return;
        }

        if (!UtilityConditionsMet())
            return;

        Vector3 origin = owningCharacter.transform.position;
        Vector3 forward = owningCharacter.transform.forward;

        Vector3 targetPos = Physics.Raycast(origin, forward, out hit, castDistance)
            ? hit.point
            : origin + forward * castDistance;

        transform.position = targetPos;

        PlayVFX();
    }

    private void PlayVFX()
    {
        if (visualEffect == null)
        {
            Debug.LogWarning("VisualEffect missing on SmokeGrenadeUtility.");
            return;
        }

        float rand = Random.value;

        if (rand <= nandoChance)
            visualEffect.SendEvent("OnNando");
        else
            visualEffect.SendEvent("OnFire");
    }

    protected override bool UtilityConditionsMet()
    {
        return canActivateUtility;
    }
}