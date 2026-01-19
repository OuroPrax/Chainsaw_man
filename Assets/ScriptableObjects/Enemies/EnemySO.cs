using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/ Enemy Base")]
public class EnemySO : ScriptableObject
{
    public GameObject prefab;
    public Health health;
    public EnemyType enemyType;
    public enum EnemyType { normal, miniboss, boss }
    [Serializable]
    public struct Health
    {
        public int maxHealth;
    }
}
