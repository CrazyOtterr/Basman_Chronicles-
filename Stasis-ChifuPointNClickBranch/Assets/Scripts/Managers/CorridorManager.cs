using UnityEngine;

public class CorridorManager : MonoBehaviour
{
    private Transform player;
    void Start()
    {
        player = FindAnyObjectByType<PnC_Player>().transform;

        if (player == null)
        {
            Debug.Log("Игрок не найден!");
        }

        switch (Stats.Instance.door)
        {
            case Stats.DoorPoses.OurCabinet:
                player.position = new Vector2(0.43f, -4f);
                break;
            case Stats.DoorPoses.ColleagueCabinet:
                player.position = new Vector2(8.15f, -4f);
                break;
            case Stats.DoorPoses.TimeMachine:
                player.position = new Vector2(15.61f, -4f);
                break;
            case Stats.DoorPoses.Corridor:
                break;
        }
    }
}
