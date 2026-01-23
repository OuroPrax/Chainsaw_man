using System.Collections;
using UnityEngine;

/// <summary>
/// Controla el movimiento del jugador basado en la orientación de la cámara.
/// El jugador se mueve en la dirección relativa a la cámara y rota suavemente
/// para mirar hacia donde se desplaza.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementController : MonoBehaviour
{
    [Header("Ajustes de movimiento")]
    [Tooltip("Velocidad de desplazamiento del jugador en unidades por segundo.")]
    [SerializeField] private float movementSpeed = 5f;
    [Tooltip("Tiempo (en seg) para suavizar cambios de velocidad.")]
    [SerializeField] private float speedSmoothTime = 0.1f;
    [Tooltip("Velocidad de rotación del jugador al cambiar de dirección.")]
    [SerializeField] private float rotationSmoothTime = 0.1f;
    [Tooltip("Fuerza de gravedad aplicada al personaje.")]
    [SerializeField] private float gravity = -9.81f;

    [Header("Inclinación al moverse lateralmente")]
    [Tooltip("Ángulo máximo de inclinación (roll) en grados.")]
    [SerializeField] private float maxTiltAngle = 15f;
    [Tooltip("Tiempo (en seg) para suavizar cambios de tilt.")]
    [SerializeField] private float tiltSmoothTime = 0.1f;

    Rigidbody rb;
    public bool IsGrounded { get; private set; }
    [SerializeField] Transform cameraTransform;

    // Estado vertical y suavizadores
    private float verticalVelocity = 0f;
    private float currentTurnVelocity;

    // Velocidad horizontal instant y suavizada
    private float currentRawSpeed = 0f;
    private float currentSmoothedSpeed = 0f;
    private float speedSmoothVelocity = 0f;

    // Tilt actual y su suavizador
    private float currentTiltAngle = 0f;
    private float tiltSmoothVelocity = 0f;

    // Constantes para snap al suelo
    private const float groundSnapDistance = 0.3f;
    private const float raycastOriginOffset = 0.05f;
    private const float minSnapThreshold = 0.01f;

    // Permite el movimiento del jugador
    public bool CanMove { get; private set; } = true;

    private void Awake()
    {
        lookDir = transform.forward;
        rb = GetComponent<Rigidbody>();
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    Vector3 moveDir;
    Vector3 lookDir;
    private void Update()
    {
        HandleMovementInput();
        SmoothHorizontalSpeed();
    }
    void HandleMovementInput()
    {
        // 1) Input
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 input = new Vector3(h, 0, v).normalized;

        // 2) Dirección según cámara
        Vector3 forward = cameraTransform.forward; forward.y = 0; forward.Normalize();
        Vector3 right = cameraTransform.right; right.y = 0; right.Normalize();
        moveDir = (forward * input.z + right * input.x).normalized;
    }
    void SmoothHorizontalSpeed()
        => currentSmoothedSpeed = Mathf.SmoothDamp(currentSmoothedSpeed, currentRawSpeed, ref speedSmoothVelocity, speedSmoothTime);


    void FixedUpdate()
    {
        HandleMovement();
        if(moveDir.sqrMagnitude > 0.01f)
            lookDir = moveDir;
        RotateAndTilt(lookDir);
    }
    private void HandleMovement()
    {
        if (!CanMove)
        {
            transform.rotation = new Quaternion(0, transform.rotation.y, 0, transform.rotation.w);
            //RotateAndTilt(new Vector3(0, transform.forward.y,0));
            return;
        }

        // 1) Si no está en el suelo, no aplicar movimiento horizontal
        if (!IsGrounded)
        {
            currentRawSpeed = 0f;
            return;
        }

        // 2) Obtener velocidad horizontal actual (sin componente vertical)
        Vector3 velocity = rb.velocity;
        Vector3 horizontalVelocity = new(velocity.x, 0f, velocity.z);
        float currentHorizontalSpeed = horizontalVelocity.magnitude;

        // 3) Si ya se mueve más rápido que el límite, no aplicar movimiento
        if (currentHorizontalSpeed > movementSpeed)
        {
            currentRawSpeed = currentHorizontalSpeed;
            return;
        }

        // 4) Calcular velocidad horizontal deseada
        Vector3 desiredHorizontalVelocity = moveDir * movementSpeed;

        // 5) Mantener velocidad vertical actual
        Vector3 targetVelocity = new(desiredHorizontalVelocity.x, velocity.y, desiredHorizontalVelocity.z);

        // 6) Aplicar velocidad
        rb.velocity = targetVelocity;

        // 7) Guardar velocidad raw
        currentRawSpeed = desiredHorizontalVelocity.magnitude;
    }
    void RotateAndTilt(Vector3 moveDir)
    {
        if (!CanMove) return;
        
        // --- Yaw (rotación horizontal) ---
        float targetYaw = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;
        float smoothedYaw = Mathf.SmoothDampAngle(
            transform.eulerAngles.y, targetYaw, ref currentTurnVelocity, rotationSmoothTime
        );

        // --- Tilt (inclinación roll) según componente X en local space ---
        Vector3 localDir = transform.InverseTransformDirection(moveDir);
        float targetTilt = -localDir.x * maxTiltAngle;
        currentTiltAngle = Mathf.SmoothDamp(
            currentTiltAngle, targetTilt, ref tiltSmoothVelocity, tiltSmoothTime
        );

        // Aplicar rotación compuesta: yaw y roll
        transform.rotation = Quaternion.Euler(0f, smoothedYaw, currentTiltAngle);
    }

    /// <summary>Velocidad horizontal suavizada (0..movementSpeed).</summary>
    public float CurrentSmoothedHorizontalSpeed => currentSmoothedSpeed;
    /// <summary>Velocidad normalizada suavizada (0..1).</summary>
    public float MovementSpeedNormalized => Mathf.Clamp01(currentSmoothedSpeed / movementSpeed);

    #region Ground

    [SerializeField] float groundCheckInterval = 0.2f;
    [SerializeField] LayerMask groundMask;

    private void OnEnable() => StartCoroutine(GroundCheckRoutine());
    private void OnDisable() => StopCoroutine(GroundCheckRoutine());
    IEnumerator GroundCheckRoutine()
    {
        var wait = new WaitForSeconds(groundCheckInterval);
        while (true)
        {
            CheckGrounded();
            yield return wait;
        }
        void CheckGrounded()
        {
            Vector3 origin = transform.position + Vector3.up * raycastOriginOffset;
            IsGrounded = Physics.Raycast(origin, Vector3.down, groundSnapDistance, groundMask);
        }
    }
    #endregion


    #region CanMove Methods
    public void ChangeCanMove(bool value) => CanMove = value;
    #endregion
}