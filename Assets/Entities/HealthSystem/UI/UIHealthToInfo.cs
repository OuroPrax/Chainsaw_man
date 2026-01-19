using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIHealthToInfo: MonoBehaviour
{
    [SerializeField] HealthToPoolingContextUpdatedEventChannelSO healthInfosUpdatedEventChannel;
    [SerializeField] GameObject indicatorPrefab;
    [SerializeField] RectTransform healthBarTransform;
    private void OnEnable() => healthInfosUpdatedEventChannel.OnEventRaised += Resolve;
    private void OnDisable() => healthInfosUpdatedEventChannel.OnEventRaised -= Resolve;
    void Resolve(List<BossEnemiesGenerator.HealthToPoolingContext> updated)
    {
        foreach (var info in updated)
            CreateIndicatorAt(info.healthRate);
    }
    void CreateIndicatorAt(float rate)
    {
        rate = Mathf.Clamp01(rate);

        var indicator = Instantiate(indicatorPrefab, healthBarTransform).GetComponent<RectTransform>();
        indicator.gameObject.SetActive(true);

        indicator.anchorMin = new Vector2(rate, indicator.anchorMin.y);
        indicator.anchorMax = new Vector2(rate, indicator.anchorMax.y);

        // Centrar el pivot
        indicator.pivot = new Vector2(0.5f, indicator.pivot.y);

        // Posición local en X debe ser 0 para que el centro coincida con el anchor
        Vector2 anchoredPos = indicator.anchoredPosition;
        anchoredPos.x = 0;
        indicator.anchoredPosition = anchoredPos;
    }
}