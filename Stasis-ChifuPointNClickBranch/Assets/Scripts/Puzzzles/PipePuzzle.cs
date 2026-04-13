using System.Collections.Generic;
using UnityEngine;

/// <summary>Вариант «перетаскивание кусков». Новая головоломка (сетка, поворот по клику, A→B):
/// см. PipeGridPuzzle, PipeGridCell в папке PipePuzzle/ и README_PipePuzzle_Variants.txt</summary>
public class PipePuzzle : MonoBehaviour
{
    public List<GameObject> pieces;

    // Логика проверки решения — реализуем позже
    public bool IsSolved()
    {
        return false;
    }
}
