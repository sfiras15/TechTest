using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Updates the UI slots for the chest
/// </summary>
public class ChestUI : MonoBehaviour
{
    [SerializeField] private ChestSlot[] chestSlots;
    private void Awake()
    {
        chestSlots = GetComponentsInChildren<ChestSlot>();
    }
    public void UpdateUI(List<Item> itemsInsideTheChest)
    {
        //for (int j = 0; j < chestSlots.Length; j++) chestSlots[j].ClearSlot();

        for (int i = 0; i < chestSlots.Length; i++)
        {
            if (i < itemsInsideTheChest.Count)
            {
                chestSlots[i].AddItem(itemsInsideTheChest[i]);
            }
            else
            {
                chestSlots[i].ClearSlot();
            }
        }
    }
}
