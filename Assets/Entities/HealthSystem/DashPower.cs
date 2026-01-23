using System.Collections;
using UnityEngine;

/// <summary>
/// Habilidad de dash del jugador, que hace que se mueva rapidamente hacia adelante por un tiempo
/// </summary>
public class DashPower : MonoBehaviour
{
    bool IsFull => powerRate.Value >= 1f;
    bool IsDashing => dashing != null;
    bool IsRecovering => recovering != null;

    [SerializeField] float speedMultiplier = 5f;
    [SerializeField] float duration = .5f;
    [SerializeField] float rateGainedPerSecond = .1f;
    [SerializeField] SharedFloat powerRate;
    PlayerMovementController playerMovementController;
    Coroutine dashing, recovering;
    private void Awake()
    {
        powerRate.Value = 0f;
        playerMovementController = GetComponent<PlayerMovementController>();
    }
    private void OnEnable()
    {
        if (!IsFull && !IsRecovering)
            recovering = StartCoroutine(Recover());
    }
    private void OnDisable()
    {
        if(recovering != null )
        {
            StopCoroutine(recovering);
            recovering = null;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && IsFull && !IsDashing && playerMovementController.CanMove)
        {
            Debug.Log("Dash");
            dashing = StartCoroutine(Dash());
        }
    }
    IEnumerator Dash()
    {
        powerRate.Value = 0f;
        var rb = GetComponent<Rigidbody>();

        var t = 0f;
        while (t < duration)
        {
            yield return null;
            t += Time.deltaTime;
            rb.velocity = transform.forward * speedMultiplier;
        }
        rb.velocity = Vector3.zero;
        dashing = null;
        recovering = StartCoroutine(Recover());
    }
    IEnumerator Recover()
    {
        while (!IsFull)
        {
            yield return null;
            powerRate.Value = Mathf.Clamp01(powerRate.Value + rateGainedPerSecond * Time.deltaTime);
        }
        recovering = null;
    }
}