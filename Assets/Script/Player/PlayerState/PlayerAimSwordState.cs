using UnityEngine;

public class PlayerAimSwordState : PlayerState
{
    public PlayerAimSwordState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        SkillManager.instance.sword.SetDotsActive(true);
    }

    public override void Update()
    {
        base.Update();

        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            stateMachine.ChangeState(player.idleState);
        }

        if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x > player.transform.position.x != player.isFacingRight)
        {
            player.Flip(!player.isFacingRight);
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Exit()
    {
        base.Exit();

        player.StartCoroutine("BusyFor", 0.2f);
    }

}
