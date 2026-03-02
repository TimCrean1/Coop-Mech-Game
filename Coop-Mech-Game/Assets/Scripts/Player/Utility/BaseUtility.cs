using System.Collections;
using UnityEngine;

public abstract class BaseUtility : MonoBehaviour
{
    [Header("Object/Component References")]
    [SerializeField] protected UtilityManagerScript utilityManager;
    [Header("Utility Stats")]
    [SerializeField] protected bool canActivateUtility = true;
    [SerializeField] protected float utilityCooldownTime = 5f;
    public abstract void ActivateUtility();
    protected virtual void DeactivateUtility() { }
    protected virtual IEnumerator UtilityCooldown(float cooldownTime)
    {
        canActivateUtility = false;
        yield return new WaitForSeconds(cooldownTime);
        canActivateUtility = true;
    }
    protected abstract bool UtilityConditionsMet();
}
