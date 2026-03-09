using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class SceneProgress // Прогресс для каждой сцены
{
    public string sceneName;
    public bool isCompleted;
}

[System.Serializable]
public class GameSaveData
{
    public List<SceneProgress> level0Scenes = new List<SceneProgress>(); // Список прогрессов сцен для первой локации
}

public class GameSaveManager : MonoBehaviour
{
    private static GameSaveManager _instance;
    public static GameSaveManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("GameSaveManager");
                _instance = go.AddComponent<GameSaveManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    private GameSaveData saveData;
    private string saveFilePath;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        saveFilePath = Path.Combine(Application.persistentDataPath, "gamesave.json");
        LoadGame();
    }

    private void LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            saveData = JsonUtility.FromJson<GameSaveData>(json);
            Debug.Log("Game loaded from: " + saveFilePath);
        }
        else
        {
            InitializeNewSave();
        }
    }

    private void InitializeNewSave()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("gamesave");
        if (jsonFile != null)
        {
            saveData = JsonUtility.FromJson<GameSaveData>(jsonFile.text);
            Debug.Log("Save initialized from Resources/gamesave.json");
        }
        else
        {
            saveData = new GameSaveData();
            saveData.level0Scenes.Add(new SceneProgress { sceneName = "OurCabinet", isCompleted = false });
            saveData.level0Scenes.Add(new SceneProgress { sceneName = "Corridor", isCompleted = false });
            saveData.level0Scenes.Add(new SceneProgress { sceneName = "ColleagueCabinet", isCompleted = false });
            saveData.level0Scenes.Add(new SceneProgress { sceneName = "TimeMachine", isCompleted = false });
            Debug.LogWarning("Resources/gamesave.json not found, using hardcoded defaults");
        }

        SaveGame();
        Debug.Log("New save initialized at: " + saveFilePath);
    }

    private void SaveGame()
    {
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Game saved to: " + saveFilePath);
    }
    // === ! Метод для маркировки пройденных сцен ! ===
    public void MarkSceneCompleted(string sceneName)
    {
        SceneProgress scene = saveData.level0Scenes.Find(s => s.sceneName == sceneName);
        if (scene != null)
        {
            scene.isCompleted = true;
            SaveGame();
            Debug.Log($"Scene '{sceneName}' marked as completed!");
        }
        else
        {
            Debug.LogWarning($"Scene '{sceneName}' not found in save data!");
        }
    }

    public bool IsSceneCompleted(string sceneName)
    {
        SceneProgress scene = saveData.level0Scenes.Find(s => s.sceneName == sceneName);
        return scene != null && scene.isCompleted;
    }

    public GameSaveData GetSaveData()
    {
        return saveData;
    }

    public string GetSaveFilePath()
    {
        return saveFilePath;
    }

    [ContextMenu("Print Save File Path")]
    public void PrintSaveFilePath()
    {
        Debug.Log("Save file location: " + saveFilePath);
    }
}