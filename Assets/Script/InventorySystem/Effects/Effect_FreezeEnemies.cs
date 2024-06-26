using UnityEngine;

[CreateAssetMenu(fileName = "Heal Effect", menuName = "Data/Effect/FreezeEnemies")]
public class Effect_FreezeEnemies : ItemEffect
{
    [SerializeField] private float freezeDuration = 1f;
    [SerializeField] private float freezeRange = 5f;
    [SerializeField] private bool isUsed = false;

    public override bool conditionCheck()
    {
        PlayerStat playerStat = PlayerManager.playerStat;
        if (playerStat.currentHealth > playerStat.getMaxHealthValue() * 0.3 || isUsed)
            return false;
        return true;
    }

    public override void execute(Transform target)
    {
        base.execute(target);

        Vector3 playerPos = PlayerManager.playerPos;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(playerPos, freezeRange);
        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent(out Enemy enemy))
            {
                enemy.FreezeTime(true);
                TimerManager.addTimer(freezeDuration, false, () => { enemy.FreezeTime(false); });
            }
        }

        isUsed = true;
        TimerManager.addTimer(5f, false, () => { isUsed = false; });
    }
}
