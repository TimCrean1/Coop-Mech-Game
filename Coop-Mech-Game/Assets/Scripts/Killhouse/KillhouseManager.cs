using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class KillhouseManager : MonoBehaviour
{
    public enum KillhouseStatus
    {
        Playing,
        Complete,
        Waiting
    }
    public KillhouseStatus currentKHStatus;
    private static KillhouseManager _instance = null;
    [SerializeField] private float timer;
    [SerializeField] private float points;
    [SerializeField] private List<float> leaderBoard;
    [SerializeField] private List<KillhouseEnemy> enemiesList;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI pointsText;

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
    void Start()
    {
        DeactivateEnemies();
        currentKHStatus = KillhouseStatus.Waiting;
    }
    public void PopulateEnemiesList(KillhouseEnemy enemy)
    {
        enemiesList.Add(enemy);
    }
    private void ActivateEnemies()
    {
        foreach (KillhouseEnemy enemy in enemiesList)
        {
            if (!enemy.isActive)
            {
                enemy.Activate();
            }
        }
    }
    private void DeactivateEnemies()
    {
        foreach (KillhouseEnemy enemy in enemiesList)
        {
            if (enemy.isActive)
            {
                enemy.Deactivate();
            }
        }
    }
    public void StartTrial()
    {
        Debug.Log("Started Kill House!");
        currentKHStatus = KillhouseStatus.Playing;
        ActivateEnemies();
    }
    public void CancelTrial()
    {
        Debug.Log("Cancelled Kill House!");
        currentKHStatus = KillhouseStatus.Waiting;
        timer = 0;
        DeactivateEnemies();
    }
    public void CompleteTrial()
    {
        currentKHStatus = KillhouseStatus.Complete;
        leaderBoard.Add(timer);
        leaderBoard.Sort((a, b) => a.CompareTo(b));
        Debug.Log("Trial Complete! Your time was: " + timer + ". You got " + points + " points!");
        timer = 0;
        currentKHStatus = KillhouseStatus.Waiting;
        DeactivateEnemies();
    }
    public void UpdatePoints(float newPoints)
    {
        points += newPoints;
        Debug.Log("Adding points: " + newPoints);
        Debug.Log("Points updated! Current points: " + points);
    }
    void Update()
    {
        if (currentKHStatus == KillhouseStatus.Playing)
        {
            timer += Time.deltaTime;
        }
        timerText.text = "Time: " + timer.ToString("F2") + "s";
        pointsText.text = "Points: " + points.ToString("F0");
    }
}
