using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionVFX : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(DestroyRoutine());
    }

    private IEnumerator DestroyRoutine()
    {
        yield return new WaitForSeconds(10f);
    }
}
