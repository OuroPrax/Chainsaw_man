using System;
using UnityEngine;

/// <summary>
/// Centraliza bloquear al personaje
/// </summary>
public class PlayerHandler : MonoBehaviour
{
    [SerializeField] PlayerCombatController playerCombatController;
    [SerializeField] PlayerMovementController playerMovementController;
    public event Action<bool> LockState;
    public event Action OnThrow;
    public event Action OnGrabbed;

    public void LockPlayer()
    {
        playerCombatController.enabled = false;
        playerMovementController.enabled = false;
        LockState?.Invoke(true);
    }
    public void UnlockPlayer()
    {
        playerCombatController.enabled = true;
        playerMovementController.enabled = true;
        LockState?.Invoke(false);
    }

    public void OnGrabbedPlayer()
    {
        OnGrabbed?.Invoke();
    }

    public void OnThrowPlayer()
    {
        OnThrow?.Invoke();
    }
}