using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    //Inputs
    private InputAction _attackInput;

    //Componente
    private Animator _animator;
    private Transform _mainCamera;
    private PlayerController _playerCotroller;


    [Header("Shooting")]
    [SerializeField] private float shootTimer;
    [SerializeField] private float shootDelay = 0.75f;

    //Shoot
    [SerializeField] private Transform _bulletSpawn;

    void Awake()
    {
        _attackInput = InputSystem.actions["Attack"];

        _animator = GetComponent<Animator>();
        _playerCotroller = GetComponent<PlayerController>();
        _mainCamera = Camera.main.transform;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instance._isDead || GameManager.Instance._isPaused) return;
        
        if(shootTimer < shootDelay)
        {
            shootTimer += Time.deltaTime;
        }

        if(_attackInput.WasPressedThisFrame() && shootTimer >= shootDelay)
        {
            Attack();
            _animator.SetTrigger("IsAttacking");
            shootTimer = 0;
        }
    }

    void Attack()
    {
        if(_playerCotroller.isAiming == false)
        {
            GameObject bullet = PoolManager.Instance.GetPooledObject("Bullet", _bulletSpawn.position, _bulletSpawn.rotation);
            bullet.SetActive(true);
        }
        else
        {
            GameObject bullet = PoolManager.Instance.GetPooledObject("Bullet", _bulletSpawn.position, _mainCamera.rotation);
            bullet.SetActive(true);
        }
    }

}
