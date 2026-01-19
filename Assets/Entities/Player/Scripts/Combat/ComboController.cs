using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Controla la secuencia de combos de ataque. 
/// Este script es completamente autónomo y NO DEPENDE de otros scripts.
/// Expone eventos para notificar:
///   - cuándo inicia cada golpe (OnComboStarted)
///   - cuándo aplicar daño (OnComboHit)
///   - cuándo finaliza la ventana de daño (OnComboHitWindowEnded)
///   - cuándo termina la animación de cada golpe (OnComboEnded)
/// </summary>
public class ComboController : MonoBehaviour
{
    #region Configuración de combo

    [Header("Configuración de Combo")]
    [Tooltip("Tiempo (en segundos) que el jugador tiene para encadenar el siguiente golpe.")]
    [SerializeField] float comboInputWindow = 0.8f;

    [Tooltip("Cantidad máxima de golpes en la secuencia de combo.")]
    [SerializeField] int maxComboCount = 3;

    [Tooltip("Collider que se van a activar a la hora de hacer daño")]
    [SerializeField] GameObject[] damangeColliders;

    #endregion

    #region Estado interno

    // Si está en medio de un ataque (animación en curso)
    public bool IsAttackInProgress { get; private set; }

    // Si hay un input encolado para el siguiente golpe
    private bool hasQueuedInput = false;

    // Paso actual del combo (1..maxComboCount). 0 significa nadie golpeando.
    private int currentComboStep = 0;

    // Rutina que espera el tiempo para resetear el combo
    private Coroutine comboResetRoutine = null;

    #endregion

    #region Eventos públicos

    /// <summary>
    /// Se dispara cuando comienza un nuevo golpe de combo.
    /// Parámetro: número del paso de combo (1..maxComboCount).
    /// </summary>
    public event Action<int> OnComboStarted;

    /// <summary>
    /// Se dispara justo en el frame de impacto para aplicar daño.
    /// Parámetro: número del paso de combo (1..maxComboCount).
    /// </summary>
    public event Action<int> OnComboHit;

    /// <summary>
    /// Se dispara cuando termina la ventana de daño (ya no se aplica más daño).
    /// Parámetro: número del paso de combo (1..maxComboCount).
    /// </summary>
    public event Action<int> OnComboHitWindowEnded;

    /// <summary>
    /// Se dispara cuando la animación del golpe completo terminó (para avanzar combo o resetear).
    /// </summary>
    public event Action OnComboEnded;

    #endregion

    #region Propiedades públicas

    /// <summary>
    /// Permite que otros scripts sepan cuántos golpes de combo hay configurados.
    /// </summary>
    public int MaxComboCount => maxComboCount;

    #endregion

    private void Awake()
    {
        var allColliders = GetComponentsInChildren<Collider>().ToHashSet();
        foreach (var item in damangeColliders)
        {
            item.GetComponent<DamageCollider>().Init(allColliders);
        }
    }
    private void OnDisable()
    {
        CleanAttacks();
    }

    #region Métodos públicos

    /// <summary>
    /// Debe llamarse desde el script de input de ataque.
    /// Si no hay ataque en curso, inicia el combo; si ya hay, encola el input.
    /// </summary>
    public void RegisterAttackInput()
    {
        if (IsAttackInProgress)
        {
            // Ya se está atacando: encolamos la petición para el siguiente golpe
            hasQueuedInput = true;
            return;
        }

        // No hay animación de ataque en curso: empezamos (o continuamos) el combo
        StartNextComboStep();
    }

    /// <summary>
    /// Este método debe llamarse desde un Animation Event (en el frame de impacto).
    /// Dispara OnComboHit(step).
    /// </summary>
    public void PerformComboHit()
    {
        if (!IsAttackInProgress)
            return;
        EnableDamangeColliders();
        OnComboHit?.Invoke(currentComboStep);
    }

    /// <summary>
    /// Este método debe llamarse desde un Animation Event en el frame donde YA NO se aplica daño
    /// (fin de la ventana de impacto). Dispara OnComboHitWindowEnded(step).
    /// </summary>
    public void NotifyHitWindowEnded()
    {
        if (!IsAttackInProgress)
            return;

        DisableDamangeColliders();
        OnComboHitWindowEnded?.Invoke(currentComboStep);
    }

    /// <summary>
    /// Este método debe llamarse desde un Animation Event al final de la animación de ataque.
    /// Decide si encadena el siguiente golpe o inicia el reset de combo.
    /// </summary>
    public void OnComboAnimationEnded()
    {
        OnComboEnded?.Invoke();

        if (hasQueuedInput)
        {
            // Si hubo input encolado, continua al siguiente paso
            hasQueuedInput = false;
            StartNextComboStep();
        }
        else
        {
            // Si no, abrimos la ventana para resetear el combo
            IsAttackInProgress = false;
            comboResetRoutine = StartCoroutine(ComboResetCoroutine());
        }
    }

    public void CleanAttacks()
    {
        ResetCombos();
        DisableDamangeColliders();
    }
    #endregion

    #region Lógica interna de combo

    /// <summary>
    /// Arranca o avanza al siguiente paso de combo.
    /// Cancela cualquier rutina de reseteo en curso y dispara OnComboStarted.
    /// </summary>
    private void StartNextComboStep()
    {
        // Si había una corrutina pendiente para resetear, la detenemos
        if (comboResetRoutine != null)
        {
            StopCoroutine(comboResetRoutine);
            comboResetRoutine = null;
        }

        // Avanzar índice de combo (1..maxComboCount)
        currentComboStep++;
        if (currentComboStep > maxComboCount)
            currentComboStep = 1;

        // Indicamos que ya estamos en un ataque
        IsAttackInProgress = true;
        hasQueuedInput = false;

        // Disparamos evento para que se reproduzca la animación del paso actual
        OnComboStarted?.Invoke(currentComboStep);
    }

    /// <summary>
    /// Corrutina que espera comboInputWindow segundos para resetear el combo si no hay nuevos inputs.
    /// </summary>
    private IEnumerator ComboResetCoroutine()
    {
        yield return new WaitForSeconds(comboInputWindow);
        ResetCombos();
    }

    public void ResetCombos()
    {
        // Reiniciar todo el estado
        currentComboStep = 0;
        hasQueuedInput = false;
        IsAttackInProgress = false;
        comboResetRoutine = null;
    }

    #endregion

    #region Metodos para colliders
    void EnableDamangeColliders()
    {
        if (damangeColliders.Length <= 0) return;

        foreach(var collider in damangeColliders)
        {
            collider.SetActive(true);
        }
    }
    public void DisableDamangeColliders()
    {
        if (damangeColliders.Length <= 0) return;

        foreach (var collider in damangeColliders)
        {
            collider.SetActive(false);
        }
    }
    #endregion

}
