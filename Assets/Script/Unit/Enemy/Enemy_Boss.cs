using UnityEngine;

public class Enemy_Boss : Enemy_Skeleton
{
    [Header("Teleport details")]
    [SerializeField] private BoxCollider2D area;
    [SerializeField] private Vector2 surroundingCheckSize;
    [SerializeField] private Vector2 surroundingCheckOffset;

    [Header("Spell Cast details")]
    [SerializeField] private GameObject spellPrefab;
    [SerializeField] public float spellSpawnCooldown;
    public int spellNum;
    [SerializeField] private float spellStateCooldown;
    public float lastTimeCast;

    public Vector2Int spellNumRandomRange = new Vector2Int(5, 15);
    public Vector2 spellSpawnCooldownRandomRange = new Vector2(0.3f, 2f);
    public Vector2 spellStateCooldownRandomRange = new Vector2(10f, 45f);

    public int ReSpawnTimes = 2;

    #region Stats
    public EnemyState teleportState;
    public EnemyState spellCastState;
    #endregion

    protected override void Start()
    {
        base.Start();
        stats.onTakeDamage.AddListener(OnTakeDamage);
    }

    protected override void setAllState()
    {
        idleState = new EnemyIdleState(this, stateMachine, "IsIdle");
        moveState = new EnemyMoveState(this, stateMachine, "IsMove");
        battleState = new BossBattleState(this, stateMachine, "IsMove");
        attackState = new EnemyAttackState(this, stateMachine, "IsAttack");
        stunnedState = new EnemyStunnedState(this, stateMachine, "IsStunned");
        deadState = new BossDeadState(this, stateMachine, "IsDead");
        teleportState = new BossTeleportState(this, stateMachine, "IsTeleport");
        spellCastState = new BossSpellCastState(this, stateMachine, "IsSpellCast");
    }

    public override void EnemyDie()
    {
        if (--ReSpawnTimes > 0)
        {
            stats.currentHealth = 0;
            stats.IncreaseHealth(stats.getMaxHealthValue());
            stateMachine.ChangeState(teleportState);
        }
        else
            stateMachine.ChangeState(deadState);
    }

    public void FindPosition()
    {
        float x = Random.Range(area.bounds.min.x + 3, area.bounds.max.x - 3);
        float y = Random.Range(area.bounds.min.y + 3, area.bounds.max.y - 3);

        transform.position = new Vector3(x, y);
        transform.position = new Vector3(transform.position.x, transform.position.y - GroundBelow().distance + ((cd as CapsuleCollider2D).size.y / 2));

        if (!GroundBelow() || SomethingIsAround())
        {
            Debug.Log("Teleport Position Cant Use, Try Find New Postion!");
            FindPosition();
        }
    }

    private RaycastHit2D GroundBelow() => Physics2D.Raycast(transform.position, Vector2.down, 100, LayerMask.GetMask("Ground"));
    private bool SomethingIsAround() => Physics2D.BoxCast((Vector2)transform.position + surroundingCheckOffset, surroundingCheckSize, 0, Vector2.zero, 0, LayerMask.GetMask("Ground"));

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - GroundBelow().distance));
        Gizmos.DrawWireCube((Vector2)transform.position + surroundingCheckOffset, surroundingCheckSize);
    }

    public void OnTakeDamage(int amount)
    {
        int maxHP = stats.getMaxHealthValue();
        int curHP = stats.currentHealth;

        if (checkHPIsLess(0.2f, curHP, maxHP, amount) || checkHPIsLess(0.4f, curHP, maxHP, amount) || checkHPIsLess(0.6f, curHP, maxHP, amount))
        {
            stateMachine.ChangeState(teleportState);
        }
    }

    public bool checkHPIsLess(float percent, int curHP, int maxHP, int amount) => curHP < maxHP * percent && curHP + amount > maxHP * percent;


    public bool CanDoSpellCast()
    {
        if (Time.time >= lastTimeCast + spellStateCooldown)
        {
            lastTimeCast = Time.time;
            return true;
        }
        return false;
    }

    public void CastSpell()
    {
        Player player = PlayerManager.GetPlayer();
        Vector3 spellPos = new Vector2(player.transform.position.x, player.transform.position.y + 1.5f) + player.GetVelocity() / 10;

        GameObject newSpell = Instantiate(spellPrefab, spellPos, Quaternion.identity);
        newSpell.GetComponent<BossSpellController>().SetupSpell(stats);
    }

    public void RandomSpellAttruibute()
    {
        spellNum = Random.Range(spellNumRandomRange.x, spellNumRandomRange.y);
        spellSpawnCooldown = Random.Range(spellSpawnCooldownRandomRange.x, spellSpawnCooldownRandomRange.y);
        spellStateCooldown = Random.Range(spellStateCooldownRandomRange.x, spellStateCooldownRandomRange.y);
    }

    public void MoveToLocation(Transform point)
    {
        transform.position = point.position;
    }

    public void OnPassAnimComplete()
    {
        this.enabled = true;
    }

    public override void LoadData(GameData data)
    {

    }
}
