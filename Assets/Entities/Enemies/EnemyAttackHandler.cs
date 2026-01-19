using System.Collections;
using UnityEngine;

public class EnemyAttackHandler : MonoBehaviour
{
    [SerializeField] Transform attackTransform;
    [SerializeField] float attackCooldown = 1.5f;
    [SerializeField] float attackRange = 2f;
    [SerializeField] int damage = 10;
    Coroutine attackCoroutine;

    public bool IsAttacking => attackCoroutine != null;
    public bool CanAttack(Transform target) => !IsAttacking && Vector3.Distance(attackTransform.position, target.position) <= attackRange;
    public void TryAttack(Transform target)
    {
        if (!CanAttack(target)) return;
        
        attackCoroutine = StartCoroutine(Attack(target));
    }
    IEnumerator Attack(Transform target)
    {
        Debug.Log("Empezo ataque");
        yield return new WaitForSeconds(attackCooldown * .5f);
        if (Vector3.Distance(attackTransform.position, target.position) <= attackRange && target.TryGetComponent(out IDamageable damageable))
            damageable.TakeDamage(damage, attackTransform.position);
        yield return new WaitForSeconds(attackCooldown * .5f);
        attackCoroutine = null;
        Debug.Log("Termino ataque");
    }
    public void TryCancelAttack()
    {
       if(!IsAttacking) return;

        StopCoroutine(attackCoroutine);
        attackCoroutine = null;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackTransform.position, attackRange);
    }
}
