using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementAnimationExecutor : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Script que expone MovementSpeedNormalized (float 0..1).")]
    [SerializeField] private PlayerMovementController movementController;
    [Tooltip("Índice de la capa en el Animator cuyo weight se actualizará.")]
    [SerializeField] private int targetLayerIndex = 1;
    [Tooltip("Si es true, el weight se invertirá: speed baja → weight sube.")]
    [SerializeField] private bool invertWeight = false;
    [SerializeField] PlayerCombatController combatController;
    [SerializeField] float layerWeightSmoth = 5f; // Ajustable: mayor valor = más rápido

    [Header("Player Handler")]
    [SerializeField] PlayerHandler playerHandler;
    bool _lockState;

    float currentLayerWeight = 0f; // Estado interno para suavizado
    private Animator animator;

    private void OnEnable()
    {
        playerHandler.LockState += LockState;
        playerHandler.OnThrow += OnThrow;
        playerHandler.OnGrabbed += OnGrabbed;
    }

    private void Awake()
    {
        // Obtener el Animator
        if (!TryGetComponent(out animator))
        {
            Debug.LogError("AnimatorLayerWeightController: no se encontró Animator en el GameObject.");
        }

        // Si no se asignó en el Inspector, intentamos buscarlo en el mismo GameObject
        if (movementController == null)
        {
            if (!TryGetComponent(out movementController))
            {
                Debug.LogError("AnimatorLayerWeightController: no se encontró PlayerMovementController en el GameObject.");
            }
        }

        // Validar que targetLayerIndex exista en el Animator
        if (targetLayerIndex < 0 || targetLayerIndex >= animator.layerCount)
        {
            Debug.LogError($"AnimatorLayerWeightController: el índice de capa {targetLayerIndex} no es válido. " +
                           $"Animator tiene {animator.layerCount} capas.");
        }
    }

    private void Update()
    {
        if (animator == null || movementController == null) return;

        animator.SetBool("Lock", _lockState);
        if (_lockState)
        {

            animator.SetLayerWeight(targetLayerIndex, 0);
            animator.SetFloat("Movement_Speed", 0);
            return;
        }

        // Obtener el valor targetweight (0..1)
        float targetWeight =  movementController.MovementSpeedNormalized;
        // Si invertimos el weight, restamos de 1
        targetWeight = invertWeight ? (1f - targetWeight) : targetWeight;
        // Hacemos una transición suave
        currentLayerWeight = Mathf.MoveTowards(currentLayerWeight, targetWeight, layerWeightSmoth * Time.deltaTime);
        // Asignar el weight suavizado a la capa objetivo
        animator.SetLayerWeight(targetLayerIndex, currentLayerWeight);

        // Actualizar parámetro de velocidad
        animator.SetFloat("Movement_Speed", movementController.CurrentSmoothedHorizontalSpeed);
    }

    void LockState(bool value)
    {
        _lockState = value;

        if (value) return;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    void OnThrow()
    {
        animator.SetTrigger("Falling_1");
    }

    void OnGrabbed() => animator.SetTrigger("Grabbed");
}
