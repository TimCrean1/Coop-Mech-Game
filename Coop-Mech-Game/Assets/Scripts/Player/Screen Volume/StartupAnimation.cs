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

    private CharacterMovement charMovement;

    void Start()
    {
        for(int i = 0; i< refStrings.Count; i++)
        {
            _material.SetFloat(refStrings[i], 0f);
        }

        if (randomizeOrder)
        {
            refStrings.Shuffle(); //using custom ListExtensions class
        }

        //eventually subscribe to an event when loading screen fades out

        GameManager.Instance.OnStartupSequence.AddListener(StartFunction);
        GameManager.Instance.OnRoundEnd.AddListener(StartShutdownRoutine);

        charMovement = GetComponent<CharacterMovement>();
    }

    private void StartFunction()
    {
        StartCoroutine(StartStartupRoutine());
    }

    private IEnumerator StartStartupRoutine() //start each screen
    {
        Debug.Log("StartingUpRoutine");
        for(int i = 0; i< refStrings.Count; i++)
        {
            if (i.IsValidIndex(refStrings))
            {
                StartCoroutine(StartupRoutine(refStrings[i]));
                yield return new WaitForSeconds(interDelay);
            }
            yield return null;
        }
        yield return null;
    }

    private IEnumerator StartupRoutine(string target) //individual screen flicker
    {
        //Debug.Log("Routine start");

        yield return new WaitForSeconds(startDelay);
        float count = 0f;
        float interval = 0.02f; //relative speed of turning on
        float brightness = 0.1f; //start value on x-axis

        while(count <= startTime)
        {
            count += interval; //count acts as minimum value of what can be picked on the x-axis of SlerpValue function
            //Debug.Log(count);

            brightness = Random.Range(SlerpValue(count/startTime), 1f) > chance ? 1f : SlerpValue(count / startTime) * 0.85f + 0.05f;
            
            _material.SetFloat(target, brightness);

            yield return new WaitForSeconds(interval);
        }
        //GetComponent<CharacterMovement>().SetCanMove(true);
        charMovement.SetCanMove(true);
        yield return null;
    }

    private void StartShutdownRoutine()
    {
        Debug.Log("Shutdown routine placeholder");
        
    }

    private IEnumerator ShutdownRoutine(string target)
    {
        float count = startTime;
        float interval = 0.02f;
        float brightness = 1f;

        while(count >= 0f)
        {
            count -= interval;
            brightness = Random.Range(0f, SlerpValue(count / startTime));
            yield return null;
        }
    }

    private float SlerpValue(float toSmooth)
    {
        return (-Mathf.Cos(Mathf.PI * toSmooth) + 1f) / 2f;
    }
    
}
