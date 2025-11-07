using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;

    [Header("Can")]
    public float health = 100f;

    public enum AIState { Idle, Patrol, Chase, Attack }
    public AIState currentState;

    [Header("Patrol (Devriye)")]
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    [Header("Attack (Saldırı)")]
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile;

    [Header("Menziller")]
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        player = GameObject.Find("Capsule").transform; 
        agent = GetComponent<NavMeshAgent>();
        
        currentState = AIState.Patrol;
    }

    private void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) currentState = AIState.Patrol;
        if (playerInSightRange && !playerInAttackRange) currentState = AIState.Chase;
        if (playerInSightRange && playerInAttackRange) currentState = AIState.Attack;

        switch (currentState)
        {
            case AIState.Idle:
                currentState = AIState.Patrol;
                break;
            case AIState.Patrol:
                Patroling();
                break;
            case AIState.Chase:
                ChasePlayer();
                break;
            case AIState.Attack:
                AttackPlayer();
                break;
        }
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();
        if (walkPointSet) agent.SetDestination(walkPoint);
        if (Vector3.Distance(transform.position, walkPoint) < 1f) walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround)) walkPointSet = true;
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            if (projectile != null)
            {
                Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
                rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
                Destroy(rb.gameObject, 3f);
            }
            
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0f)
        {
            Destroy(gameObject);
        }
    }
}