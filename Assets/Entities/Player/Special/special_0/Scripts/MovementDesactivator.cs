using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementDesactivator : MonoBehaviour
{
    [SerializeField] PlayerMovementController playerMovementController;

    private void OnEnable() => playerMovementController.ChangeCanMove(false);

    private void OnDisable() => playerMovementController.ChangeCanMove(true);
}
