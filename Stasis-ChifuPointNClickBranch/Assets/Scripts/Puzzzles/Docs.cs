using DialogueEditor;
using UnityEngine;

public class Docs : MonoBehaviour
{
    public NPCConversation conversation; // диалог после подбора
    void Start()
    {
        if (Stats.Instance != null)
            gameObject.SetActive(!Stats.Instance.isDocsPicked);
    }

    public void PutDocs()
    {
        if (Stats.Instance != null)
            Stats.Instance.isDocsPicked = true;
        ConversationManager.Instance.StartConversation(conversation); // вкл диалог
        Warden.isAppear = true; // вкл начальника
        WardenNoiseController.Instance.audioSource.Play(); // вкл звук начальника
        Destroy(gameObject);
    }
}
