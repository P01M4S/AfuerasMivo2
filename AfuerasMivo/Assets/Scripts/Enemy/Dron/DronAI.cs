using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class DronAI : MonoBehaviour
{
    private NavMeshAgent _enemyAgent;
    public enum EnemyState
    {
        Chasing,
        Attacking,
    }

    //Chasing
    [SerializeField] private float _detectionRange = 15f;

    //Attack
    [SerializeField] private float _attackRange = 12.5f;
    [SerializeField] private float _attackTimer;
    [SerializeField] private float _attackDelay = 2;

    //Player
    private Transform _player;

    public EnemyState currentState;
    
    void Awake()
    {
        _enemyAgent = GetComponent<NavMeshAgent>();
        _player = GameObject.FindWithTag("Player").transform;
    }

    void Start()
    {
        currentState = EnemyState.Chasing;
        _attackTimer = _attackDelay;
    }

    void Update()
    {
        switch(currentState)
        {
            case EnemyState.Chasing:
                Chasing();
            break;
            case EnemyState.Attacking:
                Attacking();
            break;
            default:
                Chasing();
            break;

        }
    }

    void Chasing()
    {
        if(OnRange(_detectionRange))
        {
            _enemyAgent.isStopped = false;
            _enemyAgent.SetDestination(_player.position);
        }
        if(OnRange(_attackRange))
        {
            currentState = EnemyState.Attacking;
        }
    }

    void Attacking()
    {
        _enemyAgent.isStopped = true;
        
        _attackTimer += Time.deltaTime;
        if(_attackTimer >= _attackDelay)
        {
            Attack();
        }        
    }

    void Attack()
    {
        currentState = EnemyState.Chasing;
        Debug.Log("Attack");
        _attackTimer = 0;
    }

    public bool OnRange(float distance)
    {
        float distanceToPlayer = Vector3.Distance(transform.position, _player.position);
        
        if(distanceToPlayer <= distance)
        {
         return true;    
        }
        else
        {
            return false;
        }  
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}
