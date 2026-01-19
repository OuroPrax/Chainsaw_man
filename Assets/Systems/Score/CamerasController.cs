using Cinemachine;
using UnityEngine;

/// <summary>
/// Sistema para poder cambiar entre las distantas camaras con las que cuenta el jugador
/// </summary>
public class CamerasController : MonoBehaviour
{
    [SerializeField] SharedFloat sensitivity;
    [SerializeField] CinemachineFreeLook freelookCam;
    [SerializeField] CinemachineVirtualCamera targetCam;
    Transform bossTarget;//TODO: Forzado, pero alcanza para este prototipo
    float xValue;
    float yValue;

    private void Awake()
    {
        xValue = freelookCam.m_XAxis.m_MaxSpeed;
        yValue = freelookCam.m_YAxis.m_MaxSpeed;
    }
    private void OnEnable()
    {
        sensitivity.OnValueChanged += UpdateSensitivity;
        UpdateSensitivity(sensitivity.Value);
    }
    private void OnDisable() => sensitivity.OnValueChanged -= UpdateSensitivity;
    void UpdateSensitivity(float value)
    {
        freelookCam.m_XAxis.m_MaxSpeed = xValue * value;
        freelookCam.m_YAxis.m_MaxSpeed = yValue * value;
    }

    private void Start()
    {
        targetCam.enabled = false;
        freelookCam.enabled = true;
        bossTarget = FindAnyObjectByType<BossController>().transform; //TODO: Forzado, pero alcanza para este prototipo
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
            ToggleCamera();

    }
    void ToggleCamera()
    {
        if (freelookCam.enabled)
            SetTargetCam(bossTarget);
        else
            UnsetTargetCam();
    }

    public void SetTargetCam(Transform target)
    {
        targetCam.LookAt = target;
        targetCam.enabled = true;
        freelookCam.enabled = false;
    }
    public void UnsetTargetCam()
    {    
        targetCam.enabled = false;
        freelookCam.enabled = true;
    }
}