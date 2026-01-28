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
        _material.SetFloat("RegionCenter", 0f);
        _material.SetFloat("RegionTop", 0f);
        _material.SetFloat("RegionBottom", 0f);
        _material.SetFloat("RegionLeft", 0f);
        _material.SetFloat("RegionRight", 0f);

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

            _material.SetFloat("RegionCenter", count);
            _material.SetFloat("RegionTop", count);
            _material.SetFloat("RegionBottom", count);
            _material.SetFloat("RegionLeft",count);
            _material.SetFloat("RegionRight", count);

            yield return null;
        }

        yield return null;
    }

    
}
