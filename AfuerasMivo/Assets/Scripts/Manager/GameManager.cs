using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private InputAction _stopAction;

    public bool _isPaused = false;
    public bool _isDead = false;

    void Awake()
    {
        if(Instance != this && Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else if(Instance == null)
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);

        _stopAction = InputSystem.actions["Stop"];
    }
    
    void Start()
    {
        
    }


    void Update()
    {
        if(_stopAction.WasPressedThisFrame())
        {
            Pause();
        }
    }

    void Pause()
    {
        if(_isPaused == false)
        {
            _isPaused = !_isPaused;
            Time.timeScale = 0;
        }
        else
        {
            _isPaused = !_isPaused;
            Time.timeScale = 1;
        }
        
    }
}
