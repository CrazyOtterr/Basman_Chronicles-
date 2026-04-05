using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CodeLock : MonoBehaviour
{
    [Header("Настройки замка")]
    [Tooltip("Правильный 6-значный код")]
    public int[] correctCode = new int[] { 1, 2, 3, 4, 5, 6 };
    
    [Tooltip("Объект, который активируется при открытии (дверь, сундук)")]
    public GameObject targetToActivate;
    
    [Header("UI элементы")]
    public GameObject codeLockPanel; // сама панель CodeLockPanel
    
    [Header("Цифры на замке (numbersOnLock)")]
    public TextMeshProUGUI[] lockDigits; // 6 текстов для отображения текущего кода
    
    [Header("Кнопки (buttons)")]
    public Button[] upButtons;   // 6 кнопок вверх
    public Button[] downButtons; // 6 кнопок вниз
    
    [Header("Сообщения")]
    public TextMeshProUGUI messageText; // текст для вывода сообщений
    public float messageDuration = 1.5f;
    
    [Header("Звуки (опционально)")]
    public AudioClip correctSound;
    public AudioClip wrongSound;
    public AudioClip clickSound;
    private AudioSource audioSource;
    
    [Header("События")]
    public UnityEngine.Events.UnityEvent onCodeCorrect;
    public UnityEngine.Events.UnityEvent onCodeWrong;
    
    private int[] currentCode = new int[6];
    private bool isSolved = false;
    private bool isPanelOpen = false;
    private Coroutine messageCoroutine;
    
    void Start()
    {
        // Инициализируем текущий код
        for (int i = 0; i < 6; i++)
        {
            currentCode[i] = 0;
        }
        
        // Подписываем кнопки
        for (int i = 0; i < 6; i++)
        {
            int index = i; // важно для замыкания
            
            if (upButtons[i] != null)
                upButtons[i].onClick.AddListener(() => ChangeDigit(index, 1));
            
            if (downButtons[i] != null)
                downButtons[i].onClick.AddListener(() => ChangeDigit(index, -1));
        }
        
        // Получаем или создаем AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && (correctSound != null || wrongSound != null))
            audioSource = gameObject.AddComponent<AudioSource>();
        
        // Скрываем панель при старте
        if (codeLockPanel != null)
            codeLockPanel.SetActive(false);
        
        // Скрываем сообщение при старте
        if (messageText != null)
            messageText.gameObject.SetActive(false);
        
        // Обновляем отображение
        UpdateAllDigits();
    }
    
    void ChangeDigit(int position, int delta)
    {
        if (isSolved) return;
        
        if (clickSound != null && audioSource != null)
            audioSource.PlayOneShot(clickSound);
        
        currentCode[position] += delta;
        
        // Зацикливаем от 0 до 9
        if (currentCode[position] > 9)
            currentCode[position] = 0;
        if (currentCode[position] < 0)
            currentCode[position] = 9;
        
        UpdateDigitDisplay(position);
    }
    
    void UpdateDigitDisplay(int position)
    {
        if (lockDigits[position] != null)
            lockDigits[position].text = currentCode[position].ToString();
    }
    
    void UpdateAllDigits()
    {
        for (int i = 0; i < 6; i++)
        {
            if (lockDigits[i] != null)
                lockDigits[i].text = currentCode[i].ToString();
        }
    }
    
    // Метод для проверки кода (вызывается с кнопки)
    public void CheckCode()
    {
        if (isSolved) return;
        
        bool isCorrect = true;
        
        for (int i = 0; i < 6; i++)
        {
            if (currentCode[i] != correctCode[i])
            {
                isCorrect = false;
                break;
            }
        }
        
        if (isCorrect)
        {
            OnCodeCorrect();
        }
        else
        {
            OnCodeWrong();
        }
    }
    
    void OnCodeCorrect()
    {
        Debug.Log("Код верный!");
        isSolved = true;
        
        if (correctSound != null && audioSource != null)
            audioSource.PlayOneShot(correctSound);
        
        ShowMessage("Код правильный!", Color.green);
        
        onCodeCorrect?.Invoke();
        
        // Активируем целевой объект
        if (targetToActivate != null)
            targetToActivate.SetActive(true);
        
        // Закрываем панель
        ClosePanel();
    }
    
    void OnCodeWrong()
    {
        Debug.Log("Неверный код!");
        
        if (wrongSound != null && audioSource != null)
            audioSource.PlayOneShot(wrongSound);
        
        ShowMessage("Код неправильный!", Color.red);
        
        onCodeWrong?.Invoke();
        
        // Визуальная обратная связь - мигаем красным
        StartCoroutine(WrongCodeFeedback());
    }
    
    void ShowMessage(string message, Color color)
    {
        if (messageText == null) return;
        
        if (messageCoroutine != null)
            StopCoroutine(messageCoroutine);
        
        messageCoroutine = StartCoroutine(ShowMessageCoroutine(message, color));
    }
    
    IEnumerator ShowMessageCoroutine(string message, Color color)
    {
        messageText.text = message;
        messageText.color = color;
        messageText.gameObject.SetActive(true);
        
        yield return new WaitForSeconds(messageDuration);
        
        messageText.gameObject.SetActive(false);
    }
    
    IEnumerator WrongCodeFeedback()
    {
        // Сохраняем оригинальные цвета
        Color[] originalColors = new Color[6];
        for (int i = 0; i < 6; i++)
        {
            if (lockDigits[i] != null)
                originalColors[i] = lockDigits[i].color;
        }
        
        // Мигаем красным
        for (int i = 0; i < 6; i++)
        {
            if (lockDigits[i] != null)
                lockDigits[i].color = Color.red;
        }
        
        yield return new WaitForSeconds(0.3f);
        
        // Возвращаем оригинальные цвета
        for (int i = 0; i < 6; i++)
        {
            if (lockDigits[i] != null)
                lockDigits[i].color = originalColors[i];
        }
    }
    
    // Открыть панель замка (вызывается при взаимодействии с замком)
    public void OpenPanel()
    {
        if (!isSolved && codeLockPanel != null && !isPanelOpen)
        {
            codeLockPanel.SetActive(true);
            isPanelOpen = true;
        }
    }
    
    // Закрыть панель
    public void ClosePanel()
    {
        if (codeLockPanel != null)
        {
            codeLockPanel.SetActive(false);
            isPanelOpen = false;
            
            // Скрываем сообщение при закрытии панели
            if (messageText != null)
                messageText.gameObject.SetActive(false);
        }
    }
    
    // Сброс кода (если нужно)
    public void ResetCode()
    {
        if (isSolved) return;
        
        for (int i = 0; i < 6; i++)
        {
            currentCode[i] = 0;
        }
        UpdateAllDigits();
    }
}