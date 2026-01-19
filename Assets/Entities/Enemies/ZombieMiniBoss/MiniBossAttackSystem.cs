using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public interface IAttackSystem
{
    void Attack();
    void TryCancelAttack();
}
/// <summary>
/// Sistema de ataque del miniboss, en base a si el enemigo posee las partes podra atacar o agarrar y lanzar al jugador
/// </summary>
public class MiniBossAttackSystem : MonoBehaviour, IAttackSystem
{
    public enum State { Ready, Executing, Cooldown, Cancelling }
    public State CurrentState => currentAction != null ? currentAction.CurrentState : State.Ready;

    [SerializeField] MiniBossController controller;
    [SerializeField] Transform miniBossTransform;
    [Header("Values")]
    [SerializeField] float throwForce;
    [SerializeField] float kickDamage;
    [Header("Detectors")]
    [SerializeField] EnterTriggerDetector grabDetector;
    [SerializeField] EnterTriggerDetector kickDetector;
    [SerializeField] Transform grabHand;
    [SerializeField] Vector3 grabOffsetPosition;
    Transform grabTarget;
    readonly List<IAttackAction> attackActions = new();
    IAttackAction currentAction;

    #region unity lifecyle
    private void Awake()
    {
        if(!miniBossTransform)
            miniBossTransform = transform.parent ? transform.parent : transform;

        var grabAction = new GrabAttackAction(controller, grabDetector, grabHand, grabOffsetPosition);
        var kickAction = new HitAttackAction(controller, kickDetector, kickDamage);
        attackActions.Add(grabAction);
        attackActions.Add(kickAction);
    }
    #endregion

    public void Attack()
    {
        var valids = attackActions.Where(aa => aa.CanExecute());
        var randomValids = valids.OrderBy(_ => UnityEngine.Random.value);

        if(randomValids.Any())
        {
            currentAction = randomValids.First();
            currentAction.Execute();
        }
    }
    public void TryCancelAttack() => currentAction?.TryCancel();


    #region Attack actions
    public interface IAttackAction
    {
        event Action<IAttackAction> OnStateChanged;
        State CurrentState { get; }
        bool CanExecute();
        void Execute();
        void TryCancel();
    }

    public class HitAttackAction : IAttackAction
    {
        private static readonly int KickHash = Animator.StringToHash("Kick");
        private static readonly int CrawlAttackHash = Animator.StringToHash("CrawlAttack");
        public event Action<IAttackAction> OnStateChanged;
        public State CurrentState { get; private set; } = State.Ready;

        readonly MiniBossController controller;
        readonly Animator animator;
        readonly EnterTriggerDetector kickDetector;
        readonly float damage;

        public HitAttackAction(MiniBossController controller, EnterTriggerDetector kickDetector, float damage)
        {
            this.controller = controller;
            this.animator = controller.Animator;
            this.kickDetector = kickDetector;
            this.damage = damage;
        }

        public bool CanExecute() => CurrentState == State.Ready && controller.HasActivePart(LimbData.Part.RightArm) && controller.HasActivePart(LimbData.Part.LeftArm);

        public void Execute()
        {
            if (!CanExecute()) return;

            CurrentState = State.Executing;
            OnStateChanged?.Invoke(this);

            controller.StartCoroutine(Hit());
        }
        IEnumerator Hit()
        {
            var hitHash
                = controller.HasActivePart(LimbData.Part.RightLeg) && controller.HasActivePart(LimbData.Part.LeftLeg)
                ? KickHash
                : CrawlAttackHash;
            animator.SetTrigger(hitHash);
            kickDetector.OnEnter += OnKickHit;
            yield return new WaitForSeconds(1f);
            Complete();
        }
        void Complete()
        {
            kickDetector.OnEnter -= OnKickHit;
            CurrentState = State.Ready;
            OnStateChanged?.Invoke(this);
        }

        public void TryCancel() { }

        void OnKickHit(Transform target)
        {
            if (target.TryGetComponent<IDamageable>(out var dmg))
                dmg.TakeDamage(damage, target.position);
        }
    }

    public class GrabAttackAction : IAttackAction
    {
        private static readonly int ThrowHash = Animator.StringToHash("Throw");
        private static readonly int GrabHash = Animator.StringToHash("Grab");
        private static readonly int HasGrabItHash = Animator.StringToHash("HasGrabIt");
        public event Action<IAttackAction> OnStateChanged;
        public State CurrentState { get; private set; } = State.Ready;

