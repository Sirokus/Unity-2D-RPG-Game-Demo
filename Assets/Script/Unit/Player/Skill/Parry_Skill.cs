using UnityEngine;

public class Parry_Skill : Skill
{
    public float restoreHealthPercentage = 0.05f;

    [Header("Skill Tree")]
    [SerializeField] private SkillTreeNodeUI restoreUnlockBtn;
    [SerializeField] private SkillTreeNodeUI withMirageUnlockBtn;
    bool restoreUnlock;

    protected override void Start()
    {
        base.Start();

        restoreUnlockBtn.onLockChanged += (bool val) => restoreUnlock = val;
        withMirageUnlockBtn.onLockChanged += (bool val) => SkillManager.instance.clone.canCreateCloneOnCounterAttack = val;

        addCoolDownUIWhenUseSkill = false;
    }

    public override void UseSkill(Player player)
    {
        base.UseSkill(player);

        if (!unlock)
            return;

        if (restoreUnlock)
            PlayerManager.playerStat.IncreaseHealth(Mathf.RoundToInt(PlayerManager.playerStat.getMaxHealthValue() * restoreHealthPercentage));
    }
}