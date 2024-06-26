public static class EventName
{
    //玩家动作
    public const string OnPlayerKill = nameof(OnPlayerKill);
    public const string OnPlayerInput = nameof(OnPlayerInput);
    public const string OnPlayerMoveToLoc = nameof(OnPlayerMoveToLoc);
    public const string OnPlayerDead = nameof(OnPlayerDead);
    public const string OnPlayerInteract = nameof(OnPlayerInteract);

    //任务事件
    public const string OnTaskAdd = nameof(OnTaskAdd);
    public const string OnTaskRemove = nameof(OnTaskRemove);
    public const string OnTaskComplete = nameof(OnTaskComplete);
}