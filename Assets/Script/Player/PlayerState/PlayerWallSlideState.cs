using UnityEngine;

public class PlayerWallSlideState : PlayerState
{
    public PlayerWallSlideState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
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

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        int yInput = (int)Input.GetAxisRaw("Vertical");
        float wallSlideSpeed = yInput == -1 ? 1 : .2f;

        Vector2 velocity = new Vector2(xInput * player.MoveSpeed * Time.deltaTime, player.GetVelocity().y * wallSlideSpeed);
        player.SetVelocity(velocity);
    }

    public override void Update()
    {
        base.Update();

        if (!player.IsWallDetected() || player.IsGroundDetected())
        {
            stateMachine.ChangeState(player.airState);
        }

        if (Input.GetButtonDown("Jump"))
        {
            stateMachine.ChangeState(player.jumpState);
        }
    }
}
