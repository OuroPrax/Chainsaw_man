using System;
using UnityEngine;

public class DamageDetectorInBodyPart : MonoBehaviour, IDamageable
{
    public string tagDetector;
    public event Action<DamageDetectorInBodyPart, float, Vector3> DetectDamange;

    public void TakeDamage(float amount, Vector3 position)
    {
        DetectDamange.Invoke(this, amount, position);
    }
}
