using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.VFX;

public class AlienProjExplosion : MonoBehaviour
{
    [SerializeField] private VisualEffect effect;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DEstroyRoutine());
    }

    private IEnumerator DEstroyRoutine()
    {

        effect.SendEvent("PlayDeathInst");

        yield return new WaitForSeconds(2.5f);

        Destroy(gameObject);
    }
}
