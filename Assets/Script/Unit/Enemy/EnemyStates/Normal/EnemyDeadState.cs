﻿using UnityEngine;

public class EnemyDeadState : EnemyState
{
    private Timer deadTimer;
    public EnemyDeadState(Enemy owner, EnemyStateMachine stateMachine, string AnimName) : base(owner, stateMachine, AnimName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        owner.SetVelocity(new Vector2(0, owner.GetVelocity().y));
        owner.GetComponentInChildren<HealthBarUI>()?.gameObject.SetActive(false);

        deadTimer = TimerManager.addTimer(.5f, false, () =>
        {
            deadTimer = null;
            owner.fx.CancelColorChange();
            owner.cd.enabled = false;
            owner.rb.simulated = false;
        });
    }

    public override void Update()
    {
        base.Update();

        if (deadTimer != null)
        {
            if (owner.GetVelocity().magnitude > 0.5)
                deadTimer.timer = deadTimer.coolDown;
        }
    }
}
