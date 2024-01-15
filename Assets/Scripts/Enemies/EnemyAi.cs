using System.Collections;
using UnityEngine;
using UnityEngine.AI;
/// <summary>
/// Handles the different behaviours of the enemy like chasing/patrolling and attacking the player
/// </summary>
public class EnemyAi: MonoBehaviour
{
    private EnemyAnimation enemyAnimation;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private LayerMask whatIsGround, whatIsObstacle,whatIsPlayer;
    private Transform player;

    //Patroling centrePoint
    [SerializeField] private Transform centrePoint;
    // range of the sphere we are casting
    [SerializeField] private float range;

    //Attacking
    //Get the attack animation duration
    [SerializeField] private float attackDuration;
    private bool alreadyAttacked;

    [SerializeField] float attackDamage;

    //the size of the sphere that's being casted when the enemy attacks in EnemyAnimation
    [SerializeField] private float attackSize = 0.15f;

    //States
    [SerializeField] private float sightRange, attackRange;
    private bool playerInSightRange, playerInAttackRange;

    private Collider[] collidersInOverlapSphere;

    [SerializeField] private bool wasChasing;

    [SerializeField] private bool chasingPlayer;

    [SerializeField] private float lastChasedTime;

    [SerializeField] private float timeToResetAlertness = 5f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyAnimation = GetComponentInChildren<EnemyAnimation>();
    }

    
    private void Start()
    {
        player = PlayerManager.instance.playerTransform;

        //variables used for the attackAnimation
        enemyAnimation.EnemyStats(attackDamage, attackSize, attackRange, whatIsPlayer);
    }

    private void Update()
    {
        //FindPlayer in sightRange
        collidersInOverlapSphere = Physics.OverlapSphere(transform.position, sightRange, whatIsPlayer);

        if (collidersInOverlapSphere.Length > 0)
        {
            // Check for obstacles in front of the player or if the player was being chased recently and is still inside the sightRange
            playerInSightRange = !FindObstacles(collidersInOverlapSphere[0]) || wasChasing;

            playerInAttackRange = playerInSightRange && Vector3.Distance(transform.position, collidersInOverlapSphere[0].transform.position) <= attackRange;     
        }
        else
        {
            playerInSightRange = false;
            playerInAttackRange = false;
        }

        //Debug.Log("playerInSightRange " + playerInSightRange);
        //Debug.Log("playerInAttackRange " + playerInAttackRange);

        if (!playerInSightRange && !playerInAttackRange && !alreadyAttacked) Patroling();
        if (playerInSightRange && !playerInAttackRange && !alreadyAttacked) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) AttackPlayer();

        enemyAnimation.EnemySpeed(agent.velocity.magnitude);
        
    }


    private bool FindObstacles(Collider playerCollider)
    {
        if (playerCollider == null) return false;

        Vector3 playerDirection = (playerCollider.transform.position - transform.position).normalized;
        bool obstacleFound = Physics.Raycast(transform.position, playerDirection, sightRange, whatIsObstacle);

        return obstacleFound;

    }


    private void Patroling()
    {
        if (chasingPlayer)
        {
            chasingPlayer = false;
            wasChasing = true;
            lastChasedTime = Time.time;
        }
        ResetAlertness();
        if (agent.remainingDistance <= agent.stoppingDistance) //done with path
        {
            Vector3 point;
            if (RandomPoint(centrePoint.position, range, out point)) //pass in our centre point and radius of area
            {
                Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f); //so you can see with gizmos
                agent.SetDestination(point);
            }

        }
    }

    
    private void ResetAlertness()
    {
        if (Time.time - lastChasedTime >= timeToResetAlertness)
        {
            wasChasing = false;
        }
    }
    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {

        Vector3 randomPoint = center + Random.insideUnitSphere * range; //random point in a sphere 
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
        {
            //the 1.0f is the max distance from the random point to a point on the navmesh, might want to increase if range is big
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }


    private void ChasePlayer()
    {
        chasingPlayer = true;
        wasChasing = false;
        
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        if (chasingPlayer)
        {
            chasingPlayer = false;
            wasChasing = true;      
        }
        //Make sure enemy doesn't move
        agent.SetDestination(transform.position);

        //transform.LookAt(player);

        Vector3 direction = (player.position- transform.position).normalized;
        transform.forward = new Vector3(direction.x, 0, direction.z);

        //Debug.Log(alreadyAttacked);
        if (!alreadyAttacked)
        {
            alreadyAttacked = true;
            enemyAnimation.EnemyAttackState(true);
            //Invoke(nameof(ResetAttack), attackDuration);
            StartCoroutine(ResetAttack(attackDuration));
        }
        // fix bug where the walk animation doesn't play right after the enemy attacks and he was moving
    }
    private IEnumerator ResetAttack(float duration)
    {
        yield return new WaitForSeconds(duration);
        alreadyAttacked = false;
        enemyAnimation.EnemyAttackState(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}


