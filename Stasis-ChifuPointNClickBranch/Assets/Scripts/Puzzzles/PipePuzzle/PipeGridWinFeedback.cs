using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Победа: опционально punch по полю, анимация маркера A по <see cref="PipeGridPuzzle.LastSolvedPath"/>.
/// По умолчанию анимация и punch запускаются после нажатия кнопки под полем (см. requireButtonToPlayWinFeedback).
/// Всё в одном компоненте — меньше «Missing script» из‑за лишних GUID в сцене.
/// </summary>
[RequireComponent(typeof(PipeGridPuzzle))]
public class PipeGridWinFeedback : MonoBehaviour
{
    [Header("Punch по полю")]
    [Tooltip("По умолчанию — этот объект (PipeGridField). Не вешайте PuzzlePanel.")]
    [SerializeField] private RectTransform punchTarget;

    [SerializeField] private float punch = 0.12f;
    [SerializeField] private float duration = 0.45f;

    [Header("Путь A → B")]
    [SerializeField] private bool playPathAnimation = true;

    [Tooltip("Если не задан — при первой победе создаётся Image под сеткой.")]
    [SerializeField] private RectTransform pathMarker;

    [Tooltip("Спрайт точки A (pipe_A). Если пусто — в редакторе подставится из Sprites/PipePuzzle/pipe_A, иначе белый квадрат.")]
    [SerializeField] private Sprite markerSprite;

    [SerializeField] private Vector2 markerSize = new Vector2(40f, 40f);

    [Tooltip("Доп. сдвиг позиции маркера в пространстве anchoredPosition сетки.")]
    [SerializeField] private Vector2 pathMarkerExtraOffset;

    [Tooltip("Сдвинуть маркер вниз на половину высоты клетки (по Grid Layout Group).")]
    [SerializeField] private bool pathMarkerShiftDownHalfCell = true;

    [Tooltip("Общее время пробега по всему пути (сек).")]
    [SerializeField] private float pathDuration = 2.2f;

    [SerializeField] private Color markerColor = new Color(1f, 0.92f, 0.2f, 1f);

    [SerializeField] private Ease pathSegmentEase = Ease.Linear;

    [Tooltip("Спрайт pipe_A смотрит влево (−X). Доп. поворот по Z (градусы), если нужно подогнать арт.")]
    [SerializeField] private float markerHeadingOffsetDegrees;

    [Header("Кнопка после победы")]
    [Tooltip("Если включено — анимация пути и punch только после нажатия кнопки под полем.")]
    [SerializeField] private bool requireButtonToPlayWinFeedback = true;

    [Tooltip("Необязательно: своя кнопка в иерархии. Если пусто — при первой победе создаётся под полем.")]
    [SerializeField] private Button continueAfterWinButton;

    [SerializeField] private string continueButtonLabel = "Продолжить";

    [Tooltip("Зазор между нижним краем клеток и верхним краем кнопки (автокнопка ставится по границам клеток, не по RectTransform поля).")]
    [SerializeField] private float continueButtonGapBelowGrid = 14f;

    [SerializeField] private Vector2 continueButtonSize = new Vector2(220f, 46f);

    private PipeGridPuzzle _puzzle;
    private Vector3 _baseScale;
    private Tween _pathTween;
    private Button _autoCreatedContinueButton;

    private void Awake()
    {
        _puzzle = GetComponent<PipeGridPuzzle>();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (punchTarget == null && transform is RectTransform rt)
            punchTarget = rt;
    }
#endif

    private void Reset()
    {
        punchTarget = transform as RectTransform;
#if UNITY_EDITOR
        if (markerSprite == null)
            markerSprite = AssetDatabase.LoadAssetAtPath<Sprite>(
                "Assets/Sprites/PipePuzzle/pipe_A.png");
#endif
    }

    private void OnEnable()
    {
        if (punchTarget == null)
            punchTarget = transform as RectTransform;
        if (punchTarget != null)
            _baseScale = punchTarget.localScale;
        if (_puzzle != null)
        {
            _puzzle.onPuzzleSolved.AddListener(OnSolved);
            _puzzle.onPuzzleReset.AddListener(OnPuzzleResetHandler);
        }

        WireContinueButton();
        HidePathMarker();
        HideContinueButton();
    }

    private void OnDisable()
    {
        KillPathTween();
        if (_puzzle != null)
        {
            _puzzle.onPuzzleSolved.RemoveListener(OnSolved);
            _puzzle.onPuzzleReset.RemoveListener(OnPuzzleResetHandler);
        }

        var b = GetContinueButton();
        if (b != null)
            b.onClick.RemoveListener(OnContinueAfterWinClicked);
    }

    private void OnPuzzleResetHandler()
    {
        HidePathMarker();
        HideContinueButton();
    }

    private void OnSolved()
    {
        if (requireButtonToPlayWinFeedback)
        {
            EnsureContinueButton();
            ShowContinueButton();
            return;
        }

        PlayWinFeedback();
    }

