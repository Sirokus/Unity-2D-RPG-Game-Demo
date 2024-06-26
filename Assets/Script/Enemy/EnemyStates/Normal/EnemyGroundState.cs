public class EnemyGroundState : EnemyState
{
    public EnemyGroundState(Enemy owner, EnemyStateMachine stateMachine, string AnimName) : base(owner, stateMachine, AnimName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();

        if (owner.target.TryGetComponent(out CharacterStat stat))
        {
            if (!stat.isAlive())
                return;
        }

        if (owner.TargetInAttackRange() && owner.CanAttack())
        {
            stateMachine.ChangeState(owner.attackState);
            return;
        }

        if (owner.IsGroundDetected() && (owner.SightCastTarget() || owner.TargetInMinFindDistance()) && !owner.TargetInAttackRange())
        {
            stateMachine.ChangeState(owner.battleState);
            return;
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Exit()
    {
        base.Exit();
    }
}

