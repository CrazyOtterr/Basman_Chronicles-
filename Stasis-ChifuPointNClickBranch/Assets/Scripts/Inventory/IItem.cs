using UnityEngine;

public interface IItem
{
    string Id { get; }
    string DisplayName { get; }
    Sprite Icon { get; }

    void Use();
    string GetInfo();

    bool Merge(IItem other);
}
