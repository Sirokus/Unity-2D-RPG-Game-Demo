using UnityEngine;

public class PlayerJumpState : PlayerState
{
    public PlayerJumpState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.onJump.Invoke();

        Vector2 velocity = player.GetVelocity();
        velocity.y += player.JumpForce;
        player.SetVelocity(velocity);

        AudioManager.ins.PlaySFX(Random.Range(16, 17));
        player.fx.PlayDustFx();
    }

    public override void Exit()
    {
        base.Exit();

    }

    public override void Update()
    {
        base.Update();

        if (player.GetVelocity().y <= 0) stateMachine.ChangeState(player.airState);

        if (player.IsGroundDetected()) stateMachine.ChangeState(player.idleState);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        Vector2 velocity = player.GetVelocity();
        velocity.x = xInput * player.MoveSpeed * player.AirSpeedMultiple * Time.fixedDeltaTime;
        player.SetVelocity(velocity);
    }
}
