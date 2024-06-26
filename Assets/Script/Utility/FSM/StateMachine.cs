using UnityEngine;

public class StateMachine : MonoBehaviour
{
    State curState;

    public virtual void ChangeState(State state)
    {
        if (curState != null)
            curState.Exit();
        curState = state;
        curState.Enter();
    }

    public virtual T GetCurState<T>() where T : State => curState as T;

    private void Update()
    {
        curState?.Update();
    }

    private void FixedUpdate()
    {
        curState?.FixedUpdate();
    }
}
