using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    private float _selectedDifficultyTiming = 0.25f;
    public float SelectedDifficultyTiming { get { return _selectedDifficultyTiming; } }

    [SerializeField] private List<float> _timings = new List<float>();
    public List<float> TimingsList { get { return _timings; } }

    private float _selectedDamage = 1;

    [SerializeField] private List<float> _damages = new List<float>();
    public List<float> DamagesList { get { return _damages; } }

    private static DifficultyManager _instance = null;

    // Public property to allow access to the Singleton instance
    // A property is a member that provides a flexible mechanism to read, write, or compute the value of a data field.
    public static DifficultyManager Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
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
    }

    public void SetDifficulty(int timingIndex)
    {
        _selectedDifficultyTiming = _timings[timingIndex];
        _selectedDamage = _damages[timingIndex];

    }
}
