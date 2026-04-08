using Unity.Profiling;
using UnityEngine;
using UnityEngine.UIElements;

public class Chest : MonoBehaviour, IInteractable
{
    //Booleanas
    private bool isOpen = false;
    [SerializeField] private bool _isPlayerNear = false;

    //Components
    private Animator _animator;
    private PlayerResources _playerResources;
    [SerializeField] private SphereCollider _sphereCollider;
    

    [SerializeField] private ParticleSystem _chestParticles;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _animator = GetComponent<Animator>();

        _playerResources = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerResources>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interact()
    {
        if(!isOpen)
        {
            _chestParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            isOpen = true;
            _animator.SetTrigger("IsOpen");
            _playerResources.manaPotions++;
            _playerResources.healthPotions++;
            _playerResources.ManaText();
            _playerResources.HealthText();
            
        }
        return;
    }
    void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.CompareTag("Player"))
        {
            _chestParticles.Play();
        }
    }
    void OnTriggerExit(Collider collider)
    {
        if(collider.gameObject.CompareTag("Player"))
        {
            _chestParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, _sphereCollider.radius);  
    }
}
