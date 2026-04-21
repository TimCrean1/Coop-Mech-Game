using System.Collections;
using Unity.Netcode;
using UnityEngine;

public abstract class BaseUtility : NetworkBehaviour
{
    [Header("Object/Component References")]
    [SerializeField] protected UtilityManagerScript utilityManager;
    [Header("Utility Stats")]
    [SerializeField] protected bool canActivateUtility = true;
    [SerializeField] protected float utilityCooldownTime = 5f;

    [Rpc(SendTo.Server)]
    public virtual void ActivateUtilityRpc() { }
    protected virtual void DeactivateUtility() { }
    // protected virtual IEnumerator UtilityCooldown(float cooldownTime)
    // {
    //     canActivateUtility = false;
    //     yield return new WaitForSeconds(cooldownTime);
    //     canActivateUtility = true;
    // }
    protected abstract bool UtilityConditionsMet();

    public void SetUtilityManager(UtilityManagerScript manager)
    {
        utilityManager = manager;
    }
}
