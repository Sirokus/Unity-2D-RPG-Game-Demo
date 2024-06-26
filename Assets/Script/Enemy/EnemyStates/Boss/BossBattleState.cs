using UnityEngine;

public class BossBattleState : EnemyState
{
    private int moveDir;

    public BossBattleState(Enemy owner, EnemyStateMachine stateMachine, string AnimName) : base(owner, stateMachine, AnimName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        timer = owner.battleTime;
    }

    public override void Update()
    {
        base.Update();

        //在找的到目标时持续保持定时器数值为正
        if (owner.SightCastTarget())
        {
            timer = owner.battleTime;
        }

        CharacterStat stat = owner.target.GetComponent<CharacterStat>();
        //目标合法性及可达性检测
        if (!owner.target || !owner.IsGroundDetected() || timer < 0 || Vector2.Distance(owner.target.position, owner.transform.position) > owner.GiveUpDistance || !stat.isAlive())
        {
            stateMachine.ChangeState(owner.idleState);
            return;
        }

        //持续跟随目标方向
        moveDir = owner.target.position.x > owner.transform.position.x ? 1 : -1;
        owner.Flip(moveDir > 0);

        //检测是否达到攻击要求
        if (owner.TargetInAttackRange())
        {
            owner.SetVelocity(Vector2.zero);
            stateMachine.ChangeState(owner.idleState);
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        owner.SetVelocity(new Vector2(owner.MoveSpeed * moveDir * Time.deltaTime, owner.GetVelocity().y));
    }

    public override void Exit()
    {
        base.Exit();
    }
}
