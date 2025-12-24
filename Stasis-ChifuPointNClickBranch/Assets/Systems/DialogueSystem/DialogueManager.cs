using UnityEngine;
using DialogueEditor;
using DG.Tweening;

public class DialogueManager : MonoBehaviour
{
    public NPCConversation testConversation;
    public SpriteRenderer globalDialogueBG;

    public void StartConversation()
    {
        ConversationManager.Instance.StartConversation(testConversation);

        globalDialogueBG.DOFade(0.7f, 0.5f).From(0f);
    }

    public void EndConversation()
    {
        globalDialogueBG.DOFade(0f, 0.5f).From(0.7f);
    }
}
