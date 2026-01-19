public class PushState : IEnemyState
{
    readonly EnemyBehaviour enemy;

    public PushState(EnemyBehaviour enemy) => this.enemy = enemy;

    public void Enter() { enemy.ActiveRagdoll(); }
    public void Update()
    {
    }
    public void Exit() { }
}