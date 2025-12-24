using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    //public int index;
    public void LoadLevel(int index)
    {
        SceneManager.LoadScene(index);
    }
}