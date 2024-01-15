using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
    public enum BarType
    {
        health=0,
        stamina = 1,
    }
    private Image fillBar;
    private Text barText;
    [SerializeField] private BarType barType;
    private void OnEnable()
    {
        if (barType == BarType.stamina)
        {
            InputPlayer.OnStaminaChange += UpdateBar;   
        }
        else
        {
            InputPlayer.OnHealthChange += UpdateBar;
        }
    }
    private void OnDisable()
    {
        if (barType == BarType.stamina)
        {
            InputPlayer.OnStaminaChange -= UpdateBar;
        }
        else
        {
            InputPlayer.OnHealthChange -= UpdateBar;
        }
    }
    private void Awake()
    {
        fillBar = GetComponent<Image>();

        // if it's a healthBar get the component
        if (transform.childCount != 0) barText = GetComponentInChildren<Text>();

    }
    public void UpdateBar(int maxValue, int currentValue)
    {
        //Debug.Log("maxValue = " + maxValue + " CurrentValue = " + currentValue);
        fillBar.fillAmount = (float)currentValue / maxValue;
        // if it's a healthBar get the text
        if (barText != null) barText.text = currentValue.ToString() + "/" + maxValue.ToString();
    }

    public Text GetHealthBarText
    {
        get { return barText; }
    }
}
