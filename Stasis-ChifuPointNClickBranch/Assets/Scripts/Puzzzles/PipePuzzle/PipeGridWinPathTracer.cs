using UnityEngine;

/// <summary>
/// Заглушка для старых сцен: раньше компонент жил рядом с <see cref="PipeGridWinFeedback"/>.
/// Вся логика перенесена в <see cref="PipeGridWinFeedback"/> — этот компонент можно безопасно удалить с объекта в инспекторе.
/// </summary>
[AddComponentMenu("")]
public class PipeGridWinPathTracer : MonoBehaviour
{
#if UNITY_EDITOR
    private void Reset()
    {
        Debug.Log(
            "PipeGridWinPathTracer устарел: анимация пути A→B теперь в PipeGridWinFeedback. Удали этот компонент с PipeGridField.",
            this);
    }
#endif
}
