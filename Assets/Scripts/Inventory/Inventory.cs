using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;
    [SerializeField] private int inventorySize;

    //Next free key position in the dictionary
    private int nextAvailableKey;

    private int currentInventorySize;


    // position of the key of the item.itemId if he's in the dictionary 
    private int idLocation;

    // updating inventory UI
    public static event Action onItemChanged;

    // Event for using consumables
    public static event Action<Consumable_SO> onConsumableUse;    

    public Dictionary<int, Item> inventoryDictionary = new Dictionary<int, Item>();
    
    [SerializeField] private EquipementManager equipementManager;


    // event for acquiring gear 

    public static event Action<Gear,bool> onGearEquip;

    private Item DeserializeItem(string jsonData, Type itemType)
    {
        Item deserializedItem = (Item)ScriptableObject.CreateInstance(itemType);
        JsonUtility.FromJsonOverwrite(jsonData, deserializedItem);
        return deserializedItem;
    }


    private void Start()
    {
        nextAvailableKey = 0;
        currentInventorySize = 0;
    }
    public Item GetItemByType(Item.ItemType itemType)
    {
        foreach (var kvp in inventoryDictionary)
        {
            if (kvp.Value.itemType == itemType)
            {
                return kvp.Value;
            }
        }

        return null;
    }


    public void UseWeapon(Weapon_SO weapon)
    {
        Debug.Log("weapon: " + weapon.type);
        equipementManager.EquipWeapon(weapon);
    }

    public void UsePotion(Consumable_SO potion)
    {
        onConsumableUse?.Invoke(potion);
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
       
    }
    public bool Add(Item item)
    {
        if (inventoryDictionary.Count == 0)
        {
            inventoryDictionary.Add(nextAvailableKey, item);
            item.nbOfInstances = 1;
            nextAvailableKey++;
            currentInventorySize++;
            if (item.itemType == Item.ItemType.Gear)
            {
                onGearEquip?.Invoke((Gear)item,true);
            }
            // Item added successfully
            if (onItemChanged != null)
                onItemChanged.Invoke();
            return true;
        }
        else
        {
            // Look for the itemId inside the dictionary, if found store the key's position
            bool itemFound = FindItemId(item.itemId);
            if (itemFound)
            {
                inventoryDictionary[idLocation].nbOfInstances++;
                if (item.itemType == Item.ItemType.Gear)
                {
                    onGearEquip?.Invoke((Gear)item, true);
                }
                // Item added successfully
                if (onItemChanged != null)
                    onItemChanged.Invoke();
                return true;
            }
            else
            {
                if (currentInventorySize >= inventorySize)
                {
                    // Inventory full, item not added
                    Debug.Log("Not enough room.");
                    return false;
                }
                else
                {
                    // Add later limitation on how much an item can stack
                    inventoryDictionary.Add(nextAvailableKey, item);
                    item.nbOfInstances = 1;
                    nextAvailableKey++;
                    currentInventorySize++;

                    if (item.itemType == Item.ItemType.Gear)
                    {
                        onGearEquip?.Invoke((Gear)item, true);
                    }
                    // Item added successfully
                    if (onItemChanged != null)
                        onItemChanged.Invoke();
                    return true;
                }
            }
        }
    }



    private bool FindItemId(int id)
    {
        idLocation = -1;
        for (int i = 0; i < inventoryDictionary.Count; i++)
        {
            if (inventoryDictionary[i].itemId == id)
            {
                idLocation = i;
                return true;
            }
        }
        return false;
    }


    public void RemoveItem(Item item, int amount = 1)
    {
        bool itemFound = FindItemId(item.itemId);
        if (itemFound)
        {
            if (inventoryDictionary[idLocation].nbOfInstances <= amount)
            {

                inventoryDictionary[idLocation].nbOfInstances = 0;

                inventoryDictionary.Remove(idLocation);

                if (item.itemType == Item.ItemType.Gear)
                {
                    onGearEquip?.Invoke((Gear)item, false);
                }
                //Reorganize Dictionary after removing the item from the dictionary
                ReorganizeKeys();
                currentInventorySize--;

            }
            else
            {
                inventoryDictionary[idLocation].nbOfInstances -= amount;

                if (item.itemType == Item.ItemType.Gear)
                {
                    onGearEquip?.Invoke((Gear)item, false);
                }

            }
            // Remove the weapon if it's equipped
            Debug.Log("item.itemType == Item.ItemType.Weapon :" + (item.itemType == Item.ItemType.Weapon));
            if (item.itemType == Item.ItemType.Weapon)
            {
                Debug.Log("current equipped weapon : " + equipementManager.GetCurrentEquippedWeapon);
                if (equipementManager.GetCurrentEquippedWeapon != null)
                {
                    if (item.itemId == equipementManager.GetCurrentEquippedWeapon.itemId)
                    {
                        equipementManager.RemoveWeapon(equipementManager.GetCurrentEquippedWeapon.type);
                    }
                }
                
            }
           
            if (onItemChanged != null)
                onItemChanged.Invoke();
        }
    }

    private void ReorganizeKeys()
    {
        Dictionary<int, Item> newInventory = new Dictionary<int, Item>();
        int newKey = 0;

        foreach (var kvp in inventoryDictionary)
        {
            newInventory[newKey] = kvp.Value;
            newKey++;
        }
        nextAvailableKey = newKey;
        inventoryDictionary = newInventory;
    }


    public int CurrencyCounter()
    {
        for (int i = 0; i < inventoryDictionary.Count; i++)
        {
            if (inventoryDictionary[i].itemType == Item.ItemType.Gold)
            {
                return inventoryDictionary[i].nbOfInstances; 
            }
        }
        return -1;
    }


}
