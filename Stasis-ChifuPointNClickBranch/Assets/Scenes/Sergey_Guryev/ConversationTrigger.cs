using UnityEngine;
using DialogueEditor;
using System;

public class ConversationTrigger : MonoBehaviour //триггер для диалога
{
    public NPCConversation conversation;
    public int index;

    // Событие для доставки индекса сохранённого диалога, нужно подписать Stats на это
    public static event Action<int> ConvSave;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Здесь также проверка на прочитанный диалог
        if (other.CompareTag("Player") && !Stats.Instance.conversationSaves[index])
        {
            ConversationManager.Instance.StartConversation(conversation);
            OnConvSave(index);
            Destroy(gameObject);
        }
    }

    // Для сохранения диалогов
    // Может вызываться как в своём объекте, так и из других
    public static void OnConvSave(int convIndex)
    {
        ConvSave?.Invoke(convIndex);
    }
}