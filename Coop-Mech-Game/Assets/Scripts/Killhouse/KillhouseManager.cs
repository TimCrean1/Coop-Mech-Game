using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KillhouseManager : MonoBehaviour
{
    public enum KillhouseStatus
    {
        Playing,
        Complete,
        Waiting
    }
    public KillhouseStatus currentKHStatus {get; private set;}
    private static KillhouseManager _instance = null;
    [SerializeField] private float timer;
    [SerializeField] private float points;
    [SerializeField] private List<float> leaderBoard;

    // Public property to allow access to the Singleton instance
    // A property is a member that provides a flexible mechanism to read, write, or compute the value of a data field.
    public static KillhouseManager Instance
    {
        get { return _instance; }
    }
    private void Awake()
    {
        #region Singleton

        // If an instance of the object does not already exist
        if (_instance == null)
        {
            // Make this object the one that _instance points to
            _instance = this;

            // We want this object to persist between scenes, so don't destroy it on load
            DontDestroyOnLoad(gameObject);
        }
        // Otherwise if an instance already exists and it's not this one
        else
        {
            // Destroy this object
            Destroy(gameObject);
        }

        #endregion

        currentKHStatus = KillhouseStatus.Waiting;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void StartTrial()
    {
        Debug.Log("Started Kill House!");
        currentKHStatus = KillhouseStatus.Playing;
    }
    public void CancelTrial()
    {
        Debug.Log("Cancelled Kill House!");
        currentKHStatus = KillhouseStatus.Waiting;
        timer = 0;
    }
    public void CompleteTrial()
    {
        currentKHStatus = KillhouseStatus.Complete;
        leaderBoard.Add(timer);
        leaderBoard.Sort((a, b) => a.CompareTo(b));
        Debug.Log("Trial Complete! Your time was: " + timer);
        timer = 0;
        currentKHStatus = KillhouseStatus.Waiting;
    }
    void Update()
    {
        if (currentKHStatus == KillhouseStatus.Playing)
        {
            timer += Time.deltaTime;
        }
    }
}
