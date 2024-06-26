using System;
using System.Collections.Generic;

public class TaskDataHandler
{
    private List<Task> m_taskDatas;     //�洢���������ϵ���������
    public List<Task> taskDatas
    {
        get => m_taskDatas;
    }

    public TaskDataHandler()
    {
        m_taskDatas = new List<Task>();
    }

    //���������TaskData�Ա������е�TaskDatas�������ӻ����
    public void AddData(Task taskData, bool isLoadAdded = false)
    {
        //������TaskData�����飬Ѱ�Ҷ�Ӧ��item���������˳�
        for (int i = 0, cnt = m_taskDatas.Count; i < cnt; ++i)
        {
            Task item = m_taskDatas[i];
            if (taskData.chainId == item.chainId && taskData.subId == item.subId)
            {
                return;
            }
        }

        //���û���滻��˵�������ڣ������Ϊ�µ�
        m_taskDatas.Add(taskData);
        this.TriggerEvent(EventName.OnTaskAdd, new TaskAddArgs { task = taskData, isLoadAdded = isLoadAdded });

        //���б�������򣬱�֤�����б�����
        m_taskDatas.Sort((a, b) => a.chainId.CompareTo(b.chainId));
    }

    //������ID����ID��ȡ�������
    public Task GetData(int chainId, int subId)
    {
        //���������б�Ѱ�Ҷ�Ӧ���������
        for (int i = 0, cnt = m_taskDatas.Count; i < cnt; ++i)
        {
            var item = m_taskDatas[i];
            if (chainId == item.chainId && subId == item.subId)
                return item;
        }
        return null;
    }

    //����ID����IDɾ���������
    public void RemoveData(int chainId, int subId)
    {
        //���������б�Ѱ�Ҷ�Ӧ��������󣬽���ɾ��������
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
            //ͨ������ʵ���������������
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