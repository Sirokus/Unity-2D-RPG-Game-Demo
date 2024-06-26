using UnityEngine;

public class PlayerDashState : PlayerState
{
    public PlayerDashState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    Timer dashSpawnTimer;

    public override void Enter()
    {
        base.Enter();

        timer = player.DashDuration;

        SkillManager.instance.clone.CreateCloneOnDashStart();

        AudioManager.ins.PlaySFX(Random.Range(16, 17));

        PlayerManager.playerStat.isInvincible = true;

        dashSpawnTimer = TimerManager.addTimer(player.DashShadowSpawnRate, true, () =>
        {
            ObjectPool.Get<ShadowSprite_OP>();
        }, .04f);
    }

    public override void Exit()
    {
        base.Exit();

        SkillManager.instance.clone.CreateCloneOnDashEnd();

        PlayerManager.playerStat.isInvincible = false;

        TimerManager.clearTimer(dashSpawnTimer);
    }

    public override void Update()
    {
        base.Update();

        if (timer < 0)
            stateMachine.ChangeState(player.airState);

        if (player.IsWallDetected())
            stateMachine.ChangeState(player.wallSlideState);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        Vector2 velocity = player.GetVelocity();
        velocity.x = player.DashSpeedMultiple * player.MoveSpeed * player.dir * Time.deltaTime;
        velocity.y = 0;
        player.SetVelocity(velocity);
    }


}
