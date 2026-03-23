using System;
using System.Collections.Generic;
using UnityEngine;

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
        "OurCabinet",
        "Mossovet(inside)"
    };

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            ResetAllProgress();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeLevels()
    {
        foreach (string sceneName in allLevelSceneNames)
        {
            if (PlayerPrefs.HasKey(sceneName))
            {
                levelStates[sceneName] = (LevelState)PlayerPrefs.GetInt(sceneName);
            }
            else
            {
                levelStates[sceneName] = sceneName == "OurCabinet" ? LevelState.Active : LevelState.Locked;
            }
        }

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
        }
        PlayerPrefs.Save();
    }

    public LevelState GetState(string sceneName)
    {
        return levelStates.TryGetValue(sceneName, out var state) ? state : LevelState.Locked;
    }

    public void MarkLevelAsVisited(string sceneName)
    {
        if (sceneName == "NewMapScene") return;

        if (levelStates.ContainsKey(sceneName) && levelStates[sceneName] != LevelState.Completed)
        {
            levelStates[sceneName] = LevelState.Completed;
            SaveProgress();
            OnLevelStateChanged?.Invoke(sceneName);

            int index = Array.IndexOf(allLevelSceneNames, sceneName);
            if (index >= 0 && index < allLevelSceneNames.Length - 1)
            {
                string nextLevel = allLevelSceneNames[index + 1];
                if (GetState(nextLevel) == LevelState.Locked)
                {
                    levelStates[nextLevel] = LevelState.Active;
                    SaveProgress();
                    OnLevelStateChanged?.Invoke(nextLevel);
                }
            }
        }
    }

    public void ResetAllProgress()
    {
	Debug.Log("=== RESET ALL PROGRESS CALLED ===");

        foreach (string sceneName in allLevelSceneNames)
        {
            PlayerPrefs.DeleteKey(sceneName);
	    Debug.Log($"Deleted key: {sceneName}");
        }
        PlayerPrefs.Save();

        // Устанавливаем начальные состояния
        foreach (string sceneName in allLevelSceneNames)
        {
            levelStates[sceneName] = sceneName == "OurCabinet" ? LevelState.Active : LevelState.Locked;
            Debug.Log($"Set {sceneName} to {levelStates[sceneName]}");

        }

        // Оповещаем все кнопки об обновлении
        foreach (string sceneName in allLevelSceneNames)
        {
            OnLevelStateChanged?.Invoke(sceneName);
            Debug.Log($"Invoked OnLevelStateChanged for {sceneName}");
        }
    }
}