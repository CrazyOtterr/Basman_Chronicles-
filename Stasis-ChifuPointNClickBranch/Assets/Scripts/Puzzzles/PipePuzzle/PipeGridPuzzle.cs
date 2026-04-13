using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Головоломка: трубы на сетке, поворот по клику.
/// Победа: существует непрерывный путь от Start до End, двигаясь только по открытым
/// направлениям труб (N/E/S/W) и только если у соседа есть вход в стык.
/// При каждом повороте любой клетки вызывается проверка пути (BFS).
/// </summary>
public class PipeGridPuzzle : MonoBehaviour
{
    [SerializeField] private Vector2Int startCell;
    [SerializeField] private Vector2Int endCell;

    [Tooltip("Если включено — при старте сцены сразу проверить путь (например уровень уже собран в редакторе).")]
    [SerializeField] private bool checkWinOnStart;

    /// <summary>Задать A и B (например после генератора сетки).</summary>
    public void SetEndpoints(Vector2Int start, Vector2Int end)
    {
        startCell = start;
        endCell = end;
    }

    [Header("События")]
    [Tooltip("Вызывается один раз, когда найден путь от Start до End по направлениям труб.")]
    public UnityEvent onPuzzleSolved;

    [Tooltip("Вызывается при сбросе победы и пути (новая попытка, повторное открытие окна).")]
    public UnityEvent onPuzzleReset;

    public Vector2Int StartCell => startCell;
    public Vector2Int EndCell => endCell;
    public bool IsSolved => _solved;

    private readonly Dictionary<Vector2Int, PipeGridCell> _cells = new Dictionary<Vector2Int, PipeGridCell>();
    private bool _solved;
    private List<Vector2Int> _lastSolvedPath;
    private bool _loggedMissingEndpoints;

    /// <summary>Клетки пути A→B после последнего успешного решения (для анимации). Пусто, если ещё не решено.</summary>
    public IReadOnlyList<Vector2Int> LastSolvedPath => _lastSolvedPath ?? (IReadOnlyList<Vector2Int>)System.Array.Empty<Vector2Int>();

    public bool TryGetCellRectTransform(Vector2Int gridPosition, out RectTransform rect)
    {
        if (_cells.TryGetValue(gridPosition, out var cell) && cell != null)
        {
            rect = cell.transform as RectTransform;
            return rect != null;
        }

        rect = null;
        return false;
    }

    private void Awake()
    {
        RegisterCellsFromChildren();
        SubscribeAll();
    }

    private void Start()
    {
        if (checkWinOnStart)
            TryEvaluateWinAndNotify();
    }

    private void OnDestroy()
    {
        foreach (var c in _cells.Values)
        {
            if (c != null)
                c.Rotated -= OnCellRotated;
        }
    }

    private void RegisterCellsFromChildren()
    {
        _cells.Clear();
        var found = GetComponentsInChildren<PipeGridCell>(true);
        foreach (var cell in found)
        {
            var key = cell.GridPosition;
            if (_cells.ContainsKey(key))
                Debug.LogWarning($"PipeGridPuzzle: дубликат ячейки {key}", cell);
            else
                _cells[key] = cell;
        }
    }

    private void SubscribeAll()
    {
        foreach (var c in _cells.Values)
        {
            c.Rotated -= OnCellRotated;
            c.Rotated += OnCellRotated;
        }
    }

    private void OnCellRotated(PipeGridCell _)
    {
        TryEvaluateWinAndNotify();
    }

    /// <summary>
    /// Проверяет, есть ли путь A→B по направлениям труб. Если да — фиксирует победу и вызывает onPuzzleSolved (один раз).
    /// Вызывается автоматически после каждого поворота клетки.
    /// </summary>
    public bool TryEvaluateWinAndNotify()
    {
        if (_solved)
            return false;
        if (!TryGetPathFromStartToEnd(out var path))
            return false;
        _lastSolvedPath = path;
        _solved = true;
        onPuzzleSolved?.Invoke();
        return true;
    }

    /// <summary>Только проверка пути без победы и без событий (для отладки/UI).</summary>
    public bool HasValidPathFromStartToEnd()
    {
        return TryGetPathFromStartToEnd(out _);
    }

