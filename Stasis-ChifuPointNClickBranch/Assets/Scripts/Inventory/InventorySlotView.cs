using System;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotView : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Button slotButton;

    [SerializeField] private Button useButton;
    [SerializeField] private Button useWithButton;

    [SerializeField] private GameObject glow;
    [SerializeField] private GameObject disabledOverlay;

    public IItem BoundItem { get; private set; }

    public Action OnClickSlot;
    public Action OnClickUse;
    public Action OnClickUseWith;

    private void Awake()
    {
        slotButton.onClick.AddListener(() => OnClickSlot?.Invoke());
        useButton.onClick.AddListener(() => OnClickUse?.Invoke());
        useWithButton.onClick.AddListener(() => OnClickUseWith?.Invoke());
    }

    public void Bind(IItem item)
    {
        BoundItem = item;
        if (icon != null) icon.sprite = item?.Icon;
    }

    public void SetMergeState(bool mergeMode, bool isSelf)
    {
        if (glow != null) glow.SetActive(mergeMode && !isSelf);
        if (disabledOverlay != null) disabledOverlay.SetActive(mergeMode && isSelf);

        if (useButton != null) useButton.gameObject.SetActive(!mergeMode);
        if (useWithButton != null) useWithButton.gameObject.SetActive(!mergeMode);
    }
}
