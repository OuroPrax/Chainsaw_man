using System;
using UnityEngine;

/// <summary>
/// Clase manejadora de vida que implementa <see cref="IHealth"/> e <see cref="IDamageable"/> 
/// para poder recibir dano
/// </summary>
public class HealthHandler : MonoBehaviour, IHealth, IDamageable
{
    [SerializeField] bool useParticles;
    [SerializeField] ParticleEffectCatalog particleEffectCatalog;

    public event Action</* Previous */float, /* Current */float> OnHealthChanged;
    public float MaxHealth => maxHealth;
    public float CurrentHealth { get; private set; }
    public float HealthRate => CurrentHealth/maxHealth;
    public bool NoHealth => CurrentHealth <= 0;

    bool _isInvulnerability;

    [SerializeField] int maxHealth = 100;

    private void Awake() => CurrentHealth = maxHealth;
    public void ChangeHealth(float amount)
    {
        if (NoHealth || amount == 0f) return;

        float previous = CurrentHealth;
        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0f, MaxHealth);
        OnHealthChanged?.Invoke(previous, CurrentHealth);
    }
    public void ResetHealth()
    {
        float previous = CurrentHealth;
        CurrentHealth = maxHealth;
        OnHealthChanged?.Invoke(previous, CurrentHealth);
    }
    public void TakeDamage(float amount, Vector3 position)
    {
        if (!_isInvulnerability)
        {
            if (useParticles)
                BattleServiceLocator.Instance.Get<IParticleSystemPool>().Play(particleEffectCatalog, position, transform.rotation);
            
            ChangeHealth(-Mathf.Max(amount, 0f));
        }
    }

    #region Invulnerability methods
    public void EnableInvulnerability() => _isInvulnerability = true;
    public void DisableInvulnerability() => _isInvulnerability = false;
    #endregion
}
