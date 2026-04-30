using Unity.Netcode;
using UnityEngine;

public abstract class BaseUtility : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] protected UtilityManagerScript utilityManager;
    public AudioClip utilityAudioClip;

    [Header("Stats")]
    [SerializeField] protected bool canActivateUtility = true;
    [SerializeField] protected float utilityCooldownTime = 5f;

    [Rpc(SendTo.Server)]
    public virtual void ActivateUtilityRpc() { }

    protected abstract bool UtilityConditionsMet();

    public void SetUtilityManager(UtilityManagerScript manager)
    {
        utilityManager = manager;
    }
}