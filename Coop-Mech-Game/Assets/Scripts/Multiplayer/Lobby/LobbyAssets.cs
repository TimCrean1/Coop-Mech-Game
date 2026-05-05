using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyAssets : MonoBehaviour {



    public static LobbyAssets Instance { get; private set; }


    [SerializeField] private Sprite redTeamSprite;
    [SerializeField] private Sprite blueTeamSprite;
    [SerializeField] private Sprite spectatorSprite;


    private void Awake() {
        Instance = this;
    }

    public Sprite GetSprite(LobbyManager.PlayerTeam playerTeam) {
        switch (playerTeam) {
            default:
            case LobbyManager.PlayerTeam.Red:         return redTeamSprite;
            case LobbyManager.PlayerTeam.Blue:        return blueTeamSprite;
            case LobbyManager.PlayerTeam.Spectator:   return spectatorSprite;
        }
    }

}