    private void OnContinueAfterWinClicked()
    {
        var b = GetContinueButton();
        if (b != null)
            b.interactable = false;
        HideContinueButton();
        PlayWinFeedback();
        if (b != null)
            b.interactable = true;
    }

    private void PlayWinFeedback()
    {
        if (playPathAnimation)
            PlayPathAnimation();

        if (punchTarget == null)
            return;
        Transform t = punchTarget;
        t.DOKill(false);
        t.localScale = _baseScale;
        t.DOPunchScale(Vector3.one * punch, duration, 6, 0.4f);
    }

    private Button GetContinueButton()
    {
        return continueAfterWinButton != null ? continueAfterWinButton : _autoCreatedContinueButton;
    }

    private void WireContinueButton()
    {
        if (!requireButtonToPlayWinFeedback)
            return;
        EnsureContinueButton();
        var b = GetContinueButton();
        if (b == null)
            return;
        b.onClick.RemoveListener(OnContinueAfterWinClicked);
        b.onClick.AddListener(OnContinueAfterWinClicked);
    }

    private void EnsureContinueButton()
    {
        if (!requireButtonToPlayWinFeedback)
            return;
        if (continueAfterWinButton != null)
            return;
        if (_autoCreatedContinueButton != null)
            return;

        var parent = punchTarget != null ? punchTarget : transform as RectTransform;
        if (parent == null)
            return;

        var go = new GameObject("PipePuzzleContinueButton", typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.localScale = Vector3.one;
        rt.sizeDelta = continueButtonSize;
        rt.anchoredPosition = Vector2.zero;

        var image = go.AddComponent<Image>();
        image.color = new Color(0.22f, 0.42f, 0.65f, 1f);
        image.raycastTarget = true;

        var btn = go.AddComponent<Button>();
        var colors = btn.colors;
        colors.highlightedColor = new Color(0.32f, 0.55f, 0.82f, 1f);
        colors.pressedColor = new Color(0.18f, 0.35f, 0.52f, 1f);
        btn.colors = colors;

        var textGo = new GameObject("Text", typeof(RectTransform));
        textGo.transform.SetParent(go.transform, false);
        var textRt = textGo.GetComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.offsetMin = Vector2.zero;
        textRt.offsetMax = Vector2.zero;

        var label = textGo.AddComponent<Text>();
        label.text = continueButtonLabel;
        label.alignment = TextAnchor.MiddleCenter;
        label.color = Color.white;
        label.fontSize = 20;
        label.raycastTarget = false;
        var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font != null)
            label.font = font;

        rt.SetAsLastSibling();
        _autoCreatedContinueButton = btn;
        go.SetActive(false);
    }

