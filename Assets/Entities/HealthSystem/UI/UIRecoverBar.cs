using UnityEngine;
using UnityEngine.UI;

public class UIRecoverBar : MonoBehaviour
{
    [SerializeField] Image filledImage;
    [SerializeField] RecoverHealthPower recoverHealthPower;

    private void OnEnable() => recoverHealthPower.OnRecoverChanged += UpdateRecover;
    private void OnDisable() => recoverHealthPower.OnRecoverChanged -= UpdateRecover;
    void UpdateRecover(float _) => filledImage.fillAmount = recoverHealthPower.RateValue;
}