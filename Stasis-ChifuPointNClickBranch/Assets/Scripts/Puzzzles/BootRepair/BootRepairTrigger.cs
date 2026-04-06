using UnityEngine;

public class BootRepairTrigger : MonoBehaviour
{
    private PnC_InteractiveItem item;

    [Tooltip("Ссылка на объект с Puzzle + BootRepairPuzzleController")]
    public Puzzle bootRepairPuzzle;

    void Start()
    {
        item = GetComponent<PnC_InteractiveItem>();

        // Привязка к 3-й кнопке (speakEvent) в меню PnC
        item.speakEvent.RemoveAllListeners();
        item.speakEvent.AddListener(OpenBootRepairPuzzle);
    }

    public void OpenBootRepairPuzzle()
    {
        item.CallPuzzle(bootRepairPuzzle);
    }
}
