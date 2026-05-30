using TMPro;
using UnityEngine;

public class ChatUI : MonoBehaviour
{
    [SerializeField] private ChatMessage chatMessagePrefab;
    [SerializeField] private Transform chatContent;
    [SerializeField] private TMP_InputField chatInput;

    private string myName;
    private string myTeam;

    void OnEnable()
    {
        ChatManager.OnMessageReceived += AddMessage;
        myName = ChatManager.Singleton.playerName;
    }

    void OnDisable()
    {
        ChatManager.OnMessageReceived -= AddMessage;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            
            ChatManager.Singleton.SendChatMessage(chatInput.text,myName);

            chatInput.text = "";
                
            
           
        }
    }

    void AddMessage(string msg)
    {
        ChatMessage cm =
            Instantiate(chatMessagePrefab, chatContent);

        cm.SetText(msg);
    }
}