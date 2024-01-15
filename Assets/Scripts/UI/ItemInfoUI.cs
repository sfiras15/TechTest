using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ItemInfoUI : MonoBehaviour
{
    private Gear itemToShow;
    [SerializeField] private GameObject itemInfoCanvas;
    [SerializeField] private GameObject pannel;
    [SerializeField] private Image itemImage;
    [SerializeField] private Button buyButton;
    [SerializeField] private Text buyButtonText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI speedText;

    private void OnEnable()
    {
        ItemSlot.onButtonClick += InitializeInfo;
    }
    private void OnDisable()
    {
        ItemSlot.onButtonClick -= InitializeInfo;
    }
    private void InitializeInfo(Gear item)
    {
        itemInfoCanvas.SetActive(true);
        pannel.SetActive(true);
        itemToShow = item;
        itemImage.sprite = item.itemIcon;
        costText.text = item.cost.ToString();
        healthText.text = "+" + item.healthBoost + " Health";
        damageText.text = "+" + item.damageBoost + " Damage";
        speedText.text = "+" + item.speedBoost + " Speed";

        if (CheckComponents())
        {
            buyButton.interactable = true;
            buyButtonText.color = Color.white;
            Debug.Log("components found");
        }
        else
        {
            buyButton.interactable = false;
            Color color = buyButtonText.color;
            color.a = 80f / 255f;
            buyButtonText.color = color;
            Debug.Log("components not found");
        }
            
            

    }

    private bool CheckComponents()
    {
        if (itemToShow.components != null && itemToShow.components.Length > 0)
        {
            foreach (var component in itemToShow.components)
            {
                // Check if the component is in the inventory
                if (!InventoryContainsItem(component))
                {
                    // Component not found, return false
                    return false;
                }
            }
        }

        // All components are in the inventory
        return true;
    }
    private bool InventoryContainsItem(Item item)
    {
        foreach (var kvp in Inventory.instance.inventoryDictionary)
        {
            if (kvp.Value == item)
            {
                return true;
            }
        }
        return false;
    }
    public void Buy()
    {
        //Debug.Log(Inventory.instance.CurrencyCounter());
        if (Inventory.instance.CurrencyCounter() >= itemToShow.cost)
        {
            Inventory.instance.Add(itemToShow);
            if (itemToShow.components != null && itemToShow.components.Length > 0)
            {
                foreach (var component in itemToShow.components)
                {
                    Inventory.instance.RemoveItem(component);
                }
            }    
            // Remove the amount of currency for the item
            Item keyToRemove = FindKeyByItemType(Item.ItemType.Gold);
            if (keyToRemove != null) Inventory.instance.RemoveItem(keyToRemove, itemToShow.cost);


        }
        else
        {
            Debug.Log("Not enough money");
        }
    }
    private Item FindKeyByItemType(Item.ItemType itemType)
    {
        for (int i = 0; i < Inventory.instance.inventoryDictionary.Count; i++)
        {
            if (Inventory.instance.inventoryDictionary[i].itemType == itemType)
            {
                return Inventory.instance.inventoryDictionary[i];
            }
        }
        // Return -1 if the item with the specified itemType is not found
        return null;
    }
}
