using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Inventory inventory;

    [SerializeField] private RectTransform panel;
    [SerializeField] private Button openButton;
    [SerializeField] private Button closeButton;

    [SerializeField] private GameObject dimmer;
    [SerializeField] private Transform content;
    [SerializeField] private InventorySlotView slotPrefab;

    [Header("Slide")]
    [SerializeField] private float slideDuration = 0.2f;
    [SerializeField] private float hiddenYOffset = 220f;

    private Coroutine _slideRoutine;
    private bool _isOpen;

    private IItem _selectedForMerge;
    private readonly List<InventorySlotView> _slots = new();

    private void Awake()
    {
        openButton.onClick.AddListener(Open);
        closeButton.onClick.AddListener(Close);

        if (dimmer != null) dimmer.SetActive(false);

        panel.anchoredPosition = new Vector2(panel.anchoredPosition.x, hiddenYOffset);
    }

    private void OnEnable()
    {
        inventory.Changed += Rebuild;
        Rebuild();
    }

    private void OnDisable()
    {
        inventory.Changed -= Rebuild;
    }

    public void Open()
    {
        if (_isOpen) return;
        _isOpen = true;
        SlideTo(0f);
    }

    public void Close()
    {
        if (!_isOpen) return;

        ExitMergeMode();

        _isOpen = false;
        SlideTo(hiddenYOffset);
    }

    private void SlideTo(float targetY)
    {
        if (_slideRoutine != null) StopCoroutine(_slideRoutine);
        _slideRoutine = StartCoroutine(SlideRoutine(targetY));
    }

    private IEnumerator SlideRoutine(float targetY)
    {
        float startY = panel.anchoredPosition.y;
        float t = 0f;

        while (t < slideDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / slideDuration);
            float y = Mathf.Lerp(startY, targetY, k);
            panel.anchoredPosition = new Vector2(panel.anchoredPosition.x, y);
            yield return null;
        }

        panel.anchoredPosition = new Vector2(panel.anchoredPosition.x, targetY);
    }

    private void Rebuild()
    {
        foreach (var s in _slots) if (s) Destroy(s.gameObject);
        _slots.Clear();

        foreach (var item in inventory.Items)
        {
            var slot = Instantiate(slotPrefab, content);
            slot.Bind(item);

            slot.OnClickUse = () => OnUseClicked(item);
            slot.OnClickUseWith = () => EnterMergeMode(item);

            slot.OnClickSlot = () => OnSlotClicked(item);

            _slots.Add(slot);
        }

        RefreshMergeHighlights();
    }

    private void OnUseClicked(IItem item)
    {
        if (item == null) return;
        item.Use();
        // если предмет одноразовый:
        // inventory.Remove(item);
    }

    private void EnterMergeMode(IItem item)
    {
        _selectedForMerge = item;
        if (dimmer != null) dimmer.SetActive(true);
        RefreshMergeHighlights();
    }

    private void ExitMergeMode()
    {
        _selectedForMerge = null;
        if (dimmer != null) dimmer.SetActive(false);
        RefreshMergeHighlights();
    }

    private void OnSlotClicked(IItem clicked)
    {
        if (_selectedForMerge == null) return;
        if (ReferenceEquals(clicked, _selectedForMerge)) return;

        bool merged = _selectedForMerge.Merge(clicked);

        if (merged)
        {
            ExitMergeMode();
            inventory.NotifyChanged();
        }
    }

    private void RefreshMergeHighlights()
    {
        bool mergeMode = _selectedForMerge != null;

        foreach (var slot in _slots)
        {
            if (slot == null) continue;

            bool isSelf = mergeMode && ReferenceEquals(slot.BoundItem, _selectedForMerge);
            slot.SetMergeState(mergeMode, isSelf);
        }
    }
}
