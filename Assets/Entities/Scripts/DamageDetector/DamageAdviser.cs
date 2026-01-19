using System;
using UnityEngine;

/// <summary>
/// Clase que se ocupa de avisar de un daño recibido, es utilizada para que una parte central reciba el dano avisado.
/// Implementa la interfaz <see cref="IDamageable"/> y posee un evento <see cref="OnDamageDetected"/> para avisar del dano a recibir.
/// </summary>
public class DamageAdviser : MonoBehaviour, IDamageable
{
    public event Action<float, Vector3, DamageAdviser> OnDamageDetected;
    public void TakeDamage(float amount, Vector3 position) =>  OnDamageDetected?.Invoke(amount, position, this);
}