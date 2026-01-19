using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using UnityEngine;

/// <summary>
/// Controlador del miniboss, posee una logica simple (enum y while para la FSM)
/// Utiliza <see cref="MiniBossAttackSystem"/> para manejar el ataque a realizar y controlar su flujo
/// </summary>
public class MiniBossController : MonoBehaviour, IHealth, IReseteable
{
    public static readonly int LeftArmHash = Animator.StringToHash("LeftArm");
    public static readonly int RightArmHash = Animator.StringToHash("RightArm");
    public static readonly int LeftLegHash = Animator.StringToHash("LeftLeg");
    public static readonly int RightLegHash = Animator.StringToHash("RightLeg");
    public static readonly int WalkHash = Animator.StringToHash("Walk");

    [field: SerializeField] public TargetEventChannelSO EnemyDeadEventChannel { get; private set; }
    [field: SerializeField] public Animator Animator { get; private set; }
    [field: SerializeField] public MiniBossData Data { get; private set; }

    public Transform Player { get; private set; }
    public Transform Devil { get; private set; }
    public EnemyStateMachine FSM { get; private set; }

    enum State { FollowDevil, ChasePlayer, Grab, Attack, Idle, Dead }


    #region Unity lifecycle
    private void Awake()
    {
        SetupLimbs();
        FSM = new();
    }
    private void Start()
    {
        Devil = FindAnyObjectByType<BossController>().transform;
        Player = FindAnyObjectByType<PlayerMovementController>(FindObjectsInactive.Include).transform;
        ResetMe();
    }
    private void OnEnable() => SubscribeLimbs();
    private void OnDisable() => UnSubscribeLimbs();
    void Update() => FSM.Update();
    #endregion

    public void ResetMe()
    {
        Animator.SetBool(LeftArmHash, true);
        Animator.SetBool(RightArmHash, true);

        Animator.SetBool(LeftLegHash, true);
        Animator.SetBool(RightLegHash, true);

        ResetLimbs();
        ResetHealth();
        speedMultiplier = 1f;
        FSM.SetState(new MiniBossFollowDevilState(this));
    }

    #region Limb
    public bool HasActivePart(LimbData.Part part) => limbs.TryGetValue(part, out var limb) && limb.IsAlive;
    Dictionary<LimbData.Part, Limb> limbs = new();
    void SetupLimbs()
    {
        limbs = GetComponentsInChildren<Limb>(true).ToDictionary(l => l.Part, l => l);
        foreach (var limbData in Data.limbsDatas)
        {
            if (limbs.TryGetValue(limbData.part, out var limb))
                limb.Init(limbData);
        }
    }

    void SubscribeLimbs()
    {
        foreach (var limb in limbs)
        {
            limb.Value.OnHealthChanged += ResolveLimbHealthChanged;
            limb.Value.OnDead += HandleLimbLoss;
        }
    }
    void UnSubscribeLimbs()
    {
        foreach (var limb in limbs)
        {
            limb.Value.OnHealthChanged -= ResolveLimbHealthChanged;
            limb.Value.OnDead -= HandleLimbLoss;
        }
    }
    void ResolveLimbHealthChanged(Limb limb, float previous, float current) => ChangeHealth(current - previous);
    void HandleLimbLoss(Limb limb)
    {
        switch (limb.Part)
        {
            case LimbData.Part.Head:
                SetDead();
                break;

            case LimbData.Part.LeftArm:
                Animator.SetBool(LeftArmHash, false);
                attackSystem.TryCancelAttack();
                break;
            case LimbData.Part.RightArm:
                Animator.SetBool(RightArmHash, false);
                attackSystem.TryCancelAttack();
                break;

            case LimbData.Part.LeftLeg:
                ReduceSpeedMultiplier(.5f);
                Animator.SetBool(LeftLegHash, false);
                attackSystem.TryCancelAttack();
                break;
            case LimbData.Part.RightLeg:
                ReduceSpeedMultiplier(.5f);
                Animator.SetBool(RightLegHash, false);
                attackSystem.TryCancelAttack();
                break;
        }
    }

    void ResetLimbs()
    {
        foreach (var limb in limbs)
            limb.Value.ResetLimb();
    }    
    #endregion


    #region Health
    public event Action OnDeath;
    public event Action<float, float> OnHealthChanged;
    bool IsDead => FSM.CurrentState is MiniBossDeadState;
    public float MaxHealth => Data.health.maxHealth;
    public float CurrentHealth => currentHealth;
    public float HealthRate => currentHealth/MaxHealth;
    float currentHealth;
    public void ChangeHealth(float amount)
    {
        if (IsDead || amount == 0f) return;

        float previous = currentHealth;
        currentHealth = Mathf.Clamp(currentHealth + amount, 0f, MaxHealth);
        OnHealthChanged?.Invoke(previous, currentHealth);
    }
    public void ResetHealth() => currentHealth = MaxHealth;
    void SetDead()
    {
        if(IsDead) return;

        FSM.SetState(new MiniBossDeadState(this));
    }
    #endregion


