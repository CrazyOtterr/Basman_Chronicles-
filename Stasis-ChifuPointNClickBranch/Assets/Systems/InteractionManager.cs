using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour {
    public static InteractionManager inst { get; private set; }
    private readonly Dictionary<string, ClickableThing> sceneClickables = new Dictionary<string, ClickableThing>();
    public bool isLocked = false;
    private void Update() {
        HandleInteractions();
    }

    public bool IsHoveringOverInteractor() {
        List<GameObject> objects = ScreenUtils.GetObjectsUnderMouse();
        for (int i = 0; i < objects.Count; i++) {
            ClickableThing clickable = objects[i].GetComponent<ClickableThing>();
            if (clickable != null) return true;
        }
        return false;
    }

    public void RegisterClickable(string name, ClickableThing clickable) {
        sceneClickables.Add(name, clickable);
    }
    public ClickableThing GetClickable(string name) => sceneClickables[name];

    private void HandleInteractions() {
        if (!Input.GetMouseButtonDown(0)) return;
        if (isLocked) return;
        //if (DialogueSystem.inst.IsFrozen()) return;
        List<GameObject> objects = ScreenUtils.GetObjectsUnderMouse();
        for (int i = 0; i < objects.Count; i++) {
            PnC_InteractiveItem clickable = objects[i].GetComponent<PnC_InteractiveItem>();
            if (clickable == null) continue;
            clickable.HandleClick();
            return;
        }
    }

    private void Awake() {
        if (inst != null && inst != this) {
            Destroy(this);
        } else {
            inst = this;
        }
    }
}
