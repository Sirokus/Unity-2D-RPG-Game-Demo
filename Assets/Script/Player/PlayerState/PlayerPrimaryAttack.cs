using UnityEngine;

public class PlayerPrimaryAttack : PlayerState
{
    private float comboWindow => player.ComboWindow;
    private float lastAttackedTime;
    public int comboCounter;

    public PlayerPrimaryAttack(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        if (Time.time > lastAttackedTime + comboWindow)
        {
            comboCounter = 0;
        }
        player.anim.SetInteger("comboCounter", comboCounter);

        player.SetVelocity(new Vector2(player.attackMovement[comboCounter].x * player.dir, player.GetVelocity().y + player.attackMovement[comboCounter].y));

        timer = .1f;

        if (comboCounter != 0)
            AudioManager.ins.PlaySFX(2);
    }

    public override void OnAnimationTrigger()
    {
        base.OnAnimationTrigger();

        lastAttackedTime = Time.time;
        comboCounter = (comboCounter + 1) % 3;
        player.StartCoroutine("BusyFor", .15f);

        stateMachine.ChangeState(player.idleState);
    }

    public override void Update()
    {
        base.Update();

        if (timer < 0) player.SetVelocity(new Vector2(0, player.GetVelocity().y));
    }
}
