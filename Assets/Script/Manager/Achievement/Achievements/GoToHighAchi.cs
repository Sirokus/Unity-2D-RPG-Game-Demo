using System;

public class GoToHighAchi : Achievement
{
    protected override void Start()
    {
        base.Start();

        EventManager.AddListener(EventName.OnPlayerMoveToLoc, OnPlayerMoveToLoc);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        EventManager.RemoveListener(EventName.OnPlayerInput, OnPlayerMoveToLoc);
    }

    public void OnPlayerMoveToLoc(object sender, EventArgs args)
    {
        PlayerMoveToLocArgs a = args as PlayerMoveToLocArgs;
        if (!isActived && a != null && a.wayPointName == "HighstPlace")
        {
            Active();
        }
    }
}
