using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Scripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Unity.Cinemachine;

public class PlayerController : MonoBehaviour
{

    //Componentes
    private CharacterController _controller;
    private Animator _animator;
    private Transform _mainCamera;
    private PlayerResources _playerResource;

    //Inputs
    public Vector2 _moveValue;
    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _interactAction;
    private InputAction _dashAction;
    private InputAction _aimingAction;
    private InputAction _manaAction;
    private InputAction _healthAction;
    //private InputAction Prueba;
    
    //Movimiento
    [Header("Movement")]
    public float _playerSpeed = 9;
    public float _playerMovementSpeed = 9;
    private float _playerForce = 2;
    private float _pushForce = 10;
    private float _smoothTime = 0.2f;
    private float _turnSmoothVelocity;
    public float _speed;
    public float _speedChangeRate = 10;
    public float targetAngle;

    //MovementAnimation
    public float _animationSpeed;
    private float _smoothSpeed = 0f;

    //Suelo
    [Header("Ground")]
    public Transform _sensorPosition;
    public LayerMask _groundLayer;
    [SerializeField] private float _sensorRadius = 0.5f;

    //Gravedad
    [Header("Gravity")]
    public Vector3 _playerGravity;
    private float _gravity = -9.81f;

    //Interact
    [Header("Interact")]
    public Transform _interactionPosition;
    public Vector3 _interactionRadius;

    //Jump
    [Header("Jump")]
    public float jumpTimeOut = 0.5f;
    public float fallTimeOut = 0.15f; 
    public float _jumpHeight = 2f;
    
    float _jumpTimeOutDelta;
    float _fallTimeOutDelta;
    
    //Dash
    [Header("Dash Cooldown")]
    [SerializeField] private float dashCooldown = 1.25f;
    private bool isDashOnCooldown = false;
    private Coroutine dashCooldownRoutine;

    [Header("Dash")]
    [SerializeField] private float _dashSpeed = 20;
    [SerializeField] private float _dashTime = 0.25f;
    private Vector3 _lastMoveDirection;
    private bool isDashing = false;

    //Camara
    [Header("Aim")]
    public bool isAiming = false;
    [SerializeField] private GameObject _crosshair;
    [SerializeField] private int _aimingSpeed = 4;

    //Potions
    [Header("Potions")]
    [SerializeField] private int _manaReg = 25;
    [SerializeField] private int _healthReg = 25;

    //Camera
    [Header("Camera")]
    [SerializeField] private CinemachineCamera _freeLookCam;

    [SerializeField] private float _forwardZoom = 4f;
    [SerializeField] private float _backZoom = 5.5f;
    [SerializeField] private float _currentZoom;

    void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _playerResource = GetComponent<PlayerResources>();

        _moveAction = InputSystem.actions["Move"];
        _jumpAction = InputSystem.actions["Jump"];
        _interactAction = InputSystem.actions["Interact"];
        _dashAction = InputSystem.actions["Dash"];
        _manaAction = InputSystem.actions["PotionsMana"];
        _healthAction = InputSystem.actions["PotionsHealth"];
        _aimingAction = InputSystem.actions["Aiming"];

