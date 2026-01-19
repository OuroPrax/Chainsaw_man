using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UISharedInt : MonoBehaviour
{
    public UnityEvent<int> OnValueChanged;

    [SerializeField] SharedInt sharedValue;
    [SerializeField] TextMeshProUGUI scoreText;    
    private void OnEnable() => sharedValue.OnValueChanged += UpdateText;
    private void OnDisable() => sharedValue.OnValueChanged -= UpdateText;
    private void Start() => scoreText.text = sharedValue.Value.ToString();
    void UpdateText(int amount)
    {
        scoreText.text = amount.ToString();
        OnValueChanged?.Invoke(amount);
    }
}
