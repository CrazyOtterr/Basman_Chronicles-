using UnityEngine;

public class PickupObject : MonoBehaviour, IInteractable
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private Sprite iconForItem;

    public void Interact(GameObject interactor)
    {
        IItem item = new KeyItem(iconForItem);

        if (inventory.Add(item))
        {
            Destroy(gameObject);
        }
    }
}
