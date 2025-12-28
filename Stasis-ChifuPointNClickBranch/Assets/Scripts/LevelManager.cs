using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public void LoadLevel(int index)
    {
        switch (index)
        {
            case 0:
                Stats.Instance.door = Stats.DoorPoses.OurCabinet;
                break;
            case 1:
                Stats.Instance.door = Stats.DoorPoses.ColleagueCabinet;
                break;
            case 2:
                Stats.Instance.door = Stats.DoorPoses.TimeMachine;
                break;
            case 3:
                break;
        }
        SceneManager.LoadScene(index);
    }
    public void CloseApp()
    {
        Application.Quit();
    }
}