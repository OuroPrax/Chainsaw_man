using System;
using System.Diagnostics;

/// <summary>
/// Clase simple que controla cuando se cumple el final de batalla tras derrotar al boss y todos los enemigos
/// </summary>
public class BattleEndCondictionMetChecker
{
    public event Action<BattleEndCondictionMetChecker> OnEnded;
    readonly SharedInt activeEnemies;
    readonly IHealth bossHealth;
    public bool IsChecking { get; private set; } 

    public BattleEndCondictionMetChecker(SharedInt activeEnemies, IHealth bossHealth)
    {
        this.activeEnemies = activeEnemies;
        this.bossHealth = bossHealth;
    }
    public void StartCheck()
    {
        activeEnemies.OnValueChanged += CheckConditionMet;
        bossHealth.OnHealthChanged += CheckConditionMet;
        IsChecking = true;
    }
    public void StopCheck()
    {
        activeEnemies.OnValueChanged -= CheckConditionMet;
        bossHealth.OnHealthChanged -= CheckConditionMet;
        IsChecking = false;
    }
    void CheckConditionMet(float _, float __) => CheckConditionMet();
    void CheckConditionMet(int _) => CheckConditionMet();
    void CheckConditionMet()
    {
        if (activeEnemies.Value <= 0 && bossHealth.CurrentHealth <= 0)
        {
            UnityEngine.Debug.Log($"Termino boss:{bossHealth.CurrentHealth} active enemies {activeEnemies.Value}");
            OnEnded?.Invoke(this);
        }
    }
}