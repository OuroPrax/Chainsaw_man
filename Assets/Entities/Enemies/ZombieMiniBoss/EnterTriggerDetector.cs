using System;
using UnityEngine;

public class EnterTriggerDetector: MonoBehaviour
{
    public event Action<Transform> OnEnter;
    private void OnTriggerEnter(Collider other)
    {
        OnEnter?.Invoke(other.transform);
    }
}
