using UnityEngine;
using UnityEngine.Events;

public class Player : Entity
{
    #region Attribute
    [Header("Movement")]
    public float MoveSpeed = 400;
    public float JumpForce = 16;
    public float AirSpeedMultiple = 1;
    private float defaultMoveSpeed;
    private float defaultJumpForce;
    private float defaultDashSpeedMultiple;

    [Header("Dash")]
    public float DashDuration = 0.2f;
    public float DashSpeedMultiple = 2;
    public float DashShadowSpawnRate = 0.1f;

    [Header("Battle")]
    public float ComboWindow = 1;
    public Vector2[] attackMovement;
    public float counterAttackDuration = 0.7f;

    public GameObject sword { get; private set; }
    public float SwordReturnForce = 3;
    #endregion

    [Header("Fx")]
    public new PlayerFX fx;

    [Header("InteractCheck")]
    public Transform InteractCheck;
    public float InteractCheckRadius;

    #region States
    public PlayerStateMachine stateMachine { get; private set; }
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerAirState airState { get; private set; }
    public PlayerGroundState groundState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerWallSlideState wallSlideState { get; private set; }

    public PlayerPrimaryAttack primaryAttack { get; private set; }
    public PlayerCounterAttackState counterAttackState { get; private set; }

    public PlayerAimSwordState aimSwordState { get; private set; }
    public PlayerCatchSwordState catchSwordState { get; private set; }
    public PlayerBlackholeState blackholeState { get; private set; }
    public PlayerDeadState deadState { get; private set; }
    #endregion

    #region Events
    public UnityEvent onMove;
    public UnityEvent onJump;
    public UnityEvent onDash;
    public UnityEvent onInteract;
    #endregion

    protected override void Awake()
    {
        base.Awake();

        fx = GetComponent<PlayerFX>();

        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, stateMachine, "IsIdle");
        moveState = new PlayerMoveState(this, stateMachine, "IsMove");
        jumpState = new PlayerJumpState(this, stateMachine, "IsJump");
        airState = new PlayerAirState(this, stateMachine, "IsJump");
        dashState = new PlayerDashState(this, stateMachine, "IsDash");
        wallSlideState = new PlayerWallSlideState(this, stateMachine, "IsWallSlide");

        primaryAttack = new PlayerPrimaryAttack(this, stateMachine, "IsAttack");
        counterAttackState = new PlayerCounterAttackState(this, stateMachine, "IsCounterAttack");

        aimSwordState = new PlayerAimSwordState(this, stateMachine, "IsAimSword");
        catchSwordState = new PlayerCatchSwordState(this, stateMachine, "IsCatchSword");

        blackholeState = new PlayerBlackholeState(this, stateMachine, "IsJump");

        deadState = new PlayerDeadState(this, stateMachine, "IsDead");
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);

        defaultMoveSpeed = MoveSpeed;
        defaultJumpForce = JumpForce;
        defaultDashSpeedMultiple = DashSpeedMultiple;
    }

    protected override void Update()
    {
        base.Update();

        stateMachine.currentState.Update();

        float xInput = KeyMgr.getAxisX();
        if (xInput != 0 && stateMachine.currentState != deadState) Flip(xInput > 0);

        if (Input.GetKeyDown(KeyMgr.get(GameAction.Dash)) && SkillManager.instance.dash.CanUseSkill() && stateMachine.currentState != deadState)
        {
            onDash.Invoke();
            SkillManager.instance.dash.UseSkill(this);
        }

        if (Input.GetKeyDown(KeyMgr.get(GameAction.Crystal)))
        {
            SkillManager.instance.crystal.UseSkill(this);
        }

        if (Input.GetKeyDown(KeyMgr.get(GameAction.FlaskUse)))
        {
            Inventory.instance.UseFlask();
        }

        if (KeyMgr.getKeyDown(GameAction.Interact))
        {
            onInteract.Invoke();
            Interact();
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        stateMachine.currentState.FixedUpdate();
    }

    public void AssignNewSword(GameObject _newSword)
    {
        sword = _newSword;
    }

    public void CatchSword()
    {
        stateMachine.ChangeState(catchSwordState);

        Destroy(sword);
    }

    public override bool IsWallDetected() => Physics2D.Raycast(WallCheck.position, Vector2.right * dir, WallCheckDistance, GroundMask);

    public override void Die()
    {
        base.Die();

        stateMachine.ChangeState(deadState);
    }

    Timer returnTimer;
    public override void SlowEntityBy(float slowPercentage, float slowDuration)
    {
        base.SlowEntityBy(slowPercentage, slowDuration);

        float remainPercentage = 1 - slowPercentage;
        MoveSpeed *= remainPercentage;
        JumpForce *= remainPercentage;
        DashSpeedMultiple *= remainPercentage;
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
        JumpForce = defaultJumpForce;
        DashSpeedMultiple = defaultDashSpeedMultiple;
    }

    public void setShowHealthBarUI(bool bShow)
    {
        fx.healthBarUI.SetActive(bShow);
    }

    public override void Attack()
    {
        base.Attack();

        if (primaryAttack.comboCounter == 0)
            AudioManager.ins.PlaySFX(2);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(AttackCheck.position, AttackCheckRadius);

        EnemyStat closestEnemy = null;
        float minDis = float.MaxValue;
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>())
            {
                EnemyStat target = hit.GetComponent<EnemyStat>();
                if (!target.isAlive())
                    continue;

                if (minDis > Vector2.Distance(hit.transform.position, transform.position))
                {
                    minDis = Vector2.Distance(hit.transform.position, transform.position);
                    closestEnemy = target;
                }
            }
        }

        if (closestEnemy)
        {
            Inventory.GetEquipmentByType(EquipmentType.Weapon)?.executeEffects(closestEnemy.transform);
            stats.DoDamage(closestEnemy);
            fx.ScreenShake(fx.shakeHighDamage);
        }
    }

    public virtual void Interact()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(AttackCheck.position, AttackCheckRadius);

        foreach (var hit in colliders)
        {
            if (hit.TryGetComponent(out IInteractable interactObj))
            {
                interactObj.Interact();
                this.TriggerEvent(EventName.OnPlayerInteract, new PlayerInteractArgs { interactName = interactObj.interactName });
                return;
            }
        }
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.DrawWireSphere(InteractCheck.position, InteractCheckRadius);
    }

    public override OldStateMachine GetStatMachine()
    {
        return stateMachine;
    }

    public override void SaveData(GameData data)
    {
        base.SaveData(data);

        if (stateMachine.currentState == idleState)
            data.addValue(id, gameObject.transform.position);
    }
}
