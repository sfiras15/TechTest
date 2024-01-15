using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    [SerializeField] private EnemyHealthBar healthBar;
    private Health enemyHealth;

    public static event Action onEnemyDeath;
    // Start is called before the first frame update
    void Awake()
    {
        enemyHealth = new Health(100);
    }
    public void EnemyDamaged(float damage)
    {
        // TO DO Add animation for taking damage
        enemyHealth.Damage(damage);
        //Update healthbar UI
        healthBar.UpdateHealth(enemyHealth.GetMaxHealth, enemyHealth.GetCurrentHealth);
        if (enemyHealth.GetCurrentHealth == 0)
        {
            // TO DO Add animation for death
            onEnemyDeath?.Invoke();
        }
    }

}
