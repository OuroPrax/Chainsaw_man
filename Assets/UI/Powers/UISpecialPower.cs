using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UISpecialPower : MonoBehaviour
{
    public UnityEvent OnFull;
    public UnityEvent OnChanging;

    [SerializeField] FloatEventChannelSO SpecialRateChangedEventChannel;
    [SerializeField] Image filledImage;

    private void OnEnable() => SpecialRateChangedEventChannel.OnEventRaised += UpdateRate;
    private void OnDisable() => SpecialRateChangedEventChannel.OnEventRaised -= UpdateRate;
    void UpdateRate(float currentRate)
    {
        filledImage.fillAmount = 1 - currentRate;
        (currentRate == 1 ? OnFull : OnChanging)?.Invoke(); 
    }
}
