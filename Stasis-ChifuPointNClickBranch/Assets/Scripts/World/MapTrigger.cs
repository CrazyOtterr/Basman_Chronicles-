using DialogueEditor;
using UnityEngine;

public class MapTrigger : MonoBehaviour
{
    public NPCConversation conversation;

    private void Start()
    {
        gameObject.SetActive(!Stats.Instance.conversationSaves[1]);
    }

    public void StartConversation()
    {
        ConversationManager.Instance.StartConversation(conversation);
        Stats.Instance.conversationSaves[1] = true;
    }

    public void DestroyMap()
    {
        Destroy(gameObject);
    }
}
