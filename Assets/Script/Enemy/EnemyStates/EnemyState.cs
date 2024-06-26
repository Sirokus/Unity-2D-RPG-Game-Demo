using UnityEngine;

public class EnemyState : OldState
{
    public Enemy owner;
    public EnemyStateMachine stateMachine;
    public string AnimName;
    protected float timer;

    public EnemyState(Enemy owner, EnemyStateMachine stateMachine, string AnimName)
    {
        this.owner = owner;
        this.stateMachine = stateMachine;
        this.AnimName = AnimName;
    }

    public override void Enter()
    {
        owner.anim.SetBool(AnimName, true);
    }

    public override void Update()
    {
        timer -= Time.deltaTime;
    }

    public override void Exit()
    {
        owner.anim.SetBool(AnimName, false);
        owner.AssignLastAnimName(AnimName);
    }
}
