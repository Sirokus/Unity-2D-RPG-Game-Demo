public class BossTeleportState : EnemyState
{
    public float TeleportSpeed = 1f;
    public BossTeleportState(Enemy owner, EnemyStateMachine stateMachine, string AnimName) : base(owner, stateMachine, AnimName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        owner.stats.isInvincible = true;
        owner.fx.setVisibilityEffect(false);
    }

    public override void Update()
    {
        base.Update();
    }

    public override void OnAnimationTrigger()
    {
        base.OnAnimationTrigger();

        (owner as Enemy_Boss).FindPosition();
        TimerManager.addTimer(TeleportSpeed, false, () =>
        {
            TimerManager.addTimer(1f, false, () =>
            {
                if (!owner.stats.isAlive())
                    return;
                owner.fx.setVisibilityEffect(true);
                owner.fx.playParticleFX(owner.stats.isIgnited, owner.stats.isChilled, owner.stats.isShocked);

                if ((owner as Enemy_Boss).CanDoSpellCast())
                    stateMachine.ChangeState((owner as Enemy_Boss).spellCastState);
                else
                {
                    owner.stats.isInvincible = false;
                    stateMachine.ChangeState(owner.idleState);
                }
            });
        });
    }
}

