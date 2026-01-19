using UnityEngine;

[CreateAssetMenu(fileName = "EnemySpawnConfigSO", menuName = "Game/Enemy Spawn Config")]
public class EnemySpawnConfigSO : ScriptableObject
{
    public EnemySO enemySO;
    [Space(10)]
    [Min(0)] public int minPoolSize = 5;
    [Min(1)] public int maxPoolSize = 20;
    [Space(10)]
    [Tooltip("Tiempo de espera base para generar un enemigo")]
    public float baseWaitTime = 5f;
    [Tooltip("Curva: X = ratio de enemigos activos (0 a 1), Y = multiplicador de tiempo de espera")]
    public AnimationCurve spawnCurve = AnimationCurve.Linear(0, 1, 1, 0);
    [Tooltip("Radio máximo de distancia para generar enemigos")]
    public float spawnRadius = 10f;
}