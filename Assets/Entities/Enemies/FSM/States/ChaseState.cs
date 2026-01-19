public class ChaseState : IEnemyState
{
    readonly EnemyBehaviour enemy;
    public ChaseState(EnemyBehaviour enemy) => this.enemy = enemy;

    public void Enter() { }
    public void Update()
    {
        if (!enemy.CanSeePlayer)
        {
            enemy.StateMachine.SetState(new IdleState(enemy));
            return;
        }

        if (enemy.IsPlayerInAttackRange())
        {
            enemy.StateMachine.SetState(new AttackState(enemy));
            return;
        }

        enemy.MoveTowardsPlayer();
    }
    public void Exit() { }
}
