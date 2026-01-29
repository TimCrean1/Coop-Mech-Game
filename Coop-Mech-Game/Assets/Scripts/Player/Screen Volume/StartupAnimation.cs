using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class StartupAnimation : MonoBehaviour
{

    [SerializeField] private Material _material;
    [SerializeField] private float startDelay = 2f;
    [SerializeField] private float startTime = 2f;
    [SerializeField] private float chance = 0.85f;
    [SerializeField] private float interDelay = 0.1f;
    [SerializeField] private bool randomizeOrder = false;
    [SerializeField] private List<string> refStrings = new List<string>();

    void Start()
    {
        for(int i = 0; i< refStrings.Count; i++)
        {
            _material.SetFloat(refStrings[i], 0f);
        }

        if (randomizeOrder)
        {
            ListExtensions.Shuffle(refStrings);
        }

        //_material.SetFloat("_RegionCenter", 0f);
        //_material.SetFloat("_RegionTop", 0f);
        //_material.SetFloat("_RegionBottom", 0f);
        //_material.SetFloat("_RegionLeft", 0f);
        //_material.SetFloat("_RegionRight", 0f);

        //eventually subscribe to an event when loading screen fades out
        StartCoroutine(StartStartupRoutine());
    }

    private void StartFunction()
    {
        //this is to be called if we end up using events to start the effect
    }

    private IEnumerator StartStartupRoutine()
    {
        for(int i = 0; i< refStrings.Count; i++)
        {
            StartCoroutine(StartupRoutine(refStrings[i]));
            yield return new WaitForSeconds(interDelay);
        }

        yield return null;

        //StartCoroutine(StartupRoutine("_RegionCenter"));
        //yield return new WaitForSeconds(interDelay);

        //StartCoroutine(StartupRoutine("_RegionTop"));
        //yield return new WaitForSeconds(interDelay);

        //StartCoroutine(StartupRoutine("_RegionBottom"));
        //yield return new WaitForSeconds(interDelay);

        //StartCoroutine(StartupRoutine("_RegionLeft"));
        //yield return new WaitForSeconds(interDelay);

        //StartCoroutine(StartupRoutine("_RegionRight"));
    }

    private IEnumerator StartupRoutine(string target)
    {
        Debug.Log("Routine start");

        yield return new WaitForSeconds(startDelay);
        float count = 0f;
        float interval = 0.02f;
        float brightness = 0.1f;

        while(count <= startTime)
        {
            count += interval;
            Debug.Log(count);

            brightness = Random.Range(SlerpValue(count/startTime), 1f) > chance ? 1f : SlerpValue(count / startTime) * 0.85f + 0.05f;
            
            _material.SetFloat(target, brightness);

            yield return new WaitForSeconds(interval);
        }

        yield return null;
    }

    private float SlerpValue(float toSmooth)
    {
        return (-Mathf.Cos(Mathf.PI * toSmooth) + 1f) / 2f;
    }
    
}
