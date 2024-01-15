using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class Items
{
    public Gear item;
    public ItemSlot itemSlot;
}

public class ItemRecipe : MonoBehaviour
{
    [SerializeField] private List<Items> items;


    // Start is called before the first frame update
    void Start()
    {
        
        for (int i = 0; i < items.Count; i++)
        {
            items[i].itemSlot.InitializeSlot(items[i].item);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
