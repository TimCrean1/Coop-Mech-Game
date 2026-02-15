using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static LobbyManager;


public class LobbyManager : MonoBehaviour {

    #region Variables
    public static LobbyManager Instance { get; private set; }

    [SerializeField] private GameObject LoadingScreen;
    [SerializeField] private Image LoadingBarFill;
    public static bool IsHost { get; private set; }
    public static string RelayJoinCode { get; private set; }



    public const string KEY_PLAYER_NAME = "PlayerName";
    public const string KEY_PLAYER_CHARACTER = "Character";
    public const string KEY_PLAYER_TEAM = "Red";
    public const string KEY_PLAYER_NUMBER = "One";
    public const string KEY_LOBBY_RED_TEAM = "Red";
    public const string KEY_LOBBY_BLUE_TEAM = "Blue";
    public const string KEY_GAME_MODE = "GameMode";
    public const string KEY_START_GAME = "StartGame";
    public const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";

    [SerializeField]private List<string> _redPlayers = new List<string>();
    [SerializeField]private List<string> _bluePlayers = new List<string>();

    public event EventHandler OnLeftLobby;

    public event EventHandler<LobbyEventArgs> OnJoinedLobby;
    public event EventHandler<LobbyEventArgs> OnJoinedLobbyUpdate;
    public event EventHandler<LobbyEventArgs> OnKickedFromLobby;
    public event EventHandler<LobbyEventArgs> OnLobbyGameModeChanged;
    public event EventHandler<LobbyEventArgs> OnLobbyStartGame;
    public event EventHandler<LobbyEventArgs> OnTeamChange;
    public class LobbyEventArgs : EventArgs {
        public Lobby lobby;
    }

    public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;
    public class OnLobbyListChangedEventArgs : EventArgs {
        public List<Lobby> lobbyList;
    }


    public enum GameMode {
        Practice,
        Duel
    }

    public enum PlayerTeam
    {
        Red,
        Blue,
        Spectator
    }

    public enum PlayerNumber
    {
        One,
        Two,
        Deciding
    }
    



    private float heartbeatTimer;
    private float lobbyPollTimer;
    private float refreshLobbyListTimer = 5f;
    private Lobby joinedLobby;
    private string playerName;
    private bool alreadyStartedGame;
    

    #endregion
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private void Update() {
        //HandleRefreshLobbyList(); // Disabled Auto Refresh for testing with multiple builds
        HandleLobbyHeartbeat();
        HandleLobbyPolling();
    }
    private string[] SplitString(string str)
    {
        // split the string by the comma 
        return str.Split(",");
    }

    private string CombineString(string[] str)
    {
        return str[0] + "," + str[1];
    }
    
    //private void CheckTeamValue(PlayerTeam team)
    //{
    //    Lobby lobby = GetJoinedLobby(); //grab the lobby so we can get the lobby team value

    //    // check if the team is red or blue
    //    if (team == PlayerTeam.Red) {
    //        // split the value into two strings
    //        string[] strings = SplitString(lobby.Data[KEY_LOBBY_RED_TEAM].Value);
    //        for (int i = 0; i < strings.Length; i++) {
    //            // loop through the list of strings and check if there is an empty string
    //            if (strings[i] == "_")
    //            {
    //                // if you find an empty slot, replace slot with your id
    //                strings[i] = AuthenticationService.Instance.PlayerId;
    //                UpdateLobbyRedTeam(CombineString(strings));
                    
