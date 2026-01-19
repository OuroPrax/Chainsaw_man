using UnityEngine;

/// <summary>
/// Clase que cronometra el tiempo de la batalla
/// </summary>
public class TimerHandler : MonoBehaviour
{
    [SerializeField] SharedFloat time;
    bool isRunning;

    private void Update()
    {
        if (!isRunning) return;

        time.Value += Time.deltaTime;
    }
    public void StartTimer() => isRunning = true;
    public void PauseTimer() => isRunning = false;
    public void ResetTime() => time.Value = 0f;
}