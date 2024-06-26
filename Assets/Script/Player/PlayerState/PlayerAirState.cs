using UnityEngine;

public class PlayerAirState : PlayerState
{
    public PlayerAirState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

    }

    public override void Exit()
    {
        base.Exit();

    }

    public override void Update()
    {
        base.Update();

        if (player.IsGroundDetected())
        {
            stateMachine.ChangeState(player.idleState);
            return;
        }


        if (player.IsWallDetected()) stateMachine.ChangeState(player.wallSlideState);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        Vector2 velocity = player.GetVelocity();
        velocity.x = xInput * player.MoveSpeed * player.AirSpeedMultiple * Time.fixedDeltaTime;
        player.SetVelocity(velocity);
    }
}
