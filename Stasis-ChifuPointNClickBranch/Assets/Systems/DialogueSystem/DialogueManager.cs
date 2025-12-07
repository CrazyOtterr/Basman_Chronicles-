using UnityEngine;
using DialogueEditor;

public class DialogueManager : MonoBehaviour
{
    public NPCConversation testConversation;

    public void StartConversation()
    {
        ConversationManager.Instance.StartConversation(testConversation);
    }
}
