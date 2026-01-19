using TMPro;
using UnityEngine;

public class UITimer : MonoBehaviour
{
    [SerializeField] SharedFloat sharedValue;
    [SerializeField] TextMeshProUGUI timerText;
    private void OnEnable() => sharedValue.OnValueChanged += UpdateText;
    private void OnDisable() => sharedValue.OnValueChanged -= UpdateText;
    private void Start() => UpdateText(sharedValue.Value);
    void UpdateText(float seconds)
    {
        int totalSeconds = Mathf.FloorToInt(seconds);
        int hours = totalSeconds / 3600;
        int minutes = (totalSeconds % 3600) / 60;
        int secs = totalSeconds % 60;

        timerText.text = $"{hours:D2}:{minutes:D2}:{secs:D2}";
    }
}