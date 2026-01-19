using System;

public interface IHealth
{
    public event Action<float, float> OnHealthChanged;

    float MaxHealth { get; }
    float CurrentHealth { get; }
    float HealthRate { get; }
    void ChangeHealth(float amount);
    void ResetHealth();
}
