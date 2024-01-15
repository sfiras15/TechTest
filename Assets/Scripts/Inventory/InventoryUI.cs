using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attached to the Inventory GameObject.Handles the inventory UI logic.

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private KeyCode inventoryKey;
    [SerializeField] private GameObject inventoryUI;


    public static bool isInventoryActive = false;

    // Declare an event for inventory UI state change to prevent player from attacking while using the inventory
    //public static event Action<bool> onInventoryUIStateChanged;
    private void OnEnable()
    {
        Inventory.onItemChanged += UpdateUI;
    }
    private void OnDisable()
    {
        Inventory.onItemChanged -= UpdateUI;
    }
    private void Update()
    {

        // fix a bug where the player can still attack while browsing the inventory
        if (Input.GetKeyDown(inventoryKey))
        {
            if (!inventoryUI.activeSelf)
            {
                isInventoryActive = true;
                inventoryUI.SetActive(true);
                UpdateUI();
            }
            else
            {
                isInventoryActive = false;
                inventoryUI.SetActive(false);
            }

            // call the event
            //onInventoryUIStateChanged?.Invoke(inventoryUI.activeSelf);
        }
    }

    public void UpdateUI()
    {
        InventorySlot[] slots = GetComponentsInChildren<InventorySlot>();
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < Inventory.instance.inventoryDictionary.Count)
            {
                slots[i].AddItem(Inventory.instance.inventoryDictionary[i]);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }
}
