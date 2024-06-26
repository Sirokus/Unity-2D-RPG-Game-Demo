public class EnemyStateMachine : OldStateMachine
{
    public EnemyState currentState { get; private set; }

    public void Initialize(EnemyState state)
    {
        currentState = state;
        currentState.Enter();
    }

    public void ChangeState(EnemyState state)
    {
        currentState.Exit();
        currentState = state;
        currentState.Enter();
    }

    public override OldState GetCurrentState()
    {
        return currentState;
    }
}
