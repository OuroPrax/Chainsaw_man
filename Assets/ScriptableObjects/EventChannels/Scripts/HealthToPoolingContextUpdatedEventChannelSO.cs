using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/HealthChangedInfosEventChannel")]
public class HealthToPoolingContextUpdatedEventChannelSO : ValueEventChannelSO<List<BossEnemiesGenerator.HealthToPoolingContext>>
{
}