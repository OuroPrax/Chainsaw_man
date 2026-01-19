using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDamage : MonoBehaviour
{
    public event Action<ProjectileDamage> OnCompleted;

    [SerializeField] float damageAmount = 10f;
    HashSet<Collider> ownerColliders;
    Rigidbody rb;
    public void Init(HashSet<Collider> ownerColliders, Rigidbody rb)
    {
        this.ownerColliders = ownerColliders;
        this.rb = rb;
    }
    private void OnEnable() => StartCoroutine(CheckCompleted());
    private void OnDisable() => StopAllCoroutines();
    IEnumerator CheckCompleted()
    {
        var wait = new WaitForSeconds(.1f);

        while (true)
        {
            yield return wait;
            if (rb.velocity.sqrMagnitude < 10f)
            {
                OnCompleted?.Invoke(this);
                break;  
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (ownerColliders.Contains(other)) return;
        if (!other.TryGetComponent(out IDamageable damageable)) return;

        damageable.TakeDamage(damageAmount, other.ClosestPoint(transform.position));
    }
}
