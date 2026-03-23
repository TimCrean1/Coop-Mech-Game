using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    #region Singleton

    private static GameManager _instance = null;

    public static GameManager Instance
    {
        get { return _instance; }
    }

    #endregion


    #region Serialized Fields

    [SerializeField] private GameState gameState;

    [Tooltip("The max health of the mech, is not edited during the game")]
    [SerializeField] private float teamOneMaxHealth = 5000f;
    [SerializeField] private float teamTwoMaxHealth = 5000f;

    [SerializeField] public List<PlayerController> _playerControllers;

    [SerializeField] public Transform teamOneSpawnPoint;
    [SerializeField] public Transform teamTwoSpawnPoint;

    [SerializeField] private GameObject MechOne;
    [SerializeField] private GameObject MechTwo;

    [SerializeField] private MechScreen t1HealthScreen;
    [SerializeField] private MechScreen t2HealthScreen;

    [SerializeField] private UI_Manager t1UIMgr;
    [SerializeField] private UI_Manager t2UIMgr;

    #endregion


    #region Network Variables

    public NetworkVariable<float> _teamOneHealth = new NetworkVariable<float>();
    public NetworkVariable<float> _teamTwoHealth = new NetworkVariable<float>();

    NetworkVariable<int> currRound = new NetworkVariable<int>();

    #endregion


    #region Private Runtime State

    private bool roundOver = false;
    private int maxRounds = 3;
    private int lobbyMaxPlayers;

    public int playerScore = 0;

    #endregion


    #region Events

    public UnityEvent OnStartupSequence;
    public UnityEvent OnRoundEnd;
    public UnityEvent OnBuyRoundStart;
    

    #endregion


    #region Unity Lifecycle

    public override void OnNetworkSpawn()
    {
        InitTeamHealthRpc();
    }

    private void Awake()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += WaitForConnectedPlayers;
        }

        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        OnRoundEnd.AddListener(OnRoundEndTriggered);
    }

    private void Start()
    {
        gameState = GameState.Instance;

        Cursor.visible = true;

        ResumeGame();
    }

    #endregion


    #region Game Start / Connection

    private void WaitForConnectedPlayers(ulong clientId)
    {
        lobbyMaxPlayers = BootstrapScript.Instance.maxPlayers;

        if (NetworkManager.Singleton.ConnectedClients.Count >= lobbyMaxPlayers)
        {
            Debug.Log("GameManagerStartupSequence");

            if (IsOwner)
            {
                StartCoroutine(StartTimeDelay());
            }
        }
    }

    private IEnumerator StartTimeDelay()
    {
        yield return new WaitForSeconds(3f);

        StartGameRpc();
        OnStartupSequence?.Invoke();
    }

    [Rpc(SendTo.NotServer)]
    private void StartGameRpc()
    {
        OnStartupSequence?.Invoke();
    }

    #endregion


    #region Round Management

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

        if (IsServer)
        {
            currRound.Value += 1;
        }

        StartCoroutine(RoundEndCoroutine());
    }

    private IEnumerator RoundEndCoroutine()
    {
        SetTimeScale(0.5f);
        SetTimeScaleClientRpc(0.5f);

        yield return new WaitForSeconds(3f);

        SetTimeScale(1f);
        SetTimeScaleClientRpc(1f);

        // DisablePlayerMovement();
        ResetPlayerPositionRpc();
        InitTeamHealthRpc();

        OnBuyRoundStart.Invoke();

        roundOver = false;
    }

    #endregion


    #region Game End

    [Rpc(SendTo.NotServer)]
    private void OnGameEndRpc()
    {
        StartCoroutine(EndTimeDelay());
    }

    private IEnumerator EndTimeDelay()
    {
        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene(0);
    }

    [Rpc(SendTo.Server)]
    private void MatchOverRpc()
    {
        Debug.Log("Match Over!");
        Time.timeScale = 0f;
    }

    #endregion


    #region Time Control

    private void SetTimeScale(float newTimeScale)
    {
        Time.timeScale = newTimeScale;
    }

    [Rpc(SendTo.NotServer)]
    private void SetTimeScaleClientRpc(float newTimeScale)
    {
        Time.timeScale = newTimeScale;
    }

    #endregion


    #region Player Management

    public void AddController(PlayerController ctrl)
    {
        _playerControllers.Add(ctrl);
        Debug.Log("added controller" + ctrl);
    }

    public int SetPlayerIndex()
    {
        return NetworkManager.Singleton.ConnectedClients.Count;
    }

    public void EnablePlayerMovement()
    {
        foreach (PlayerController player in _playerControllers)
        {
            player.GetCharacterMovement().SetCanMove(true);
        }
    }

    public void DisablePlayerMovement()
    {
        foreach (PlayerController player in _playerControllers)
        {
            player.GetCharacterMovement().SetCanMove(false);
        }
    }

    #endregion


    #region Game State Control

    public void PauseGame()
    {
        bool didPause = gameState.UpdateGameStatus(GameStatus.Paused);

        if (didPause)
        {
            Time.timeScale = 0f;
        }
    }

    public void ResumeGame()
    {
        bool didResume = gameState.UpdateGameStatus(GameStatus.Playing);

        if (didResume)
        {
            Time.timeScale = 1f;
        }
    }

    public void TogglePause()
    {
        if (gameState.IsPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PlayerWon()
    {
        gameState.UpdateGameStatus(GameStatus.PlayerWon);
        Debug.Log("Player won!");
    }

    public void PlayerLost()
    {
        gameState.UpdateGameStatus(GameStatus.PlayerLost);
        Debug.Log("Player lost!");
    }

    #endregion


    #region Position Reset

    [Rpc(SendTo.Server)]
    private void ResetPlayerPositionRpc()
    {
        MechOne.transform.position = teamOneSpawnPoint.transform.position;
        MechTwo.transform.position = teamTwoSpawnPoint.transform.position;
    }

    #endregion


    #region Health System

    [Rpc(SendTo.Server)]
    private void InitTeamHealthRpc()
    {
        _teamOneHealth.Value = teamOneMaxHealth;
        _teamTwoHealth.Value = teamTwoMaxHealth;

        if (t1HealthScreen != null)
        {
            Changet1HealthTextClientRpc(teamOneMaxHealth, _teamOneHealth.Value);
        }

        if (t2HealthScreen != null)
        {
            Changet2HealthTextClientRpc(teamTwoMaxHealth, _teamTwoHealth.Value);
        }
    }

    [Rpc(SendTo.Server)]
    public void DamageTeamRpc(int teamNumToDamage, float damage)
    {
        if (teamNumToDamage == 1)
        {
            _teamOneHealth.Value = _teamOneHealth.Value - damage;

            Debug.Log("Damaging Team: " + teamNumToDamage + " by: " + damage + " damage to new health: " + _teamOneHealth.Value);

            if (t1HealthScreen != null)
            {
                Changet1HealthTextClientRpc(teamOneMaxHealth, _teamOneHealth.Value);
            }
        }
        else if (teamNumToDamage == 2)
        {
            _teamTwoHealth.Value = _teamTwoHealth.Value - damage;

            Debug.Log("Damaging Team: " + teamNumToDamage + " by: " + damage + " damage to new health: " + _teamTwoHealth.Value);

            if (t2HealthScreen != null)
            {
                Changet2HealthTextClientRpc(teamTwoMaxHealth, _teamTwoHealth.Value);
            }
        }

        if (_teamOneHealth.Value <= 0f && roundOver == false)
        {
            OnRoundEnd?.Invoke();
        }
        else if (_teamTwoHealth.Value <= 0f && roundOver == false)
        {
            OnRoundEnd?.Invoke();
        }
    }

    [Rpc(SendTo.Server)]
    public void HealTeamRpc(int teamNumToHeal, float healAmt)
    {
        if (teamNumToHeal == 1)
        {
            float h = teamOneMaxHealth - _teamOneHealth.Value;

            if (h > healAmt)
            {
                _teamOneHealth.Value = _teamOneHealth.Value + healAmt;
            }
            else
            {
                _teamOneHealth.Value = _teamOneHealth.Value + h;
            }

            Debug.Log("Healing Team: " + teamNumToHeal + " by: " + healAmt + " health to new health: " + _teamOneHealth.Value);

            if (t1HealthScreen != null)
            {
                Changet1HealthTextClientRpc(teamOneMaxHealth, _teamOneHealth.Value);
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
                Changet2HealthTextClientRpc(teamTwoMaxHealth, _teamTwoHealth.Value);
            }
        }
    }

    #endregion


    #region UI Updates

    [ClientRpc]
    private void Changet1HealthTextClientRpc(float MechMaxHealth, float MechCurrHealth)
    {
        t1HealthScreen.ChangeText(((MechCurrHealth / MechMaxHealth) * 100f).ToString(), false);
        if (t1UIMgr) { t1UIMgr.SetHealthBarPercent(MechMaxHealth, MechCurrHealth); }
    }

    [ClientRpc]
    private void Changet2HealthTextClientRpc(float MechMaxHealth, float MechCurrHealth)
    {
        t2HealthScreen.ChangeText(((MechCurrHealth / MechMaxHealth) * 100f).ToString(), false);
        if (t2UIMgr) { t2UIMgr.SetHealthBarPercent(MechMaxHealth, MechCurrHealth); }
    }

    #endregion
}