        readonly MiniBossController controller;
        readonly Animator animator;
        readonly EnterTriggerDetector grabDetector;
        readonly Transform grabHand;
        readonly Vector3 grabOffset;
        readonly float throwForce;
        Transform grabTarget;
        Coroutine grabCor;

        public GrabAttackAction(MiniBossController controller, EnterTriggerDetector grabDetector, Transform grabHand, Vector3 grabOffset)
        {
            this.controller = controller;
            this.animator = controller.Animator;
            this.grabDetector = grabDetector;
            this.grabHand = grabHand;
            this.grabOffset = grabOffset;
        }

        public bool CanExecute() => CurrentState == State.Ready && (controller.HasActivePart(LimbData.Part.RightLeg) || controller.HasActivePart(LimbData.Part.LeftLeg));

        public void Execute()
        {
            if (!CanExecute()) return;

            CurrentState = State.Executing;
            OnStateChanged?.Invoke(this);

            grabCor = controller.StartCoroutine(GrabCor());
        }
        IEnumerator GrabCor()
        {
            animator.SetTrigger(GrabHash);

            grabDetector.OnEnter += ResolveGrab;
            yield return new WaitForSeconds(3f);
            Complete();
        }
        void ResolveGrab(Transform target)
        {
            if (CurrentState != State.Executing) return;

            if (!target.TryGetComponent<PlayerHandler>(out var playerHandler)) return;

            //Si encontramos un target
            this.grabTarget = target;
            playerHandler.LockPlayer();
            playerHandler.OnGrabbedPlayer();

            target.GetComponent<PushForceSystem>().TargetRigidbody.isKinematic = true;
            if (target.TryGetComponent(out BoneLocator boneLocator))
                grabHand.position = boneLocator.Neck.position;

            target.SetParent(grabHand);
            target.SetLocalPositionAndRotation(grabOffset, Quaternion.identity);
            grabHand.localPosition = Vector3.zero;

            if(grabCor != null)
                controller.StopCoroutine(grabCor);

            grabCor = controller.StartCoroutine(RotateThenThrow());
        }
        IEnumerator RotateThenThrow()
        {
            controller.Animator.SetTrigger(HasGrabItHash);

            float rotateDuration = 0.5f;

            bool hasDevil = controller.Devil;
            Vector3 desiredDir = hasDevil ? (controller.transform.position - controller.Devil.transform.position) : controller.transform.forward;
            Quaternion startRot = controller.transform.rotation;
            Quaternion endRot = Quaternion.LookRotation(desiredDir);

            float elapsed = 0f;
            while (elapsed < rotateDuration)
            {
                controller.transform.rotation = Quaternion.Slerp(startRot, endRot, elapsed / rotateDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            controller.transform.rotation = endRot;

            grabCor = controller.StartCoroutine(OnThrowRelease());
        }
        IEnumerator OnThrowRelease()
        {
            controller.Animator.SetTrigger(ThrowHash);
            yield return new WaitForSeconds(1.1f);

            grabTarget.TryGetComponent<PlayerHandler>(out var playerHandler);
            playerHandler.OnThrowPlayer();

            grabTarget.SetParent(null);

            var pushSystem = grabTarget.GetComponent<PushForceSystem>();
            pushSystem.TargetRigidbody.isKinematic = false;
            pushSystem.TargetRigidbody.velocity = Vector3.zero;
            Vector3 dir = controller.transform.forward + controller.transform.up;
            dir.Normalize();
            pushSystem.ApplyPush(dir * throwForce);

            controller.StartCoroutine(EnablePlayerAfterDelay(grabTarget, 1f));
            grabTarget = null;

            Complete();
        }
        IEnumerator EnablePlayerAfterDelay(Transform targetToEnable, float delay)
        {
            yield return new WaitForSeconds(delay);
            targetToEnable.GetComponent<PlayerHandler>().UnlockPlayer();
        }

        public void TryCancel()
        {
            if (grabTarget != null)
            {
                grabTarget.GetComponent<PlayerHandler>()?.UnlockPlayer();
                grabTarget.SetParent(null);
            }

            CurrentState = State.Cancelling;
            OnStateChanged?.Invoke(this);

            Complete();
        }
        void Complete()
        {
            grabDetector.OnEnter -= ResolveGrab;
            grabTarget = null;
            CurrentState = State.Ready;
            OnStateChanged?.Invoke(this);
        }
    }
    #endregion
}



