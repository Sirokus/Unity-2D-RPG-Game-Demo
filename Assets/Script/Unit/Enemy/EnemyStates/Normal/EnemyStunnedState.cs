using UnityEngine;

public class EnemyStunnedState : EnemyState
{
    public EnemyStunnedState(Enemy owner, EnemyStateMachine stateMachine, string AnimName) : base(owner, stateMachine, AnimName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        owner.fx.InvokeRepeating("RedColorBlink", 0, .1f);

        timer = owner.stunDuration;
        owner.SetVelocity(new Vector2(-owner.dir * owner.stunDirection.x, owner.stunDirection.y));
    }

    public override void Update()
    {
        base.Update();

        if (timer < 0) stateMachine.ChangeState(owner.idleState);
    }

    public override void Exit()
    {
        base.Exit();

        owner.SetVelocity(Vector2.zero);

        owner.fx.Invoke("CancelColorChange", 0);
    }
}