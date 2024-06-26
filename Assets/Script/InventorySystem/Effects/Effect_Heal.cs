using UnityEngine;

[CreateAssetMenu(fileName = "Heal Effect", menuName = "Data/Effect/Heal")]
public class Effect_Heal : ItemEffect
{
    [Range(0, 1)]
    [SerializeField] private float healPercent;

    public override bool conditionCheck()
    {
        bool condition = PlayerManager.playerStat.currentHealth < PlayerManager.playerStat.getMaxHealthValue();
        if (!condition)
        {
            if (!dontShowTips)
                TipsUI.AddTip("血量已满，无需治疗！");
            return false;
        }
        return true;
    }

    public override void execute(Transform target)
    {
        base.execute(target);

        PlayerStat playerStat = PlayerManager.GetPlayer().GetComponent<PlayerStat>();

        int healAmount = Mathf.RoundToInt(playerStat.getMaxHealthValue() * healPercent);

        playerStat.IncreaseHealth(healAmount);
    }
}
