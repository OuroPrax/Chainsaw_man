using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clase simple para hacer daño al entrar en contacto con un trigger que implementa <see cref="IDamageable"/>
/// Posee un metodo <see cref="Init(HashSet{Collider}))"/> para agregar los colliders propios y poder ignorarlos
/// </summary>
public class DamageCollider : MonoBehaviour
{
    public float DamageValue;
    HashSet<Collider> ownColliders = new();

    public void Init(HashSet<Collider> ownColliders)
    {
        this.ownColliders = ownColliders;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IDamageable>(out var receiver) && !ownColliders.Contains(other))
        {
            receiver.TakeDamage(DamageValue, other.ClosestPoint(transform.position));
        }
    }
}