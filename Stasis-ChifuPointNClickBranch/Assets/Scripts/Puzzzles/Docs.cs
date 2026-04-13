using DialogueEditor;
using UnityEngine;

public class Docs : MonoBehaviour
{
    public NPCConversation conversation; // הטאכמד ןמסכו ןמהבמנא
    void Start()
    {
        if (Stats.Instance != null)
            gameObject.SetActive(!Stats.Instance.isDocsPicked);
    }

    public void PutDocs()
    {
        if (Stats.Instance != null)
            Stats.Instance.isDocsPicked = true;
        ConversationManager.Instance.StartConversation(conversation);
        Destroy(gameObject);
    }
}
