using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;

public class AuthenticateUI : MonoBehaviour {


    [SerializeField] private Button authenticateButton;


    private void Awake() {
        authenticateButton.onClick.AddListener(() => {
            LobbyManager.Instance.Authenticate(EditPlayerName.Instance.GetPlayerName());
            Hide();
        });
        //try
        //{
        //    if (AuthenticationService.Instance.IsSignedIn)
        //    {
        //        Hide();
        //    }
        //}catch(ServicesInitializationException e)
        //{
        //    Debug.LogWarning("AuthenticationService not yet initialized, most likely just started game");
        //}
    }
    private async void Start()
    {
        await UnityServices.InitializeAsync();

        if (AuthenticationService.Instance.IsSignedIn)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }
    private void Hide() {
        gameObject.SetActive(false);
    }
    private void Show()
{
    gameObject.SetActive(true);
}
}