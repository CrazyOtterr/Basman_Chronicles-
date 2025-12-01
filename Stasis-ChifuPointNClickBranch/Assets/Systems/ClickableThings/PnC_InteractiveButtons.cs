using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using System;

public class PnC_InteractiveButtons : MonoBehaviour
{
    public Sprite circleSprite;
    [field: SerializeField] public List<Sprite> spriteButtons;
    private List<string> buttonNames = new List<string>() { "WatchButton", "TakeButton", "ObserveButton"};
    
    private float deltaY = 250;
    private float deltaX = 200;
    public async Task Init(PnC_InteractiveItem source, List<Action> someEvents)
    {
        bool t = false;
        for(int i = 0; i < 3; i++)
        {
            GameObject g = new GameObject(buttonNames[i]);
            
            source.buttons[i] = g;

            g.transform.parent = GameObject.Find("For UI canvas").transform;
            g.transform.position = transform.position;
            g.transform.localScale = Vector3.one * 1.7f;
            
            Image sr = g.AddComponent<Image>();
            sr.sprite = circleSprite;
            sr.color = Color.gray;

            g.AddComponent<CircleCollider2D>().isTrigger = true;

            Button b = g.AddComponent<Button>();
            b.onClick.AddListener(someEvents[i].Invoke);
    
            GameObject forSprite = new GameObject(buttonNames[i] + " sprite");
            forSprite.transform.parent = g.transform;
            forSprite.transform.position = transform.position;
            forSprite.transform.localScale = Vector3.one * 0.8f;

            Image srForSub = forSprite.AddComponent<Image>();
            srForSub.sprite = spriteButtons[i];
            srForSub.raycastTarget = false;

            Vector3 deltaVector = new Vector2((i - 1) * deltaX, deltaY);
            g.GetComponent<RectTransform>().DOAnchorPos((Vector3)ScreenUtils.ScreenPosition(g.transform.position) + deltaVector - new Vector3(Screen.width/2, Screen.height/2), 0.5f).onComplete = () => t = true;
        }
        while(!t) await Task.Yield();
    }
}
