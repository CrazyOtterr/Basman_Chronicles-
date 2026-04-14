using System.Collections.Generic;
using DialogueEditor;
using UnityEngine;

public class MapPuzzle : MonoBehaviour
{
    public List<GameObject> pieces;
    public int pieceCounter;

    public NPCConversation conversation; // диалог после сборки

    public bool IsAllPiecesCorrect()
    {
        if (pieceCounter == pieces.Count)
        {
            ConversationManager.Instance.StartConversation(conversation);
            return true;
        }
        return false;
    }
}
