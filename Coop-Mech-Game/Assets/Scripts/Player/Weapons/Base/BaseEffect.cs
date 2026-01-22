using UnityEngine;
using UnityEngine.VFX;
using System.Collections;

public abstract class BaseEffect : MonoBehaviour
{
    [SerializeField] private VisualEffect effect;
    [SerializeField] private float lifetime = 5f;

    protected virtual void OnEnable()
    {
        effect.SendEvent("OnEnable");
    }

    protected virtual IEnumerator LifeRoutine()
    {
        yield return new WaitForSeconds(lifetime);
        gameObject.SetActive(false);
    }
}
