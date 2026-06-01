using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.VisualScripting;
using System;
using Unity.Collections;

public class ChatManager : NetworkBehaviour
{
    public static ChatManager Singleton;

    public static event Action<string> OnMessageReceived;

    //[SerializeField] ChatMessage chatMessagePrefab;
    //[SerializeField] CanvasGroup chatContent;
    //[SerializeField] TMP_InputField chatInput;
    // moved to ChatUI

    public string playerName;
    public FixedString32Bytes playerTeam;

    // need some way to detect if the player wants to chat in team/all chat
    // possibly enum?

    void Awake()
    { Singleton = this; }

    public override void OnNetworkSpawn()
    {
        SetPlayerInfo();
    }

    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Return))
    //    {
    //        SendChatMessage(chatInput.text, playerName);
    //        chatInput.text = "";
    //    }
    //}

    public void SendChatMessage(string _message, string _fromWho = null)
    {
        if (string.IsNullOrWhiteSpace(_message)) return;

        string S = _fromWho + " > " + _message;
        SendChatMessageServerRpc(S);
    }

    public void SendTeamChatMessage(string _message, string _team, string _fromWho = null)
    {
        if (string.IsNullOrWhiteSpace(_message)) { return; }

        
         // wait this doesnt actually work
         string S = _fromWho + " > " + _message;
         SendChatMessageServerRpc(S, _team);
        
    }
    //void AddMessage(string msg)
    //{
    //    ChatMessage CM = Instantiate(chatMessagePrefab, chatContent.transform.position, chatContent.transform.rotation, chatContent.transform);
    //    CM.SetText(msg);
    //}

    [Rpc(SendTo.Server)]
    void SendChatMessageServerRpc(string message, string team = null)
    {
        //if (playerTeam == team) {
            // hopefully only the player's team should receive this message
            ReceiveChatMessageClientRpc(message);
        //}
    }

    [Rpc(SendTo.Everyone)]
    void ReceiveChatMessageClientRpc(string message)
    {
        //AddMessage(message);
        OnMessageReceived?.Invoke(message);
    }

    private void SetPlayerInfo()
    {
        if (NetworkManager.Singleton.IsConnectedClient) {

            var client = NetworkManager.Singleton.ConnectedClients[NetworkManager.Singleton.LocalClientId];

            if (client.PlayerObject.GetComponent<TestPlayerObjectScript>()) { 

                var playerObject = client.PlayerObject.GetComponent<TestPlayerObjectScript>();
            
                playerTeam = playerObject.GetPlayerTeam();
                playerName = playerObject.GetPlayerName();

            }
        }
    }
}