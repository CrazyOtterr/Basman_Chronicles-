using System;
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class LevelStateManager : MonoBehaviour
{
    private static LevelStateManager instance;
    public static LevelStateManager Instance => instance;

    public static event Action<string> OnLevelStateChanged;

    private Dictionary<string, LevelState> levelStates = new Dictionary<string, LevelState>();

    public enum LevelState { Locked, Active, Completed }

    [Header("Настройки уровней")]
    public string[] allLevelSceneNames = new string[]
    {
        "QuestSceneMain",
        "MossovetScene"
    };

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // ВРЕМЕННО: принудительно сбрасываем всё при каждом запуске
            PlayerPrefs.DeleteAll();  // ← ДОБАВЬТЕ ЭТУ СТРОКУ
            PlayerPrefs.Save();       // ← И ЭТУ

            InitializeLevels();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void InitializeLevels()
    {
        // Сначала устанавливаем все уровни как Locked
        foreach (string sceneName in allLevelSceneNames)
        {
            levelStates[sceneName] = LevelState.Locked;
        }

        // Загружаем сохранения
        LoadProgress();
    }

    void LoadProgress()
    {
        // Проверяем, есть ли сохранения
        bool hasAnySavedData = false;
        foreach (string sceneName in allLevelSceneNames)
        {
            if (PlayerPrefs.HasKey(sceneName))
            {
                hasAnySavedData = true;
                break;
            }
        }

        if (hasAnySavedData)
        {
            // Загружаем сохранения
            foreach (string sceneName in allLevelSceneNames)
            {
                if (PlayerPrefs.HasKey(sceneName))
                {
                    levelStates[sceneName] = (LevelState)PlayerPrefs.GetInt(sceneName);
                    Debug.Log($"Загружен уровень {sceneName}: {levelStates[sceneName]}");
                }
            }
        }
        else
        {
            // Первый запуск - устанавливаем начальные состояния
            Debug.Log("Первый запуск игры - устанавливаем начальные состояния");

            // Первый уровень - активный, остальные Locked (уже установлены)
            if (System.Array.Exists(allLevelSceneNames, name => name == "QuestSceneMain"))
            {
                levelStates["QuestSceneMain"] = LevelState.Active;
            }

            // Сохраняем начальные состояния
            SaveProgress();
        }

        // Вызываем событие для обновления всех кнопок
        foreach (string sceneName in allLevelSceneNames)
        {
            OnLevelStateChanged?.Invoke(sceneName);
        }
    }

    void SaveProgress()
    {
        foreach (var level in levelStates)
        {
            PlayerPrefs.SetInt(level.Key, (int)level.Value);
            Debug.Log($"Сохранен уровень {level.Key}: {level.Value}");
        }
        PlayerPrefs.Save();
    }

    public LevelState GetState(string sceneName)
    {
        if (levelStates.TryGetValue(sceneName, out var state))
            return state;

        Debug.LogWarning($"Уровень '{sceneName}' не найден, возвращаем Locked");
        return LevelState.Locked;
    }

    public void SetCompleted(string sceneName)
    {
        if (levelStates.ContainsKey(sceneName) && levelStates[sceneName] != LevelState.Completed)
        {
            levelStates[sceneName] = LevelState.Completed;
            Debug.Log($"Уровень '{sceneName}' отмечен как пройденный");
            SaveProgress();
            OnLevelStateChanged?.Invoke(sceneName);
            UnlockNextLevel(sceneName);
        }
    }

    public void SetActive(string sceneName)
    {
        if (levelStates.ContainsKey(sceneName) && levelStates[sceneName] != LevelState.Active)
        {
            levelStates[sceneName] = LevelState.Active;
            Debug.Log($"Уровень '{sceneName}' активирован");
            SaveProgress();
            OnLevelStateChanged?.Invoke(sceneName);
        }
    }

    void UnlockNextLevel(string completedSceneName)
    {
        int index = Array.IndexOf(allLevelSceneNames, completedSceneName);
        if (index >= 0 && index < allLevelSceneNames.Length - 1)
        {
            string nextLevel = allLevelSceneNames[index + 1];
            if (GetState(nextLevel) == LevelState.Locked)
            {
                SetActive(nextLevel);
                Debug.Log($"Следующий уровень '{nextLevel}' разблокирован!");
            }
        }
    }

    // Метод для принудительного сброса (можно вызвать из консоли)
    [ContextMenu("Сбросить прогресс")]
    public void ResetAllProgress()
    {
        Debug.Log("Сброс прогресса всех уровней");

        // Удаляем все сохранения
        foreach (string sceneName in allLevelSceneNames)
        {
            PlayerPrefs.DeleteKey(sceneName);
        }
        PlayerPrefs.Save();

        // Сбрасываем состояния
        foreach (string sceneName in allLevelSceneNames)
        {
            levelStates[sceneName] = LevelState.Locked;
        }

        // Первый уровень делаем активным
        if (System.Array.Exists(allLevelSceneNames, name => name == "QuestSceneMain"))
        {
            levelStates["QuestSceneMain"] = LevelState.Active;
        }

        // Сохраняем
        SaveProgress();

        // Оповещаем все кнопки
        foreach (string sceneName in allLevelSceneNames)
        {
            OnLevelStateChanged?.Invoke(sceneName);
        }

        Debug.Log("Прогресс сброшен");
    }

}