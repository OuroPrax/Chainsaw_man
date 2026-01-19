public class AttackState : IEnemyState
{
    readonly EnemyBehaviour enemy;

    public AttackState(EnemyBehaviour enemy)
    {
        this.enemy = enemy;
    }
    public void Enter() { enemy.StopMoving(); }
    public void Update()
    {
        if(enemy.IsAttacking) return;
        
        if (!enemy.IsPlayerInAttackRange())
        {
            enemy.StateMachine.SetState(new ChaseState(enemy));
            return;
        }

        enemy.TryAttackPlayer();
    }
    public void Exit() 
    {
        enemy.TryCancelAttack();
    }
}
