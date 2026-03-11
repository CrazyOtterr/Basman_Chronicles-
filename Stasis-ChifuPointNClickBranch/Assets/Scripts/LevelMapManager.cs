using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class LevelMapManager : MonoBehaviour
{
    private static LevelMapManager instance;
    public static LevelMapManager Instance => instance;

    [Header("Информационная панель")]
    public GameObject infoPanel;
    public TextMeshProUGUI levelNameText;
    public TextMeshProUGUI levelDescriptionText;
    public Button playButton;
    public Button completeButton;
    public Button closeButton;

    private ButtonState currentSelectedLevel;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (infoPanel != null)
            infoPanel.SetActive(false);

        if (playButton != null)
            playButton.onClick.AddListener(PlaySelectedLevel);

        if (completeButton != null)
            completeButton.onClick.AddListener(CompleteSelectedLevel);

        if (closeButton != null)
            closeButton.onClick.AddListener(HideInfoPanel);
    }

    public void ShowLevelInfo(ButtonState levelButton) // параметр - выбранная кнопка
    {
        if (levelButton == null) return;

        currentSelectedLevel = levelButton; // сохраняем ссылку на выбранную кнопку

        // Заполняем информацию из кнопки
        if (levelNameText != null)
            levelNameText.text = levelButton.levelDisplayName;

        if (levelDescriptionText != null)
            levelDescriptionText.text = levelButton.levelDescription;

        UpdateButtonsState();

        if (infoPanel != null)
            infoPanel.SetActive(true);
    }

    void UpdateButtonsState()
    {
        if (currentSelectedLevel == null) return;

        var state = currentSelectedLevel.CurrentState;

        if (playButton != null)
            playButton.interactable = (state != LevelStateManager.LevelState.Locked);

        if (completeButton != null)
        {
            completeButton.interactable = (state == LevelStateManager.LevelState.Active);
        }
    }

    void CompleteSelectedLevel()
    {
        if (currentSelectedLevel != null)
        {
            LevelStateManager.Instance?.SetCompleted(currentSelectedLevel.levelSceneName);
            currentSelectedLevel.RefreshState();
            UpdateButtonsState();
            Debug.Log($"Уровень '{currentSelectedLevel.levelDisplayName}' отмечен как выполненный!");
        }
    }

    void HideInfoPanel()
    {
        if (infoPanel != null)
            infoPanel.SetActive(false);
        currentSelectedLevel = null;
    }

    void PlaySelectedLevel()
    {
        if (currentSelectedLevel != null)
        {
            currentSelectedLevel.LoadLevel();
            HideInfoPanel();
        }
    }
}