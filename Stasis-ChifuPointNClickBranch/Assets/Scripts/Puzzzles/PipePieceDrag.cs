using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Drag & drop для кусков труб. В отличие от PieceDrag — нет привязки к target,
/// куски всегда можно перетаскивать.
/// </summary>
public class PipePieceDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rect;
    private Canvas canvas;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        rect.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Нет snap — кусок остаётся там, где отпустили
    }
}
