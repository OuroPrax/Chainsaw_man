using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using static UnityEngine.Rendering.DebugUI;

public class ColliderDetector : MonoBehaviour
{
    event Action OnEnter;
    event Action OnExit;

    private void OnEnable()
    {
        
    }

    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnEnter?.Invoke();        
    }

    private void OnCollisionExit(Collision collision)
    {
        OnExit?.Invoke();
    }

}
