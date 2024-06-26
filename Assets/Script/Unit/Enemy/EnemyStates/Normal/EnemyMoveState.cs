using UnityEngine;

public class EnemyMoveState : EnemyGroundState
{
    public EnemyMoveState(Enemy owner, EnemyStateMachine stateMachine, string AnimName) : base(owner, stateMachine, AnimName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        timer = Random.Range(owner.moveTime.x, owner.moveTime.y);
    }

    public override void Update()
    {
        base.Update();

        if (timer < 0 || !owner.IsGroundDetected() || owner.IsWallDetected())
        {
            owner.Flip(!owner.isFacingRight);
            stateMachine.ChangeState(owner.idleState);
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        owner.SetVelocity(new Vector2(owner.MoveSpeed * owner.dir * Time.deltaTime, owner.GetVelocity().y));
    }

    public override void Exit()
    {
        base.Exit();
    }

}
