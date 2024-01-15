using System;
using UnityEngine;


public class ItemPickup : MonoBehaviour, IInteractable
{
    [SerializeField] private int id;
    [SerializeField] private Item item;

    private bool collected;

    private bool despawnTimerOn;
    [SerializeField] private float lastSpawnerTime;
    [SerializeField] private float despawnTime = 10;

    private InteractableUI interactableUI;


    //Event to update the UI in case we are trying to pick an item and the inventory is full
    public static event Action<bool> onErrorPickUp;
    
    private void Awake()
    {
        interactableUI = GetComponentInChildren<InteractableUI>();
    }
    private void Start()
    {
        interactableUI.InitializeItemName(item.itemName.ToString());
    }
    private void Update()
    {
        if (gameObject.activeSelf)
        {
            if (!despawnTimerOn)
            {
                despawnTimerOn = true;
                lastSpawnerTime = Time.time;
            }
            
            if (Time.time - lastSpawnerTime > despawnTime && despawnTimerOn)
            {
                
                collected = true;
                this.gameObject.SetActive(false);
                
            }

        }
        else
        {
            despawnTimerOn = false;
        }
    }

    public void ShowUI()
    {
        interactableUI.Show();
    }
    public void HideUI()
    {
        interactableUI.Hide();
    }
    public void Interact()
    {
        CollectItem();
    }
    public void CollectItem()
    {
        collected = Inventory.instance.Add(item);
        if (collected) this.gameObject.SetActive(false);
        else
        {
            onErrorPickUp?.Invoke(true);
            Invoke(nameof(ResetErrorUI), 1.5f);
            interactableUI.ShowError("Inventory Full.");
        }

    }
    public void ResetErrorUI()
    {
        interactableUI.HideError();
        
    }
    

}
