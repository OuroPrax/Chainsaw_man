using DG.Tweening;
using System.Collections;
using UnityEngine;

public class UIShaker : MonoBehaviour
{
    [Header("Shake Settings")]
    [SerializeField] float duration = 0.3f;
    [SerializeField] float strength = 10f;
    [SerializeField] int vibrato = 10;
    RectTransform rect;
    private void Awake() => rect = GetComponent<RectTransform>();
    private void OnDestroy() => rect.DOKill();
    public void Shake()
    {
        rect.DOKill(); // Cancelar tweens previos si hay
        rect.DOShakePosition(duration, strength, vibrato)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => rect.anchoredPosition = Vector2.zero);
    }
}
