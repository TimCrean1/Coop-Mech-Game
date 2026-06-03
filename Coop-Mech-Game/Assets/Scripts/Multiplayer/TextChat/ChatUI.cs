using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChatUI : MonoBehaviour
{
    [SerializeField] private ChatMessage chatMessagePrefab;
    [SerializeField] private Transform chatContent;
    [SerializeField] private TMP_InputField chatInput;
    [SerializeField] private PlayerInputActions playerInputActions;

    private string myName;
    private FixedString32Bytes myTeam;
 
    void OnEnable()
    {
        ChatManager.OnMessageReceived += AddMessage;
        myName = ChatManager.Singleton.playerName;
        
        //chatInput.onSubmit.AddListener(SubmitChatMessage);
    }

    void OnDisable()
    {
        ChatManager.OnMessageReceived -= AddMessage;
        //chatInput.onSubmit.RemoveListener(SubmitChatMessage);
    }
    public void SubmitChatMessage(string message)
    {
        Debug.Log("Send message: " + chatInput.text);

        if(chatInput.text == null)
        {
            Debug.LogError("chatInput could not be read");
        }

        if(chatInput.text == "")
        {
            Debug.Log("Message is empty");
        }

        ChatManager.Singleton.SendChatMessage(chatInput.text, myName);



        chatInput.text = "";

        chatInput.ActivateInputField();

    }

    void AddMessage(string msg)
    {
        ChatMessage cm =
            Instantiate(chatMessagePrefab, chatContent);

        cm.SetText(msg);
    }
}