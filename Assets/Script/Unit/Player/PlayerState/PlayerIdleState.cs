using UnityEngine;

public class PlayerIdleState : PlayerGroundState
{

    public PlayerIdleState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();

        Vector2 velocity = player.GetVelocity();
        velocity.x = 0;
        player.SetVelocity(velocity);
    }

    public override void Exit()
    {
        base.Exit();

    }

    public override void Update()
    {
        base.Update();

        if (xInput != 0 && !player.IsWallDetected() && !player.isBusy)
        {
            player.TriggerEvent(EventName.OnPlayerInput, new PlayerInputArgs { action = xInput > 0 ? GameAction.MoveRight : GameAction.MoveLeft });
            stateMachine.ChangeState(player.moveState);
        }
    }
}
