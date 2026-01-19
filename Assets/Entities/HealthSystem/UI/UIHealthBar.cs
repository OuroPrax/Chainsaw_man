using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIHealthBar: MonoBehaviour
{
    public UnityEvent OnChanged;
    [SerializeField] Image filledImage;
    [SerializeField] HealthHandler healthHandler;

    private void OnEnable() => healthHandler.OnHealthChanged += UpdateHealth;
    private void Start() => filledImage.fillAmount = healthHandler.HealthRate;
    private void OnDisable() => healthHandler.OnHealthChanged -= UpdateHealth;
    void UpdateHealth(float _, float __) => UpdateHealth();
    void UpdateHealth()
    {
        filledImage.fillAmount = healthHandler.HealthRate;
        OnChanged?.Invoke();
    }
}