    #region Detection
    [field: SerializeField] public float DetectionRadius { get; private set; }
    float DistanceToPlayer() => Vector3.Distance(transform.position, Player.position);
    #endregion


    #region Attack
    [Header("Attack")]
    [SerializeField] MiniBossAttackSystem attackSystem;
    [SerializeField] float attackCD;
    [SerializeField] float attackRange;
    float nextAttackTime;
    public bool CanAttack => attackSystem.CurrentState == MiniBossAttackSystem.State.Ready && nextAttackTime <= Time.time;
    void PerformAttack()
    {
        attackSystem.Attack();
    }
    #endregion


    #region Movement
    [Header("Movement")]
    [SerializeField] float followSpeed;
    [SerializeField] float chaseSpeed;
    [SerializeField] float walkHeight = 0f; 
    [SerializeField] float rotationSpeed = 360f; // grados por segundo
    [SerializeField] float angleThreshold = 5f;  // tolerancia para empezar a moverse
    public void SetFollow() => currentSpeed = followSpeed;
    public void SetChase() => currentSpeed = chaseSpeed;

    float currentSpeed;
    float speedMultiplier = 1f;
    void MoveTowards(Vector3 target)
    {
        Vector3 flatDir = target - transform.position;
        flatDir.y = 0f;

        Quaternion targetRot = Quaternion.LookRotation(flatDir.normalized);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRot,
            rotationSpeed * Time.deltaTime
        );

        float angle = Quaternion.Angle(transform.rotation, targetRot);
        if (angle < angleThreshold)
        {
            // Solo avanza si está casi alineado
            Vector3 moveDir = flatDir.normalized;
            moveDir.y = walkHeight;
            transform.position += speedMultiplier * currentSpeed * Time.deltaTime * moveDir;
        }
    }
    void ReduceSpeedMultiplier(float amount) => speedMultiplier = Mathf.Max(speedMultiplier - amount, 0f);
    #endregion


    #region states
    public class MiniBossFollowDevilState : IEnemyState
    {
        readonly MiniBossController controller;

        public MiniBossFollowDevilState(MiniBossController ctrl) => controller = ctrl;

        public void Enter()
        {
            controller.SetFollow();
            controller.Animator.SetBool(WalkHash, true);
        }

        public void Update()
        {
            controller.MoveTowards(controller.Devil.position);

            if (controller.DistanceToPlayer() <= controller.DetectionRadius)
                controller.FSM.SetState(new MiniBossChaseState(controller));
        }

        public void Exit()
        {
            controller.Animator.SetBool(WalkHash, false);
        }
    }
    public class MiniBossChaseState : IEnemyState
    {
        readonly MiniBossController controller;

        public MiniBossChaseState(MiniBossController ctrl) => controller = ctrl;

        public void Enter()
        {
            controller.SetChase();
            controller.Animator.SetBool(WalkHash, true);
        }

        public void Update()
        {

            controller.MoveTowards(controller.Player.position);


            if (controller.DistanceToPlayer() <= controller.attackRange)
            {
                Vector3 toPlayer = controller.Player.position - controller.transform.position;
                toPlayer.y = 0f;
                float angleToPlayer = Vector3.Angle(controller.transform.forward, toPlayer.normalized);

                if (angleToPlayer <= controller.angleThreshold && controller.CanAttack)
                    controller.FSM.SetState(new MiniBossAttackState(controller));
            }      
        }

        public void Exit()
        {
            controller.Animator.SetBool(WalkHash, false);
        }
    }
    public class MiniBossAttackState: IEnemyState
    {
        readonly MiniBossController controller;

        public MiniBossAttackState(MiniBossController ctrl) => controller = ctrl;

        public void Enter()
        {
            controller.PerformAttack();
        }

        public void Update()
        {
            if (controller.attackSystem.CurrentState != MiniBossAttackSystem.State.Executing)
                controller.FSM.SetState(new MiniBossFollowDevilState(controller));
        }

        public void Exit()
        {
            controller.attackSystem.TryCancelAttack();
            controller.nextAttackTime = Time.time + controller.attackCD;
        }
    }
    public class MiniBossDeadState : IEnemyState
    {
        readonly MiniBossController controller;

        public MiniBossDeadState(MiniBossController ctrl) => controller = ctrl;

        public void Enter()
        {
            controller.EnemyDeadEventChannel.RaiseEvent(controller.transform);
        }

        public void Update()
        {
        }

        public void Exit()
        {
        }
    }
    #endregion
}