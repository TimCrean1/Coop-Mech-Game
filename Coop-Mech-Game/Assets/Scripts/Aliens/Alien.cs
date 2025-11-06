using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Alien : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRend;
    [SerializeField] private GameObject gradualEffect;
    [SerializeField] private GameObject instantEffect;
    [Tooltip("X and Z set range start and stop, Y sets midpoint for effect selection, all should be non-negative")]
    [SerializeField] private Vector3 effectRangeAndMid;

    private float rand;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile"))
        {
            Debug.Log("projectile trigger entered by alien");

            StartCoroutine(DistRoutine(other));
        }
    }

    private IEnumerator DistRoutine(Collider other)
    {
        yield return null;

        rand = Random.Range(effectRangeAndMid.x, effectRangeAndMid.z);

        //gradualEffect.SetActive(true);


        if (rand <= effectRangeAndMid.y)
        {
            instantEffect.SetActive(true);
            yield return null;
        }
        else if(rand > effectRangeAndMid.y)
        {
            gradualEffect.SetActive(true);
            yield return null;
        }

        yield return null;

    }
}
