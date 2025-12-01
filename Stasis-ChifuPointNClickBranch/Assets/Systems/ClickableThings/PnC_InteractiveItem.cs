using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PnC_InteractiveItem : MonoBehaviour
{
    [field: SerializeField] public UnityEvent observeEvent { get; private set; } = new UnityEvent();
    [field: SerializeField] public UnityEvent takeEvent { get; private set; } = new UnityEvent();
    [field: SerializeField] public UnityEvent speakEvent { get; private set; } = new UnityEvent();
    public Transform targetPosition;
    public GameObject buttonsPrefab;
    [HideInInspector] public List<GameObject> buttons = new List<GameObject>();

    [SerializeField] private Puzzle _puzzlePrefab;
    public async void HandleClick()
    {
        for(int i = 0; i < 3; i++) buttons.Add(null);
        if(buttons[0] != null || buttons[1] != null || buttons[2] != null)
        {
            DestroyButtons();
        }
        GameObject g = Instantiate(buttonsPrefab, GameObject.Find("For UI canvas").transform);
        g.transform.position = transform.position;

        await g.GetComponent<PnC_InteractiveButtons>().Init
        (
            this,
            new List<Action>() 
            { 
                () => observeEvent.Invoke(),
                () => {PnC_Player.inst.controller.MoveTo(targetPosition.position, () => {takeEvent.Invoke();});},
                () => {PnC_Player.inst.controller.MoveTo(targetPosition.position, () => {speakEvent.Invoke();});},
            }
        );
        for(int i = 0; i < 3; i++)
        {
            buttons[i].GetComponent<Button>().onClick.AddListener(DestroyButtons);
        }
    }

    public void PrintSome(string s)
    {
        print(s);
    }
    public void CallPuzzle(Puzzle puzzle)
    {
        puzzle.gameObject.SetActive(true);
    }
    public void DestroyButtons()
    {
        for(int j = 0; j < 3; j++) if(buttons[j] != null) Destroy(buttons[j]);
    }
}
