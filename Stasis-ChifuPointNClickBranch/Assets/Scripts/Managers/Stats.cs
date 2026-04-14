using System;
using DialogueEditor;
using UnityEngine;

public class Stats : MonoBehaviour
{
    public static Stats Instance;

    public DoorPoses door;
    public bool isMapSolved;
    public bool isDocsPicked;

    [Header("Прочитанные диалоги")]
    public bool[] conversationSaves = new bool[8];

    [Header("Для работы с позицией Начальника")]
    public float wardenCycleTime;
    public float wardenRestTime;

    public float currentTime;
    public float currentRestTime;
    public bool direction; // право/лево
    public bool isResting;
    public float WardenTimeRatio;

    public enum DoorPoses
    {
        OurCabinet,
        ColleagueCabinet,
        TimeMachine,
        Corridor,
        CuriousCollegue
    }

    // Обработчик события
    public void OnConvSave(int convIndex)
    {
        conversationSaves[convIndex] = true;
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

        ConversationTrigger.ConvSave += OnConvSave; // подписываем сохранялку
    }
    private void Update()
    {
        if (Warden.isAppear)
        {
            if (!isResting)
            {
                if (!direction)
                {
                    currentTime += Time.deltaTime;

                    if (currentTime > wardenCycleTime)
                    {
                        currentTime = wardenCycleTime;
                        isResting = true;
                    }
                }
                else
                {
                    currentTime -= Time.deltaTime;

                    if (currentTime < 0)
                    {
                        currentTime = 0;
                        isResting = true;
                    }
                }
            }

            else if (isResting)
            {
                currentRestTime += Time.deltaTime;

                if (currentRestTime > wardenRestTime)
                {
                    currentRestTime = 0;
                    isResting = false;
                    direction = !direction;
                }
            }

            WardenTimeRatio = currentTime / wardenCycleTime;
        }
    }
}
