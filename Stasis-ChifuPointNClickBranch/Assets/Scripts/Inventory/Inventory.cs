using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private int capacity = 12;

    private readonly List<IItem> _items = new();
    public IReadOnlyList<IItem> Items => _items;

    public event Action Changed;

    public bool Add(IItem item)
    {
        if (item == null) return false;
        if (_items.Count >= capacity) return false;

        _items.Add(item);
        Changed?.Invoke();
        return true;
    }

    public bool Remove(IItem item)
    {
        if (item == null) return false;
        bool ok = _items.Remove(item);
        if (ok) Changed?.Invoke();
        return ok;
    }

    public void NotifyChanged()
    {
        Changed?.Invoke();
    }
}
