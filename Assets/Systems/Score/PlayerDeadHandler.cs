using UnityEngine;
using UnityEngine.Events;

public class PlayerDeadHandler: MonoBehaviour
{
    public UnityEvent OnPlayerDead;
    [SerializeField] HealthHandler healthHandler;
    private void OnEnable() => healthHandler.OnHealthChanged += CheckDead;
    private void OnDisable() => healthHandler.OnHealthChanged -= CheckDead;
    private void CheckDead(float previous, float current)
    {
        if (current > 0) return;

        BattleServiceLocator.Instance.Get<BattleEndCondictionMetChecker>().StopCheck();
        OnPlayerDead?.Invoke();
    }
}
