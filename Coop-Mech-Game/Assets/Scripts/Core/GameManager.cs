
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    #region Variables

    // A cached reference to the GameState
    // Since we'll be using this reference often, we'll cache it instead of always using the static Instance variable
    [SerializeField] private GameState gameState;

    // Static (global) reference to the single existing instance of the object
    private static GameManager _instance = null;

    [SerializeField] private float _playerHealth = 50;
    public int playerScore = 0;
    

    // Public property to allow access to the Singleton instance
    // A property is a member that provides a flexible mechanism to read, write, or compute the value of a data field.
    public static GameManager Instance
    {
        get { return _instance; }
    }

    [SerializeField] private PlayerController _playerController;
    public PlayerController PlayerCharacter { get { return _playerController; } }

    [SerializeField] private PlayerDamageManager _playerDamageManager;

    #endregion

    #region Unity Functions

    private void Awake()
    {
        #region Singleton

        // If an instance of the GameManager does not already exist
        if (_instance == null)
        {
            // Make this object the one that _instance points to
            _instance = this;
        }
        // Otherwise if an instance already exists and it's not this one
        else
        {
            // Destroy this GameManager
            Destroy(gameObject);
        }

        #endregion

        
    }

    private void Start()
    {
        // Cache a reference to the GameState when the game starts
        // Start is called after Awake (where GameState.Instance is initialized) so the Instance should exist by now
        gameState = GameState.Instance;

        // Resume the game so we don't start paused when the game loads a scene
        ResumeGame();
    }

    

    #endregion

    #region Custom Functions

    // This function is called by some external script in order to set the game state to paused.
    public void PauseGame()
    {
        // Update the Game's Status in the GameState
        bool didPause = gameState.UpdateGameStatus(GameStatus.Paused);

        // If we successfully paused the game
        if (didPause)
        {
            // Here we will set the TimeScale of our game to 0. This means that anything based on time (including
            // physics) will no longer occur until we reset the TimeScale to 1.
            Time.timeScale = 0f;
        }
    }

    public void ResumeGame()
    {
        // Update the Game's Status in the GameState
        bool didResume = gameState.UpdateGameStatus(GameStatus.Playing);

        // If we successfully resumed the game
        if (didResume)
        {
            // Here we will set the TimeScale of our game back to 1. This means that anything that is based off time (including physics) will once again occur.
            Time.timeScale = 1f;
        }
    }

    // If the game is paused, resume it, otherwise pause it
    public void TogglePause()
    {
        if (gameState.IsPaused) { ResumeGame(); }
        else { PauseGame(); }
    }

    public void PlayerWon()
    {
        // Update the Game's Status in the GameState
        gameState.UpdateGameStatus(GameStatus.PlayerWon);
        Debug.Log("Player won!");
    }

    public void PlayerLost()
    {
        // Update the Game's Status in the GameState
        gameState.UpdateGameStatus(GameStatus.PlayerLost);
        Debug.Log("Player lost!");

    }

    public void DamagePlayer(float damage)
    {
        _playerHealth = _playerHealth - damage;
        _playerDamageManager.DamageTaken();

        if (_playerHealth <= 0)
        {
            Application.Quit();
        }
    }

    #endregion
}
