using System;
using System.Collections;
using UnityEngine;


/// <summary>
/// Logica de enemigo utilizada por los zombies, implementa una FSM <see cref="EnemyStateMachine"/>
/// </summary>
public class EnemyBehaviour : MonoBehaviour, IReseteable
{
    [SerializeField] SourceXTargetEventChannelSO playerKilledEnemyEventChannel;
    public TargetEventChannelSO EnemyDeadEventChannel;
    [Header("Sistemas")]
    [SerializeField] HealthHandler healthHandler;
    [SerializeField] RagdollToggle ragdollToggle;
    [SerializeField] EnemyAttackHandler attackHandler;
    [SerializeField] EnemyMovementHandler movementHandler;
    [SerializeField] DamageDetector damageDetector;
    [SerializeField] Animator animator;
    [Header("Detección")]
    [SerializeField] float detectionRadius = 10f;
    [SerializeField] LayerMask playerLayer;
    bool isDead;
    public EnemyStateMachine StateMachine => fsm;
    public bool CanSeePlayer { get; private set; }

    readonly EnemyStateMachine fsm = new();
    Transform playerTransform;

    #region unity lifecycle
    private void OnEnable()
    {
        healthHandler.OnHealthChanged += ResolveHealthChanged;
        ragdollToggle.DesactiveRagdoll();
        fsm.SetState(new IdleState(this));
        StartCoroutine(DetectionLoop());
    }
    private void OnDisable()
    {
        StopCoroutine(DetectionLoop());
        healthHandler.OnHealthChanged -= ResolveHealthChanged;
    }

    void ResolveHealthChanged(float previous, float current)
    {
        if(isDead || current > 0) return;

        fsm.SetState(new DeadState(this));
        isDead = true;
        playerKilledEnemyEventChannel.RaiseEvent(playerTransform, transform);
    }

    private void Update() => fsm.Update();
    IEnumerator DetectionLoop()
    {
        var wait = new WaitForSeconds(0.3f);

        while (true)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);
            if (hits.Length > 0)
            {
                playerTransform = hits[0].transform;
                CanSeePlayer = true;
            }
            else
            {
                playerTransform = null;
                CanSeePlayer = false;
            }

            yield return wait;
        }
    }
    #endregion

    #region movement
    public void StopMoving()
    {
        animator.SetBool("Walk", false);
        movementHandler.Stop();
    }
    public void MoveTowardsPlayer()
    {
        if (!playerTransform) return;
        animator.SetBool("Walk", true);
        movementHandler.MoveTo(playerTransform.position);
    }
    public void MoveToPosition(Vector3 position)
    {
        movementHandler.MoveTo(position);
    }
    #endregion

    #region attack
    public bool IsPlayerInAttackRange() => playerTransform != null && attackHandler.CanAttack(playerTransform);
    public void TryAttackPlayer()
    {
        animator.SetTrigger("Attack");
        attackHandler.TryAttack(playerTransform);
    }
    public bool IsAttacking => attackHandler.IsAttacking;
    public void TryCancelAttack() => attackHandler.TryCancelAttack();
    #endregion

    #region Health
    public void ResetMe()
    {
        healthHandler.ResetHealth();
        fsm.SetState(new IdleState(this));
        damageDetector.ResetParts();
        isDead = false;
        var me = this.GetComponent<IReseteable>();
        foreach (var item in GetComponentsInChildren<IReseteable>())
        {
            if(item == me) continue;
            item.ResetMe();
        }
    }
    public void ActiveRagdoll() 
    {
        animator.enabled = false;
        ragdollToggle.ActiveRagdoll();
    } 
    public void DesactiveRagdoll()
    {
        animator.enabled = true;
        ragdollToggle.DesactiveRagdoll();
    }
    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
