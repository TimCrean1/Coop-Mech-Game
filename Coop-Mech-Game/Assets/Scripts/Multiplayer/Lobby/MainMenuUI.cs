using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button PlayButton;
    [SerializeField] private Button OptionsButton;
    [SerializeField] private Button QuitButton;


    [SerializeField] private CinemachineCamera _camera;
    [SerializeField] private CinemachineCamera _camera2;
    
    void Start()
    {
        PlayButton.onClick.AddListener(() =>
        {
            _camera.Priority = 0;
            _camera2.Priority = 1;
            Hide();
            
        });

        OptionsButton.onClick.AddListener(() =>
        {
            
        });

        QuitButton.onClick.AddListener(() =>
        {
            Application.Quit();
            if (Application.isEditor)
            {
               EditorApplication.isPlaying = false;
            }
        });
    }

    

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