    //                //print(strings[0] + "," + strings[1]);
    //                if(i == 0)
    //                {
    //                    Debug.Log("player set to one - red");
    //                    UpdatePlayerNumber(PlayerNumber.One);
    //                }
    //                else
    //                {
    //                    Debug.Log("player set to two - red");
    //                    UpdatePlayerNumber(PlayerNumber.Two);
    //                }

                    
    //                //Player localPlayer = lobby.Players.Find(p =>
    //                //p.Id == AuthenticationService.Instance.PlayerId);
    //                //Debug.Log(localPlayer.Data[KEY_PLAYER_NUMBER].Value);
    //                //Debug.Log(strings[i].ToString());
    //                break;
    //            }
    //        }
    //    }
    //    if(team == PlayerTeam.Blue){
    //        string[] strings = SplitString(lobby.Data[KEY_LOBBY_BLUE_TEAM].Value);
    //        for (int i = 0; i < strings.Length; i++)
    //        {
    //            if (strings[i] == "_")
    //            {
    //                strings[i] = AuthenticationService.Instance.PlayerId;
    //                UpdateLobbyBlueTeam(CombineString(strings));
    //                //print(strings);
    //                if (i == 0)
    //                {
    //                    Debug.Log("player set to one - blue");
    //                    UpdatePlayerNumber(PlayerNumber.One);
    //                }
    //                else
    //                {
    //                    Debug.Log("player set to two - blue");
    //                    UpdatePlayerNumber(PlayerNumber.Two);
    //                }
    //                //Player localPlayer = lobby.Players.Find(p =>
    //                //p.Id == AuthenticationService.Instance.PlayerId);
    //                //Debug.Log(localPlayer.Data[KEY_PLAYER_NUMBER].Value);
    //                //Debug.Log(strings[i].ToString());
    //                break;
    //            }
    //        }
    //    }
    //}
    public async void Authenticate(string playerName) {
        playerName = playerName.Replace(" ", "_");

       
        this.playerName = playerName;
        InitializationOptions initializationOptions = new InitializationOptions();
        initializationOptions.SetProfile(playerName);

        await UnityServices.InitializeAsync(initializationOptions);

        AuthenticationService.Instance.SignedIn += () => {
            // do nothing
            Debug.Log("Signed in! " + AuthenticationService.Instance.PlayerId);

            RefreshLobbyList();
        };
        
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private void HandleRefreshLobbyList() {
        if (UnityServices.State == ServicesInitializationState.Initialized && AuthenticationService.Instance.IsSignedIn) {
            refreshLobbyListTimer -= Time.deltaTime;
            if (refreshLobbyListTimer < 0f) {
                float refreshLobbyListTimerMax = 5f;
                refreshLobbyListTimer = refreshLobbyListTimerMax;

                RefreshLobbyList();
            }
        }
    }

    private async void HandleLobbyHeartbeat() {
        if (IsLobbyHost()) {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer < 0f) {
                float heartbeatTimerMax = 15f;
                heartbeatTimer = heartbeatTimerMax;

                Debug.Log("Heartbeat");
                await LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }
    }

    private async void HandleLobbyPolling() {
        //Debug.Log("POLL UPDATE");
        if (joinedLobby != null) {
            lobbyPollTimer -= Time.deltaTime;
            if (lobbyPollTimer < 0f) {
                float lobbyPollTimerMax = 1.1f;
                lobbyPollTimer = lobbyPollTimerMax;

                joinedLobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);

                OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });

                if (!IsLobbyHost()) {
                    if (joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value != "") {
                        JoinGame(joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value);
                    }
                }
                
                //if (!alreadyStartedGame) {
                //    if (IsLobbyHost()) {
                //        if (joinedLobby.Players.Count == 2) {
                //            // Two players have joined, start game
                //            StartGame();
                //        }
                //    }
                //}


                if (!IsPlayerInLobby()) {
                    // Player was kicked out of this lobby
                    Debug.Log("Kicked from Lobby!");

                    OnKickedFromLobby?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });

