using System;

public class FirstMoveAchi : Achievement
{
    protected override void Start()
    {
        base.Start();

        EventManager.AddListener(EventName.OnPlayerInput, OnPlayerInput);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();

        EventManager.RemoveListener(EventName.OnPlayerInput, OnPlayerInput);
    }


    public void OnPlayerInput(object sender, EventArgs args)
    {
        PlayerInputArgs a = args as PlayerInputArgs;
        if (!isActived && a != null && (a.action == GameAction.MoveLeft || a.action == GameAction.MoveRight))
        {
            Active();
        }
    }
}
