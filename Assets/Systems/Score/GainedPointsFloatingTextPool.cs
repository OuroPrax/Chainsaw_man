using System;
using UnityEngine;
using UnityEngine.Pool;

public class GainedPointsFloatingTextPool : MonoBehaviour
{
    [SerializeField] IntEventChannelSO ScoreGainedEvent;
    [SerializeField] Transform spawnTransform;
    [SerializeField] GameObject prefab;
    [SerializeField] int poolSize = 10;

    private ObjectPool<FloatingText> pool;

    void Awake()
    {
        pool = new ObjectPool<FloatingText>(
            CreateText,
            OnGetText,
            OnReleaseText,
            OnDestroyText,
            false,
            2,
            poolSize
        );
    }
    private void OnEnable() => ScoreGainedEvent.OnEventRaised += ResolveGained;
    private void OnDisable() => ScoreGainedEvent.OnEventRaised -= ResolveGained;
    void ResolveGained(int arg0) => SpawnText(arg0.ToString());
    public void SpawnText(string value)
    {
        var ft = pool.Get();
        ft.Show(value, spawnTransform.position, () => pool.Release(ft));
    }
    FloatingText CreateText() => Instantiate(prefab, transform).GetComponent<FloatingText>();
    void OnGetText(FloatingText ft) => ft.gameObject.SetActive(true);
    void OnReleaseText(FloatingText ft) => ft.gameObject.SetActive(false);
    void OnDestroyText(FloatingText ft) => Destroy(ft.gameObject);
}
