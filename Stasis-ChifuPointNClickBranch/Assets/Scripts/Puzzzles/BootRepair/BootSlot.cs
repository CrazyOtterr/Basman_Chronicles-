using UnityEngine;
using UnityEngine.UI;

public class BootSlot : MonoBehaviour
{
    [Tooltip("Индекс слота (0-3). Правильный материал: itemId == slotIndex")]
    public int slotIndex;

    [HideInInspector] public Image img;
    [HideInInspector] public Image slotOverlay;
    [HideInInspector] public BootRepairItem placedItem;
    [HideInInspector] public bool isLocked;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        img = GetComponent<Image>();
        slotOverlay = transform.childCount > 0 ? transform.GetChild(0).GetComponent<Image>() : null;
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        if (isLocked) return;
        BootRepairPuzzleController.Instance.PlaceItemInSlot(this);
    }

    public void SetItem(BootRepairItem item)
    {
        placedItem = item;
        if (slotOverlay != null && item != null)
        {
            slotOverlay.sprite = item.img.sprite;
            slotOverlay.color = Color.white;
            slotOverlay.enabled = true;
        }
    }

    public void ClearSlot()
    {
        placedItem = null;
        isLocked = false;
        img.color = new Color(1f, 1f, 1f, 0.2f);
        if (slotOverlay != null)
        {
            slotOverlay.sprite = null;
            slotOverlay.color = Color.clear;
            slotOverlay.enabled = false;
        }
    }

    public void ShowFeedback(Color color)
    {
        img.color = color;
    }
}
