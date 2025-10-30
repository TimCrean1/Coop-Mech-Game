
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // We need this include to use the Input System package
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public enum EPlayerState
{
    Moving,
    Paused,
    
}

public class PlayerController : MonoBehaviour // We inherit from MonoBehaviour to make this class a Unity script
{
    #region Variables

    [Header("Player State")]
    public EPlayerState currentState = EPlayerState.Moving;

    [Header("Player Input")]
    private PlayerInputActions playerInputActions; // This is the object that listens for inputs at the hardware level
    [SerializeField] private Vector2 P1MovementInput;

    public Vector2 getP1MovementInput { get { return P1MovementInput; } }

    [Header("Component / Object References")]
    [SerializeField] private BaseMovement baseMovement;
    //[SerializeField] private PlayerInteractManager playerInteractManager;

    #endregion

    #region Unity Functions

    // Awake is called before Start() when an object is created or when the level is loaded
    private void Awake()
    {
        // Set up our player actions in code
        // This class name is based on what you named your .inputactions asset
        playerInputActions = new PlayerInputActions();
    }

    private void Start()
    {
        //InventoryUI.rootVisualElement.style.display = DisplayStyle.None;
        // GameState callback listeners
        //GameState.Instance.OnGamePaused.AddListener(OnGamePausedReceived);
        //GameState.Instance.OnGameResumed.AddListener(OnGameResumedReceived);
        

    }

    private void Update()
    {
        //if (GameManager.Instance.playerLives == 0)
        //{
        //    GameManager.Instance.PlayerLost();
        //    SceneManager.LoadScene("Lose Scene");
        //}
    }

    private void OnEnable()
    {
        // Here we can subscribe functions to our
        // input actions to make code occur when
        // our input actions occur
        SubscribeInputActions();

        // We need to enable our "Player" action map so Unity will listen for our input
        SwitchActionMap(EPlayerState.Moving);
    }

    private void OnDisable()
    {
        // Here we can unsubscribe our functions
        // from our input actions so our object
        // doesn't try to call functions after
        // it is destroyed
        UnsubscribeInputActions();

        //InventoryUI.enabled = false;

        // Disable all action maps
        SwitchActionMap(currentState);
    }

    #endregion

    #region Custom Functions

    #region Input Handling

    private void SubscribeInputActions()
    {
        // Here we can bind our input actions to functions
        playerInputActions.Player.P1Move.started += P1MoveAction;
        playerInputActions.Player.P1Move.performed += P1MoveAction;
        playerInputActions.Player.P1Move.canceled += P1MoveAction;

        playerInputActions.Player.P2Move.started += P2MoveAction;
        playerInputActions.Player.P2Move.performed += P2MoveAction;
        playerInputActions.Player.P2Move.canceled += P2MoveAction;

        playerInputActions.Player.Jump.performed += JumpActionPerformed;
        playerInputActions.Player.Jump.canceled += JumpActionCanceled;

        //playerInputActions.Player.TogglePause.performed += TogglePauseActionPerformed;
        //playerInputActions.UI.TogglePause.performed += TogglePauseActionPerformed;

        playerInputActions.Player.Sprint.performed += SprintActionPerformed;
        playerInputActions.Player.Sprint.canceled += SprintActionCancelled;

        //playerInputActions.Player.OpenInventory.performed += InventoryOpened;
        

        //playerInputActions.Player.Interact.performed += InteractActionPerformed;
       

       
    }

    private void UnsubscribeInputActions()
    {
        // It is important to unbind and actions that we bind
        // when our object is destroyed, or this can cause issues
        playerInputActions.Player.P1Move.started -= P1MoveAction;
        playerInputActions.Player.P1Move.performed -= P1MoveAction;
        playerInputActions.Player.P1Move.canceled -= P1MoveAction;

        playerInputActions.Player.P2Move.started -= P2MoveAction;
        playerInputActions.Player.P2Move.performed -= P2MoveAction;
        playerInputActions.Player.P2Move.canceled -= P2MoveAction;

        playerInputActions.Player.Jump.performed -= JumpActionPerformed;
        playerInputActions.Player.Jump.canceled -= JumpActionCanceled;

        //playerInputActions.Player.TogglePause.performed -= TogglePauseActionPerformed;
        //playerInputActions.UI.TogglePause.performed -= TogglePauseActionPerformed;

        playerInputActions.Player.Sprint.performed -= SprintActionPerformed;
        playerInputActions.Player.Sprint.canceled -= SprintActionCancelled;

        //playerInputActions.Player.OpenInventory.performed -= InventoryOpened;
        //playerInputActions.Inventory.Exit.performed -= InventoryClosed;

        //playerInputActions.Player.Interact.performed -= InteractActionPerformed;
       


        
    }

