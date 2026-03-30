using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Одна ячейка поля: труба фиксирована, по клику поворачивается на 90°.
/// Shape = Empty — пустая клетка без направлений, путь через неё не идёт, клик игнорируется.
/// Клетки в координатах Start / End у <see cref="PipeGridPuzzle"/> не крутятся (точки A и B зафиксированы).
/// </summary>
[DisallowMultipleComponent]
public class PipeGridCell : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Vector2Int gridPosition;
    [Tooltip("Empty — нет открытых сторон, по клетке нельзя пройти; спрайт пустой клетки в Image.")]
    [SerializeField] private PipeShapeKind shape = PipeShapeKind.Straight;
    [Tooltip("Текущее число четверть-оборотов по часовой (0–3).")]
    [SerializeField] private int rotationQuarters;
    /// <summary>Снимок угла из сцены при старте (для сброса при повторном открытии окна).</summary>
    private int _initialRotationQuarters;
    [Tooltip("Масштаб только графики трубы (спрайт часто с большими полями в текстуре).")]
    [SerializeField] private float pipeGraphicScale = 1.65f;
    [Tooltip("Если задан — поворот применяется к нему (дочерний Image со спрайтом). Иначе — к корню ячейки.")]
    [SerializeField] private RectTransform pipeGraphicRoot;

    private RectTransform rectTransform;
    private PipeGridPuzzle _puzzle;

    public Vector2Int GridPosition => gridPosition;
    public PipeShapeKind Shape => shape;
    public int RotationQuarters => rotationQuarters;

    public System.Action<PipeGridCell> Rotated;

    private void Awake()
    {
        rectTransform = transform as RectTransform;
        _puzzle = GetComponentInParent<PipeGridPuzzle>();
        if (pipeGraphicRoot == null)
            TryFindPipeGraphicChild();
        _initialRotationQuarters = ((rotationQuarters % 4) + 4) % 4;
        ApplyPipeGraphicScale();
        ApplyVisualRotation();
    }

    /// <summary>Вернуть поворот к значению из сцены (раскладка уровня).</summary>
    public void ResetToInitialRotation()
    {
        rotationQuarters = ((_initialRotationQuarters % 4) + 4) % 4;
        ApplyVisualRotation();
    }

    /// <summary>Зафиксировать текущий угол как «начальный» (после генератора в рантайме и т.п.).</summary>
    public void CaptureCurrentAsInitialRotation()
    {
        _initialRotationQuarters = rotationQuarters;
    }

    public void Configure(Vector2Int pos, PipeShapeKind pipeShape, int startRotationQuarters = 0, float? graphicScale = null, RectTransform pipeGraphicTransform = null)
    {
        gridPosition = pos;
        shape = pipeShape;
        rotationQuarters = ((startRotationQuarters % 4) + 4) % 4;
        if (graphicScale.HasValue)
            pipeGraphicScale = Mathf.Max(0.1f, graphicScale.Value);
        if (pipeGraphicTransform != null)
            pipeGraphicRoot = pipeGraphicTransform;
        if (pipeGraphicRoot == null)
            TryFindPipeGraphicChild();
        _initialRotationQuarters = ((rotationQuarters % 4) + 4) % 4;
        ApplyPipeGraphicScale();
        ApplyVisualRotation();
    }

    private void TryFindPipeGraphicChild()
    {
        var t = transform.Find("PipeGraphic");
        if (t != null)
            pipeGraphicRoot = t as RectTransform;
    }

    private void ApplyPipeGraphicScale()
    {
        if (rectTransform == null)
            rectTransform = transform as RectTransform;
        var target = pipeGraphicRoot != null ? pipeGraphicRoot : rectTransform;
        if (target != null)
            target.localScale = Vector3.one * pipeGraphicScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (shape == PipeShapeKind.Empty)
            return;
        if (_puzzle != null && (gridPosition == _puzzle.StartCell || gridPosition == _puzzle.EndCell))
            return;
        if (_puzzle != null && _puzzle.IsSolved)
            return;
        rotationQuarters = (rotationQuarters + 1) % 4;
        ApplyVisualRotation();
        Rotated?.Invoke(this);
    }

    private void ApplyVisualRotation()
    {
        if (rectTransform == null)
            rectTransform = transform as RectTransform;
        var rotTarget = pipeGraphicRoot != null ? pipeGraphicRoot : rectTransform;
        if (rotTarget != null)
            rotTarget.localRotation = Quaternion.Euler(0f, 0f, -rotationQuarters * 90f);
    }

    /// <summary>Маска открытых сторон в мировых координатах (север = вверх по экрану).</summary>
    public int GetOpenSidesMask()
    {
        return PipeTopology.MaskAfterRotation(shape, rotationQuarters);
    }
}
