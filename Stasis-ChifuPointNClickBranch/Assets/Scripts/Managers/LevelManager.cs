using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static void LoadLevel(int index)
    {
        Debug.Log($"Downloading Scene: {index}");

        if (FadeTransitionManager.Instance != null)
        {
            FadeTransitionManager.Instance.LoadLevelWithFade(index);
        }
        else
        {
            SceneManager.LoadScene(index);
        }
    }
}