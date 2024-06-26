using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class SerTask
{
    public int chainId, subId;
    public Dictionary<string, pair<int, int>> condition;
    public bool isFirstComplete;
}

public class Task
{
    public int chainId, subId;

    public bool isFirstComplete = true;

    public Dictionary<string, pair<int, int>> condition;
    public float progress
    {
        get
        {
            int max = 0, cur = 0;
            foreach (var (_, pair) in condition)
            {
                max += pair.key;
                cur += pair.value;
            }
            return cur / max;
        }
    }
    public bool checkSingleConditionComplete(string name)
    {
        return condition[name].value >= condition[name].key;
    }
    public virtual bool isComplete()
    {
        return progress == 1;
    }

    public Action<Task> onUpdate, onComplete;

    public virtual void Init(int chainId, int subId, Dictionary<string, pair<int, int>> condition)
    {
        this.chainId = chainId;
        this.subId = subId;
        this.condition = condition;
    }

    public void Init(int chainId, int subId, Dictionary<string, int> condition)
    {
        //通过配置文件的集合构造初始化任务集合存入Task
        Dictionary<string, pair<int, int>> dic = new Dictionary<string, pair<int, int>>();
        foreach (var item in condition)
        {
            dic.Add(item.Key, new pair<int, int>(item.Value, 0));
        }

        Init(chainId, subId, dic);
    }

    public virtual List<Tuple<string, int, int>> GetTaskTargetInfo()
    {
        List<Tuple<string, int, int>> res = new List<Tuple<string, int, int>>();
        foreach (var item in condition)
        {
            res.Add(new Tuple<string, int, int>(getMappingName(item.Key), item.Value.value, item.Value.key));
        }
        return res;
    }

    public virtual string getMappingName(string name)
    {
        return name;
    }

    protected virtual void Complete()
    {
        this.TriggerEvent(EventName.OnTaskComplete, new TaskCompleteArgs { task = this, isFirstComplete = isFirstComplete });
        onComplete?.Invoke(this);

        isFirstComplete = false;
    }
}

public class Task_Kill : Task
{
    public override void Init(int chainId, int subId, Dictionary<string, pair<int, int>> conditions)
    {
        base.Init(chainId, subId, conditions);

        if (!isFirstComplete)
        {
            Complete();
            return;
        }

        TaskCfgItem cfg = TaskCfg.ins.GetCfgItem(chainId, subId);

        EventManager.AddListener(EventName.OnPlayerKill, onPlayerKill);
    }

    public void onPlayerKill(object sender, EventArgs _args)
    {
        PlayerKillArgs args = _args as PlayerKillArgs;

        if (args != null && condition.ContainsKey(args.enemyType) && !checkSingleConditionComplete(args.enemyType))
        {
            condition[args.enemyType].value++;
            onUpdate?.Invoke(this);

            if (isComplete())
            {
                EventManager.RemoveListener(EventName.OnPlayerKill, onPlayerKill);
                Complete();
            }
        }
    }

    public override string getMappingName(string name)
    {
        switch (name)
        {
        case "Skeleton":
            return "骷髅";
        case "Archer":
            return "弓箭手";
        case "Chest":
            return "宝箱";
        case "Slime":
            return "史莱姆";
        default:
            return name;
        }
    }
}

public class Task_MoveToLoc : Task
{
    public override void Init(int chainId, int subId, Dictionary<string, pair<int, int>> conditions)
    {
        base.Init(chainId, subId, conditions);

        if (!isFirstComplete)
        {
            Complete();
            return;
        }

        WayPointManager.ResetWayPoint(condition.First().Key);
        EventManager.AddListener(EventName.OnPlayerMoveToLoc, OnPlayerMoveToLoc);
    }

    public void OnPlayerMoveToLoc(object sender, EventArgs _args)
    {
        PlayerMoveToLocArgs args = _args as PlayerMoveToLocArgs;
        if (args !=null && args.wayPointName == condition.First().Key)
        {
            condition[args.wayPointName].value++;
            EventManager.RemoveListener(EventName.OnPlayerMoveToLoc, OnPlayerMoveToLoc);
            Complete();
        }
    }

    public override List<Tuple<string, int, int>> GetTaskTargetInfo()
    {
        return new List<Tuple<string, int, int>> {
            new Tuple<string, int, int>("到达 - " + condition.First().Key, condition.First().Value.value, condition.First().Value.key) };
    }
}

public class Task_GetItem : Task
{
    public override void Init(int chainId, int subId, Dictionary<string, pair<int, int>> conditions)
    {
        base.Init(chainId, subId, conditions);

        if (!isFirstComplete)
        {
            Complete();
            return;
        }

        Inventory.instance.onItemEnter += OnGetItem;
    }

    public void OnGetItem(InventoryItem item)
    {
        if (condition.ContainsKey(item.data.itemID) && !checkSingleConditionComplete(item.data.itemID))
        {
            condition[item.data.itemID].value++;
            onUpdate?.Invoke(this);

            if (isComplete())
            {
                Inventory.instance.onItemEnter -= OnGetItem;
                Complete();
            }
        }
    }

    public override string getMappingName(string name)
    {
        int itemId = int.Parse(name);
        return Inventory.GetItemDataByID(itemId).itemName;
    }
}

public class Task_Wait : Task
{
    Timer timer;

    public override void Init(int chainId, int subId, Dictionary<string, pair<int, int>> conditions)
    {
        base.Init(chainId, subId, conditions);

        if (isFirstComplete)
        {
            float seconds = float.Parse(condition.First().Key);
            timer = TimerManager.addTimer(seconds, false, () =>
            {
                condition.First().Value.value++;
                Complete();
            });
        }
        else
            Complete();
    }

    public override List<Tuple<string, int, int>> GetTaskTargetInfo()
    {
        TaskCfgItem cfg = TaskCfg.ins.GetCfgItem(chainId, subId);
        return new List<Tuple<string, int, int>> {
            new Tuple<string, int, int>("等待 " + cfg.condition.First().Key + " 秒", condition.First().Value.value, condition.First().Value.key)};
    }
}

public class Task_Interact : Task
{
    public string interactName;
    public override void Init(int chainId, int subId, Dictionary<string, pair<int, int>> conditions)
    {
        base.Init(chainId, subId, conditions);

        if (!isFirstComplete)
        {
            Complete();
            return;
        }

        interactName = condition.First().Key;
        EventManager.AddListener(EventName.OnPlayerInteract, OnPlayerInteract);
    }

    public void OnPlayerInteract(object sender, EventArgs _args)
    {
        PlayerInteractArgs args = _args as PlayerInteractArgs;
        if (args != null && args.interactName == interactName)
        {
            Complete();
        }
    }

    public override List<Tuple<string, int, int>> GetTaskTargetInfo()
    {
        return new List<Tuple<string, int, int>> {
            new Tuple<string, int, int>("互动 - " + interactName, condition.First().Value.value, condition.First().Value.key) };
    }
}