using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public void LoadLevel(string levelName)
    {
        Debug.Log("Загружаем: " + levelName);
        SceneManager.LoadScene(levelName);
    }
}