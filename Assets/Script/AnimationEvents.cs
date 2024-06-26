using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    private Entity entity => GetComponentInParent<Entity>();
    private Player player => GetComponentInParent<Player>();
    private Enemy enemy => GetComponentInParent<Enemy>();
    private Enemy_Boss boss => GetComponentInParent<Enemy_Boss>();


    private void AnimationTrigger() => entity.GetStatMachine().GetCurrentState().OnAnimationTrigger();
    private void AttackTrigger()
    {
        entity.Attack();
    }


    private void PlayerThrowSword()
    {
        SkillManager.instance.sword.CreateSword();
    }

    private void PlayerStepDust()
    {
        player.fx.PlayDustFx();
    }


    private void OpenCounterAttackWindowTrigger()
    {
        if (enemy.GetComponent<CharacterStat>().isAlive())
            enemy.OpenCounterAttackWindow();
    }
    private void CloseCounterAttackWindowTrigger()
    {
        enemy.CloseCounterAttackWindow();
    }
}
