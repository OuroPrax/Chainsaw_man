using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderSensitivityHandler : MonoBehaviour
{
    [SerializeField] SharedFloat sensitivity;
    [SerializeField] Slider slider;
    [SerializeField] TextMeshProUGUI valueText;

    private void OnEnable()
    {
        slider.value = sensitivity.Value;
        valueText.text = slider.value.ToString("F2");
        slider.onValueChanged.AddListener(InvokeOnAudioValueChange);
    }
    private void OnDisable()
    {
        slider.onValueChanged.RemoveListener(InvokeOnAudioValueChange);
    }

    void InvokeOnAudioValueChange(float value)
    {
        sensitivity.Value = slider.value;
        valueText.text = slider.value.ToString("F2");
        ValuesUtil.SaveSensitivity(sensitivity.Value);
    }
}
