using System.Collections;
using UnityEngine;

public class FireSlash : MonoBehaviour
{
    private Rigidbody _rigidBody;

    [SerializeField] private float _fireSlashVelocity = 15;

    void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        StartCoroutine(Slash());
    }

    void Update()
    {
        _rigidBody.linearVelocity = transform.forward * _fireSlashVelocity;
    }

    IEnumerator Slash()
    {
        yield return new WaitForSeconds(2);

        gameObject.SetActive(false);
    }
}
