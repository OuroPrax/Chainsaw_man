using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] float duration = 1.5f;
    [SerializeField] Vector3 moveDirection = Vector3.up * 50f; // Pixels in UI space
    [SerializeField] Vector3 randomInterval;
    CanvasGroup canvasGroup;
    Action onComplete;

    void Awake()
    {
        if (!TryGetComponent(out canvasGroup))
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void Show(string value, Vector3 position, Action releaseCallback)
    {
        transform.position = position;
        text.text = value;
        onComplete = releaseCallback;

        canvasGroup.alpha = 1f;
        transform.localPosition = Vector3.zero;

        // Kill previous tweens if any
        transform.DOKill();
        canvasGroup.DOKill();

        Vector3 randomOffset = new(
           UnityEngine.Random.Range(-randomInterval.x, randomInterval.x),
           UnityEngine.Random.Range(-randomInterval.y, randomInterval.y),
           UnityEngine.Random.Range(-randomInterval.z, randomInterval.z)
       );

        // Animate position
        transform.DOMove(transform.position + randomOffset + moveDirection, duration).SetEase(Ease.OutQuad);

        // Animate fade
        canvasGroup.DOFade(0f, duration).SetEase(Ease.InQuad).OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }
}
