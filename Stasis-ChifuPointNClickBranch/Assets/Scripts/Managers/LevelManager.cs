using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public void LoadLevel(int index)
    {
        Debug.Log($"Downloading Scene: {index}");
        SceneManager.LoadScene(index);
    }
}