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
        DifficultyManager.Instance.SetDifficulty(0);
    }

    public void SecondDifficulty()
    {
        DifficultyManager.Instance.SetDifficulty(1);
    }
    public void ThirdDifficulty()
    {
        DifficultyManager.Instance.SetDifficulty(2);
    }
    public void FourthDifficulty()
    {
        DifficultyManager.Instance.SetDifficulty(3);
    }

}
