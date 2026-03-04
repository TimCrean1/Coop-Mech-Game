
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    #region Variables

    // A cached reference to the GameState
    // Since we'll be using this reference often, we'll cache it instead of always using the static Instance variable
    [SerializeField] private GameState gameState;

    // Static (global) reference to the single existing instance of the object
    private static GameManager _instance = null;

    [Tooltip("The max health of the mech, is not edited during the game")]
    [SerializeField] private float teamOneMaxHealth = 5000f;
    [SerializeField] private float teamTwoMaxHealth = 5000f;
    
    [SerializeField] public List<PlayerController> _playerControllers; // this needs to be synced on the server
    //private NetworkList<PlayerController> _playerControllers;
    //private NetworkVariable<int> playerMechIndex = new NetworkVariable<int>();
    // team -> clientId
    [SerializeField] public Transform teamOneSpawnPoint;
    [SerializeField] public Transform teamTwoSpawnPoint;

    [SerializeField] private GameObject MechOne;
    [SerializeField] private GameObject MechTwo;

    [SerializeField] private MechScreen t1HealthScreen;
    [SerializeField] private MechScreen t2HealthScreen;

    //this is the health of the mech, edited at runtime
    public NetworkVariable<float> _teamOneHealth = new NetworkVariable<float>();
    public NetworkVariable<float> _teamTwoHealth = new NetworkVariable<float>();

    //private int currentRound = 0;
    NetworkVariable<int> currRound = new NetworkVariable<int>();
    private bool roundOver = false;
    private int maxRounds = 3;
    private int lobbyMaxPlayers;
    public int playerScore = 0;
    public UnityEvent OnStartupSequence; //Invoke when all clients are connected to scene
    public UnityEvent OnRoundEnd;
    //public event Action<bool> RoundEnd = null;

    // Public property to allow access to the Singleton instance
    // A property is a member that provides a flexible mechanism to read, write, or compute the value of a data field.
    public static GameManager Instance
    {
        get { return _instance; }
    }

    //[SerializeField] private PlayerController _playerController;
    //public PlayerController PlayerCharacter { get { return _playerController; } }

    //[SerializeField] private PlayerDamageManager _playerDamageManager;

    #endregion

    #region Unity Functions
    public override void OnNetworkSpawn()
    {
        InitTeamHealthRpc();

    }
    private void Awake()
    {
        if (NetworkManager.Singleton != null) {
            NetworkManager.Singleton.OnClientConnectedCallback += WaitForConnectedPlayers;
        }
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
        OnRoundEnd.AddListener(OnRoundEndTriggered);

    }

    private void Start()
    {
        // Cache a reference to the GameState when the game starts
        // Start is called after Awake (where GameState.Instance is initialized) so the Instance should exist by now
        gameState = GameState.Instance;
        Cursor.visible = true;
       // _playerController = new List<PlayerController>();
        // Resume the game so we don't start paused when the game loads a scene
        ResumeGame();

    }


    #endregion

    #region Custom Functions



    #region Game Start Functions
    private void WaitForConnectedPlayers(ulong clientId)
    {
        lobbyMaxPlayers = BootstrapScript.Instance.maxPlayers;

        if (NetworkManager.Singleton.ConnectedClients.Count >= lobbyMaxPlayers)
        {
            Debug.Log("GameManagerStartupSequence");
            //invoke start event here
            if (IsOwner)
            {
                StartCoroutine(StartTimeDelay());
            }

        }

    }
    // This function is called by some external script in order to set the game state to paused.
    [Rpc(SendTo.Server)]
    private void ResetPlayerPositionRpc()
    {
        MechOne.transform.position = teamOneSpawnPoint.transform.position;
        MechTwo.transform.position = teamTwoSpawnPoint.transform.position;
    }
    private IEnumerator StartTimeDelay()
    {
        yield return new WaitForSeconds(3f);
        StartGameRpc();
        OnStartupSequence?.Invoke();
    }
    [Rpc(SendTo.NotServer)]
    private void OnGameEndRpc()
    {
        StartCoroutine(EndTimeDelay());
    }
    private IEnumerator EndTimeDelay()
    {
        //MatchOverRpc();

        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(0);
        
    }
    [Rpc(SendTo.NotServer)]
    private void StartGameRpc()
    {
        OnStartupSequence?.Invoke();
    }

    #endregion

    void OnRoundEndTriggered()
    {
        roundOver = true;
        if (currRound.Value >= maxRounds)
        {
            StartCoroutine(EndTimeDelay());
            OnGameEndRpc();
            if (IsServer)
            {
                NetworkManager.Singleton.Shutdown(true);
            }
            return;
        }
        // when the round ends, do things here
        if (IsServer)
        {
            currRound.Value += 1;
        }
        StartCoroutine(RoundEndCoroutine());
        
        
    }
    
    private void SetTimeScale(float newTimeScale)
    {
        Time.timeScale = newTimeScale;
    }

    [Rpc(SendTo.NotServer)]
    private void SetTimeScaleClientRpc(float newTimeScale)
    {
        Time.timeScale = newTimeScale;
    }
    
    private IEnumerator RoundEndCoroutine()
    {
        SetTimeScale(0.5f);
        SetTimeScaleClientRpc(0.5f);
        yield return new WaitForSeconds(3f);
        SetTimeScale(1f);
        SetTimeScaleClientRpc(1f);
        ResetPlayerPositionRpc();
        InitTeamHealthRpc();
        roundOver = false;
    }
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
    public void AddController(PlayerController ctrl)
    {
        _playerControllers.Add(ctrl);
        Debug.Log("added controller" + ctrl);
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

    public int SetPlayerIndex()
    {
        return NetworkManager.Singleton.ConnectedClients.Count;
    }

    [Rpc(SendTo.Server)]
    private void InitTeamHealthRpc()
    {
        _teamOneHealth.Value = teamOneMaxHealth;
        _teamTwoHealth.Value = teamTwoMaxHealth;
        if(t1HealthScreen != null)
        {
            //t1HealthScreen.ChangeText(((_teamOneHealth.Value / teamOneMaxHealth) * 100f).ToString(), false);
            Changet1HealthTextClientRpc(teamOneMaxHealth, _teamOneHealth.Value);
        }
        if (t2HealthScreen != null)
        {
            Changet2HealthTextClientRpc(teamTwoMaxHealth, _teamTwoHealth.Value);
            //t2HealthScreen.ChangeText(((_teamTwoHealth.Value / teamTwoMaxHealth) * 100f).ToString(), false);
        }

        
    }

    [Rpc(SendTo.Server)]
    public void DamageTeamRpc(int teamNumToDamage, float damage)
    {
        if(teamNumToDamage == 1)
        {
            _teamOneHealth.Value = _teamOneHealth.Value - damage;
            Debug.Log("Damaging Team: " +  teamNumToDamage + " by: " + damage + " damage to new health: " + _teamOneHealth.Value);
            if(t1HealthScreen != null)
            {
                //t1HealthScreen.ChangeText(((_teamOneHealth.Value / teamOneMaxHealth) * 100f).ToString(), false);
                Changet1HealthTextClientRpc(teamOneMaxHealth, _teamOneHealth.Value);
            }
            else
            {
                Debug.LogWarning("t1 health screen not set in GM");
            }
        }
        else if(teamNumToDamage == 2)
        {
            _teamTwoHealth.Value = _teamTwoHealth.Value - damage;
            Debug.Log("Damaging Team: " + teamNumToDamage + " by: " + damage + " damage to new health: " + _teamTwoHealth.Value);
            if(t2HealthScreen != null)
            {
                //t2HealthScreen.ChangeText(((_teamTwoHealth.Value / teamTwoMaxHealth) * 100f).ToString(), false);
                Changet2HealthTextClientRpc(teamTwoMaxHealth, _teamTwoHealth.Value);
            }
            else
            {
                Debug.LogWarning("t2 health screen not set in GM");
            }

        }

        if (_teamOneHealth.Value <= 0f && roundOver == false)
        {
            OnRoundEnd?.Invoke();
            //MatchOverRpc();
        }
        else if(_teamTwoHealth.Value <= 0f && roundOver == false)
        {

            OnRoundEnd?.Invoke();
            //MatchOverRpc();
        }
    }

    [Rpc(SendTo.Server)]
    public void HealTeamRpc(int teamNumToHeal, float healAmt)
    {
        
        if (teamNumToHeal == 1)
        {
            float h = teamOneMaxHealth - _teamOneHealth.Value;

            if(h > healAmt)
            {
                _teamOneHealth.Value = _teamOneHealth.Value + healAmt;
            }
            else{
                _teamOneHealth.Value = _teamOneHealth.Value + h;
            }

            
            Debug.Log("Healing Team: " + teamNumToHeal + " by: " + healAmt + " health to new health: " + _teamOneHealth.Value);
            if (t1HealthScreen != null)
            {
                //t1HealthScreen.ChangeText(((_teamOneHealth.Value / teamOneMaxHealth) * 100f).ToString(), false);
                Changet1HealthTextClientRpc(teamOneMaxHealth, _teamOneHealth.Value);
            }
            else
            {
                Debug.LogWarning("t1 health screen not set in GM");
            }

        }
        else if (teamNumToHeal == 2)
        {
            float h = teamTwoMaxHealth - _teamTwoHealth.Value;

            if (h > healAmt)
            {
                _teamTwoHealth.Value = _teamTwoHealth.Value + healAmt;
            }
            else
            {
                _teamTwoHealth.Value = _teamTwoHealth.Value + h;
            }

            _teamTwoHealth.Value = _teamTwoHealth.Value + healAmt;
            Debug.Log("Healing Team: " + teamNumToHeal + " by: " + healAmt + " health to new health: " + _teamTwoHealth.Value);
            if (t2HealthScreen != null)
            {
                //t2HealthScreen.ChangeText(((_teamTwoHealth.Value / teamTwoMaxHealth) * 100f).ToString(), false);
                Changet2HealthTextClientRpc(teamTwoMaxHealth, _teamTwoHealth.Value);
            }
            else
            {
                Debug.LogWarning("t2 health screen not set in GM");
            }

        }
    }

    [ClientRpc]
    private void Changet1HealthTextClientRpc(float MechMaxHealth, float MechCurrHealth)
    {
        t1HealthScreen.ChangeText(((MechCurrHealth / MechMaxHealth) * 100f).ToString(), false);
    }
    [ClientRpc]
    private void Changet2HealthTextClientRpc(float MechMaxHealth, float MechCurrHealth)
    {
        t2HealthScreen.ChangeText(((MechCurrHealth / MechMaxHealth) * 100f).ToString(), false);
    }

    [Rpc(SendTo.Server)]
    private void MatchOverRpc()
    {
        
        Debug.Log("Match Over!");
        Time.timeScale = 0f;
    }

    #endregion
}
