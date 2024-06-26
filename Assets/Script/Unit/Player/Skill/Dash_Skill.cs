using UnityEngine;

public class Dash_Skill : Skill
{
    [Header("Skill Tree")]
    public SkillTreeNodeUI cloneOnDashUnlockedUI;
    public SkillTreeNodeUI cloneOnArrivalUnlockedUI;

    protected override void Start()
    {
        base.Start();

        cloneOnDashUnlockedUI.onLockChanged += (bool val) => { SkillManager.instance.clone.canCreateCloneOnDashStart = val; };
        cloneOnArrivalUnlockedUI.onLockChanged += (bool val) => { SkillManager.instance.clone.canCreateCloneOnDashEnd = val; };
    }

    public override void UseSkill(Player player)
    {
        base.UseSkill(player);

        if (!unlock)
            return;

        if (player.stateMachine.currentState == player.wallSlideState)
        {
            player.Flip(!player.isFacingRight);
        }
        else if (player.IsWallDetected())
            return;

        player.stateMachine.ChangeState(player.dashState);
    }
}
