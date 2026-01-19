using UnityEngine;

/// <summary>
/// En base a un vector fuerza impulsa un rigidbody seteado por inspector, pudiendole aplicar un multiplicador
/// </summary>
public class PushForceSystem : MonoBehaviour
{
    public Rigidbody TargetRigidbody => targetRigidbody;
    [SerializeField] Rigidbody targetRigidbody;
    [SerializeField] float pushMultiplier=1f;

    public void ApplyPush(Vector3 vectorPush)
    {
        if (!targetRigidbody) return;

        targetRigidbody.AddForce(vectorPush * pushMultiplier, ForceMode.Impulse);
    }
}
