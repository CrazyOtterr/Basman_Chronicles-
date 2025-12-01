using UnityEngine;
using UnityEngine.Events;

public class ClickableThing : MonoBehaviour {
    [field: SerializeField] public UnityEvent callback { get; private set; } = new UnityEvent();
    [field: SerializeField] public UnityEvent postCallback { get; private set; } = new UnityEvent();
    [field: SerializeField] public string key { get; private set; }
    [field:SerializeField] public bool isReachableEverywhere { get; private set; } = false;
    [SerializeField] private Transform targetPosition;
    private static int clickableId = 0;
    private void Start() {
        if (key == null || key == "" || string.IsNullOrEmpty(key)) {
            clickableId++;
            key = clickableId.ToString();
        }
        InteractionManager.inst.RegisterClickable(key, this);
    }

    public virtual void HandleClick() {
        callback.Invoke();
        if (isReachableEverywhere) {
            HandleInteraction();
        } else {
            if (targetPosition == null) targetPosition = transform;
            PnC_Player.inst.controller.MoveTo(targetPosition.position, () => {
                HandleInteraction();
            });
        }
    }

    public virtual void HandleInteraction() {
        postCallback.Invoke();
    }
}