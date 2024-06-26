using System;

public class FirstDeadAchi : Achievement
{
    protected override void Start()
    {
        base.Start();

        EventManager.AddListener(EventName.OnPlayerDead, OnPlayerDead);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        EventManager.RemoveListener(EventName.OnPlayerDead, OnPlayerDead);
    }

    public void OnPlayerDead(object sender, EventArgs args)
    {
        if (!isActived)
            Active();
    }
}
