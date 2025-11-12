using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class AlienInstantEffect : MonoBehaviour
{
    [SerializeField] private VisualEffect effect;
    [SerializeField] private GameObject parent;
    private MeshRenderer mRend;

    private void OnEnable()
    {
        mRend = parent.GetComponent<MeshRenderer>();

        StartCoroutine(InstantDeathRoutine(effect.GetVector2("LifetimeRange").y));
    }

    private IEnumerator InstantDeathRoutine(float waitTime)
    {
        Debug.Log("instant routine called");

        effect.SendEvent("PlayDeathInst");

        mRend.enabled = false;

        gameObject.SetActive(true);

        yield return new WaitForSeconds(waitTime + 0.5f);

        gameObject.SetActive(false);

        if (parent != null)
        {
            SentryAlien sa = parent.GetComponent<SentryAlien>();
            if (sa != null)
            {
                IEnumerator fireRoutine = sa.FireRoutine();
                if (fireRoutine != null)
                {
                    StopCoroutine(fireRoutine);
                }
            }
            Destroy(parent);
        }
        yield return null;
    }
}
