using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("GameplayScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void FirstDifficulty()
    {
        GameState.Instance.SetDifficulty(0);
    }

    public void SecondDifficulty()
    {
        GameState.Instance.SetDifficulty(1);
    }
    public void ThirdDifficulty()
    {
        GameState.Instance.SetDifficulty(2);
    }
    public void FourthDifficulty()
    {
        GameState.Instance.SetDifficulty(3);
    }

}
