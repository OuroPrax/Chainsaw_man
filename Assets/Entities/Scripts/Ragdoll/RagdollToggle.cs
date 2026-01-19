using System;
using System.Linq;
using UnityEngine;

/// <summary>
/// logica para activar o desactivar ragdoll
/// </summary>
[DefaultExecutionOrder(-10)]
public class RagdollToggle : MonoBehaviour
{
    public event Action<RagdollToggle, bool> OnChangeState;
    public GameObject[] GetBodyParts => bodyParts;
    public bool active = false;
    [SerializeField] GameObject[] bodyParts;
    Rigidbody[] rigidbodies;

    private void Awake()
    {
        rigidbodies = bodyParts.Select(p=>p.GetComponent<Rigidbody>()).ToArray();
    }
    void Start()
    {
        if (active) ActiveRagdoll();
        else DesactiveRagdoll();
    }

    public void ActiveRagdoll()
    {
        foreach (var rb in rigidbodies)
        {
            rb.isKinematic = false;
        }
        if (active) return;
        active = true;
        OnChangeState?.Invoke(this, active);
    }

    public void DesactiveRagdoll()
    {
        foreach (var rb in rigidbodies)
        {
            rb.isKinematic = true;
        }
        if(!active) return;
        active = false;
        OnChangeState?.Invoke(this, active);
    }
}