    private void ShowContinueButton()
    {
        var b = GetContinueButton();
        if (b != null)
        {
            b.interactable = true;
            if (_autoCreatedContinueButton != null)
                LayoutAutoContinueButtonBelowCells();
            b.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// RectTransform поля (punchTarget) часто меньше реальной сетки; ставим кнопку под нижним краем всех клеток.
    /// </summary>
    private void LayoutAutoContinueButtonBelowCells()
    {
        if (_autoCreatedContinueButton == null || punchTarget == null || _puzzle == null)
            return;

        var btnRt = _autoCreatedContinueButton.transform as RectTransform;
        if (btnRt == null)
            return;

        var glg = _puzzle.GetComponentInChildren<GridLayoutGroup>(true);
        if (glg != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(glg.GetComponent<RectTransform>());
        Canvas.ForceUpdateCanvases();

        var field = punchTarget;
        var corners = new Vector3[4];
        var bottomY = float.PositiveInfinity;
        var found = false;
        foreach (var cell in _puzzle.GetComponentsInChildren<PipeGridCell>(true))
        {
            var crt = cell.transform as RectTransform;
            if (crt == null)
                continue;
            crt.GetWorldCorners(corners);
            for (var i = 0; i < 4; i++)
            {
                var ly = field.InverseTransformPoint(corners[i]).y;
                if (ly < bottomY)
                    bottomY = ly;
                found = true;
            }
        }

        if (!found)
            bottomY = field.rect.yMin;

        btnRt.SetParent(field, false);
        btnRt.anchorMin = btnRt.anchorMax = new Vector2(0.5f, 0.5f);
        btnRt.pivot = new Vector2(0.5f, 0.5f);
        btnRt.localScale = Vector3.one;
        btnRt.localRotation = Quaternion.identity;
        btnRt.sizeDelta = continueButtonSize;
        var halfH = continueButtonSize.y * 0.5f;
        btnRt.anchoredPosition = new Vector2(0f, bottomY - continueButtonGapBelowGrid - halfH);
        btnRt.SetAsLastSibling();
    }

    private void HideContinueButton()
    {
        var b = GetContinueButton();
        if (b != null)
            b.gameObject.SetActive(false);
    }

    private void PlayPathAnimation()
    {
        KillPathTween();

        var path = _puzzle.LastSolvedPath;
        if (path == null || path.Count == 0)
            return;

        if (!_puzzle.TryGetCellRectTransform(path[0], out var templateCell))
            return;

        var gridRoot = templateCell.parent as RectTransform;
        if (gridRoot == null)
            return;

        LayoutRebuilder.ForceRebuildLayoutImmediate(gridRoot);

        if (!EnsurePathMarker(gridRoot, templateCell))
            return;

        CopyMarkerLayoutFromCell(templateCell);
        ApplyMarkerSpriteAndSize();

        var pathOffset = ComputePathOffset(gridRoot, templateCell);
        var points = new List<Vector2>(path.Count);
        for (var i = 0; i < path.Count; i++)
        {
            if (!_puzzle.TryGetCellRectTransform(path[i], out var cellRt))
                return;
            points.Add(cellRt.anchoredPosition + pathOffset);
        }

        pathMarker.gameObject.SetActive(true);
        pathMarker.SetAsLastSibling();
        pathMarker.anchoredPosition = points[0];

        if (points.Count == 1)
        {
            pathMarker.localEulerAngles = new Vector3(0f, 0f, markerHeadingOffsetDegrees);
            return;
        }

        var seq = DOTween.Sequence();
        var per = pathDuration / (points.Count - 1);
        for (var seg = 0; seg < points.Count - 1; seg++)
        {
            var z = MarkerRotationZDegrees(points[seg], points[seg + 1]) + markerHeadingOffsetDegrees;
            seq.AppendCallback(() => pathMarker.localEulerAngles = new Vector3(0f, 0f, z));
            seq.Append(pathMarker.DOAnchorPos(points[seg + 1], per).SetEase(pathSegmentEase));
        }

        _pathTween = seq;
    }

    /// <summary>
    /// Спрайт по умолчанию указывает влево (−X). Угол Z в UI: вектор движения в anchored space (Y вверх).
    /// </summary>
    private static float MarkerRotationZDegrees(Vector2 from, Vector2 to)
    {
        var d = to - from;
        if (d.sqrMagnitude < 1e-6f)
            return 0f;
        d.Normalize();
        var angleFromRightDeg = Mathf.Atan2(d.y, d.x) * Mathf.Rad2Deg;
        return angleFromRightDeg - 180f;
    }

    private bool EnsurePathMarker(RectTransform gridRoot, RectTransform templateCell)
    {
        if (pathMarker != null)
        {
            if (pathMarker.parent != gridRoot)
                pathMarker.SetParent(gridRoot, false);
            return true;
        }

        var go = new GameObject("PathWinMarker", typeof(RectTransform));
        go.transform.SetParent(gridRoot, false);
        pathMarker = go.GetComponent<RectTransform>();
        var image = go.AddComponent<Image>();
        image.sprite = markerSprite != null
            ? markerSprite
            : Sprite.Create(
                Texture2D.whiteTexture,
                new Rect(0f, 0f, Texture2D.whiteTexture.width, Texture2D.whiteTexture.height),
                new Vector2(0.5f, 0.5f),
                100f);
        image.color = markerSprite != null ? Color.white : markerColor;
        image.preserveAspect = markerSprite != null;
        image.raycastTarget = false;
        CopyMarkerLayoutFromCell(templateCell);
        pathMarker.sizeDelta = markerSprite != null ? markerSprite.rect.size : markerSize;
        return true;
    }

    private void ApplyMarkerSpriteAndSize()
    {
        if (!pathMarker.TryGetComponent<Image>(out var img))
            return;
        if (markerSprite != null)
        {
            img.sprite = markerSprite;
            img.color = Color.white;
            img.preserveAspect = true;
            pathMarker.sizeDelta = markerSprite.rect.size;
        }
        else
        {
            img.color = markerColor;
            pathMarker.sizeDelta = markerSize;
        }
    }

    private Vector2 ComputePathOffset(RectTransform gridRoot, RectTransform templateCell)
    {
        var delta = pathMarkerExtraOffset;
        if (!pathMarkerShiftDownHalfCell)
            return delta;
        if (gridRoot.TryGetComponent<GridLayoutGroup>(out var glg))
            delta.y -= 0.5f * glg.cellSize.y;
        else
            delta.y -= 0.5f * templateCell.rect.height;
        return delta;
    }

    private void CopyMarkerLayoutFromCell(RectTransform templateCell)
    {
        pathMarker.anchorMin = templateCell.anchorMin;
        pathMarker.anchorMax = templateCell.anchorMax;
        pathMarker.pivot = templateCell.pivot;
        pathMarker.localScale = Vector3.one;
    }

    private void HidePathMarker()
    {
        KillPathTween();
        if (pathMarker != null)
        {
            pathMarker.localRotation = Quaternion.identity;
            pathMarker.gameObject.SetActive(false);
        }
    }

    private void KillPathTween()
    {
        if (_pathTween != null && _pathTween.IsActive())
            _pathTween.Kill();
        _pathTween = null;
        if (pathMarker != null)
            pathMarker.DOKill(false);
    }
}
