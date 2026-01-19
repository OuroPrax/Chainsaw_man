using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIRatePower : MonoBehaviour
{
    public UnityEvent OnFull;
    public UnityEvent OnChanging;

    [SerializeField] SharedFloat powerRate;
    [SerializeField] Image filledImage;

    private void OnEnable() => powerRate.OnValueChanged += UpdateRate;
    private void OnDisable() => powerRate.OnValueChanged -= UpdateRate;
    void UpdateRate(float currentRate)
    {
        filledImage.fillAmount = 1 - currentRate;
        (currentRate == 1 ? OnFull : OnChanging)?.Invoke();
    }
}