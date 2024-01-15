using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Detects gameObjects that can be interacted with and shows's the UI element
/// </summary>
public class InteractableDetection : MonoBehaviour
{
    [SerializeField] private KeyCode interractButtonKey;
    [SerializeField] private float interactDistance = 1.5f;
    [SerializeField] private float detectionDistance = 6f;
    [SerializeField] private LayerMask whatIsItem;
    [SerializeField] private Collider[] interactables;
    [SerializeField] private Collider closestInteractable;

    //Initializing interractButtonKey
    public static event Action<KeyCode> onInitializeUI;
    private bool inventoryError;

    private void OnEnable()
    {
        ItemPickup.onErrorPickUp += InventoryState;


    }
    private void OnDisable()
    {
        ItemPickup.onErrorPickUp -= InventoryState;
    }

    private void Awake()
    {
        if (onInitializeUI != null) onInitializeUI?.Invoke(interractButtonKey);
    }

    private void InventoryState(bool state)
    {
        inventoryError = state;
        Invoke(nameof(ResetInventoryState), 1.5f);
    }
    
    private void ResetInventoryState()
    {
        inventoryError = false;
    }

    // Update is called once per frame
    void Update()
    {
        interactables = Physics.OverlapSphere(transform.position, detectionDistance, whatIsItem);
        if (interactables.Length > 0)
        {
            closestInteractable = GetClosestInteractable(interactables);

            if (closestInteractable != null)
            {
                foreach (var interactable in interactables)
                {
                    if (interactable.TryGetComponent(out IInteractable iInteractable))
                    {
                        Bounds interactableBounds = interactable.bounds;
                        Vector3 closestPoint = interactableBounds.ClosestPoint(transform.position);

                        if (interactable == closestInteractable && Vector3.Distance(transform.position, closestPoint) <= interactDistance && !inventoryError)
                        {
                            // Show the UI for the closest interactable
                            iInteractable.ShowUI();
                            if (Input.GetKeyDown(interractButtonKey)) iInteractable.Interact();
                        }
                        else
                        {
                            // Hide the UI for all other interactables
                            iInteractable.HideUI();
                        }
                    }
                }
            }
        }
    }


    private Collider GetClosestInteractable(Collider[] colliders)
    {
        Collider closestCollider = colliders[0];
        float closestDistance = Vector3.Distance(transform.position, closestCollider.bounds.ClosestPoint(transform.position));

        for (int i = 1; i < colliders.Length; i++)
        {
            float distance = Vector3.Distance(transform.position, colliders[i].bounds.ClosestPoint(transform.position));
            if (distance <= closestDistance)
            {
                closestCollider = colliders[i];
                closestDistance = distance;
            }
        }
        return closestCollider;
    }
}
