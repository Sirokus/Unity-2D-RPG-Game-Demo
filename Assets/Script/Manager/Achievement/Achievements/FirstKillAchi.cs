using System;

public class FirstKillAchi : Achievement
{
    protected override void Start()
    {
        base.Start();

        EventManager.AddListener(EventName.OnPlayerKill, OnPlayerKill);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        EventManager.RemoveListener(EventName.OnPlayerInput, OnPlayerKill);
    }

    public void OnPlayerKill(object sender, EventArgs args)
    {
        PlayerKillArgs a = args as PlayerKillArgs;
        if (!isActived && a != null && a.enemyType != "Chest")
        {
            Active();
        }
    }
}
