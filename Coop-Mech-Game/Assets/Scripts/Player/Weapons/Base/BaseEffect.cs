using UnityEngine;
using UnityEngine.VFX;
using System.Collections;

public abstract class BaseEffect : MonoBehaviour
{
    [SerializeField] private VisualEffect effect;
    [SerializeField] private float lifetime = 1f;

    protected virtual void Awake()
    {
        lifetime = effect.GetVector2("Lifetime Rnge").y + 0.1f;
    }

    protected virtual void OnEnable()
    {
        effect.SendEvent("OnEnable");
    }

    public virtual void PrepPlay(Vector3 normal)
    {
        transform.eulerAngles = normal;
    }

    protected virtual IEnumerator LifeRoutine()
    {
        yield return new WaitForSeconds(lifetime);
        gameObject.SetActive(false);
        effect.SendEvent("OnStop");
    }
}
