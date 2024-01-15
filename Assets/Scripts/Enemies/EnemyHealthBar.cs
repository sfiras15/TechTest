using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Image fillBar;
    private Transform mainCameraTransform;

    public void UpdateHealth(int maxHealth,int currentHealth)
    {
        fillBar.fillAmount = (float) currentHealth / maxHealth;
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
