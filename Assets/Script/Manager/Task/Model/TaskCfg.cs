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
    public int chain_id;    //������ID
    public int sub_id;      //��������ID
    public string name;     //��������
    public string desc;     //����˵��
    public string type;     //�������ͣ��絽��ĳ������ȡĳ���ɱĳ�˵�
    public Dictionary<string, int> condition;    //�����ɵ����������������µ�ϸ������
    public TaskAward award;        //�����ɵĽ���
    public string open_chain;   //������ɺ���Զ���������һ������
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

    //��һ���ֵ䰴�������񣬵ڶ����ֵ䰴����������׶�
    private Dictionary<int, Dictionary<int, TaskCfgItem>> taskDic = new Dictionary<int, Dictionary<int, TaskCfgItem>>();

    //��ȡ���ã�ͨ��Json���ݹ���taskDic
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

    //��ȡ�����������ID����IDȷ��Ψһ��������
    public TaskCfgItem GetCfgItem(int chainId, int taskSubId)
    {
        if (taskDic.Count == 0)
            Init();

        if (taskDic.ContainsKey(chainId) && taskDic[chainId].ContainsKey(taskSubId))
            return taskDic[chainId][taskSubId];
        return null;
    }
}
