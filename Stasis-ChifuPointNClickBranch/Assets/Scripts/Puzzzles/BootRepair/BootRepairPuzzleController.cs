using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BootRepairPuzzleController : MonoBehaviour
{
    public static BootRepairPuzzleController Instance;

    [Header("UI")]
    public Button checkButton;
    public TMP_Text resultText;

    [Header("Предметы в ящике (9 штук, правильные: ID 0-3)")]
    public BootRepairItem[] drawerItems;

    [Header("Ячейки на сапоге (4 штуки)")]
    public BootSlot[] bootSlots;

    [Header("Цвета обратной связи")]
    public Color colorCorrect = new Color(0.2f, 0.8f, 0.2f, 0.7f);
    public Color colorWrongSlot = new Color(0.9f, 0.9f, 0.1f, 0.7f);
    public Color colorWrongItem = new Color(0.9f, 0.2f, 0.2f, 0.7f);

    private BootRepairItem selectedItem;
    private int lockedCount;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        checkButton.onClick.AddListener(OnCheck);
        if (resultText != null) resultText.gameObject.SetActive(false);
    }

    // Игрок кликнул на предмет в ящике
    public void SelectItem(BootRepairItem item)
    {
        if (selectedItem != null)
            selectedItem.SetSelected(false);

        selectedItem = item;
        selectedItem.SetSelected(true);
    }

    // Игрок кликнул на ячейку сапога
    public void PlaceItemInSlot(BootSlot slot)
    {
        if (selectedItem == null) return;
        if (slot.isLocked) return;

        // Если в слоте уже есть предмет — вернуть его в ящик
        if (slot.placedItem != null)
        {
            ReturnItemToDrawer(slot.placedItem);
            slot.ClearSlot();
        }

        // Поместить выбранный предмет в слот
        slot.SetItem(selectedItem);
        selectedItem.isInDrawer = false;
        selectedItem.SetSelected(false);
        selectedItem.SetVisible(false);
        selectedItem = null;

        HideMessage();
    }

    // Кнопка "Проверить"
    private void OnCheck()
    {
        // Проверить, что все незалоченные слоты заполнены
        int filledCount = 0;
        for (int i = 0; i < bootSlots.Length; i++)
        {
            if (bootSlots[i].isLocked || bootSlots[i].placedItem != null)
                filledCount++;
        }

        if (filledCount < bootSlots.Length)
        {
            ShowMessage("Заполни все ячейки!");
            return;
        }

        StartCoroutine(CheckWithFeedback());
    }

    private IEnumerator CheckWithFeedback()
    {
        checkButton.interactable = false;
        bool allCorrect = true;

        for (int i = 0; i < bootSlots.Length; i++)
        {
            if (bootSlots[i].isLocked) continue;

            BootRepairItem item = bootSlots[i].placedItem;
            if (item == null) continue;

            bool isCorrectItem = item.itemId >= 0 && item.itemId <= 3;
            bool isCorrectSlot = item.itemId == bootSlots[i].slotIndex;

            if (isCorrectSlot)
            {
                // Зеленый — правильный предмет в правильной ячейке
                bootSlots[i].ShowFeedback(colorCorrect);
                bootSlots[i].isLocked = true;
                lockedCount++;
            }
            else if (isCorrectItem)
            {
                // Желтый — правильный предмет, но не в своей ячейке
                bootSlots[i].ShowFeedback(colorWrongSlot);
                allCorrect = false;
            }
            else
            {
                // Красный — вообще не тот предмет
                bootSlots[i].ShowFeedback(colorWrongItem);
                allCorrect = false;
            }
        }

        yield return new WaitForSeconds(1.5f);

        // Вернуть неправильные предметы в ящик
        for (int i = 0; i < bootSlots.Length; i++)
        {
            if (bootSlots[i].isLocked) continue;

            if (bootSlots[i].placedItem != null)
            {
                ReturnItemToDrawer(bootSlots[i].placedItem);
                bootSlots[i].ClearSlot();
            }
        }

        checkButton.interactable = true;

        if (lockedCount >= bootSlots.Length)
        {
            ShowMessage("Сапог починен!");
            OnPuzzleSolved();
        }
        else if (allCorrect)
        {
            // Не должно случиться, но на всякий случай
            ShowMessage("Сапог починен!");
            OnPuzzleSolved();
        }
    }

    private void ReturnItemToDrawer(BootRepairItem item)
    {
        item.isInDrawer = true;
        item.SetVisible(true);
        item.SetSelected(false);
    }

    private void OnPuzzleSolved()
    {
        checkButton.interactable = false;
        Debug.Log("BootRepairPuzzle: Solved!");
        // Здесь можно вызвать Stats или закрыть окно головоломки
        // Например: FindFirstObjectByType<Puzzle>()?.Disable();
    }

    private void ShowMessage(string msg)
    {
        if (resultText == null) return;
        resultText.text = msg;
        resultText.gameObject.SetActive(true);
    }

    private void HideMessage()
    {
        if (resultText == null) return;
        resultText.gameObject.SetActive(false);
    }

    // Полный сброс (можно вызвать извне)
    public void ResetPuzzle()
    {
        for (int i = 0; i < bootSlots.Length; i++)
        {
            if (bootSlots[i].placedItem != null)
                ReturnItemToDrawer(bootSlots[i].placedItem);
            bootSlots[i].ClearSlot();
        }

        if (selectedItem != null)
        {
            selectedItem.SetSelected(false);
            selectedItem = null;
        }

        lockedCount = 0;
        checkButton.interactable = true;
        HideMessage();
    }
}
