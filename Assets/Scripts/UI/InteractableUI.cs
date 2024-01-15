using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractableUI : MonoBehaviour
{
    [SerializeField] private GameObject buttonContainer;
    [SerializeField] private TextMeshProUGUI errorContainer;
    private Transform mainCameraTransform;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private TextMeshProUGUI itemNameText;


    private void OnEnable()
    {
        InteractableDetection.onInitializeUI += InitializeIconUI;
    }
    private void OnDisable()
    {
        InteractableDetection.onInitializeUI -= InitializeIconUI;
    }

    private void InitializeIconUI(KeyCode interactKey)
    {
        buttonText.text = interactKey.ToString();
    }
    public void InitializeItemName(string itemName)
    {
        itemNameText.text = itemName;
    }

    public void Show()
    {
        buttonContainer.gameObject.SetActive(true);
    }
    public void ShowError(string text)
    {
        errorContainer.text = text;
        errorContainer.gameObject.SetActive(true);
    }

    public void Hide()
    {
        buttonContainer.gameObject.SetActive(false);
    }
    public void HideError()
    {
        errorContainer.gameObject.SetActive(false);
    }
    private void Start()
    {
        // Find the main camera's transform
        mainCameraTransform = Camera.main.transform;
       
    }

    private void LateUpdate()
    {
        // Make the health bar canvas face the camera
        transform.LookAt(transform.position + mainCameraTransform.rotation * Vector3.forward, mainCameraTransform.rotation * Vector3.up);
    }
}
