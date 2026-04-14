using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PnC_Player : MonoBehaviour
{
    public static PnC_Player inst { get; private set; }
    [field: SerializeField] public PnC_PlayerController controller { get; private set; }

    public void ResetData()
    {
        Stats.Instance.door = Stats.DoorPoses.OurCabinet;
        Stats.Instance.isMapSolved = false;
        Stats.Instance.isDocsPicked = false;
        for (int i = 0; i < Stats.Instance.conversationSaves.Length; i++)
        {
            Stats.Instance.conversationSaves[i] = false;
        }
        Stats.Instance.currentTime = 0;
        Stats.Instance.currentRestTime = 0;
        Stats.Instance.direction = false;
        Stats.Instance.isResting = false;

        LevelManager.LoadLevel(0);
    }

    private void Awake() 
    {
        if (inst != null && inst != this) 
        {
            Destroy(this);
        } 
        else 
        {
            inst = this;
        }
    }
}
