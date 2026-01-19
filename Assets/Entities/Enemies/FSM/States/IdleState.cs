using UnityEngine;

public class IdleState : IEnemyState
{
    readonly EnemyBehaviour enemy;

    public IdleState(EnemyBehaviour enemy) => this.enemy = enemy;

    public void Enter() { enemy.StopMoving(); }
    public void Update()
    {
        if (enemy.CanSeePlayer)
            enemy.StateMachine.SetState(new ChaseState(enemy));
    }
    public void Exit() { }
}
