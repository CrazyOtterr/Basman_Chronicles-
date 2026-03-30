using UnityEngine;
using DialogueEditor;

public class ConversationTrigger : MonoBehaviour //триггер для диалога
{
    public NPCConversation conversation;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ConversationManager.Instance.StartConversation(conversation);
            Destroy(gameObject);
        }
    }
}