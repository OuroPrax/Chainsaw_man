using UnityEngine;

public class EnemyMovementHandler : MonoBehaviour
{
    private static readonly int hasToRunpId = Animator.StringToHash("HasToRun");

    [SerializeField] float speed = 3f;
    [SerializeField] float runMultiplier = 1.5f;
    [SerializeField] float rotationSpeed = 10f;
    [SerializeField] Animator animator;
    bool hasToRun;
    public bool HasToRun
    {
        get => hasToRun;
        set
        {
            if (hasToRun == value) return;

            hasToRun = value;
            if(animator)
                animator.SetBool(hasToRunpId, value);
        }
    }

    public void MoveTo(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position);
        direction.y = 0f; // Evita que mire hacia arriba o abajo si hay diferencia de altura

        if (direction.sqrMagnitude < 0.01f) return;

        // Movimiento
        Vector3 moveDir = direction.normalized;
        transform.position += (hasToRun ? speed * runMultiplier : speed) * Time.deltaTime * moveDir;

        // Rotación suave hacia la dirección de movimiento
        Quaternion targetRotation = Quaternion.LookRotation(moveDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

    }

    public void Stop()
    {
    }
}
