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
    
    private void TeamLayering()
    {
        NetworkObject netObj = GetComponentInParent<NetworkObject>();

        if (netObj == null)
        {
            Debug.LogError("No NetworkObject found!");
            return;
        }

        ulong ownerId = netObj.OwnerClientId;

        if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(ownerId, out var client))
        {
            Debug.LogError("Could not find owner client");
            return;
        }

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
        Debug.Log(
                $"Gun spawned. Owner={ownerId}, Team={team}, IsServer={IsServer}"
            );
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
