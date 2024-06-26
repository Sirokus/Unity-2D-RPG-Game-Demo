using System;
using System.Collections.Generic;

public class TaskDataHandler
{
    private List<Task> m_taskDatas;     //存储该任务链上的所有任务
    public List<Task> taskDatas
    {
        get => m_taskDatas;
    }

    public TaskDataHandler()
    {
        m_taskDatas = new List<Task>();
    }

    //根据输入的TaskData对本对象中的TaskDatas进行增加或更新
    public void AddData(Task taskData, bool isLoadAdded = false)
    {
        //遍历本TaskData的数组，寻找对应的item，存在则退出
        for (int i = 0, cnt = m_taskDatas.Count; i < cnt; ++i)
        {
            Task item = m_taskDatas[i];
            if (taskData.chainId == item.chainId && taskData.subId == item.subId)
            {
                return;
            }
        }

        //如果没有替换过说明不存在，则添加为新的
        m_taskDatas.Add(taskData);
        this.TriggerEvent(EventName.OnTaskAdd, new TaskAddArgs { task = taskData, isLoadAdded = isLoadAdded });

        //对列表进行排序，保证整个列表有序
        m_taskDatas.Sort((a, b) => a.chainId.CompareTo(b.chainId));
    }

    //按照链ID和子ID获取任务对象
    public Task GetData(int chainId, int subId)
    {
        //遍历整个列表，寻找对应的任务对象
        for (int i = 0, cnt = m_taskDatas.Count; i < cnt; ++i)
        {
            var item = m_taskDatas[i];
            if (chainId == item.chainId && subId == item.subId)
                return item;
        }
        return null;
    }

    //按链ID和子ID删除任务对象
    public void RemoveData(int chainId, int subId)
    {
        //遍历整个列表，寻找对应的任务对象，进行删除并保存
        for (int i = 0, cnt = m_taskDatas.Count; i < cnt; ++i)
        {
            var item = m_taskDatas[i];
            if (chainId == item.chainId && subId == item.subId)
            {
                m_taskDatas.Remove(item);
                this.TriggerEvent(EventName.OnTaskRemove, new TaskRemoveArgs { task = item });
                return;
            }
        }
    }

    public void LoadData(GameData data)
    {
        foreach (var task in data.taskDataList)
        {
            //通过反射实例化具体的任务类
            string type = TaskCfg.ins.GetCfgItem(task.chainId, task.subId).type;
            Task newTask = Activator.CreateInstance(Type.GetType(type)) as Task;
            newTask.isFirstComplete = task.isFirstComplete;
            newTask.Init(task.chainId, task.subId, task.condition);
            AddData(newTask, true);
        }

        //if(data.taskDataList.Count <= 0)
        //{
        //    TaskManager.ins.AddTask(1, 1);
        //}    
    }

    public void SaveData(GameData data)
    {
        data.taskDataList.Clear();
        foreach (Task task in taskDatas)
        {
            SerTask savedTask = new SerTask();
            savedTask.chainId = task.chainId;
            savedTask.subId = task.subId;
            savedTask.condition = task.condition;
            savedTask.isFirstComplete = task.isFirstComplete;
            data.taskDataList.Add(savedTask);
        }
    }
}