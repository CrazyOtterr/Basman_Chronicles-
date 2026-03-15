using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [Header("Настройки")]
    public string levelName;

    void Start()
    {
        if (string.IsNullOrEmpty(levelName))
        {
            levelName = SceneManager.GetActiveScene().name;
        }
    }

    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    public void ReturnToMap()
    {
        if (LevelStateManager.Instance != null && !string.IsNullOrEmpty(levelName))
        {
            LevelStateManager.Instance.MarkLevelAsVisited(levelName);
        }
        SceneManager.LoadScene("NewMapScene");
    }
}