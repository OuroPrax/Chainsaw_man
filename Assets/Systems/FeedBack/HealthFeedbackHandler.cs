using Cinemachine;
using DG.Tweening;
using System;
using UnityEngine;

public class HealthFeedbackHandler : MonoBehaviour
{
    [SerializeField] HealthHandler healthHandler;
    [SerializeField] CinemachineImpulseSource impulseSource;
    [Header("Overlay")]
    [SerializeField] CanvasGroup damageOverlay;
    private void OnEnable() => healthHandler.OnHealthChanged += ResolveHealthChanged;
    private void OnDisable() => healthHandler.OnHealthChanged -= ResolveHealthChanged;
    private void OnDestroy() => damageOverlay.DOKill();
    private void ResolveHealthChanged(float previous, float current)
    {
        if (previous <= current) return;

        TriggerDamageImpulse();
        FlashDamage();
    }
    void TriggerDamageImpulse() 
    {
        Vector3 randomDir = UnityEngine.Random.insideUnitSphere;
        impulseSource.GenerateImpulse(randomDir);
    }
    void FlashDamage() => damageOverlay.DOFade(1f, 0.1f).SetEase(Ease.OutQuad).OnComplete(() => damageOverlay.DOFade(0f, 0.3f));
}
