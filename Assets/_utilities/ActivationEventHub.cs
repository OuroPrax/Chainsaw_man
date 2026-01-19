using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActivationEventHub : MonoBehaviour
{
    [SerializeField] UnityEvent OnActivated;
    [SerializeField] UnityEvent OnDeactivated;

    void OnEnable() => OnActivated?.Invoke();

    void OnDisable() => OnDeactivated?.Invoke();
}
