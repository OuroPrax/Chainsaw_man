public class EnemyStateMachine
{
    public IEnemyState CurrentState { get; private set; }

    public void SetState(IEnemyState newState)
    {
        if(newState == CurrentState) return;
        UnityEngine.Debug.Log(newState);
        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

    public void Update() => CurrentState?.Update();
}
