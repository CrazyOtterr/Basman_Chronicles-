using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private bool useTrigger2D;
    [SerializeField] private string requiredTag = "";

    public void LoadConfigured()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("Scene name is empty.");
            return;
        }
        SceneManager.LoadScene(sceneName);
    }

    public void LoadByName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogWarning("Scene name is empty.");
            return;
        }
        SceneManager.LoadScene(name);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!useTrigger2D) return;
        if (!string.IsNullOrEmpty(requiredTag) && !other.CompareTag(requiredTag)) return;
        LoadConfigured();
    }
}
