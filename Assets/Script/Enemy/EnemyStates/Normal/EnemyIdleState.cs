using UnityEngine;

public class EnemyIdleState : EnemyGroundState
{
    public EnemyIdleState(Enemy owner, EnemyStateMachine stateMachine, string AnimName) : base(owner, stateMachine, AnimName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        timer = Random.Range(owner.idleTime.x, owner.idleTime.y);
    }

    public override void Update()
    {
        base.Update();

        if (timer < 0 && owner.anim.speed != 0)
        {
            stateMachine.ChangeState(owner.moveState);
        }
    }

    public override void Exit()
    {
        base.Exit();

        AudioManager.ins.PlaySFX(owner.moveAudioIndex, owner.transform.position);
        owner.MoveSpeed = owner.defaultMoveSpeed + Random.Range(owner.SpeedRandomRange.x, owner.SpeedRandomRange.y);
    }
}