                    joinedLobby = null;
                }
            }
        }
    }
    // write method for checking if either team has more than two players, and then force team to autobalance
   
    public void StartIfHost()
    {
        if (!alreadyStartedGame)
        {
            if (IsLobbyHost())
            {
                
                if (joinedLobby.Players.Count == joinedLobby.MaxPlayers)
                {
                   
                    StartGame();
                }
            }
        }
    }
    public Lobby GetJoinedLobby() {
        return joinedLobby;
    }

    public bool IsLobbyHost() {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    private bool IsPlayerInLobby() {
        if (joinedLobby != null && joinedLobby.Players != null) {
            foreach (Player player in joinedLobby.Players) {
                if (player.Id == AuthenticationService.Instance.PlayerId) {
                    // This player is in this lobby
                    return true;
                }
            }
        }
        return false;
    }

    private Player GetPlayer() {
        return new Player(AuthenticationService.Instance.PlayerId, null, new Dictionary<string, PlayerDataObject> {
            { KEY_PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerName) },
            { KEY_PLAYER_TEAM, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, PlayerTeam.Spectator.ToString())},
            { KEY_PLAYER_NUMBER, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, PlayerNumber.Deciding.ToString())}

        });
    }

    public void ChangeGameMode() {
        if (IsLobbyHost()) {
            GameMode gameMode =
                Enum.Parse<GameMode>(joinedLobby.Data[KEY_GAME_MODE].Value);


            switch (gameMode) {
                default:
                case GameMode.Practice:
                    gameMode = GameMode.Duel;
                    
                    break;
                case GameMode.Duel:
                    gameMode = GameMode.Practice;
                    break;
            }

            UpdateLobbyGameMode(gameMode);
        }
    }

    public async void CreateLobby(string lobbyName, int maxPlayers, bool isPrivate, GameMode gameMode) {
        Player player = GetPlayer();

        CreateLobbyOptions options = new CreateLobbyOptions {
            Player = player,
            IsPrivate = isPrivate,
            Data = new Dictionary<string, DataObject> {
                { KEY_GAME_MODE, new DataObject(DataObject.VisibilityOptions.Public, gameMode.ToString()) },
                { KEY_RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, "") },
                { KEY_LOBBY_RED_TEAM, new DataObject(DataObject.VisibilityOptions.Member, "_,_") },
                { KEY_LOBBY_BLUE_TEAM, new DataObject(DataObject.VisibilityOptions.Member, "_,_") }
            }
        };

        if (gameMode == GameMode.Practice) {
            maxPlayers = 2;
        }
        else if (gameMode == GameMode.Duel)
        {
            maxPlayers = 4;
        }
        //Debug.Log("maxplayers set to " + maxPlayers);
        Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);

        joinedLobby = lobby;

        OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });

        Debug.Log("Created Lobby " + lobby.Name);
    }

    public async void RefreshLobbyList() {
        try {
            QueryLobbiesOptions options = new QueryLobbiesOptions();
            options.Count = 25;

            // Filter for open lobbies only
            options.Filters = new List<QueryFilter> {
                new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT,
                    value: "0")
            };

            // Order by newest lobbies first
            options.Order = new List<QueryOrder> {
                new QueryOrder(
                    asc: false,
                    field: QueryOrder.FieldOptions.Created)
            };

            QueryResponse lobbyListQueryResponse = await LobbyService.Instance.QueryLobbiesAsync();

            OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs { lobbyList = lobbyListQueryResponse.Results });
        } catch (LobbyServiceException e) {
            Debug.Log(e);
        }
    }

    public async void JoinLobbyByCode(string lobbyCode) {
        Player player = GetPlayer();

        Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, new JoinLobbyByCodeOptions {
            Player = player
        });

        joinedLobby = lobby;

        OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
    }

    public async void JoinLobby(Lobby lobby) {
        Player player = GetPlayer();

        joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id, new JoinLobbyByIdOptions {
            Player = player
        });

        OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
    }
    private int GetTeamCount(PlayerTeam team)
    {
        int count = 0;

        foreach (Player player in joinedLobby.Players)
        {
            if (player.Data.TryGetValue(KEY_PLAYER_TEAM, out var teamData))
            {
                if (teamData.Value == team.ToString())
                    count++;
            }
        }

        return count;
    }
    public async void UpdatePlayerTeam(PlayerTeam newTeam)
    {
        if (joinedLobby == null) return;

        int teamCount = GetTeamCount(newTeam);

        PlayerNumber numberToAssign =
            teamCount == 0 ? PlayerNumber.One : PlayerNumber.Two;

        try
        {
            UpdatePlayerOptions options = new UpdatePlayerOptions
            {
                Data = new Dictionary<string, PlayerDataObject>
            {
                {
                    KEY_PLAYER_TEAM,
                    new PlayerDataObject(
                        PlayerDataObject.VisibilityOptions.Public,
                        newTeam.ToString())
                },
                {
                    KEY_PLAYER_NUMBER,
                    new PlayerDataObject(
                        PlayerDataObject.VisibilityOptions.Public,
                        numberToAssign.ToString())
                }
            }
            };

            string playerId = AuthenticationService.Instance.PlayerId;

            joinedLobby = await LobbyService.Instance.UpdatePlayerAsync(
                joinedLobby.Id,
                playerId,
                options);

            OnJoinedLobbyUpdate?.Invoke(this,
                new LobbyEventArgs { lobby = joinedLobby });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log("Team update failed: " + e);
        }
    }
    public async void UpdatePlayerName(string playerName) {
        this.playerName = playerName;

        if (joinedLobby != null) {
            try {
                UpdatePlayerOptions options = new UpdatePlayerOptions();

                options.Data = new Dictionary<string, PlayerDataObject>() {
                    {
                        KEY_PLAYER_NAME, new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Public,
                            value: playerName)
                    }
                };

                string playerId = AuthenticationService.Instance.PlayerId;

                Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, playerId, options);
                joinedLobby = lobby;

                OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
            } catch (LobbyServiceException e) {
                Debug.Log("When trying to update players name: " + e);
            }
        }
    }
    
    public async void UpdatePlayerNumber(string num)
    {
        if (joinedLobby != null)
        {
            try
            {
                UpdatePlayerOptions options = new UpdatePlayerOptions();

                options.Data = new Dictionary<string, PlayerDataObject>() {
                    {
                        KEY_PLAYER_NUMBER, new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Public,
                            value: num.ToString())
                    }
                };

                string playerId = AuthenticationService.Instance.PlayerId;

                Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, playerId, options);
                joinedLobby = lobby;

                OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
            }
            catch (LobbyServiceException e)
            {
                Debug.Log("When trying to update players number: " + e);
            }
        }
    }
    //public async void UpdatePlayerCharacter(PlayerCharacter playerCharacter) {
    //    if (joinedLobby != null) {
    //        try {
    //            UpdatePlayerOptions options = new UpdatePlayerOptions();

    //            options.Data = new Dictionary<string, PlayerDataObject>() {
    //                {
    //                    KEY_PLAYER_CHARACTER, new PlayerDataObject(
    //                        visibility: PlayerDataObject.VisibilityOptions.Public,
    //                        value: playerCharacter.ToString())
    //                }
    //            };

    //            string playerId = AuthenticationService.Instance.PlayerId;

    //            Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, playerId, options);
    //            joinedLobby = lobby;

    //            OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
    //        } catch (LobbyServiceException e) {
    //            Debug.Log("When trying to update players character: " + e);
    //        }
    //    }
    //}

    //public async void UpdatePlayerTeam(PlayerTeam newTeam) 
    //{
    //    //Debug.Log("TEAM CLICK");
    //    if (joinedLobby != null)
    //    {
    //        try
    //        {
    //            UpdatePlayerOptions options = new UpdatePlayerOptions();

    //            options.Data = new Dictionary<string, PlayerDataObject>() {
    //                {
    //                    KEY_PLAYER_TEAM, new PlayerDataObject(
    //                visibility: PlayerDataObject.VisibilityOptions.Public,
    //                value: newTeam.ToString())
    //                }
    //            };


    //            string playerId = AuthenticationService.Instance.PlayerId;

    //            Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, playerId, options);
    //            joinedLobby = lobby;
    //            CheckTeamValue(newTeam);
                

    //            OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
               
    //        }
    //        catch (LobbyServiceException e) {
    //            Debug.Log("When trying to update players team: " + e);
    //        }
    //    }
    //}
    public async void UpdatePlayerNumber(PlayerNumber newNumber)
    {
        if (joinedLobby != null)
        {
            try
            {
                UpdatePlayerOptions options = new UpdatePlayerOptions();

                options.Data = new Dictionary<string, PlayerDataObject>() {
                    {
                        KEY_PLAYER_NUMBER, new PlayerDataObject(
                    visibility: PlayerDataObject.VisibilityOptions.Public,
                    value: newNumber.ToString())
                    }
                };


                string playerId = AuthenticationService.Instance.PlayerId;

                Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, playerId, options);
                joinedLobby = lobby;

                OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
            }
            catch (LobbyServiceException e)
            {
                Debug.Log("When trying to update players number: " + e);
            }
        }
    }
    public async void QuickJoinLobby() {
        try {
            QuickJoinLobbyOptions options = new QuickJoinLobbyOptions();

            Lobby lobby = await LobbyService.Instance.QuickJoinLobbyAsync(options);
            joinedLobby = lobby;

            OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
        } catch (LobbyServiceException e) {
            Debug.Log(e);
        }
    }

    public async void LeaveLobby() {
        if (joinedLobby != null) {
            try {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);

                joinedLobby = null;

                OnLeftLobby?.Invoke(this, EventArgs.Empty);
            } catch (LobbyServiceException e) {
                Debug.Log(e);
            }
        }
    }

    public async void KickPlayer(string playerId) {
        if (IsLobbyHost()) {
            try {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
            } catch (LobbyServiceException e) {
                Debug.Log(e);
            }
        }
    }

    public async void UpdateLobbyGameMode(GameMode gameMode) {
        try {
            Debug.Log("UpdateLobbyGameMode " + gameMode);
            
            Lobby lobby = await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions {
                Data = new Dictionary<string, DataObject> {
                    { KEY_GAME_MODE, new DataObject(DataObject.VisibilityOptions.Public, gameMode.ToString()) }
                }
            });

            joinedLobby = lobby;

            OnLobbyGameModeChanged?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
        } catch (LobbyServiceException e) {
            Debug.Log(e);
        }
    }
    public async void UpdateLobbyRedTeam(string newString)
    {
        //currently only actually updates when the host runs the code
        try
        {
            Debug.Log("UpdateLobbyRedTeam " + newString);

            Lobby lobby = await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject> {
                    { KEY_LOBBY_RED_TEAM, new DataObject(DataObject.VisibilityOptions.Member, newString) }
                }
            });

            joinedLobby = lobby;

            OnLobbyGameModeChanged?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    public async void UpdateLobbyBlueTeam(string newString)
    {
        try
        {
            Debug.Log("UpdateLobbyRedTeam " + newString);

            Lobby lobby = await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject> {
                    { KEY_LOBBY_BLUE_TEAM, new DataObject(DataObject.VisibilityOptions.Member, newString) }
                }
            });

            joinedLobby = lobby;

            OnLobbyGameModeChanged?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    public void LoadScene(int sceneId)
    {
        StartCoroutine(LoadSceneAsync(sceneId));
    }
    IEnumerator LoadSceneAsync(int sceneId)
    {

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId);

        LoadingScreen.SetActive(true);

        while (!operation.isDone)
        {

            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);

            LoadingBarFill.fillAmount = progressValue;

            yield return null;
        }


    }
    public async void StartGame() {
        try {
            Debug.Log("StartGame");

            Lobby lobby = await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions {
                Data = new Dictionary<string, DataObject> {
                    { KEY_START_GAME, new DataObject(DataObject.VisibilityOptions.Public, "1") }
                }
            });

            joinedLobby = lobby;

            IsHost = true;
            alreadyStartedGame = true;
            //SceneManager.LoadScene(1);
            if (lobby.Data[KEY_GAME_MODE].Value == "Practice")
            {
                LoadScene(1);
            }else if(lobby.Data[KEY_GAME_MODE].Value == "Duel")
            {
                LoadScene(2);
            }
           

            OnLobbyStartGame?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
        } catch (LobbyServiceException e) {
            Debug.Log(e);
        }
    }

    private void JoinGame(string relayJoinCode) {
        Debug.Log("JoinGame " + relayJoinCode);
        if (string.IsNullOrEmpty(relayJoinCode)) {
            Debug.Log("Invalid Relay code, wait");
            return;
        }

        IsHost = false;
        RelayJoinCode = relayJoinCode;
        //SceneManager.LoadScene(1);
        if (joinedLobby.Data[KEY_GAME_MODE].Value == "Practice")
        {
            SceneManager.LoadScene(1);
        }
        else if (joinedLobby.Data[KEY_GAME_MODE].Value == "Duel")
        {
            SceneManager.LoadScene(2);
        }
        alreadyStartedGame = true;
        OnLobbyStartGame?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
    }

    public async void SetRelayJoinCode(string relayJoinCode) {
        try {
            Debug.Log("SetRelayJoinCode " + relayJoinCode);

            Lobby lobby = await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions {
                Data = new Dictionary<string, DataObject> {
                    { KEY_RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) }
                }
            });

            joinedLobby = lobby;
        } catch (LobbyServiceException e) {
            Debug.Log(e);
        }
    }
    #region TeamMethods
    private async void CheckTeamForPlayer(List<string> players)
    {
        try
        {
            UpdatePlayerOptions options;
            if (players.Count <= 0)
            {
                options = new UpdatePlayerOptions();

                options.Data = new Dictionary<string, PlayerDataObject>() {
                    {
                        KEY_PLAYER_NUMBER, new PlayerDataObject(
                    visibility: PlayerDataObject.VisibilityOptions.Public,
                    value: "One")
                    }
                };
            }
            else
            {
                options = new UpdatePlayerOptions();

                options.Data = new Dictionary<string, PlayerDataObject>() {
                    {
                        KEY_PLAYER_NUMBER, new PlayerDataObject(
                    visibility: PlayerDataObject.VisibilityOptions.Public,
                    value: "Two")
                    }
                };
            }
            string playerId = AuthenticationService.Instance.PlayerId;

            Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, playerId, options);
            joinedLobby = lobby;

            OnLobbyGameModeChanged?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
        }
        catch (LobbyServiceException e) {
            Debug.Log(e);
        }
    }

    private void JoinTeam(List<string> players)
    {
        string playerId = AuthenticationService.Instance.PlayerId;

        players.Add(playerId);
    }
    #endregion
}