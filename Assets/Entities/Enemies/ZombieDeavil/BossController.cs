using UnityEngine;

[RequireComponent(typeof(IHealth))]
public class BossController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] MovementPattern movement;
    [SerializeField] AllyLauncher allyLauncher;
    [SerializeField] BossEnemiesGenerator enemiesGenerator;
    IHealth health;
    private void Awake() => health = GetComponent<IHealth>();

    public void Activate()
    {
        enemiesGenerator.StartSpawn(transform);
        movement.BeginRoute();
        allyLauncher.BeginLaunching(FindAnyObjectByType<PlayerCombatController>(FindObjectsInactive.Include).transform);
    }
    public void Deactivate()
    {
        enemiesGenerator.StopSpawn();
        movement.StopMovement();
        allyLauncher.StopLaunching();
    }
    private void OnEnable() => health.OnHealthChanged += CheckDeath;
    private void OnDisable() => health.OnHealthChanged -= CheckDeath;
    void CheckDeath(float previous, float current)
    {
        if (current > 0) return;

        Deactivate();
    }
}
