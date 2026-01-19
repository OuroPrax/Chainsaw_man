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
    Coroutine dashing, recovering;

    private void Start()
    {
        powerRate.Value = 0f;
        if(!IsFull && !IsRecovering)
           recovering = StartCoroutine(Recover());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && IsFull && !IsDashing)
        {
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