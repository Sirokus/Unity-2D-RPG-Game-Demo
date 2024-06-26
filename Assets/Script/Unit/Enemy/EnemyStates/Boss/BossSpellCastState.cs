using UnityEngine;

public class BossSpellCastState : EnemyState
{
    public BossSpellCastState(Enemy owner, EnemyStateMachine stateMachine, string AnimName) : base(owner, stateMachine, AnimName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        Enemy_Boss boss = owner as Enemy_Boss;
        boss.RandomSpellAttruibute();

        timer = boss.spellNum * boss.spellSpawnCooldown + 1f;
    }

    public override void Exit()
    {
        base.Exit();

        (owner as Enemy_Boss).lastTimeCast = Time.time;

    }

    public override void Update()
    {
        base.Update();

        if (timer < 0)
            stateMachine.ChangeState((owner as Enemy_Boss).teleportState);
    }

    public override void OnAnimationTrigger()
    {
        base.OnAnimationTrigger();

        Enemy_Boss boss = owner as Enemy_Boss;

        Timer spellTimer = null;
        TimerManager.addTimer(boss.spellSpawnCooldown, true, () =>
        {
            if (!boss.stats.isAlive())
            {
                TimerManager.clearTimer(spellTimer);
                return;
            }

            boss.CastSpell();
        }, 0, boss.spellNum);
    }
}
