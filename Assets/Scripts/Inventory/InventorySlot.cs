using TMPro;
using UnityEngine;
using UnityEngine.UI;


//Attached to the inventory Slot. each slot store information about the item to properly render it on the menu.

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image itemButtonIcon;
    [SerializeField] private Button exitButton;
    [SerializeField] private Item item;
    [SerializeField] private TextMeshProUGUI instanceNumber;

    private void OnEnable()
    {
        instanceNumber.gameObject.SetActive(false);
    }
    public void AddItem(Item newItem)
    {
        item = newItem;
        instanceNumber.gameObject.SetActive(true);
        instanceNumber.text = "x" + item.nbOfInstances.ToString();
        itemButtonIcon.sprite = newItem.itemIcon;
        itemButtonIcon.enabled = true;
        exitButton.interactable = true;
    }
    public void ClearSlot()
    {
        item = null;
        instanceNumber.gameObject.SetActive(false);
        itemButtonIcon.sprite = null;
        itemButtonIcon.enabled = false;
        exitButton.interactable = false;
    }
    public void RemoveFromInventory()
    {
        if (item.nbOfInstances > 0) Inventory.instance.RemoveItem(item);
        else ClearSlot();

    }
    public void UseItem()
    {
        if (item != null)
        {
            item.Use();
        }
    }
}
