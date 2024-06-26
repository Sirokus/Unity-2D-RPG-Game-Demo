using UnityEngine;

public class PlayerState : OldState
{
    [Header("StateBase")]
    protected PlayerStateMachine stateMachine;
    protected Player player;
    private string animBoolName;
    protected float xInput;

    [Header("StateTimer")]
    protected float timer = 0;
    protected float coolDown = 1;

    public PlayerState(Player player, PlayerStateMachine stateMachine, string animBoolName)
    {
        this.player = player;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
    }

    public override void Enter()
    {
        player.anim.SetBool(animBoolName, true);
    }

    public override void Update()
    {
        xInput = KeyMgr.getAxisX();
        timer -= Time.deltaTime;

        player.anim.SetFloat("yVelocity", player.GetVelocity().y);
        player.anim.SetBool("IsGround", player.IsGroundDetected());
    }

    public override void Exit()
    {
        player.anim.SetBool(animBoolName, false);
    }
}
