using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellCasing : MonoBehaviour
{
    [SerializeField] ParticleSystem shellCasingSystem;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            EjectShell();
        }
    }

    public void EjectShell()
    {
        shellCasingSystem.Play();
    }
}
