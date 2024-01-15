using System;
using UnityEngine;

public class MiningNode : MonoBehaviour,IInteractable
{
    [SerializeField] private Item nodeReward;
    [SerializeField] private float miningTime = 1f;
    [SerializeField] private int MineCounter = 5;
    private InteractableUI interactableUI;

    public static Action<float> OnMining;

    private bool isMining = false;

    private void Awake()
    {
        interactableUI = GetComponentInChildren<InteractableUI>();
    }
    private void Start()
    {
        interactableUI.InitializeItemName("Mine");
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
        if (!isMining) MineNode();


    }

    private void MineNode()
    {
        Debug.Log("mining...");
        isMining = true;
        OnMining?.Invoke(miningTime);
        Invoke(nameof(EndMining), miningTime);
        
    }

    private void EndMining()
    {
        Inventory.instance.Add(nodeReward);
        isMining = false;
        MineCounter--;
        if (MineCounter == 0) gameObject.SetActive(false);

    }
    // Update is called once per frame
    void Update()
    {
       
    }
}
