using UnityEngine;

public class PnC_Door : MonoBehaviour
{
    public Stats.DoorPoses CurrentDoorPose;

    public int index;

    public void Enter()
    {
        Stats.Instance.door = CurrentDoorPose;
        LevelManager.LoadLevel(index);
    }
}
