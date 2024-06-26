using UnityEngine;

public class PlayerCatchSwordState : PlayerState
{
    private Transform sword;

    public PlayerCatchSwordState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        sword = player.sword.transform;

        if (sword.transform.position.x > player.transform.position.x != player.isFacingRight)
        {
            player.Flip(!player.isFacingRight);
        }

        player.SetVelocity(new Vector2(player.GetVelocity().x - player.dir * player.SwordReturnForce, player.GetVelocity().y));

        player.fx.ScreenShake(player.fx.shakeSwordPower);
    }

    public override void Update()
    {
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Exit()
    {
        base.Exit();

        player.StartCoroutine("BusyFor", 0.1f);
    }

    public override void OnAnimationTrigger()
    {
        base.OnAnimationTrigger();

        stateMachine.ChangeState(player.idleState);
    }

}
