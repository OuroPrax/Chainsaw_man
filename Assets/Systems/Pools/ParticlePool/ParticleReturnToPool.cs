using System;
using UnityEngine;

public class ParticleReturnToPool : MonoBehaviour
{
    private ParticleSystem ps;
    private Action returnToPool;

    public void Initialize(Action returnToPool)
    {
        this.returnToPool = returnToPool;
        ps = GetComponent<ParticleSystem>();
    }
    private void OnParticleSystemStopped() => returnToPool?.Invoke();
}
