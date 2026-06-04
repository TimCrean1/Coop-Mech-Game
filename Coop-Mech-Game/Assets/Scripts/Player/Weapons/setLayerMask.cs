using System;
using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Matchmaker.Models;
using UnityEngine;

public class setLayerMask : NetworkBehaviour
{

   
    
    public override void OnNetworkSpawn()
    {
        TeamLayering(); 
    }

    //public Tuple<int, PlayerController> GrabPlayerFunction()
    //{
    //    var client = NetworkManager.Singleton.ConnectedClients[NetworkManager.Singleton.LocalClientId];
    //    var playerObject = client.PlayerObject.GetComponent<TestPlayerObjectScript>();

    //    FixedString32Bytes team = playerObject.GetPlayerTeam();
    //    FixedString32Bytes num = playerObject.GetPlayerNum();

    //    PlayerController controller;

    //    if (team == "Red")
    //    {
    //        controller = GameManager.Instance._playerControllers[0];
    //    }
    //    else if (team == "Blue")
    //    {
    //        controller = GameManager.Instance._playerControllers[1];
    //    }
    //    else
    //    {
    //        Debug.LogError($"Invalid team: {team}");
    //        return new Tuple<int, PlayerController>(-1, null);
    //    }

        
    //}
    private void TeamLayering()
    {
        var client = NetworkManager.Singleton.ConnectedClients[NetworkManager.Singleton.LocalClientId];
        var playerObject = client.PlayerObject.GetComponent<TestPlayerObjectScript>();

        FixedString32Bytes team = playerObject.GetPlayerTeam();
        

        int layer = 0;

        if (team == "Red")
        {
            layer = 15;
        }
        else if (team == "Blue")
        {
            layer = 16;
        }
        else
        {
            Debug.LogError($"Invalid team: {team}");
            return;
        }
       
        SetLayerRecursively(gameObject, layer);
    }
    void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }


}
