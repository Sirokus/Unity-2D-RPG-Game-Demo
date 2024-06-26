using UnityEngine;

public class PlayerBlackholeState : PlayerState
{
    private float flyTime = .4f;
    private bool skillUsed;

    private float defaultGravity;

    public PlayerBlackholeState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        skillUsed = false;
        timer = flyTime;

        defaultGravity =  player.rb.gravityScale;
        player.rb.gravityScale = 0;

        AudioManager.ins.PlaySFX(6);

        KeyMgr.ins.pauseInput = true;
    }

    public override void Update()
    {
        base.Update();

        if (timer < 0)
        {
            player.rb.velocity = new Vector2(0, -0.1f);

            if (!skillUsed)
            {
                SkillManager.instance.blackhole.UseSkill(player);
                skillUsed = true;
            }
        }
        else
        {
            player.rb.velocity = new Vector2(0, 15);
        }

        if (SkillManager.instance.blackhole.IsBlackholeFinished())
        {
            stateMachine.ChangeState(player.airState);
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Exit()
    {
        base.Exit();

        player.rb.gravityScale = defaultGravity;
        KeyMgr.ins.pauseInput = false;
    }

    public override void OnAnimationTrigger()
    {
        base.OnAnimationTrigger();
    }

    public override void OnAttackTrigger()
    {
        base.OnAttackTrigger();
    }
}
