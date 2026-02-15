using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeMachineLever : MonoBehaviour
{
    private PnC_InteractiveItem item;

    public Puzzle congratsPanel;

    void Start()
    {
        item = GetComponent<PnC_InteractiveItem>();

        // Настройка observeEvent программно (для тестирования)
        item.observeEvent.RemoveAllListeners();
        item.observeEvent.AddListener(CompleteScene);

        Debug.Log("[TimeMachineLever] observeEvent configured to call CompleteScene");
    }

    public void TurnLever()
    {
        if (Stats.Instance.isDocsPicked && Stats.Instance.isMapSolved)
        {
            item.CallPuzzle(congratsPanel);
        }
    }

    /// <summary>
    /// Тестовый метод для сохранения прогресса и перехода на следующую сцену.
    /// Вызывается при клике на левую кнопку (WatchButton).
    /// </summary>
    public void CompleteScene()
    {
        Debug.Log("[TimeMachineLever] CompleteScene called - marking scene as completed");

        // Сохранение прогресса
        GameSaveManager.Instance.MarkSceneCompleted("TimeMachine");

        Debug.Log("[TimeMachineLever] Loading scene: OurCabinet");

        // Переход на следующую сцену
        SceneManager.LoadScene("OurCabinet");
    }
}