    /// <summary>Есть ли путь от A к B (устаревшее имя, то же что HasValidPathFromStartToEnd).</summary>
    public bool IsPathFromStartToEnd()
    {
        return TryGetPathFromStartToEnd(out _);
    }

    /// <summary>
    /// BFS по сетке: из клетки можно перейти в соседа только если у текущей трубы открыт выход
    /// в эту сторону и у соседа открыт вход с противоположной стороны.
    /// </summary>
    public bool TryGetPathFromStartToEnd(out List<Vector2Int> path)
    {
        path = null;
        if (!_cells.TryGetValue(startCell, out _) || !_cells.TryGetValue(endCell, out _))
        {
            if (!_loggedMissingEndpoints)
            {
                _loggedMissingEndpoints = true;
                Debug.LogWarning(
                    $"PipeGridPuzzle: старт {startCell} или финиш {endCell} не найдены среди клеток. Проверьте Grid Position и Endpoints.",
                    this);
            }
            return false;
        }

        var q = new Queue<Vector2Int>();
        var parent = new Dictionary<Vector2Int, Vector2Int>();
        q.Enqueue(startCell);
        parent[startCell] = startCell;

        while (q.Count > 0)
        {
            var cur = q.Dequeue();
            if (cur == endCell)
            {
                path = ReconstructPath(parent);
                return true;
            }

            if (!_cells.TryGetValue(cur, out var cell))
                continue;

            int mask = cell.GetOpenSidesMask();
            TryEnqueue(cur, PipeTopology.N, mask, parent, q);
            TryEnqueue(cur, PipeTopology.E, mask, parent, q);
            TryEnqueue(cur, PipeTopology.S, mask, parent, q);
            TryEnqueue(cur, PipeTopology.W, mask, parent, q);
        }

        return false;
    }

    private List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> parent)
    {
        var list = new List<Vector2Int>();
        var v = endCell;
        while (true)
        {
            list.Add(v);
            if (v == startCell)
                break;
            v = parent[v];
        }
        list.Reverse();
        return list;
    }

    private void TryEnqueue(Vector2Int cur, int dirBit, int mask, Dictionary<Vector2Int, Vector2Int> parent, Queue<Vector2Int> q)
    {
        if (!PipeTopology.HasOpenSide(mask, dirBit))
            return;

        var next = Neighbor(cur, dirBit);
        if (!_cells.TryGetValue(next, out var neighborCell))
            return;

        int neighborMask = neighborCell.GetOpenSidesMask();
        int need = PipeTopology.Opposite(dirBit);
        if (!PipeTopology.HasOpenSide(neighborMask, need))
            return;
        if (parent.ContainsKey(next))
            return;

        parent[next] = cur;
        q.Enqueue(next);
    }

    private static Vector2Int Neighbor(Vector2Int c, int dirBit)
    {
        if (dirBit == PipeTopology.N) return new Vector2Int(c.x, c.y + 1);
        if (dirBit == PipeTopology.E) return new Vector2Int(c.x + 1, c.y);
        if (dirBit == PipeTopology.S) return new Vector2Int(c.x, c.y - 1);
        if (dirBit == PipeTopology.W) return new Vector2Int(c.x - 1, c.y);
        return c;
    }

    /// <summary>Сброс флага победы (например, при открытии окна заново).</summary>
    public void ResetSolvedState()
    {
        _solved = false;
        _lastSolvedPath = null;
        onPuzzleReset?.Invoke();
    }

    /// <summary>Сброс победы и поворотов всех клеток к раскладке из сцены (начало попытки).</summary>
    public void ResetPuzzleToInitialLayout()
    {
        foreach (var cell in _cells.Values)
        {
            if (cell != null)
                cell.ResetToInitialRotation();
        }
        ResetSolvedState();
    }

    /// <summary>Пересобрать словарь клеток (если иерархия менялась в рантайме).</summary>
    public void RefreshCellsFromHierarchy()
    {
        foreach (var c in _cells.Values)
        {
            if (c != null)
                c.Rotated -= OnCellRotated;
        }
        RegisterCellsFromChildren();
        SubscribeAll();
        ResetSolvedState();
    }
}
