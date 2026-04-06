using UnityEngine;
using UnityEngine.UI;

public class BootRepairItem : MonoBehaviour
{
    [Tooltip("Уникальный ID материала (0-8). Правильные материалы: ID 0-3")]
    public int itemId;

    [HideInInspector] public Image img;
    [HideInInspector] public bool isInDrawer = true;

    private Button button;
    private Color normalColor = Color.white;

    private void Awake()
    {
        img = GetComponent<Image>();
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        if (!isInDrawer) return;
        BootRepairPuzzleController.Instance.SelectItem(this);
    }

    public void SetSelected(bool selected)
    {
        img.color = selected ? new Color(1f, 1f, 0.6f) : normalColor;
    }

    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }
}
