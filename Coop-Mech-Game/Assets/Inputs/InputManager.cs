using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    public PlayerInputActions InputActions;

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);

            InputActions = new PlayerInputActions();

            // Load saved rebinds
            if (PlayerPrefs.HasKey("rebinds"))
            {
                string rebinds = PlayerPrefs.GetString("rebinds");

                InputActions.asset.LoadBindingOverridesFromJson(rebinds);
            }

            InputActions.Enable();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.SetString("rebinds", InputActions.asset.SaveBindingOverridesAsJson());
    }
}