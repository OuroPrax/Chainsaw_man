using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SliderSoundHandler : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Slider slider;
    [SerializeField] TextMeshProUGUI valueText;
    [SerializeField] GameAudioType audioType;

    private void Awake() => slider.onValueChanged.AddListener(InvokeOnAudioValueChange);
    private void OnEnable()
    {
        slider.value = ValuesUtil.LoadAudioPercentage(audioType);
        valueText.text = slider.value.ToString("F0");
    }
    void InvokeOnAudioValueChange(float value)
    {
        audioMixer.SetFloat(audioType.ToString(), ValuesUtil.GetVolumeForPencentage(value));
        ValuesUtil.SaveAudioPercentage(audioType, slider.value);
        valueText.text = slider.value.ToString("F0");
    }
}
