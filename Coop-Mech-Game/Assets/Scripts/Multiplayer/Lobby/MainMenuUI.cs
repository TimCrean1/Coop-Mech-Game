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
    public static MainMenuUI Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }
    void Start()
    {
        PlayButton.onClick.AddListener(() =>
        {
            giveCameraTwoPriority();
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


    public void giveCameraOnePriority()
    {
        _camera.Priority = 1;
        _camera2.Priority = 0;
    }

    public void giveCameraTwoPriority()
    {
        _camera.Priority = 0;
        _camera2.Priority = 1;
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}
