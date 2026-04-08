using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{

    public int sceneCharging;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.CompareTag("Player"))
        {
            SceneCharge(sceneCharging);
        }
    }

    void SceneCharge(int numberScene)
    {
        SceneManager.LoadScene(numberScene);
    }
}
