using UnityEngine;
using UnityEngine.EventSystems;

public class PieceDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Настройки")]
    public RectTransform target; // сюда привязываем правильную позицию
    public float snapDistance = 50f; // расстояние магнита

    private MapPuzzle mapPuzzle;
    private Puzzle currentPuzzleWindow;
    private Stats playerStats;

    private RectTransform rect;
    private Canvas canvas;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        mapPuzzle = FindFirstObjectByType<MapPuzzle>();
        currentPuzzleWindow = FindFirstObjectByType<Puzzle>();
        playerStats = FindFirstObjectByType<Stats>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Чтобы быть поверх других
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Перемещение вместе с курсором
        rect.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float dist = Vector2.Distance(rect.anchoredPosition, target.anchoredPosition);

        if (dist < snapDistance)
        {
            // Автоприцепление
            rect.anchoredPosition = target.anchoredPosition;
            enabled = false; // запретить дальнейшее перетягивание

            mapPuzzle.pieceCounter++;

            Debug.Log($"Карта: {mapPuzzle.IsAllPiecesCorrect()}");

            // Проверка на всю собранную карту
            if (mapPuzzle.IsAllPiecesCorrect())
            {
                currentPuzzleWindow.Disable();
                playerStats.isMapSolved = true;
            }
                
        }
    }
}
