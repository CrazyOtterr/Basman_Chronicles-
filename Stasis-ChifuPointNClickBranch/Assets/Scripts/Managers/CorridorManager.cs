using UnityEngine;

public class CorridorManager : MonoBehaviour
{
    private Transform player;

    // Для вычисления позиции в коридоре
    public Transform ourCabinet;
    public Transform colleagueCabinet;
    public Transform timeMachine;
    public Transform curiousCollegue;


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
                player.position = ourCabinet.position;
                break;
            case Stats.DoorPoses.ColleagueCabinet:
                player.position = colleagueCabinet.position;
                break;
            case Stats.DoorPoses.TimeMachine:
                player.position = timeMachine.position;
                break;
            case Stats.DoorPoses.Corridor:
                // ничего не делает
                break;
            case Stats.DoorPoses.CuriousCollegue:
                player.position = curiousCollegue.position;
                break;
        }
    }
}
