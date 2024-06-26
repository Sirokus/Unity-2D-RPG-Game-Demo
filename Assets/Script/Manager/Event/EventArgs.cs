using System;

//传入玩家所杀怪物种类
public class PlayerKillArgs : EventArgs
{
    public string enemyType;
}

//传入玩家进行的操作
public class PlayerInputArgs : EventArgs
{
    public GameAction action;
}

//传入玩家到达的路标
public class PlayerMoveToLocArgs : EventArgs
{
    public string wayPointName;
}

//传入激活的成就名称
public class AchievementActiveArgs : EventArgs
{
    public string AchiName;
}

//传入交互的物品/人物名称
public class PlayerInteractArgs : EventArgs
{
    public string interactName;
}

//传入添加的任务
public class TaskAddArgs : EventArgs
{
    public Task task;
    public bool isLoadAdded = false;
}

//传入移除的任务
public class TaskRemoveArgs : EventArgs
{
    public Task task;
}

//传入完成的任务
public class TaskCompleteArgs : EventArgs
{
    public Task task;
    public bool isFirstComplete;
}