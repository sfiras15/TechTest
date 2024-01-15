using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Health System for the enemies and the player eventually
public class Stamina
{
    private int currentStamina;
    private int maxStamina;

    public Stamina(int value)
    {
        this.maxStamina = value;
        this.currentStamina = maxStamina;
    }
    public int GetCurrentStamina
    {
        get { return currentStamina; }
    }
    public int GetMaxStamina
    {
        get { return maxStamina; }
    }

    public int SetCurrentStamina
    {
        set { currentStamina = value; }
    }
    public int SetMaxStamina
    {
        set { maxStamina = value; }
    }
    public void Drain(float amount)
    {
        this.currentStamina -= Mathf.RoundToInt(amount);
        if (this.currentStamina < 0) this.currentStamina = 0;
    }
    public void Recover(float amount)
    {
        this.currentStamina += Mathf.RoundToInt(amount);
        if (this.currentStamina > maxStamina) currentStamina = maxStamina;
    }

    
}
