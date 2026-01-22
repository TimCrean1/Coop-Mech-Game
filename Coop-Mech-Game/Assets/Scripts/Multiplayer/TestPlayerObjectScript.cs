using Unity.Netcode;
using Unity.Services.Matchmaker.Models;
using UnityEngine;

public class TestPlayerObjectScript : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {


        if (GameManager.Instance._playerControllers[0].player1 == null)
        {
            GameManager.Instance._playerControllers[0].player1 = this;
        }
        else
        {
            GameManager.Instance._playerControllers[0].player2 = this;
        }
    }

    private void Start()
    {
        //if (GameManager.Instance._playerControllers[0].player1 == null)
        //{
        //    GameManager.Instance._playerControllers[0].player1 = this;
        //}
        //else
        //{
        //    GameManager.Instance._playerControllers[0].player2 = this;
        //}
    }
}
