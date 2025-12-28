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
                player.position = new Vector2(6.16f, -4f);
                break;
            case Stats.DoorPoses.ColleagueCabinet:
                player.position = new Vector2(13.88f, -4f);
                break;
            case Stats.DoorPoses.TimeMachine:
                player.position = new Vector2(21.34f, -4f);
                break;
            case Stats.DoorPoses.Corridor:
                break;
        }
    }
}
