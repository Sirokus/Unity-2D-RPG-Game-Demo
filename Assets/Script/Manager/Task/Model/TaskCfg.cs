using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TaskAward
{
    public int currency;
    public Dictionary<string, int> items;
}

public class TaskCfgItem
{
    public int chain_id;    //主任务ID
    public int sub_id;      //子链任务ID
    public string name;     //任务名称
    public string desc;     //任务说明
    public string type;     //任务类型，如到达某处，获取某物，击杀某人等
    public Dictionary<string, int> condition;    //任务达成的条件，任务类型下的细化条件
    public TaskAward award;        //任务达成的奖励
    public string open_chain;   //任务完成后可自动开启的下一个任务
}

[Serializable]
public class TaskCfgItemJsonHelper
{
    public List<TaskCfgItem> list;
}

public class TaskCfg
{
    private static TaskCfg instance;
    public static TaskCfg ins
    {
        get
        {
            if (instance == null)
                instance = new TaskCfg();
            return instance;
        }
    }

    //第一个字典按链存任务，第二个字典按子链存任务阶段
    private Dictionary<int, Dictionary<int, TaskCfgItem>> taskDic = new Dictionary<int, Dictionary<int, TaskCfgItem>>();

    //读取配置，通过Json数据构建taskDic
    public void Init()
    {
        if (taskDic.Count != 0)
            return;

        string json = Resources.Load<TextAsset>("Task/TaskConfig").text;
        List<TaskCfgItem> list = JsonConvert.DeserializeObject<TaskCfgItemJsonHelper>(json).list;

        for (int i = 0, cnt = list.Count; i< cnt; i++)
        {
            TaskCfgItem item = list[i];

            if (!taskDic.ContainsKey(item.chain_id))
            {
                taskDic[item.chain_id] = new Dictionary<int, TaskCfgItem>();
            }
            taskDic[item.chain_id].Add(item.sub_id, item);
        }
    }

    //获取配置项，根据链ID和子ID确定唯一任务配置
    public TaskCfgItem GetCfgItem(int chainId, int taskSubId)
    {
        if (taskDic.Count == 0)
            Init();

        if (taskDic.ContainsKey(chainId) && taskDic[chainId].ContainsKey(taskSubId))
            return taskDic[chainId][taskSubId];
        return null;
    }
}
