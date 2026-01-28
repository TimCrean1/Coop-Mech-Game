using UnityEngine;
using System.Collections;

public class StartupAnimation : MonoBehaviour
{

    [SerializeField] private Material _material;
    [SerializeField] private float startDelay = 2f;
    [SerializeField] private float startTime = 1f;
    private float count = 0f;

    void Start()
    {
        _material.SetFloat("_RegionCenter", 0f);
        _material.SetFloat("_RegionTop", 0f);
        _material.SetFloat("_RegionBottom", 0f);
        _material.SetFloat("_RegionLeft", 0f);
        _material.SetFloat("_RegionRight", 0f);

        //eventually subscribe to an event when loading screen fades out
        StartCoroutine(StartupRoutine());
    }

    private IEnumerator StartupRoutine()
    {
        Debug.Log("Routine start");

        yield return new WaitForSeconds(startDelay);

        while(count <= startTime)
        {
            count += Time.deltaTime;
            Debug.Log(count);

            _material.SetFloat("_RegionCenter", count);
            _material.SetFloat("_RegionTop", count);
            _material.SetFloat("_RegionBottom", count);
            _material.SetFloat("_RegionLeft",count);
            _material.SetFloat("_RegionRight", count);

            yield return null;
        }

        yield return null;
    }

    
}
