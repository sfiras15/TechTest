using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Health System for the enemies and the player eventually
public class Health
{
    private int currentHealth;
    private int maxHealth;

    public Health(int value)
    {
        this.maxHealth = value;
        this.currentHealth = maxHealth;
    }
    public int GetCurrentHealth
    {
        get { return currentHealth; }
    }
    public int GetMaxHealth
    {
        get { return maxHealth; }
    }

    public int SetCurrentHealth
    {
        set { currentHealth = value; }
    }
    public int SetMaxHealth
    {
        set { maxHealth = value; }
    }
    public void Damage(float amount)
    {
        this.currentHealth -= Mathf.RoundToInt(amount);
        if (this.currentHealth < 0 ) this.currentHealth = 0;
    }
    public void Heal(float amount)
    {
        this.currentHealth += Mathf.RoundToInt(amount);
        if (this.currentHealth > maxHealth) currentHealth=maxHealth;
    }


}
