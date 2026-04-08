using UnityEngine;

public class ArrowBullet : MonoBehaviour
{

    private Rigidbody _rigidBody;

    [SerializeField] private float _arrowVelocity = 5;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _rigidBody.linearVelocity = transform.forward * _arrowVelocity;
    }

        void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.layer == 3 || collider.gameObject.layer == 6 || collider.gameObject.layer == 7)
        {
            gameObject.SetActive(false);
        }
    }
}
