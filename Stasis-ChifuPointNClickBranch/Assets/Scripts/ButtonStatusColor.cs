using UnityEngine;
using UnityEngine.UI;

public class ButtonSessionSaver : MonoBehaviour
{
    public Button myButton;
    public Color normalColor = Color.green;
    public Color pressedColor = Color.red;

    // Статическая переменная живет только пока работает программа
    private static bool isClickedThisSession = false;

    void Start()
    {
        // При старте сцены проверяем состояние переменной
        UpdateVisuals();

        myButton.onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick()
    {
        isClickedThisSession = true;
        UpdateVisuals();
    }

    void UpdateVisuals()
    {
        Color targetColor = isClickedThisSession ? pressedColor : normalColor;

        ColorBlock cb = myButton.colors;
        cb.normalColor = targetColor;
        myButton.colors = cb;
    }
}
