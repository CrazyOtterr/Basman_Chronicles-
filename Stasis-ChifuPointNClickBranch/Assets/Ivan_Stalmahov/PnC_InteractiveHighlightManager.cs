using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// При запуске сцены находит все объекты с PnC_InteractiveItem, создаёт для каждого
/// белый полупрозрачный дубликат спрайта и показывает его при наведении курсора.
/// Добавь этот скрипт на любой GameObject в сцене (например, пустой "HighlightManager").
/// </summary>
public class PnC_InteractiveHighlightManager : MonoBehaviour
{
    [Tooltip("Прозрачность подсветки (0–1).")]
    [Range(0.1f, 0.9f)]
    [SerializeField] private float highlightAlpha = 0.45f;

    [Tooltip("Смещение порядка отрисовки подсветки относительно спрайта (подсветка поверх).")]
    [SerializeField] private int sortingOrderOffset = 1;

    private struct ItemHighlight
    {
        public PnC_InteractiveItem item;
        public GameObject highlightObject;
    }

    private List<ItemHighlight> _highlights = new List<ItemHighlight>();

    private void Start()
    {
        PnC_InteractiveItem[] items = FindObjectsByType<PnC_InteractiveItem>(FindObjectsSortMode.None);
        foreach (PnC_InteractiveItem item in items)
        {
            CreateHighlightFor(item);
        }
    }

    private void CreateHighlightFor(PnC_InteractiveItem item)
    {
        SpriteRenderer sourceRenderer = item.GetComponentInChildren<SpriteRenderer>();
        if (sourceRenderer == null || sourceRenderer.sprite == null)
            return;

        // Ведём подсветку дочерним к тому же объекту, где висит спрайт — тогда она точно по контуру, не вылезает за объект
        GameObject highlightObj = new GameObject("Highlight");
        highlightObj.transform.SetParent(sourceRenderer.transform, false);
        highlightObj.transform.localPosition = Vector3.zero;
        highlightObj.transform.localScale = Vector3.one;
        highlightObj.transform.localRotation = Quaternion.identity;

        SpriteRenderer highlightRenderer = highlightObj.AddComponent<SpriteRenderer>();
        highlightRenderer.sprite = sourceRenderer.sprite;
        highlightRenderer.color = new Color(1f, 1f, 1f, highlightAlpha);
        highlightRenderer.sortingLayerID = sourceRenderer.sortingLayerID;
        highlightRenderer.sortingOrder = sourceRenderer.sortingOrder + sortingOrderOffset;
        highlightRenderer.drawMode = sourceRenderer.drawMode;
        highlightRenderer.size = sourceRenderer.size;
        highlightRenderer.enabled = false;

        _highlights.Add(new ItemHighlight { item = item, highlightObject = highlightObj });
    }

    private void Update()
    {
        List<GameObject> underMouse = ScreenUtils.GetObjectsUnderMouse();
        foreach (var h in _highlights)
        {
            if (h.highlightObject == null) continue;
            bool hover = underMouse.Contains(h.item.gameObject);
            h.highlightObject.GetComponent<SpriteRenderer>().enabled = hover;
        }
    }
}
