using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class AperionAI : MonoBehaviour
{
    private NavMeshAgent _enemyAgent;
    public enum EnemyState
    {
        Patrolling,
        Chasing,
        Attacking,
    }

    public EnemyState currentState;

    //Chasing
    [SerializeField] private Transform[] _patrolPoints;

    //OnRange
    [SerializeField] private float _detectionRange = 7f;

    //Attacking
    [SerializeField] private float _attackRange = 2f;
    [SerializeField] private float _attackTimer;
    [SerializeField] private float _attackDelay = 5;

    //Attack
    [SerializeField] private Transform _attackPosition;
    [SerializeField] private float _attackRadius = 5f;
    [SerializeField] private int _damage = 25;

    //Life
    [SerializeField] private int _currentLife;
    [SerializeField] private int _maxLife = 150;

    private Transform _player;
    void Awake()
    {
        _enemyAgent = GetComponent<NavMeshAgent>();
        _player = GameObject.FindWithTag("Player").transform;

    }
    void Start()
    {
        _currentLife = _maxLife;
        currentState = EnemyState.Patrolling;
        _enemyAgent.SetDestination(_player.position);
        _attackTimer = _attackDelay;
        PatrollingPoints();
    }


    void Update()
    {
        switch(currentState)
        {
            case EnemyState.Patrolling:
                Patrolling();
            break;
            case EnemyState.Chasing:
                Chasing();
            break;
            case EnemyState.Attacking:
                Attacking();
            break;
            default:
                Patrolling();
            break;
        }

        
    }

    void Patrolling()
    {
        if(OnRange(_detectionRange))
        {
            currentState = EnemyState.Chasing;
        }
        if(!OnRange(_detectionRange))
        {
           if(_enemyAgent.remainingDistance < 0.5)
            {
                PatrollingPoints();
            }
        }
    }

    void PatrollingPoints()
    {
        _enemyAgent.SetDestination(_patrolPoints[Random.Range(0, _patrolPoints.Length)].position);
    }

    void Chasing()
    {
        if(!OnRange(_detectionRange))
        {
            currentState = EnemyState.Patrolling;
            return;
        }
        if(OnRange(_attackRange))
        {
            currentState = EnemyState.Attacking;
            return;
        }
        if(OnRange(_detectionRange))
        {
            _enemyAgent.SetDestination(_player.position);
        }
    }

    void Attacking()
    {
        if(!OnRange(_attackRange))
        {
            _enemyAgent.isStopped = false;
            currentState = EnemyState.Chasing;
            return;
        }
        _enemyAgent.isStopped = true;
        _attackTimer += Time.deltaTime;
        if(_attackTimer > _attackDelay)
            {
                Attack();
                Debug.Log("Attacking!");
                _attackTimer = 0;
            }
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

    void Attack()
    {
        Collider[] players = Physics.OverlapSphere(_attackPosition.position, _attackRadius);
            foreach (Collider item in players)
            {
                if(item.gameObject.CompareTag("Player"))
                {
                    PlayerResources _playerResources = item.GetComponent<PlayerResources>();
                    
                    if(_playerResources != null)
                    {
                        _playerResources.TakeDamage(25);
                    }
                }
            }
    }

    void TakeDamage()
    {
        _currentLife -= 20;
    }

    void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.CompareTag("Arrow"))
        {
            collider.gameObject.SetActive(false);
            TakeDamage();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_attackPosition.position, _attackRadius);
    
        Gizmos.color = Color.yellow;
        foreach (Transform point in _patrolPoints)
        {
            Gizmos.DrawWireSphere(point.position, 5f);
        }
    }
}
