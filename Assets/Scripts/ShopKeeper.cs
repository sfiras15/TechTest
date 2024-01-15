using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopKeeper : MonoBehaviour, IInteractable
{
    [Header("References")]
    private InteractableUI interactableUI;

    private GameObject player;

    [SerializeField] private float interactDistance = 2f;
    [SerializeField] private GameObject shopUI;

    private Bounds interactableBounds;

    private void Awake()
    {
        interactableUI = GetComponentInChildren<InteractableUI>();
        player = GameObject.FindGameObjectWithTag("Player");
        CalculateInteractableBounds();
    }

    private void Start()
    {
        interactableUI.InitializeItemName("Shop");
        shopUI.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!IsPlayerInRange())
        {
            CloseUI();
        }
    }

    private void CalculateInteractableBounds()
    {
        // Assuming your ShopKeeper collider is on the same GameObject
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            interactableBounds = collider.bounds;
        }
        else
        {
            // If the collider is on a child GameObject, you might need to adjust accordingly
            interactableBounds = new Bounds(transform.position, Vector3.zero);
        }
    }

    private bool IsPlayerInRange()
    {
        // Use Bounds.ClosestPoint to get the closest point on the bounds to the player's position
        Vector3 closestPoint = interactableBounds.ClosestPoint(player.transform.position);
        return Vector3.Distance(player.transform.position, closestPoint) < interactDistance;
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
        Debug.Log("yo");
        OpenUI();
    }

    public void OpenUI()
    {
        shopUI.SetActive(true);
    }

    public void CloseUI()
    {
        shopUI.SetActive(false);
    }
}
