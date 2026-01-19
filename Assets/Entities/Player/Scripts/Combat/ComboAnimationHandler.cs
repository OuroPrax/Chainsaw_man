using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Este script se encarga de recibir los eventos emitidos por el ComboController
/// y disparar las animaciones correspondientes. Además, provee métodos públicos
/// que serán invocados mediante Animation Events para notificar al ComboController
/// cuándo aplicar daño y cuándo terminó la animación de cada golpe.
/// </summary>
[RequireComponent(typeof(Animator))]
public class ComboAnimationHandler : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Referencia al ComboController que emite los eventos de combo.")]
    [SerializeField] private ComboController comboController;

    private Animator animator;

    private void Awake()
    {
        // Si no se arrastra desde el Inspector, intentamos obtenerlo del mismo GameObject
        if (comboController == null)
        {
            if (!TryGetComponent<ComboController>(out comboController))
            {
                Debug.LogError("ComboAnimationHandler: No se encontró ComboController en el mismo GameObject.");
            }
        }

        if (!TryGetComponent<Animator>(out animator))
        {
            Debug.LogError("ComboAnimationHandler: No se encontró un Animator en el GameObject.");
        }
    }

    private void OnEnable()
    {
        // Nos suscribimos al evento que notifica el inicio de cada paso de combo
        comboController.OnComboStarted += HandleComboStarted;
    }

    private void OnDisable()
    {
        // Desuscribimos para evitar referencias pendientes
        comboController.OnComboStarted -= HandleComboStarted;
    }

    /// <summary>
    /// Se invoca cada vez que ComboController inicia un nuevo paso de combo (1..maxComboCount).
    /// Aquí reproducimos la animación correspondiente.
    /// </summary>
    /// <param name="step">Número de paso de combo (1, 2, 3, ...)</param>
    private void HandleComboStarted(int step)
    {
        // Asumimos que los clips de animación se nombraron "Combo1", "Combo2", "Combo3", etc.
        // Ajusta este nombre según cómo hayas llamado tus Animation Clips.
        string clipName = "Combo" + step;
        animator.Play(clipName);
    }

    #region Métodos para Animation Events

    /// <summary>
    /// Este método debe agregarse como Animation Event en el frame exacto donde
    /// deba aplicarse daño (frame de impacto). Invoca PerformComboHit() en el ComboController.
    /// </summary>
    public void AnimationEvent_PerformHit() => comboController.PerformComboHit();

    /// <summary>
    /// Este método debe agregarse como Animation Event al final de cada animación de combo.
    /// Su propósito es notificar al ComboController que esta animación de golpe terminó.
    /// </summary>
    public void AnimationEvent_CombatAnimEnded() => comboController.OnComboAnimationEnded();

    #endregion
}
