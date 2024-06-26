public class Enemy_Skeleton : Enemy
{
    protected override void Awake()
    {
        base.Awake();

        setAllState();
    }

    protected virtual void setAllState()
    {
        idleState = new EnemyIdleState(this, stateMachine, "IsIdle");
        moveState = new EnemyMoveState(this, stateMachine, "IsMove");
        battleState = new EnemyBattleState(this, stateMachine, "IsMove");
        attackState = new EnemyAttackState(this, stateMachine, "IsAttack");
        stunnedState = new EnemyStunnedState(this, stateMachine, "IsStunned");
        deadState = new EnemyDeadState(this, stateMachine, "IsDead");
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);

        target = PlayerManager.instance.player.transform;
    }

    public override void Stunned(Entity Instigator)
    {
        base.Stunned(Instigator);

        if (canStunned)
        {
            CloseCounterAttackWindow();

            stateMachine.ChangeState(stunnedState);
        }
    }

    public override void Die()
    {
        base.Die();

        EnemyDie();
    }

    public virtual void EnemyDie()
    {
        stateMachine.ChangeState(deadState);
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
    }
}
