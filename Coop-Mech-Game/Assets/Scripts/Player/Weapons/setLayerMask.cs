using System;
using Unity.Netcode;
using Unity.Services.Matchmaker.Models;
using UnityEngine;

public class setLayerMask : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!TryGetComponent<NetworkObject>(out var netObj))
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

        string team = playerObject.GetPlayerTeam();

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
