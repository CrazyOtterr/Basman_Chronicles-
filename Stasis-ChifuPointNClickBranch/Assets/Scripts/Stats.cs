using System;
using UnityEngine;

public class Stats : MonoBehaviour
{
    public static Stats Instance;

    public DoorPoses door;
    public bool isMapSolved;
    public bool isDocsPicked;
    public float gameTime;

    public enum DoorPoses
    {
        OurCabinet,
        ColleagueCabinet,
        TimeMachine,
        Corridor
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        gameTime += Time.deltaTime;
        //Debug.Log(gameTime);
    }
}
