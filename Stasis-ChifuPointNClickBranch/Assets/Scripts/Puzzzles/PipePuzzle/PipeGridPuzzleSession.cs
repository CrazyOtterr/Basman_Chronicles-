using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Жизненный цикл окна с PipeGrid: при каждом открытии (OnEnable) сбрасывает попытку
/// и опционально повороты к раскладке из сцены. Вешайте на корень окна головоломки (рядом с Puzzle).
/// </summary>
[DefaultExecutionOrder(-50)]
public class PipeGridPuzzleSession : MonoBehaviour
{
    [Tooltip("Пусто — ищется в дочерних объектах.")]
    [SerializeField] private PipeGridPuzzle pipeGridPuzzle;

    [Tooltip("Если включено — при открытии окна трубы возвращаются к углам из сцены (новая попытка). Если выключено — только сбрасывается флаг «решено», повороты остаются.")]
    [SerializeField] private bool resetRotationsWhenWindowOpens = true;

    [Header("События")]
    [SerializeField] private UnityEvent onPipePuzzleWindowOpened;

    private void OnEnable()
    {
        if (pipeGridPuzzle == null)
            pipeGridPuzzle = GetComponentInChildren<PipeGridPuzzle>(true);
        if (pipeGridPuzzle == null)
            return;

        onPipePuzzleWindowOpened?.Invoke();

        if (resetRotationsWhenWindowOpens)
            pipeGridPuzzle.ResetPuzzleToInitialLayout();
        else
            pipeGridPuzzle.ResetSolvedState();
    }
}
