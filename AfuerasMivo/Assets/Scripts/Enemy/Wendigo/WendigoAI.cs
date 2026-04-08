using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class WendigoAI : MonoBehaviour
{
    private NavMeshAgent _enemyAgent;
    public enum EnemyState
    {
        Chasing,
        Charging,
        Attacking,
    }

    public EnemyState currentState;

    //Chasing
    [SerializeField] private float _detectionRange = 7f;

    //Attack
    [SerializeField] private float _attackRange = 2f;
    [SerializeField] private float _attackTimer;
    [SerializeField] private float _attackDelay = 2;
    [SerializeField] private Transform _attackPosition;
    [SerializeField] private int _attackRadius = 5;

    //Charging
    [SerializeField] private float _chargingTimer;
    [SerializeField] private float _chargingDelay = 5;

    //Player
    private Transform _player;

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
            case EnemyState.Charging:
                Charging();
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
        _enemyAgent.SetDestination(_player.position);
        _enemyAgent.isStopped = false;
        if(OnRange(_attackRange))
        {
            currentState = EnemyState.Attacking;
        }
    }

    void Charging()
    {
        _enemyAgent.isStopped = true;
        _enemyAgent.ResetPath();

        _chargingTimer += Time.deltaTime;

        if(_chargingTimer >= _chargingDelay)
        {
            currentState = EnemyState.Chasing;
            _chargingTimer = 0;
        }
    }

    void Attacking()
    {
        if(OnRange(_attackRange))
        {
            _enemyAgent.isStopped = true;

            _attackTimer += Time.deltaTime;

            if(_attackTimer >= _attackDelay)
            {
                Attack();
                _attackTimer = 0;
                currentState = EnemyState.Charging;
            }
        }
        if(!OnRange(_attackRange))
        {
            currentState = EnemyState.Chasing;
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
    

    public void TurnToCharging()
    {
        currentState = EnemyState.Charging;
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
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);

        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(_attackPosition.position, _attackRadius);
    }
}
