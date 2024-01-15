using System;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    private Gear item;

    [SerializeField] private Image itemIcon;

    public static event Action<Gear> onButtonClick;
    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitializeSlot(Gear item)
    {
        this.item = item;

        itemIcon.sprite = item.itemIcon;

    }
    public void ButtonClick()
    {
        onButtonClick?.Invoke(item);
        //Load the item's information in the items info UI
    }
}
