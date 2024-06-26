using UnityEngine;

public class PlayerMoveState : PlayerGroundState
{
    public PlayerMoveState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();

        player.onMove.Invoke();
        AudioManager.ins.PlaySFX(14);
    }

    public override void Exit()
    {
        base.Exit();

        player.SetVelocity(new Vector2(0, player.GetVelocity().y));
        AudioManager.ins.StopSFX(14);
    }

    public override void Update()
    {
        base.Update();

        if (xInput == 0 || player.IsWallDetected()) stateMachine.ChangeState(player.idleState);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        Vector2 velocity = player.rb.velocity;
        velocity.x = xInput * player.MoveSpeed * Time.fixedDeltaTime;
        player.SetVelocity(velocity);
    }
}
