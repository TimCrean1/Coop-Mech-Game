using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.VisualScripting;
using System;

public class ChatManager : NetworkBehaviour
{
    public static ChatManager Singleton;

    [SerializeField] ChatMessage chatMessagePrefab;
    [SerializeField] CanvasGroup chatContent;
    [SerializeField] TMP_InputField chatInput;

    public string playerName;
    public string playerTeam;
    

    void Awake()
    { ChatManager.Singleton = this; }

    public override void OnNetworkSpawn()
    {
        SetPlayerInfo();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SendChatMessage(chatInput.text, playerName);
            chatInput.text = "";
        }
    }

    public void SendChatMessage(string _message, string _fromWho = null)
    {
        if (string.IsNullOrWhiteSpace(_message)) return;

        string S = _fromWho + " > " + _message;
        SendChatMessageServerRpc(S);
    }

    public void SendTeamChatMessage(string _message, string _team, string _fromWho = null)
    {
        if (string.IsNullOrWhiteSpace(_message)) { return; }

        if (_team == playerTeam)
        {
            string S = _fromWho + " > " + _message;
            SendChatMessageServerRpc(S);
        }
    }
    void AddMessage(string msg)
    {
        ChatMessage CM = Instantiate(chatMessagePrefab, chatContent.transform.position, chatContent.transform.rotation, chatContent.transform);
        CM.SetText(msg);
    }

    [Rpc(SendTo.Server)]
    void SendChatMessageServerRpc(string message)
    {
        ReceiveChatMessageClientRpc(message);
    }

    [Rpc(SendTo.Everyone)]
    void ReceiveChatMessageClientRpc(string message)
    {
        AddMessage(message);
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