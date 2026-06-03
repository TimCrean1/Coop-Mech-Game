using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChatUI : MonoBehaviour
{
    [SerializeField] private ChatMessage chatMessagePrefab;
    [SerializeField] private Transform chatContent;
    [SerializeField] private TMP_InputField chatInput;
    [SerializeField] private PlayerInputActions playerInputActions;

    private string myName;
    private string myTeam;
 
    void OnEnable()
    {
        ChatManager.OnMessageReceived += AddMessage;
        myName = ChatManager.Singleton.playerName;
        //playerInputActions.Chat.SendMessage.performed += SendMessage;
        //playerInputActions.Chat.SendMessage.canceled += SendMessage;
        chatInput.onSubmit.AddListener(SubmitChatMessage);
    }

    void OnDisable()
    {
        ChatManager.OnMessageReceived -= AddMessage;
        //playerInputActions.Chat.SendMessage.performed -= SendMessage;
        //playerInputActions.Chat.SendMessage.canceled -= SendMessage;
    }
    private void SubmitChatMessage(string message)
    {
        Debug.Log("Send message: " + chatInput.text);

        ChatManager.Singleton.SendChatMessage(chatInput.text, myName);

        chatInput.text = "";

        chatInput.Select();

    }

    void AddMessage(string msg)
    {
        ChatMessage cm =
            Instantiate(chatMessagePrefab, chatContent);

        cm.SetText(msg);
    }
}