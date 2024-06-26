using UnityEngine;

public class PlayerCounterAttackState : PlayerState
{
    public PlayerCounterAttackState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        PlayerUI.AddCooldownUI(SkillManager.instance.parry.SkillIcon, SkillManager.instance.parry.SkillName + " ººƒ‹¿‰»¥", SkillManager.instance.parry.coolDown);

        player.SetVelocity(Vector2.zero);

        timer = player.counterAttackDuration;
        player.anim.SetBool("CounterAttackSuccess", false);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.AttackCheck.position, player.AttackCheckRadius);

        foreach (var hit in colliders)
        {
            var enemy = hit.GetComponent<Enemy>();
            if (enemy && enemy.canStunned)
            {
                timer = 99;
                player.anim.SetBool("CounterAttackSuccess", true);

                enemy.Stunned(player);

                SkillManager.instance.clone.CreateCloneOnCounterAttack();
                SkillManager.instance.parry.UseSkill(player);
            }

            if (hit.TryGetComponent(out Bullet arrow))
            {
                if (arrow.flipped)
                    continue;
                timer = 99;
                player.anim.SetBool("CounterAttackSuccess", true);
                arrow.Flip();
            }
        }
    }

    public override void Update()
    {
        base.Update();

        if (timer < 0)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }

    public override void OnAnimationTrigger()
    {
        base.OnAnimationTrigger();

        stateMachine.ChangeState(player.idleState);
    }


}
