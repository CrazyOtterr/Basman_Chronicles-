using UnityEngine;

[System.Serializable]
public class KeyItem : IItem
{
    public string Id => "key_small";
    public string DisplayName => "Маленький ключ";
    public Sprite Icon { get; private set; }

    public KeyItem(Sprite icon) => Icon = icon;

    public void Use()
    {
        Debug.Log("Ключ использован (в воздух).");
    }

    public string GetInfo()
    {
        return "Небольшой ключ. Возможно, открывает ящик.";
    }

    public bool Merge(IItem other)
    {
        if (other == null) return false;

        if (other.Id == "lock_small")
        {
            Debug.Log("Ключ подошёл к замку!");
            return true;
        }

        Debug.Log("Нельзя объединить эти предметы.");
        return false;
    }
}