        _mainCamera = Camera.main.transform;
    }
    void Update()
    {
        if(GameManager.Instance._isDead || GameManager.Instance._isPaused) return;
        _moveValue = _moveAction.ReadValue<Vector2>();

        //Acciones
        if(_jumpAction.WasPerformedThisFrame() && IsGrounded() && isAiming == false)
        {
            Jump();
        }
        if(_interactAction.WasPressedThisFrame())
        {
            Interact();
        }
        if(_dashAction.WasPressedThisFrame() && _moveValue != Vector2.zero && !isDashing && !isDashOnCooldown)
        {
            if(isAiming)
            {
                Aiming();
            }
            StartCoroutine(Dash());
        }

        if(_aimingAction.WasPressedThisFrame() && IsGrounded())
        {
            Mouse();
            Aiming();
        }




        /*if(Prueba.WasPressedThisFrame())
        {
            LoseHealth();
        }*/

        Movement();

        Gravity();
    }

    /*void Start()
    {
        _currentZoom = _forward;
    }*/ 

    
    void Movement()
    {
        if(isDashing) return;
        
        if(isAiming == false)
        {
            
            Vector3 direction = new Vector3(_moveValue.x, 0, _moveValue.y);

            float targetSpeed = _playerSpeed;
            
            if(direction == Vector3.zero)
            {
                targetSpeed = 0;
            }

            _speed = Mathf.SmoothDamp(_speed, targetSpeed * direction.magnitude, ref _smoothSpeed, 0.1f);

            _animationSpeed = Mathf.Lerp(_animationSpeed, targetSpeed, Time.deltaTime * _speedChangeRate);

            if(_animationSpeed < 0.1f)
            {
                _animationSpeed = 0;
            }

            _animator.SetFloat("Speed", _animationSpeed);

            if (direction != Vector3.zero)
            {
                targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _mainCamera.eulerAngles.y;
                float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, _smoothTime);
                transform.rotation = Quaternion.Euler(0, smoothAngle, 0);

                _lastMoveDirection = (Quaternion.Euler(0, targetAngle, 0) * Vector3.forward).normalized;
            }

            Vector3 moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            _controller.Move(_speed * Time.deltaTime * moveDirection.normalized  + _playerGravity * Time.deltaTime);
        }
        else if(isAiming == true)
        {
            
            Vector3 direction = new Vector3(_moveValue.x, 0, _moveValue.y);

            _animator.SetFloat("Horizontal", _moveValue.x);
            _animator.SetFloat("Vertical", _moveValue.y);

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _mainCamera.eulerAngles.y;
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, _mainCamera.eulerAngles.y, ref _turnSmoothVelocity, _smoothTime);

            transform.rotation = Quaternion.Euler(0, smoothAngle, 0);

            if (direction != Vector3.zero)
            {
                Vector3 moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

                _controller.Move(moveDirection.normalized * _playerSpeed * Time.deltaTime);
            } 
        }
    }

    void Aiming()
    {
        isAiming = !isAiming;
        _crosshair.SetActive(isAiming);
        _animator.SetBool("IsAiming", isAiming);
        Debug.Log(isAiming);
        CameraAim();
        if(isAiming)
        {
            _playerSpeed = _aimingSpeed;
        }
        if(!isAiming)
        {
            _playerSpeed = _playerMovementSpeed;
        }
    }

    void Jump()
    {
        if(_jumpTimeOutDelta <= 0)
        {
            _animator.SetTrigger("Jump");
            _playerGravity.y = Mathf.Sqrt(_jumpHeight * -2 * _gravity);
        }
    }

    void Gravity()
    {
        _animator.SetBool("Grounded", IsGrounded());

        if(IsGrounded())
        {
            _fallTimeOutDelta = fallTimeOut;
            
            //_animator.SetBool("Jump", false);
            _animator.SetBool("Fall", false);
            if(_playerGravity.y < 0)
            {
                _playerGravity.y = -2;
            }

            if(_jumpTimeOutDelta >= 0)
            {
                _jumpTimeOutDelta -= Time.deltaTime;
            }
        }
        else
        {
            _jumpTimeOutDelta = jumpTimeOut;

            if(_fallTimeOutDelta >= 0)
            {
                _fallTimeOutDelta -= Time.deltaTime;
            }
            else
            {
                _animator.SetBool("Fall", true);
            }
            
            _playerGravity.y += _gravity * Time.deltaTime;
        }
    }

    bool IsGrounded()
    {
        return Physics.CheckSphere(_sensorPosition.position, _sensorRadius, _groundLayer);
    }

    IEnumerator Dash()
    {
        isDashing = true;
        float timer = 0;

        while(timer < _dashTime)
        {
            _controller.Move(_lastMoveDirection.normalized * _dashSpeed * Time.deltaTime);

            timer += Time.deltaTime;
            yield return null;
        }
        
        isDashing = false;
        dashCooldownRoutine = StartCoroutine(DashCoolDown());
    }

    IEnumerator DashCoolDown()
    {
        isDashOnCooldown = true;
        yield return new WaitForSecondsRealtime(dashCooldown);
        isDashOnCooldown = false;
    }

    void Interact()
    {
        Collider[] objectsToGrab = Physics.OverlapBox(_interactionPosition.position, _interactionRadius);
            foreach (Collider item in objectsToGrab)
            {
                if(item.gameObject.layer == 6)
                {
                    IInteractable interactableObject = item.GetComponent<IInteractable>();
                    if(interactableObject != null)
                    {
                        interactableObject.Interact(); 
                    }
                }
            }
    }

    /*void LoseHealth()
    {
        _playerResource.currentHealth -= 25;
        _playerResource.UpdateHealthBar();
    }*/

    void Mouse()
    {
        if(Cursor.visible == true)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    void CameraAim()
    {
        var orbital = _freeLookCam.GetComponent<CinemachineOrbitalFollow>();
        var rotationComposer = _freeLookCam.GetComponent<CinemachineRotationComposer>();

        var settings = orbital.Orbits;

        if(isAiming)
        {
            //TargetOffset
            orbital.TargetOffset = new Vector3(0.39f, 0.84f, -0.17f);

            //Height
            settings.Top.Height = 2.76f;
            settings.Center.Height = 2.27f;
            settings.Bottom.Height = 1.22f;

            //Radius
            settings.Top.Radius = 1.34f;
            settings.Center.Radius = 1.65f;
            settings.Bottom.Radius = 1.34f;

            //TargetOffsetRotation
            rotationComposer.TargetOffset = new Vector3(0.39f, 2.2f, 0.31f);

            orbital.Orbits = settings;

            Debug.Log("Hola");

            return;
        }
        else
        {
            //TargetOffset
            orbital.TargetOffset = new Vector3(0.2f, 0f, 0f);

            //Height
            settings.Top.Height = 5f;
            settings.Center.Height = 2.25f;
            settings.Bottom.Height = 0.1f;

            //Radius
            settings.Top.Radius = 2f;
            settings.Center.Radius = 5.5f;
            settings.Bottom.Radius = 2.5f;

            //TargetOffsetRotation
            rotationComposer.TargetOffset = new Vector3(0f, 0f, 0f);

            orbital.Orbits = settings;

            Debug.Log("Holi");

            return;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(_sensorPosition.position, _sensorRadius);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(_interactionPosition.position, _interactionRadius);
    }
}