    /// <summary>
    /// Helper function to switch to a particular action map
    /// in our player's Input Actions Asset.
    /// </summary>
    /// <param name="mapName">The name of the map we want to switch to.</param>
    private void SwitchActionMap(EPlayerState state)
    {
        playerInputActions.Player.Disable();
        playerInputActions.UI.Disable();
       

        switch (state)
        {
            case EPlayerState.Moving:
                // We need to enable our "Player" action map so Unity will listen for our player input.
                playerInputActions.Player.Enable();

                // Since we are switching into gameplay, we will no longer need control of our mouse cursor
                UnityEngine.Cursor.visible = false;
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                break;

            case EPlayerState.Paused:
                // We need to enable our "UI" action map so Unity will listen for our UI input.
                playerInputActions.UI.Enable();

                // Since we are switching into a UI, we will also need control of our mouse cursor
                UnityEngine.Cursor.visible = true;
                UnityEngine.Cursor.lockState = CursorLockMode.None;
                break;


            default:
                // Show the mouse cursor
                UnityEngine.Cursor.visible = true;
                UnityEngine.Cursor.lockState = CursorLockMode.None;
                break;
        }
    }

    #endregion

    #region Input Actions

    private void P1MoveAction(InputAction.CallbackContext context)
    {
        // Read in the Vector2 of our player input.
        P1MovementInput = context.ReadValue<Vector2>();

        //Debug.Log("The player is trying to P1Move: " + P1MovementInput);

        baseMovement.SetP1MovementInput(P1MovementInput);
    }

    private void P2MoveAction(InputAction.CallbackContext context)
    {
        // Read in the Vector2 of our player input.
        Vector2 P2MovementInput = context.ReadValue<Vector2>();

        //Debug.Log("The player is trying to P2Move: " + P2MovementInput);

        baseMovement.SetP2MovementInput(P2MovementInput);
    }

    private void JumpActionPerformed(InputAction.CallbackContext context)
    {
        //Debug.Log("The player is trying to Jump!");

        baseMovement.Jump();
    }

    private void JumpActionCanceled(InputAction.CallbackContext context)
    {
        //Debug.Log("The player is trying to Stop Jumping!");

        baseMovement.CancelJump();
    }

    private void TogglePauseActionPerformed(InputAction.CallbackContext context)
    {
        //Debug.Log("The player is trying to pause/unpause!");

        GameManager.Instance.TogglePause();
    }

    private void SprintActionPerformed(InputAction.CallbackContext context)
    {
        //Debug.Log("Player is starting to sprint");
        baseMovement.StartSprinting();
    }

    private void SprintActionCancelled(InputAction.CallbackContext context)
    {
        //Debug.Log("Player is cancelling sprint");
        baseMovement.StopSprinting();
    }

    #endregion
    #region Inventory Actions

    //private void InventoryOpened(InputAction.CallbackContext context)
    //{
    //    //InventoryUI.enabled = true;
    //    InventoryUI.rootVisualElement.style.display = DisplayStyle.Flex;
    //    SwitchActionMap(EPlayerState.Inventory);
    //}
    //private void InventoryClosed(InputAction.CallbackContext context)
    //{
    //    InventoryUI.rootVisualElement.style.display = DisplayStyle.None;
    //    SwitchActionMap(EPlayerState.Moving);
    //}

    //private void InteractActionPerformed(InputAction.CallbackContext context)
    //{
    //    playerInteractManager.Interact();
    //}

   

    #endregion

    #region Callbacks

    private void OnGamePausedReceived()
    {
        SwitchActionMap(EPlayerState.Paused);
    }

    private void OnGameResumedReceived()
    {
        SwitchActionMap(EPlayerState.Moving);
    }

    

    

    #endregion

    #endregion
}
