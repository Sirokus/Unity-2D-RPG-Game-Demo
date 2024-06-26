using UnityEngine;

public class PlayerDeadState : PlayerState
{
    private Timer deadTimer;

    public PlayerDeadState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.SetVelocity(new Vector2(0, player.GetVelocity().y));
        player.GetComponentInChildren<HealthBarUI>()?.gameObject.SetActive(false);

        deadTimer = TimerManager.addTimer(.5f, false, () =>
        {
            deadTimer = null;
            player.fx.CancelColorChange();
            player.cd.enabled = false;
            player.rb.simulated = false;
        });
    }

    public override void Update()
    {
        base.Update();

        if (deadTimer != null)
        {
            if (player.GetVelocity().magnitude > 0.5)
                deadTimer.timer = deadTimer.coolDown;
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Exit()
    {
        base.Exit();
    }

}
