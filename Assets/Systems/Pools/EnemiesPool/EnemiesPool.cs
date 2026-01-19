using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[DefaultExecutionOrder(-100)]
public class EnemiesPool : MonoBehaviour
{
    [SerializeField] TargetEventChannelSO enemyDeadEventChannel;
    public event Action<GameObject> OnActiveEnemy;
    public event Action<GameObject> OnUnactiveEnemy;

    public int CountActives => pool.CountActive;
    public float ActivesMaxRate => (float) CountActives / maxSize;
    public bool IsFool => pool.CountActive >= maxSize;

    [SerializeField] GameObject enemyPrefab;
    [SerializeField] int minSize;
    [SerializeField] int maxSize;
    ObjectPool<GameObject> pool;
    HashSet<GameObject> activeEnemies = new();

    #region pool generation
    private void Awake() => pool = GeneratePool(enemyPrefab);
    private void OnEnable() => enemyDeadEventChannel.OnEventRaised += TryReleaseEnemy;
    private void OnDisable() => enemyDeadEventChannel.OnEventRaised -= TryReleaseEnemy;
    void TryReleaseEnemy(Transform enemyTransform)
    {
        if(!activeEnemies.Contains(enemyTransform.gameObject)) return;

        pool.Release(enemyTransform.gameObject);
    }

    public GameObject GetNewEnemy() => pool.Get();
    ObjectPool<GameObject> GeneratePool(GameObject prefab)
    =>  new (
            () => CreateInstance(prefab),
            OnGet,
            OnRelease,
            OnDestroy_,
            false,
            minSize,
            maxSize
        );
    GameObject CreateInstance(GameObject prefab)
    {
        var instance = Instantiate(prefab);
        instance.SetActive(false);
        return instance;
    }
    void OnGet(GameObject go)
    {
        go.SetActive(true);
        go.GetComponent<IReseteable>().ResetMe();
        activeEnemies.Add(go);
        OnActiveEnemy?.Invoke(go);
    }
    void OnRelease(GameObject go)
    {
        activeEnemies.Remove(go);
        OnUnactiveEnemy?.Invoke(go);
    }
    void OnDestroy_(GameObject go) => Destroy(go.gameObject);
    #endregion
}

/// <summary>
/// Clase que manjea un pool de enemigos, los enemigos deben implementar <see cref="IReseteable"/> para poder resetear sus valores
/// al ser obtenidos de la pool
/// </summary>
public class NewEnemiesPool: IDisposable
{
    public event Action<GameObject> OnActiveEnemy;
    public event Action<GameObject> OnUnactiveEnemy;
    public int CountActives => pool.CountActive;
    public float ActivesMaxRate => (float)CountActives / maxSize;
    public bool IsFool => pool.CountActive >= maxSize;
    readonly TargetEventChannelSO enemyDeadEventChannel;
    readonly GameObject enemyPrefab;
    readonly int minSize;
    readonly int maxSize;
    readonly ObjectPool<GameObject> pool;
    readonly HashSet<GameObject> activeEnemies = new();

    public NewEnemiesPool(TargetEventChannelSO enemyDeadEventChannel, GameObject enemyPrefab, int minSize, int maxSize)
    {
        this.enemyDeadEventChannel = enemyDeadEventChannel;
        this.enemyPrefab = enemyPrefab;
        this.minSize = minSize;
        this.maxSize = maxSize;
        enemyDeadEventChannel.OnEventRaised += TryReleaseEnemy;
        pool = GeneratePool(enemyPrefab);
    }
    public void Dispose()
    {
        enemyDeadEventChannel.OnEventRaised -= TryReleaseEnemy;
    }

    public GameObject GetNewEnemy() => pool.Get();

    #region pool generation
    void TryReleaseEnemy(Transform enemyTransform)
    {
        if (!activeEnemies.Contains(enemyTransform.gameObject)) return;

        pool.Release(enemyTransform.gameObject);
    }
    ObjectPool<GameObject> GeneratePool(GameObject prefab)
    => new(
            () => CreateInstance(prefab),
            OnGet,
            OnRelease,
            OnDestroy_,
            false,
            minSize,
            maxSize
        );
    GameObject CreateInstance(GameObject prefab)
    {
        var instance = GameObject.Instantiate(prefab);
        instance.SetActive(false);
        return instance;
    }
    void OnGet(GameObject go)
    {
        go.SetActive(true);
        go.GetComponent<IReseteable>().ResetMe();
        activeEnemies.Add(go);
        OnActiveEnemy?.Invoke(go);
    }
    void OnRelease(GameObject go)
    {
        activeEnemies.Remove(go);
        OnUnactiveEnemy?.Invoke(go);
    }
    void OnDestroy_(GameObject go) => GameObject.Destroy(go.gameObject);
    #endregion
}
