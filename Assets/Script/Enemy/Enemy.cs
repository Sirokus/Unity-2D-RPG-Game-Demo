using System.Collections;
using UnityEngine;

public enum EnemyType
{
    Skeleton,
    Archer,
    Chest,
    Slime,
    Boss
}

public class Enemy : Entity
{
    public EnemyType enemyType;

    [Header("Sight")]
    [SerializeField] public float SeekDistance = 7f;
    [SerializeField] public float MinFindDistance = 1f;
    [SerializeField] public float GiveUpDistance = 12f;
    [SerializeField] protected LayerMask PlayerMask;

    [Header("Movement")]
    public float MoveSpeed = 100f;
    [HideInInspector] public float defaultMoveSpeed;
    public Vector2 SpeedRandomRange = new Vector2(-10, 10);
    public Vector2 idleTime = new Vector2(0.1f, 2f);
    public Vector2 moveTime = new Vector2(5, 20);
    public float battleTime = 5;

    [Header("Battle")]
    [SerializeField] public float attackDistance = 0.5f;
    [SerializeField] public float attackCooldown = 1f;
    [HideInInspector] public float lastAttackTime;
    [HideInInspector] public Transform target;

    [Header("Stunned")]
    public float stunDuration = 0.7f;
    public Vector2 stunDirection = new Vector2(5, 5);
    [HideInInspector] public bool canStunned;
    [SerializeField] protected GameObject counterImage;

    [Header("Audio")]
    public int moveAudioIndex;
    public int attackAudioIndex;

    #region States
    public EnemyStateMachine stateMachine;
    [HideInInspector] public string lastAnimBoolName;
    public EnemyState idleState;
    public EnemyState moveState;
    public EnemyState battleState;
    public EnemyState attackState;
    public EnemyState stunnedState;
    public EnemyState deadState;
    #endregion

    protected override void Awake()
    {
        base.Awake();
        stateMachine = new EnemyStateMachine();
        defaultMoveSpeed = MoveSpeed;
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        stateMachine.currentState?.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        stateMachine.currentState?.FixedUpdate();
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Vector2 position = new Vector2(transform.position.x, transform.position.y - 0.1f);
        Gizmos.DrawLine(position, new Vector2(position.x + dir * SeekDistance, position.y));
    }

    public virtual void AssignLastAnimName(string _animBoolName)
    {
        lastAnimBoolName = _animBoolName;
    }

    public virtual void OpenCounterAttackWindow()
    {
        canStunned = true;
        counterImage.SetActive(true);
    }

    public virtual void CloseCounterAttackWindow()
    {
        canStunned = false;
        counterImage.SetActive(false);
    }

    public virtual void FreezeTime(bool _timeFrozen)
    {
        if (_timeFrozen)
        {
            MoveSpeed = 0;
            anim.speed = 0;
        }
        else
        {
            MoveSpeed = defaultMoveSpeed;
            anim.speed = 1;
        }
    }

    protected virtual IEnumerator FreeezeTimerFor(float _seconds)
    {
        FreezeTime(true);

        yield return new WaitForSeconds(_seconds);

        FreezeTime(false);
    }

    public override void Flip(bool right)
    {
        base.Flip(right);
    }

    public void Damage(Transform Instigator, float _freezeTimeDuration = 0, bool isNeedFreeze = false)
    {
        base.Damage(Instigator);

        if (_freezeTimeDuration > 0 || isNeedFreeze)
        {
            StartCoroutine("FreeezeTimerFor", _freezeTimeDuration);
        }

    }

    public virtual void Stunned(Entity Instigator)
    {
        MakeSureReverseTo(Instigator.transform);
    }

    public bool CanAttack() => Time.time > lastAttackTime + attackCooldown;
    public virtual RaycastHit2D SightCastTarget() => Physics2D.Raycast(transform.position, Vector2.right * dir, SeekDistance, PlayerMask);
    public bool TargetInAttackRange() => SightCastTarget() && SightCastTarget().distance < attackDistance;
    public bool TargetInMinFindDistance() => Physics2D.Raycast(transform.position, Vector2.left * dir, MinFindDistance, PlayerMask);


    Timer returnTimer;
    public override void SlowEntityBy(float slowPercentage, float slowDuration)
    {
        base.SlowEntityBy(slowPercentage, slowDuration);

        float remainPercentage = 1 - slowPercentage;
        MoveSpeed *= remainPercentage;
        anim.speed *= remainPercentage;

        if (returnTimer != null)
        {
            returnTimer.timer = slowDuration;
        }
        else
        {
            returnTimer = TimerManager.addTimer(slowDuration, false, () =>
            {
                ReturnDefaultSpeed();
                returnTimer = null;
            });
        }
    }

    public override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();

        MoveSpeed = defaultMoveSpeed;
    }

    public override void Die()
    {
        base.Die();
    }

    public override void Attack()
    {
        base.Attack();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(AttackCheck.position, AttackCheckRadius);

        AudioManager.ins.PlaySFX(attackAudioIndex);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Player>())
            {
                PlayerStat target = hit.GetComponent<PlayerStat>();
                if (!target.isAlive())
                    continue;

                stats.DoMagicalDamage(target);
                stats.DoDamage(target);

                return;
            }
        }
    }

    public override OldStateMachine GetStatMachine()
    {
        return stateMachine;
    }

    public override void SaveData(GameData data)
    {
        base.SaveData(data);

        if (stateMachine.currentState != deadState)
            data.addValue(id, gameObject.transform.position);
    }
}
