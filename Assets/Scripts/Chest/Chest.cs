using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 
/// </summary>
public class Chest : MonoBehaviour, IInteractable
{
    [SerializeField] private int chestID;
    //[SerializeField] private List<Item> items;
    private InteractableUI interactableUI;

    private GameObject player;

    //[SerializeField] private GameObject chestCanvas;
    [SerializeField] private float interactDistance = 2f;

    // Array for the prefabs that will initialize inside the chest
    [SerializeField] private Item[] prefabsInsideTheChest;

    //Array for the items currently inside The chest
    [SerializeField] private List<Item> itemsInsideTheChest = new List<Item>();

    private ChestUI chestUI;
    private void OnEnable()
    {
        ChestSlot.onItemCollected += ArrangeItemList;
    }

    private void OnDisable()
    {
        ChestSlot.onItemCollected -= ArrangeItemList;
    }

    public void ArrangeItemList(Item item)
    {
        //This solution does not remove the exact item's place if there's duplicates of the same item inside the chest , fix later by changing itemsList to a dictionary that tracks 
        // the number of instances
        itemsInsideTheChest.Remove(item);
    }
  

    private void Awake()
    {
        interactableUI = GetComponentInChildren<InteractableUI>();
        player = GameObject.FindGameObjectWithTag("Player");

        chestUI = GetComponentInChildren<ChestUI>();
        
        //Initialize the Items inside the chest
        for (int i = 0; i < prefabsInsideTheChest.Length; i++)
        {
            itemsInsideTheChest.Add(prefabsInsideTheChest[i]);
        }
        //chestUI.UpdateUI(itemsInsideTheChest);
    }

    private void Start()
    {
        interactableUI.InitializeItemName("Chest");
        chestUI.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (Vector3.Distance(player.transform.position,transform.position) >= interactDistance)
        {
            CloseUI();
        }
        if (chestUI.gameObject.activeSelf) chestUI.UpdateUI(itemsInsideTheChest);
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
        OpenUI();
    }

    public void OpenUI()
    {
        chestUI.gameObject.SetActive(true);
    }
    public void CloseUI()
    {
        chestUI.gameObject.SetActive(false);
    }

   
}
