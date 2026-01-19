using UnityEngine;

/// <summary>
/// Este script recibe los eventos de ComboController y dispara los triggers en el Animator.
/// Además, define nuevos métodos que Unity invoca como Animation Events:
///   - PerformHit: para informar a ComboController que aplique daño.
///   - HitWindowEnd: para informar que ya no aplica más daño.
///   - AnimationEnded: para informar que terminó la animación de ataque.
/// </summary>
[RequireComponent(typeof(Animator))]
public class ComboAnimationExecutor : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Referencia al ComboController que gestiona la lógica de combo.")]
    [SerializeField] private ComboController comboController;

    private Animator animator;

    private void Awake()
    {
        // Si no se asignó desde el Inspector, lo buscamos en el mismo GameObject
        if (comboController == null)
        {
            if (!TryGetComponent(out comboController))
            {
                Debug.LogError("ComboAnimationExecutor: no se encontró ComboController en el GameObject.");
            }
        }

        if (!TryGetComponent(out animator))
        {
            Debug.LogError("ComboAnimationExecutor: no se encontró un Animator en el GameObject.");
        }
    }

    private void OnEnable()
    {
        // Nos suscribimos al evento que indica que comienza cada golpe de combo (1..maxComboCount)
        comboController.OnComboStarted += PlayComboAnimation;
    }

    private void OnDisable()
    {
        comboController.OnComboStarted -= PlayComboAnimation;
    }

    /// <summary>
    /// Se invoca cuando ComboController inicia un paso de combo.
    /// Reproduce la animación correspondiente mediante triggers.
    /// </summary>
    private void PlayComboAnimation(int step)
    {
        // Limpiamos triggers previos (evita solapamiento si hubo transiciones rápidas)
        for (int i = 1; i <= comboController.MaxComboCount; i++)
        {
            animator.ResetTrigger("Attack_" + i);
        }

        // Disparamos el trigger del paso actual: "Attack1", "Attack2", "Attack3", ...
        string triggerName = "Attack_" + step;
        animator.SetTrigger(triggerName);
    }

    #region Métodos para Animation Events

    /// <summary>
    /// Animation Event en el frame de impacto: indica aplicar daño.
    /// </summary>
    public void AnimationEvent_PerformHit()
    {
        comboController.PerformComboHit();
    }

    /// <summary>
    /// Animation Event en el frame donde ya NO se aplica más daño.
    /// Llama a ComboController.NotifyHitWindowEnded().
    /// Debe ubicarse justo después del frame de impacto en el clip.
    /// </summary>
    public void AnimationEvent_HitWindowEnded()
    {
        comboController.NotifyHitWindowEnded();
    }

    /// <summary>
    /// Animation Event al final del clip de animación de ataque.
    /// Indica que la animación terminó completamente.
    /// </summary>
    public void AnimationEvent_AnimationEnded()
    {
        comboController.OnComboAnimationEnded();
    }

    #endregion
}
