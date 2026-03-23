using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelMapManager : MonoBehaviour
{
    private static LevelMapManager instance;
    public static LevelMapManager Instance => instance;

    [Header("Информационная панель")]
    public GameObject infoPanel;
    public TextMeshProUGUI levelNameText;
    public TextMeshProUGUI levelDescriptionText;
    public Button playButton;
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

        if (closeButton != null)
            closeButton.onClick.AddListener(HideInfoPanel);
    }

    public void ShowLevelInfo(ButtonState levelButton)
    {
        if (levelButton == null) return;

        currentSelectedLevel = levelButton;

        if (levelNameText != null)
            levelNameText.text = levelButton.levelDisplayName;

        if (levelDescriptionText != null)
            levelDescriptionText.text = levelButton.levelDescription;

        if (infoPanel != null)
            infoPanel.SetActive(true);
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