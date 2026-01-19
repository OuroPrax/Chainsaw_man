using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UISpecialBar : MonoBehaviour
{
    public UnityEvent OnFull;
    public UnityEvent OnChanging;

    [SerializeField] FloatEventChannelSO SpecialRateChangedEventChannel;
    [SerializeField] Image filledImage;

    private void Start() => filledImage.fillAmount = 0;

    private void OnEnable() => SpecialRateChangedEventChannel.OnEventRaised += UpdateRate;
    private void OnDisable() => SpecialRateChangedEventChannel.OnEventRaised -= UpdateRate;
    void UpdateRate(float currentRate)
    {
        filledImage.fillAmount = currentRate;
        (currentRate == 1 ? OnChanging : OnFull)?.Invoke();
    }
}
