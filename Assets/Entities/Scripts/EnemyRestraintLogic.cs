using System;
using System.Linq;
using UnityEngine;

public class EnemyRestraintLogic : MonoBehaviour,IRestrainable, IReseteable
{

    public event Action<bool> OnRestrainedChanged;
    public Transform Root => root;
    public bool IsRestrained { get; private set; }

    [SerializeField] Transform root;
    [SerializeField] PushForceSystem pushForceSystem;
    [SerializeField] EnemyBehaviour enemyBehaviour;
    [SerializeField] ProjectileDamage projectileDamage;

    private void Awake() => projectileDamage.OnCompleted += ResolveProjectileEnded;
    private void Start()
    {
         projectileDamage.Init(root.GetComponentsInChildren<Collider>().ToHashSet(), pushForceSystem.TargetRigidbody);
    }
    private void OnDestroy() => projectileDamage.OnCompleted -= ResolveProjectileEnded;
    void ResolveProjectileEnded(ProjectileDamage projectileDamage)
    {
        projectileDamage.gameObject.SetActive(false);
        enemyBehaviour.StateMachine.SetState(new DeadState(enemyBehaviour));
    }

    public bool TryRestrain()
    {
        if (IsRestrained) return false;

        enemyBehaviour.StateMachine.SetState(new PushState(enemyBehaviour));

        var rb = pushForceSystem.TargetRigidbody;
        if (!rb.isKinematic)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        rb.isKinematic = true;
        IsRestrained = true;
        OnRestrainedChanged?.Invoke(IsRestrained);
        return true;
    }
    public void Release()
    {
        if (!IsRestrained) return;

        pushForceSystem.TargetRigidbody.isKinematic = false;
        IsRestrained = false;
        OnRestrainedChanged?.Invoke(IsRestrained);
    }
    public void Launch(Vector3 force)
    {
        Release();

        gameObject.SetActive(false);
        pushForceSystem.ApplyPush(force);
        projectileDamage.gameObject.SetActive(true);
    }

    public void ResetMe()
    {
        projectileDamage.gameObject.SetActive(false);
        gameObject.SetActive(true);
        Release();
    }
}
