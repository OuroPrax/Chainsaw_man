using UnityEngine;

public class DeadState : IEnemyState
{
    readonly EnemyBehaviour enemy;
    float timer;
    bool hasRaisedEvent;
    public DeadState(EnemyBehaviour enemy) => this.enemy = enemy;
    public void Enter() 
    {
        timer = 2f;
        hasRaisedEvent = false;
        enemy.ActiveRagdoll();
    }
    public void Update() 
    {
        if(hasRaisedEvent) return;

        timer -= Time.deltaTime;
        if (timer < 0)
        {
            enemy.EnemyDeadEventChannel.RaiseEvent(enemy.transform);
            hasRaisedEvent= true;
        }
    }
    public void Exit() { enemy.DesactiveRagdoll(); }
}
