using System;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class ChangeBindingsScript : MonoBehaviour
{
    // Reference to the generated Input Actions class
    private PlayerInputActions inputActions;

    [Header("UI Text")]
    [SerializeField] private TMP_Text shootText;
    [SerializeField] private TMP_Text jumpText;
    [SerializeField] private TMP_Text dashText;
    [SerializeField] private TMP_Text reloadText;
    [SerializeField] private TMP_Text utilityText;
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private TMP_Text upText;
    [SerializeField] private TMP_Text downText;
    [SerializeField] private TMP_Text leftText;
    [SerializeField] private TMP_Text rightText;

    // =====================================================
    // UNITY FUNCTIONS
    // =====================================================

    private void Awake()
    {
        inputActions = InputManager.Instance.InputActions;
    }

    private void OnEnable()
    {
        // Enable all input actions
        inputActions.Enable();
    }

    private void OnDisable()
    {
        // Disable all input actions
        inputActions.Disable();
    }

    // =====================================================
    // GENERIC REBIND FUNCTION
    // =====================================================

    /// <summary>
    /// Starts an interactive rebind operation.
    ///
    /// The player presses a new key and the selected action
    /// will update to use that key.
    ///
    /// Duplicate actions can also automatically copy
    /// the same binding.
    /// </summary>
    ///
    /// <param name="primaryAction">
    /// The main action being rebound
    /// </param>
    ///
    /// <param name="duplicateActions">
    /// Any actions that should receive the same keybind
    /// </param>
    ///
    /// <param name="bindingIndex">
    /// Which binding to rebind
    ///
    /// Example:
    /// 0 = regular button binding
    ///
    /// Composite bindings:
    /// 1 = up
    /// 2 = down
    /// 3 = left
    /// 4 = right
    /// </param>
    public void RebindAction(InputAction primaryAction, InputAction[] duplicateActions = null, int bindingIndex = 0)
    {
        Debug.Log($"Rebinding {primaryAction.name}");

        // Disable the action while rebinding
        // to avoid accidental input triggers
        primaryAction.Disable();

        // Start listening for a new key press
        primaryAction.PerformInteractiveRebinding(bindingIndex)

            // Allow escape key to cancel rebinding
            .WithCancelingThrough("<Keyboard>/escape")

            // Called when rebinding is completed
            .OnComplete(operation =>
            {
                // Get the newly assigned binding path
                string newBinding = primaryAction.bindings[bindingIndex].overridePath;

                Debug.Log($"{primaryAction.name} rebound to {newBinding}");

                // Apply the same binding to any duplicate actions
                if (duplicateActions != null)
                {
                    foreach (InputAction duplicate in duplicateActions)
                    {
                        duplicate.ApplyBindingOverride(bindingIndex, newBinding);

                        Debug.Log($"Copied binding to {duplicate.name}");
                    }
                }

                // Re-enable the action
                primaryAction.Enable();

                // Save bindings to PlayerPrefs
                SaveBindings();

                RefreshUI();

                // Clean up the rebind operation
                operation.Dispose();
            })

            // Begin the rebind process
            .Start();
    }

    // =====================================================
    // BUTTON REBIND EXAMPLES
    // =====================================================

    /// <summary>
    /// Rebinds shoot for both players.
    /// </summary>
    public void RebindShoot()
    {
        RebindAction(
            inputActions.Player.P1Shoot,
            new InputAction[]
            {
                inputActions.Player.P2Shoot
            });
    }

    /// <summary>
    /// Rebinds jump for both players.
    /// </summary>
    public void RebindJump()
    {
        RebindAction(
            inputActions.Player.P1Jump,
            new InputAction[]
            {
                inputActions.Player.P2Jump
            });
    }

    /// <summary>
    /// Rebinds dash for both players.
    /// </summary>
    public void RebindDash()
    {
        RebindAction(
            inputActions.Player.P1Dash,
            new InputAction[]
            {
                inputActions.Player.P2Dash
            });
    }

    /// <summary>
    /// Rebinds reload for both players.
    /// </summary>
    public void RebindReload()
    {
        RebindAction(
            inputActions.Player.P1Reload,
            new InputAction[]
            {
                inputActions.Player.P2Reload
            });
    }

    /// <summary>
    /// Rebinds utility for both players.
    /// </summary>
    public void RebindUtility()
    {
        RebindAction(
            inputActions.Player.P1Utility,
            new InputAction[]
            {
                inputActions.Player.P2Utility
            });
    }

    /// <summary>
    /// Rebinds countdown for both players.
    /// </summary>
    public void RebindCountdown()
    {
        RebindAction(
            inputActions.Player.P1Countdown,
            new InputAction[]
            {
                inputActions.Player.P2Countdown
            });
    }

    // =====================================================
    // MOVEMENT REBINDS
    // =====================================================

    /*
        Movement uses a 2D Vector Composite.

        Binding indexes:

        0 = Composite itself
        1 = Up
        2 = Down
        3 = Left
        4 = Right
    */

    /// <summary>
    /// Rebinds the Up movement key.
    /// </summary>
    public void RebindMoveUp()
    {
        RebindAction(
            inputActions.Player.P1Move,
            new InputAction[]
            {
                inputActions.Player.P2Move
            },
            1);
    }

    /// <summary>
    /// Rebinds the Down movement key.
    /// </summary>
    public void RebindMoveDown()
    {
        RebindAction(
            inputActions.Player.P1Move,
            new InputAction[]
            {
                inputActions.Player.P2Move
            },
            2);
    }

    /// <summary>
    /// Rebinds the Left movement key.
    /// </summary>
    public void RebindMoveLeft()
    {
        RebindAction(
            inputActions.Player.P1Move,
            new InputAction[]
            {
                inputActions.Player.P2Move
            },
            3);
    }

    /// <summary>
    /// Rebinds the Right movement key.
    /// </summary>
    public void RebindMoveRight()
    {
        RebindAction(
            inputActions.Player.P1Move,
            new InputAction[]
            {
                inputActions.Player.P2Move
            },
            4);
    }

    // =====================================================
    // SAVE / LOAD FUNCTIONS
    // =====================================================

    /// <summary>
    /// Saves all binding overrides to PlayerPrefs.
    ///
    /// Unity converts all modified bindings into JSON.
    /// </summary>
    private void SaveBindings()
    {
        string rebinds =
            inputActions.asset.SaveBindingOverridesAsJson();

        PlayerPrefs.SetString("rebinds", rebinds);

        PlayerPrefs.Save();

        Debug.Log("Bindings Saved");
    }

    /// <summary>
    /// Refreshes the UI text to show current binding names.
    ///
    /// Should be called after rebinding to update the display.
    /// </summary>

    private void RefreshUI()
    {
        shootText.text =
            inputActions.Player.P1Shoot.GetBindingDisplayString();

        jumpText.text =
            inputActions.Player.P1Jump.GetBindingDisplayString();

        dashText.text =
            inputActions.Player.P1Dash.GetBindingDisplayString();

        reloadText.text =
            inputActions.Player.P1Reload.GetBindingDisplayString();

        utilityText.text =
            inputActions.Player.P1Utility.GetBindingDisplayString();

        countdownText.text =
            inputActions.Player.P1Countdown.GetBindingDisplayString();

        upText.text =
            inputActions.Player.P1Move.bindings[1]
            .ToDisplayString();

        downText.text =
            inputActions.Player.P1Move.bindings[2]
            .ToDisplayString();

        leftText.text =
            inputActions.Player.P1Move.bindings[3]
            .ToDisplayString();

        rightText.text =
            inputActions.Player.P1Move.bindings[4]
            .ToDisplayString();
    }

    // =====================================================
    // DISPLAY HELPERS
    // =====================================================

    /// <summary>
    /// Returns a readable key name for UI display.
    ///
    /// Example outputs:
    /// "Space"
    /// "Left Shift"
    /// "Mouse Left Button"
    /// </summary>
    public string GetBindingName(
        InputAction action,
        int bindingIndex = 0)
    {
        return action.bindings[bindingIndex]
            .ToDisplayString();
    }
}