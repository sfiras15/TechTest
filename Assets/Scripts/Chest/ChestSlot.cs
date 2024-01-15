using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Stores information about the item inside the chest
/// </summary>
public class ChestSlot : MonoBehaviour
{
    [SerializeField] private Image itemButtonIcon;
    [SerializeField] private Item item;

    public static event Action<Item> onItemCollected;


    public void AddItem(Item newItem)
    {
        item = newItem;
        itemButtonIcon.sprite = newItem.itemIcon;
        itemButtonIcon.enabled = true;
    }

    // Function called when we click on the item in the chest UI
    public void CollectItem()
    {
        if (item != null)
        {
            Inventory.instance.Add(item);
            onItemCollected?.Invoke(item);
            ClearSlot();
        }
    }

    public void ClearSlot()
    {
        item = null;
        itemButtonIcon.sprite = null;
        itemButtonIcon.enabled = false;
    }
}
