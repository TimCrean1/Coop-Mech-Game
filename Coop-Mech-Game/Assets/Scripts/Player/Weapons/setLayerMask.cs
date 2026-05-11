using System;
using Unity.Netcode;
using Unity.Services.Matchmaker.Models;
using UnityEngine;

public class setLayerMask : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var client = NetworkManager.Singleton.ConnectedClients[NetworkManager.Singleton.LocalClientId];
        var playerObject = client.PlayerObject.GetComponent<TestPlayerObjectScript>();

        string team = playerObject.GetPlayerTeam();
        string num = playerObject.GetPlayerNum();
        ShopManager.Instance.GrabPlayerFunction();
        

        if (team == "Red")
        {
            //controller = GameManager.Instance._playerControllers[0];
            gameObject.layer = 15;
        }
        else if (team == "Blue")
        {
            gameObject.layer = 16;
            // controller = GameManager.Instance._playerControllers[1];
        }
        else
        {
            Debug.LogError($"Invalid team: {team}");
        }
        foreach (Transform child in gameObject.GetComponentsInChildren<Transform>(true))
        {
            if (team == "Red")
            {
                //controller = GameManager.Instance._playerControllers[0];
                child.gameObject.layer = 15;
            }
            else if (team == "Blue")
            {
                child.gameObject.layer = 16;
                // controller = GameManager.Instance._playerControllers[1];
            }
            else
            {
                Debug.LogError($"Invalid team: {team}");
            }
        }

    }

   
}
