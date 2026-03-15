using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

using Image = UnityEngine.UI.Image;

public class ButtonState : MonoBehaviour, IPointerClickHandler
{
    [Header("Настройки уровня")]
    public string levelSceneName = "QuestSceneMain";
    public string levelDisplayName = "Уровень 1";
    [TextArea(2, 3)]
    public string levelDescription = "Описание уровня";

    [Header("Цвета")]
    public Color lockedColor = Color.red;
    public Color activeColor = Color.white;
    public Color completedColor = Color.green;

    private Button button;
    private Image image;
    private LevelStateManager.LevelState currentState;

    void Awake()
    {
        button = GetComponent<Button>();
        image = GetComponent<Image>();
    }

    void Start()
    {
        UpdateColor();
        if (LevelStateManager.Instance != null)
        {
            LevelStateManager.OnLevelStateChanged += OnLevelStateChanged;
        }
    }

    void OnDestroy()
    {
        if (LevelStateManager.Instance != null)
        {
            LevelStateManager.OnLevelStateChanged -= OnLevelStateChanged;
        }
    }

    void OnLevelStateChanged(string sceneName)
    {
        if (sceneName == levelSceneName)
        {
            UpdateColor();
        }
    }

    void UpdateColor()
    {
        if (LevelStateManager.Instance == null) return;

        LevelStateManager.LevelState state = LevelStateManager.Instance.GetState(levelSceneName);
        currentState = state;

        Color targetColor = state switch
        {
            LevelStateManager.LevelState.Locked => lockedColor,
            LevelStateManager.LevelState.Active => activeColor,
            LevelStateManager.LevelState.Completed => completedColor,
            _ => activeColor
        };

        if (button != null)
        {
            var colors = button.colors;
            colors.normalColor = targetColor;
            button.colors = colors;
            button.interactable = state != LevelStateManager.LevelState.Locked;
        }

        if (image != null)
        {
            image.color = targetColor;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentState == LevelStateManager.LevelState.Locked)
            return;

        if (eventData.clickCount == 2)
        {
            LoadLevel();
        }
        else if (eventData.clickCount == 1)
        {
            ShowLevelInfo();
        }
    }

    public void LoadLevel()
    {
        if (!string.IsNullOrEmpty(levelSceneName))
        {
            SceneManager.LoadScene(levelSceneName);
        }
    }

    public void ShowLevelInfo()
    {
        if (LevelMapManager.Instance != null)
        {
            LevelMapManager.Instance.ShowLevelInfo(this);
        }
    }

    public void RefreshState()
    {
        UpdateColor();
    }

    public LevelStateManager.LevelState CurrentState
    {
        get { return currentState; }
    }
}