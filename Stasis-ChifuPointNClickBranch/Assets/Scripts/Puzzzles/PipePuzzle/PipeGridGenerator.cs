using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Создаёт сетку ячеек (Image + PipeGridCell) в редакторе одной командой.
/// Вешается на тот же объект, что и PipeGridPuzzle (обычно PipeGridField).
/// </summary>
[RequireComponent(typeof(RectTransform))]
[DisallowMultipleComponent]
public class PipeGridGenerator : MonoBehaviour
{
    [Min(1)] public int columns = 8;
    [Min(1)] public int rows = 5;
    public Vector2 cellSize = new Vector2(90f, 90f);
    public Vector2 spacing = new Vector2(8f, 8f);
    [Tooltip("Масштаб только спрайта трубы (клетка остаётся того же размера; линии станут толще/крупнее).")]
    [Min(0.1f)] public float pipeGraphicScale = 1.6f;
    [Tooltip("Спрайт по умолчанию для всех клеток (можно потом поменять на каждой).")]
    public Sprite cellSprite;
    [Tooltip("Empty — по умолчанию для «пустой» клетки (совпадает с pipe_empty); смените на Straight/Corner и т.д. для другого дефолта.")]
    public PipeShapeKind defaultShape = PipeShapeKind.Empty;
    [Range(0, 3)] public int defaultRotationQuarters = 0;
    public bool randomizeStartRotation = false;
    [Tooltip("Заполняется автоматически с этого же объекта, если пусто.")]
    public PipeGridPuzzle pipeGridPuzzle;

#if UNITY_EDITOR
    [ContextMenu("Удалить детей и сгенерировать сетку")]
    private void Editor_RegenerateGrid()
    {
        Undo.RegisterFullObjectHierarchyUndo(gameObject, "Pipe Grid Generate");

        for (int i = transform.childCount - 1; i >= 0; i--)
            Undo.DestroyObjectImmediate(transform.GetChild(i).gameObject);

        var glg = GetComponent<GridLayoutGroup>();
        if (glg == null)
            glg = Undo.AddComponent<GridLayoutGroup>(gameObject);
        else
            Undo.RecordObject(glg, "Pipe Grid Layout");

        glg.cellSize = cellSize;
        glg.spacing = spacing;
        glg.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        glg.constraintCount = columns;
        glg.startCorner = GridLayoutGroup.Corner.LowerLeft;
        glg.startAxis = GridLayoutGroup.Axis.Horizontal;
        glg.childAlignment = TextAnchor.MiddleCenter;
        glg.padding = new RectOffset(12, 12, 12, 12);

        System.Random rnd = randomizeStartRotation ? new System.Random() : null;
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                int rot = randomizeStartRotation ? rnd.Next(0, 4) : defaultRotationQuarters;
                Editor_CreateCell(x, y, rot);
            }
        }

        var puzzle = pipeGridPuzzle != null ? pipeGridPuzzle : GetComponent<PipeGridPuzzle>();
        if (puzzle != null)
        {
            Undo.RecordObject(puzzle, "Pipe Grid Endpoints");
            puzzle.SetEndpoints(new Vector2Int(0, 0), new Vector2Int(columns - 1, rows - 1));
            EditorUtility.SetDirty(puzzle);
        }

        EditorUtility.SetDirty(this);
        if (glg != null)
            EditorUtility.SetDirty(glg);
    }

    private void Editor_CreateCell(int x, int y, int rot)
    {
        var go = new GameObject($"Cell_{x}_{y}", typeof(RectTransform));
        Undo.RegisterCreatedObjectUndo(go, "Pipe Cell");
        go.transform.SetParent(transform, false);

        var bg = go.AddComponent<Image>();
        bg.sprite = cellSprite;
        bg.color = new Color(1f, 1f, 1f, 0f);
        bg.raycastTarget = true;

        var pipeGo = new GameObject("PipeGraphic", typeof(RectTransform));
        Undo.RegisterCreatedObjectUndo(pipeGo, "Pipe Graphic");
        pipeGo.transform.SetParent(go.transform, false);
        var pipeRt = pipeGo.GetComponent<RectTransform>();
        pipeRt.anchorMin = Vector2.zero;
        pipeRt.anchorMax = Vector2.one;
        pipeRt.offsetMin = Vector2.zero;
        pipeRt.offsetMax = Vector2.zero;
        pipeRt.pivot = new Vector2(0.5f, 0.5f);
        pipeRt.localScale = Vector3.one;

        var pipeImg = pipeGo.AddComponent<Image>();
        pipeImg.sprite = cellSprite;
        pipeImg.color = Color.white;
        pipeImg.raycastTarget = false;
        pipeImg.preserveAspect = false;

        var cell = go.AddComponent<PipeGridCell>();
        cell.Configure(new Vector2Int(x, y), defaultShape, rot, pipeGraphicScale, pipeRt);
        EditorUtility.SetDirty(cell);
    }
#endif
}
