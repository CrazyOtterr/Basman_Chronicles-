using System.Collections.Generic;
using UnityEngine;

public class MapPuzzle : MonoBehaviour
{
    public List<GameObject> pieces;
    public int pieceCounter;

    public bool IsAllPiecesCorrect()
    {
        if (pieceCounter == pieces.Count)
        {
            return true;
        }
        return false;
    }
}
