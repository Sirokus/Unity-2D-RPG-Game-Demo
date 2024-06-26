using UnityEngine;

public class EnemyAttackState : EnemyState
{
    public EnemyAttackState(Enemy owner, EnemyStateMachine stateMachine, string AnimName) : base(owner, stateMachine, AnimName)
    {
    }

    public override void Exit()
    {
        base.Exit();

        owner.CloseCounterAttackWindow();
    }

    public override void OnAnimationTrigger()
    {
        base.OnAnimationTrigger();

        owner.lastAttackTime = Time.time;
        stateMachine.ChangeState(owner.battleState);
    }
}


