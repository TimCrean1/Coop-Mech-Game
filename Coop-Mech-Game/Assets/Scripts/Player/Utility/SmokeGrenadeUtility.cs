using Unity.Netcode;
using UnityEngine;
using UnityEngine.VFX;

public class SmokeGrenadeUtility : BaseUtility
{
    [Header("Smoke Settings")]
    [SerializeField] private VisualEffect visualEffect;
    [SerializeField] private float smokeCooldown = 20f;
    [SerializeField] private float castDistance = 10f;
    [Range(0f, 1f)]
    [SerializeField] private float nandoChance = 0.05f;

    private RaycastHit hit;

    private CharacterMovement owningCharacter;
    private bool isInitialized;

    #region Initialization

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        TryInitialize();
    }

    private void TryInitialize()
    {
        if (utilityManager == null)
        {
            Debug.LogWarning($"[{name}] UtilityManager not set yet. Waiting for injection.");
            return;
        }

        owningCharacter = utilityManager.GetCharacterMovement();
        isInitialized = owningCharacter != null;
    }

    public void SetOwningCharacter(CharacterMovement character)
    {
        owningCharacter = character;
        isInitialized = character != null;
    }

    #endregion

    #region Activation

    public override void ActivateUtilityRpc()
    {
        if (!isInitialized)
        {
            TryInitialize();

            if (!isInitialized)
            {
                Debug.LogWarning($"[{name}] SmokeGrenade not initialized properly.");
                return;
            }
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
        float rand = Random.value;

        if (visualEffect == null)
        {
            Debug.LogWarning("VisualEffect missing on SmokeGrenadeUtility.");
            return;
        }

        if (rand <= nandoChance)
        {
            visualEffect.SendEvent("OnNando");
        }
        else
        {
            visualEffect.SendEvent("OnFire");
        }
    }

    #endregion

    #region Conditions

    protected override bool UtilityConditionsMet()
    {
        return canActivateUtility && owningCharacter != null;
    }

    #endregion
}