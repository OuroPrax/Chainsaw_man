using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossEnemiesGenerator : MonoBehaviour
{
    [SerializeField] SharedInt activeEnemies;
    [SerializeField] HealthToPoolingContextUpdatedEventChannelSO healthInfosUpdatedEventChannel;
    [SerializeField] TargetEventChannelSO targetEventChannel;
    [SerializeField] Transform enemiesParent;

    [Header("Pooling")]
    [SerializeField] HealthToPoolingContext[] healthToContext;
    [SerializeField] EnemySpawnConfigSO[] enemySpawnConfigs;
    Dictionary<EnemySO, EnemySpawnConfigSO> enemySOToConfig;
    Dictionary<EnemySO, NewEnemiesPool> enemySOToPool;
    readonly Dictionary<EnemySO, PoolingContext> enemyToContext = new();

    Dictionary<EnemySO, List<HealthToPoolingContext>> enemySOTohealthToContextSorted;
    readonly List<Coroutine> coroutines = new();
    HealthHandler targetHealthHandler;

    private void Awake()
    {
        activeEnemies.Value = 0;
        enemySOTohealthToContextSorted = new();
        foreach (var item in healthToContext.OrderBy(x => x.healthRate))
        {
            var enemySO = item.poolingContext.enemySO;

            if (!enemySOTohealthToContextSorted.ContainsKey(enemySO))
                enemySOTohealthToContextSorted[enemySO] = new();

            enemySOTohealthToContextSorted[enemySO].Add(item);
        }
        enemySOToPool = enemySpawnConfigs.ToDictionary(c => c.enemySO, c => CreatePool(c));
        enemySOToConfig = enemySpawnConfigs.ToDictionary(c => c.enemySO, c => c);
    }
    NewEnemiesPool CreatePool(EnemySpawnConfigSO spawnConfig)
    {
        NewEnemiesPool pool =  new(targetEventChannel, spawnConfig.enemySO.prefab, spawnConfig.minPoolSize, spawnConfig.maxPoolSize);
        pool.OnActiveEnemy += AddEnemy; 
        pool.OnUnactiveEnemy += RemoveEnemy; 
        return pool;
    }
    void AddEnemy(GameObject enemy) => activeEnemies.Value++;
    void RemoveEnemy(GameObject enemy) => activeEnemies.Value--;
    private void Start() => healthInfosUpdatedEventChannel.RaiseEvent(healthToContext.ToList());

    private void OnDestroy()
    {
        if (targetHealthHandler != null)
            targetHealthHandler.OnHealthChanged -= UpdateGeneration;

        foreach (var item in enemySOToPool)
        {
            if (item.Value == null) continue;

            item.Value.OnActiveEnemy -= AddEnemy;
            item.Value.OnUnactiveEnemy -= RemoveEnemy;
            item.Value.Dispose();
        }
    }

    public void StartSpawn(Transform bossTransform)
    {
        StopSpawn();

        targetHealthHandler = bossTransform.GetComponent<HealthHandler>();
        targetHealthHandler.OnHealthChanged += UpdateGeneration;
        UpdateGeneration(0, 0);

        foreach (var ctp in enemySOToPool)
        {
            coroutines.Add(StartCoroutine(ResolveEnemyLoop(bossTransform, ctp.Value, enemySOToConfig[ctp.Key])));
        }
    }
    public void StopSpawn()
    {
        if (targetHealthHandler != null)
            targetHealthHandler.OnHealthChanged -= UpdateGeneration;

        foreach (var cor in coroutines)
            StopCoroutine(cor);
        coroutines.Clear();
    }

    void UpdateGeneration(float previous, float current)
    {
        var healthRate = targetHealthHandler.HealthRate;
        foreach (var kvp in enemySOTohealthToContextSorted)
        {
            foreach (var item in kvp.Value)
            {
                if (healthRate <= item.healthRate)
                {
                    enemyToContext[item.poolingContext.enemySO] = item.poolingContext;
                    break;
                }
            }
        }
    }

    IEnumerator ResolveEnemyLoop(Transform transformSpawn, NewEnemiesPool pool, EnemySpawnConfigSO enemySpawnConfig)
    {
        while (true)
        {
            if (enemyToContext.TryGetValue(enemySpawnConfig.enemySO, out var context) && pool.CountActives < context.maxSpawn)
            {
                var spawnPoint = transformSpawn.position;
                spawnPoint.y = enemiesParent.transform.position.y;
                var go = pool.GetNewEnemy();
                go.transform.SetParent(enemiesParent, false);
                go.transform.position = GetRandomPositionInRadius(spawnPoint, enemySpawnConfig.spawnRadius);

                foreach (var item in context.atributes)
                {
                    if (item.key == "HasToRun" && go.TryGetComponent(out EnemyMovementHandler enemyMovementHandler))
                    {
                        enemyMovementHandler.HasToRun = int.Parse(item.value) > UnityEngine.Random.Range(0, 100);
                        Debug.Log(int.Parse(item.value));
                    }
                }
            }
            else if (!pool.IsFool)
            {
                var spawnPoint = transformSpawn.position;
                spawnPoint.y = enemiesParent.transform.position.y;
                var go = pool.GetNewEnemy();
                go.transform.SetParent(enemiesParent, false);
                go.transform.position = GetRandomPositionInRadius(spawnPoint, enemySpawnConfig.spawnRadius);
            }

            yield return new WaitForSeconds(GetWaitTime(enemySpawnConfig, pool.ActivesMaxRate));
        }
    }

    /// <summary>
    /// Devuelve una posicion random en el plano XZ dentro del rango del radio y usando un punto central
    /// </summary>
    /// <returns></returns>
    Vector3 GetRandomPositionInRadius(Vector3 spawnPoint, float spawnRadius)
    {
        Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * spawnRadius;
        return spawnPoint + new Vector3(randomCircle.x, 0f, randomCircle.y);
    }
    /// <summary>
    /// Utiliza una curva de animacion para evaluar en base al ratio de enemigos activos actuales cuanto esperar para generar el proximo enemigo
    /// </summary>
    float GetWaitTime(EnemySpawnConfigSO spawnConfig, float rate) => spawnConfig.spawnCurve.Evaluate(Mathf.Clamp01(rate)) * spawnConfig.baseWaitTime;


    [Serializable]
    public class HealthToPoolingContext
    {
        [Range(0f, 1f)] public float healthRate;
        public PoolingContext poolingContext;
    }
    [Serializable]
    public class PoolingContext
    {
        public EnemySO enemySO;
        public int maxSpawn;
        public KeyValue[] atributes;
        [Serializable]
        public struct KeyValue
        {
            public string key;
            public string value;
        }
    }
}