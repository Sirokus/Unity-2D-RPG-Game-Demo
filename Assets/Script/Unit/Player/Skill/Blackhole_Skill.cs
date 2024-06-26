using UnityEngine;

public class Blackhole_Skill : Skill
{
    [SerializeField] private GameObject blackHolePrefab;
    [SerializeField] private float maxSize;
    [SerializeField] private float growSpeed;
    [SerializeField] private int attackAmount;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float blackholeDuration;

    Blackhole_Skill_Controller currentBlackhole;

    public override bool CanUseSkill()
    {
        return base.CanUseSkill();
    }

    public override void UseSkill(Player player)
    {
        base.UseSkill(player);

        GameObject newBlackHole = Instantiate(blackHolePrefab, player.transform.position, Quaternion.identity);

        currentBlackhole = newBlackHole.GetComponent<Blackhole_Skill_Controller>();

        currentBlackhole.SetupBlackhole(maxSize, growSpeed, attackAmount, attackCooldown, blackholeDuration);
    }

    public bool IsBlackholeFinished()
    {
        if (currentBlackhole && currentBlackhole.playerCanExitState)
        {
            currentBlackhole = null;
            return true;
        }

        return false;
    }

    public float GetBlackholeRadius()
    {
        return maxSize / 2;
    }
}
