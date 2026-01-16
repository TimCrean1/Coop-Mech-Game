using UnityEngine;
using Unity.Netcode;

public class MultiplayerPartnerTest : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnNetworkSpawn()
    {
        Debug.Log(OwnerClientId);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
