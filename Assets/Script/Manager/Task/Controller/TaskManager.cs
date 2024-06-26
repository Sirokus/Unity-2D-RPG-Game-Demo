using System;
using UnityEngine;

public class TaskManager : MonoBehaviour, ISaveManager
{
    public static TaskManager ins;

    private TaskDataHandler taskDataHandler = new TaskDataHandler();

    private void Awake()
    {
        if (ins != null)
        {
            Destroy(this);
            return;
        }
        ins = this;

        //初始化任务配置表
        TaskCfg.ins.Init();
    }

    public void AddTask(int chainId, int subId)
    {
        TaskCfgItem taskCfg = TaskCfg.ins.GetCfgItem(chainId, subId);
        if (taskCfg != null)
        {
            Task task = Activator.CreateInstance(Type.GetType(taskCfg.type)) as Task;
            task.Init(chainId, subId, taskCfg.condition);
            taskDataHandler.AddData(task);
        }
    }

    public void GetAward(int chainId, int subId, Action<int, TaskAward> cb)
    {
        var data = taskDataHandler.GetData(chainId, subId);
        if (data == null)
        {
            cb(-1, null);          //指定的ID无数据
            return;
        }

        if (data.progress == 1)          //可获取
        {
            taskDataHandler.AddData(data);
            GoNext(chainId, subId);
            var cfg = TaskCfg.ins.GetCfgItem(data.chainId, data.subId);
            cb(0, cfg.award);
        }
        else
        {
            cb(-2, null);     //指定的ID已领取过物品
        }
    }

    private void GoNext(int chainId, int subId)
    {
        var data = taskDataHandler.GetData(chainId, subId);
        var cfg = TaskCfg.ins.GetCfgItem(data.chainId, data.subId);
        var nextCfg = TaskCfg.ins.GetCfgItem(data.chainId, data.subId + 1);

        //删除该任务
        taskDataHandler.RemoveData(chainId, subId);

        //开启下一子任务
        if (nextCfg != null)
        {
            AddTask(nextCfg.chain_id, nextCfg.sub_id);
        }

        //开启支线任务
        if (!string.IsNullOrEmpty(cfg.open_chain))
        {
            string[] chains = cfg.open_chain.Split(',');
            for (int i = 0, cnt = chains.Length; i < cnt; ++i)
            {
                string[] subTaskIDs = chains[i].Split("|");
                int subChainId = int.Parse(subTaskIDs[0]);
                int subSubId = int.Parse(subTaskIDs[1]);

                TaskCfgItem subTaskCfg = TaskCfg.ins.GetCfgItem(subChainId, subSubId);
                AddTask(subTaskCfg.chain_id, subTaskCfg.sub_id);
            }
        }
    }

    public void LoadData(GameData data)
    {
        taskDataHandler.LoadData(data);
    }

    public void SaveData(GameData data)
    {
        taskDataHandler.SaveData(data);
    }
}
