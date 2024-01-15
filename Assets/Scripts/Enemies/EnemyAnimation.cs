using System;
using UnityEngine;


public class EnemyAnimation : MonoBehaviour
{
    private Animator animator;

    private float enemyDamage;
    private float enemyAttackSize;
    private float enemyRange;
    private LayerMask playerLayer;
    private bool attackState;
    private float enemySpeed;

    [SerializeField] private float deathAnimationTime = 1f;

    private void OnEnable()
    {
        Enemy.onEnemyDeath += DeathSequence;
    }
    private void OnDisable()
    {
        Enemy.onEnemyDeath -= DeathSequence;
    }
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }


    private void DeathSequence()
    {
        animator.SetTrigger("Death");
        Invoke(nameof(EndDeathSequence), deathAnimationTime);
        
    }
    private void EndDeathSequence()
    {
        gameObject.GetComponentInParent<Enemy>().gameObject.SetActive(false);
    }
    public void EnemyStats(float damage,float attackSize,float range,LayerMask playerLayer)
    {
        enemyDamage = damage;
        enemyAttackSize = attackSize;
        enemyRange = range;
        this.playerLayer = playerLayer;
    }

    public void EnemyAttackState(bool attackState)
    {
        this.attackState = attackState;
    }
    public void EnemySpeed(float speed)
    {
        enemySpeed = speed;

    }
    // Update is called once per frame

    void Update()
    {    
        if (attackState)
        {
            animator.SetTrigger("Attack");
        }
        animator.SetFloat("Speed", enemySpeed);
    }

    public void StartEnemyAttack()
    {
        Physics.SphereCast(transform.position + Vector3.up, enemyAttackSize, transform.forward, out var hitInfo, enemyRange, playerLayer);
        if (hitInfo.collider != null)
        {
           if (hitInfo.collider.CompareTag("Player"))
           {
                //
                PlayerManager.instance.UpdatePlayerHealth(enemyDamage);
           }
           

        }
    }

    
}
