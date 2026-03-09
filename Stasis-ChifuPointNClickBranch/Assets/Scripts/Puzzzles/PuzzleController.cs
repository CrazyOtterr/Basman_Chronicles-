using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PuzzleController : MonoBehaviour
{
    public static PuzzleController Instance;

    public TMP_Text resultText;
    public Button checkButton;
    public GameObject puzzleWindow;
    public Color[] pairColors = new Color[3];

    private Folder[] folderSlots = new Folder[3];
    private Card[] cardSlots = new Card[3];
    private int pairCount = 0;

    private Folder currentFolder;
    private Card currentCard;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        checkButton.onClick.AddListener(OnCheck);
        if (resultText != null) resultText.gameObject.SetActive(false);
    }

    // ------------------------
    // Выбор папки
    // ------------------------
    public void SelectFolder(Folder folder)
    {
        // если нажали на папку, которая в слоте — удаляем соответствующую пару
        int slot = FindFolderSlot(folder);
        if (slot != -1)
        {
            RemovePair(slot);
            return;
        }

        // если игрок до этого выбрал другую папку для текущего формирования пары — сбросить её
        if (currentFolder != null && currentFolder != folder)
        {
            currentFolder.img.color = Color.white;
            currentFolder = null;
        }

        // если эта папка уже отмечена как wasUsed (редкий случай) — снять её из старого слота
        if (folder.wasUsed)
        {
            int old = FindFolderSlot(folder);
            if (old != -1) RemovePair(old);
        }

        // если выбранная карточка уже используется в одном из слотов, мы не будем связывать с ней:
        // но если текущая карточка (для новой пары) стоит в слоте, снимем её выбор
        if (currentCard != null && currentCard.wasUsed)
        {
            currentCard.img.color = Color.white;
            currentCard = null;
        }

        // присвоить как текущую папку и подсветить цветом следующего слота
        currentFolder = folder;
        int colorIndex = Mathf.Clamp(pairCount, 0, pairColors.Length - 1);
        currentFolder.img.color = pairColors[colorIndex];

        TryCompletePair();
    }

    // ------------------------
    // Выбор карточки
    // ------------------------
    public void SelectCard(Card card)
    {
        int slot = FindCardSlot(card);
        if (slot != -1)
        {
            RemovePair(slot);
            return;
        }

        if (currentCard != null && currentCard != card)
        {
            currentCard.img.color = Color.white;
            currentCard = null;
        }

        if (card.wasUsed)
        {
            int old = FindCardSlot(card);
            if (old != -1) RemovePair(old);
        }

        if (currentFolder != null && currentFolder.wasUsed)
        {
            currentFolder.img.color = Color.white;
            currentFolder = null;
        }

        currentCard = card;
        int colorIndex = Mathf.Clamp(pairCount, 0, pairColors.Length - 1);
        currentCard.img.color = pairColors[colorIndex];

        TryCompletePair();
    }

    // ------------------------
    // Попытка завершить пару
    // ------------------------
    private void TryCompletePair()
    {
        if (currentFolder == null || currentCard == null) return;
        if (pairCount >= 3) return;

        // Безопасность: если какой-либо из элементов вдруг уже в слоте — удаляем старые слоты сначала
        int fSlot = FindFolderSlot(currentFolder);
        if (fSlot != -1) RemovePair(fSlot);
        int cSlot = FindCardSlot(currentCard);
        if (cSlot != -1) RemovePair(cSlot);

        // фиксируем в слот pairCount
        folderSlots[pairCount] = currentFolder;
        cardSlots[pairCount] = currentCard;
        folderSlots[pairCount].wasUsed = true;
        cardSlots[pairCount].wasUsed = true;

        // убедиться, что их цвет — цвет этого слота
        int colorIndex = Mathf.Clamp(pairCount, 0, pairColors.Length - 1);
        folderSlots[pairCount].img.color = pairColors[colorIndex];
        cardSlots[pairCount].img.color = pairColors[colorIndex];

        pairCount++;

        // очистить текущие выборы для новой пары
        currentFolder = null;
        currentCard = null;
    }

    // ------------------------
    // Найти слот по элементу
    // ------------------------
    private int FindFolderSlot(Folder f)
    {
        for (int i = 0; i < pairCount; i++)
            if (folderSlots[i] == f) return i;
        return -1;
    }

    private int FindCardSlot(Card c)
    {
        for (int i = 0; i < pairCount; i++)
            if (cardSlots[i] == c) return i;
        return -1;
    }

    // ------------------------
    // Удаление пары по индексу
    // ------------------------
    private void RemovePair(int index)
    {
        if (index < 0 || index >= pairCount) return;

        // сброс флагов и цветов
        if (folderSlots[index] != null)
        {
            folderSlots[index].wasUsed = false;
            folderSlots[index].img.color = Color.white;
        }
        if (cardSlots[index] != null)
        {
            cardSlots[index].wasUsed = false;
            cardSlots[index].img.color = Color.white;
        }

        // удаляем слот и сдвигаем остальные влево
        for (int i = index; i < pairCount - 1; i++)
        {
            folderSlots[i] = folderSlots[i + 1];
            cardSlots[i] = cardSlots[i + 1];
        }
        // очистить последний
        folderSlots[pairCount - 1] = null;
        cardSlots[pairCount - 1] = null;
        pairCount--;

        // пересчитать цвета для всех существующих слотов
        for (int i = 0; i < pairCount; i++)
        {
            int colorIndex = Mathf.Clamp(i, 0, pairColors.Length - 1);
            if (folderSlots[i] != null) folderSlots[i].img.color = pairColors[colorIndex];
            if (cardSlots[i] != null) cardSlots[i].img.color = pairColors[colorIndex];
        }

        // если до удаления у игрока был currentFolder/currentCard — они должны быть сброшены,
        // чтобы не получить ситуацию "один текущий + слоты с цветом"
        if (currentFolder != null)
        {
            currentFolder.img.color = Color.white;
            currentFolder = null;
        }
        if (currentCard != null)
        {
            currentCard.img.color = Color.white;
            currentCard = null;
        }
    }

    // ------------------------
    // Кнопка проверки
    // ------------------------
    private void OnCheck()
    {Debug.Log(" check starts");
        if (pairCount < 3)
        {
            ShowMessage("Выбрано не три пары.");
            return;
        }

        // проверка — использую ту же логику перестановки, что и у тебя
        bool correct =
            folderSlots[0].id == cardSlots[2].id &&
            folderSlots[1].id == cardSlots[1].id &&
            folderSlots[2].id == cardSlots[0].id;

        if (correct)
        {
            ShowMessage("Правильно!");
            Debug.Log("correct check");
            //StartCoroutine(CloseAfterDelay(5f));
        }
        else
        {            Debug.Log("wrong check");
            ShowMessage("Неверно. Попробуй снова.");
            ResetPuzzle();
        }
    }

    private IEnumerator CloseAfterDelay(float t)
    {
        yield return new WaitForSeconds(t);
        Debug.Log("Закрыто");
        //ClosePuzzle();
    }

    private void ClosePuzzle()
    {
        puzzleWindow.SetActive(false);
    }

    // ------------------------
    // Полный сброс
    // ------------------------
    private void ResetPuzzle()
    {
        for (int i = 0; i < pairCount; i++)
        {
            if (folderSlots[i] != null)
            {
                folderSlots[i].wasUsed = false;
                folderSlots[i].img.color = Color.white;
                folderSlots[i] = null;
            }
            if (cardSlots[i] != null)
            {
                cardSlots[i].wasUsed = false;
                cardSlots[i].img.color = Color.white;
                cardSlots[i] = null;
            }
        }

        pairCount = 0;

        // также сбросить любые текущие выборы
        if (currentFolder != null)
        {
            currentFolder.img.color = Color.white;
            currentFolder = null;
        }
        if (currentCard != null)
        {
            currentCard.img.color = Color.white;
            currentCard = null;
        }

        if (resultText != null) resultText.gameObject.SetActive(false);
    }

    private void ShowMessage(string msg)
    {
        if (resultText == null) return;
        resultText.text = msg;
        resultText.gameObject.SetActive(true);
    }
}
