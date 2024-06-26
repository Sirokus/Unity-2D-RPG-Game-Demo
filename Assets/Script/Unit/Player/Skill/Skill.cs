using UnityEngine;

public class Skill : MonoBehaviour
{
    public string SkillName;
    public Sprite SkillIcon;
    [SerializeField] public float coolDown;
    protected float coolDownTimer;

    protected Player player;

    public SkillTreeNodeUI unlockUI;
    public bool unlock;

    protected bool addCoolDownUIWhenUseSkill = true;

    protected virtual void Start()
    {
        player = PlayerManager.instance.player;
        unlockUI.onLockChanged += (bool val) => { unlock = val; };
    }

    protected virtual void Update()
    {
        coolDownTimer -= Time.deltaTime;
    }

    public virtual bool CanUseSkill()
    {
        if (!unlock)
            return false;

        if (coolDownTimer <= 0)
        {
            coolDownTimer = coolDown;
            return true;
        }

        TipsUI.AddTip(SkillName + " 技能尚在冷却阶段！");

        return false;
    }

    public virtual void UseSkill(Player player)
    {
        if (!unlock)
            return;
        if (addCoolDownUIWhenUseSkill)
            PlayerUI.AddCooldownUI(SkillIcon, SkillName + " 技能冷却", coolDown);
    }

    protected virtual Transform FindClosetEnemy(Transform _checkTransform, float _TraceEnemyRadius = 1.5f)
    {
        float MinDistance = _TraceEnemyRadius;
        Transform MinEnemy = null;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(_checkTransform.position, _TraceEnemyRadius);
        foreach (var hit in colliders)
        {
            if (hit.TryGetComponent(out Enemy enemy))
            {
                if (!enemy.GetComponent<CharacterStat>().isAlive())
                    continue;

                float TempDistance = Vector2.Distance(enemy.transform.position, _checkTransform.position);
                if (TempDistance < MinDistance)
                {
                    MinDistance = TempDistance;
                    MinEnemy = enemy.transform;
                }
            }
        }

        return MinEnemy;
    }
}
