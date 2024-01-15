using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MiningBar : MonoBehaviour
{
    [SerializeField] private Image fillBar;
    [SerializeField] private Image progressiveBar;

    private void OnEnable()
    {
        MiningNode.OnMining += FillUI;
    }

    private void OnDisable()
    {
        MiningNode.OnMining -= FillUI;
    }

    private void FillUI(float miningTime)
    {
        Color progressBarColor = progressiveBar.color;
        progressBarColor.a = 110f / 255f; // Set alpha to 110/255
        progressiveBar.color = progressBarColor;

        StartCoroutine(FillOverTime(miningTime));
    }

    private IEnumerator FillOverTime(float miningTime)
    {
        float elapsedTime = 0f;

        while (elapsedTime < miningTime)
        {
            elapsedTime += Time.deltaTime;

            // Calculate fill amount based on elapsed time
            float fillAmount = elapsedTime / miningTime;

            // Limit fill amount to [0, 1]
            fillAmount = Mathf.Clamp01(fillAmount);

            // Set fill amount
            fillBar.fillAmount = fillAmount;

            yield return null;
        }

        // Hide the progressive bar after filling is complete
        Color progressBarColor = progressiveBar.color;
        progressBarColor.a = 0f;
        progressiveBar.color = progressBarColor;
    }
}
