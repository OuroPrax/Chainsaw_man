using UnityEngine;

public class _test_FollowPlayer: MonoBehaviour
{
    [SerializeField] float speed = 5f;
    Transform playerTransform;
    private void Awake() => playerTransform = FindAnyObjectByType<CharacterController>().transform;

    private void Update()
    {
        Vector3 direction = playerTransform.position - transform.position;
        direction.y = 0f; // Ignorar altura (Y) para moverse solo en XZ
        direction.Normalize();

        transform.position += speed * Time.deltaTime * direction;
    }

}