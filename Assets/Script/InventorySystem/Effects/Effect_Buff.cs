using UnityEngine;

public enum EStat
{
    none,
    strength,
    agility,
    intelligence,
    vitality,

    maxHealth,
    armor,
    evasion,

    damage,
    critChance,
    critPower,

    fireDamage,
    iceDamage,
    lightningDamage,
    magicResistance,

    isIgnited,
    isChilled,
    isShocked
}

[CreateAssetMenu(fileName = "buff Effect", menuName = "Data/Effect/Buff")]
public class Effect_Buff : ItemEffect
{
    [SerializeField] private Sprite buffIcon;
    [SerializeField] private string buffName;
    [SerializeField] private EStat stat;
    [SerializeField] private int buffAmount;
    [SerializeField] private float buffDuration;

    public override void execute(Transform target)
    {
        base.execute(target);

        PlayerStat playerStat = PlayerManager.playerStat;

        Stat targetStat = playerStat.getStatByType(stat);
        if (targetStat == null)
        {
            Debug.Log("Buff's attribute " + stat.ToString() + " was not found!");
            return;
        }

        PlayerUI.AddBuffUI(buffIcon, buffName, buffDuration, buffAmount);
        playerStat.IncreaseStatBy(targetStat, buffAmount, buffDuration);
    }
}
