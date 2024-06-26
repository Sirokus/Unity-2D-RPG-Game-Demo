using UnityEngine;

public class PlayerGroundState : PlayerState
{
    public PlayerGroundState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        bool isGround = player.IsGroundDetected();

        if (!isGround)
        {
            stateMachine.ChangeState(player.airState);
        }

        if (Input.GetKeyDown(KeyMgr.get(GameAction.Jump)) && isGround)
        {
            player.TriggerEvent(EventName.OnPlayerInput, new PlayerInputArgs { action = GameAction.Jump });
            stateMachine.ChangeState(player.jumpState);
        }

        if (Input.GetKey(KeyMgr.get(GameAction.Attack)) && !player.isBusy && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            stateMachine.ChangeState(player.primaryAttack);
        }

        if (Input.GetKeyDown(KeyMgr.get(GameAction.Counter)) && SkillManager.instance.parry.CanUseSkill())
        {
            stateMachine.ChangeState(player.counterAttackState);
        }

        if (Input.GetKeyDown(KeyMgr.get(GameAction.Sword)) && HasNoSword() && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            stateMachine.ChangeState(player.aimSwordState);
        }

        if (Input.GetKeyDown(KeyMgr.get(GameAction.BlackHole)))
        {
            stateMachine.ChangeState(player.blackholeState);
        }
    }

    private bool HasNoSword()
    {
        if (!player.sword) return true;
        player.sword.GetComponent<Sword_Skill_Controller>().ReturnSword();
        return false;
    }
}
