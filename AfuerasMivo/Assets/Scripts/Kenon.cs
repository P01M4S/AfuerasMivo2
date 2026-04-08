using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.VFX;

public class Kenon : MonoBehaviour
{


    [SerializeField] private VisualEffect[] _kenonEffectSlashR = new VisualEffect[3];
    [SerializeField] private VisualEffect[] _kenonEffectSlashL = new VisualEffect[3];

    void Awake()
    {

    }
    void Start()
    {
        
    }


    void Update()
    {

    }

    public void KanonAttackR()
    {
        foreach(VisualEffect vfx in _kenonEffectSlashR)
        {
            if(vfx != null)
            {
                vfx.Play();
            }
        }
    }
        public void KanonAttackL()
    {
        foreach(VisualEffect vfx in _kenonEffectSlashL)
        {
            if(vfx != null)
            {
                vfx.Play();
            }
        }
    }
}
