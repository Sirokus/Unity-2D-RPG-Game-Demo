using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TaskUI : MonoBehaviour
{
    //TaskList
    public Transform taskSlotParent;
    public GameObject taskSlotUIPrefab;
    public List<TaskSlotUI> taskSlotUIs = new List<TaskSlotUI>();
    public TaskSlotUI curTaskSlotUI;

    public TaskInfoUI taskInfoUI;
    public GameObject taskInfoNullUI;

    public TaskRequestUI taskRequestUI;

    private void Start()
    {
        EventManager.AddListener(EventName.OnTaskAdd, OnTaskAdd);
        EventManager.AddListener(EventName.OnTaskRemove, OnTaskRemove);
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener(EventName.OnTaskAdd, OnTaskAdd);
        EventManager.RemoveListener(EventName.OnTaskRemove, OnTaskRemove);
    }

    private void OnTaskAdd(object sender, EventArgs _args)
    {
        TaskAddArgs args = _args as TaskAddArgs;
        if (args != null && args.task != null)
        {
            TaskSlotUI ui = Instantiate(taskSlotUIPrefab).GetComponent<TaskSlotUI>();
            ui.gameObject.SetActive(true);
            ui.transform.SetParent(taskSlotParent, false);
            ui.Init(args.task);
            taskSlotUIs.Add(ui);

            if (taskSlotParent.childCount == 2)
            {
                SelectTask(args.task);
            }
        }
    }

    private void OnTaskRemove(object sender, EventArgs _args)
    {
        TaskRemoveArgs args = _args as TaskRemoveArgs;
        if (args  != null && args.task != null)
        {
            foreach (var ui in taskSlotUIs)
            {
                if (ui.task == args.task)
                {
                    bool isActiveUI = ui == curTaskSlotUI;
                    Destroy(ui.gameObject);
                    taskSlotUIs.Remove(ui);

                    if (taskSlotUIs.Count == 0)
                    {
                        taskInfoUI.gameObject.SetActive(false);
                        taskInfoNullUI.SetActive(true);
                    }
                    else if (isActiveUI)
                    {
                        SelectTask(taskSlotUIs[0].task);
                    }
                    break;
                }
            }
        }
    }

    public void SelectTask(Task task)
    {
        if (curTaskSlotUI != null)
            curTaskSlotUI.Select(false);

        foreach (var ui in taskSlotUIs)
        {
            if (ui.task == task)
            {
                ui.Select(true);
                taskInfoUI.Setup(ui.task);
                taskInfoUI.gameObject.SetActive(true);
                taskInfoNullUI.SetActive(false);
                curTaskSlotUI = ui;
                break;
            }
        }
    }

    public void requestTask(int chainId, int subId)
    {
        //实例化一个task
        TaskCfgItem cfg = TaskCfg.ins.GetCfgItem(chainId, subId);
        Task task = Activator.CreateInstance(Type.GetType(cfg.type)) as Task;
        task.Init(chainId, subId, cfg.condition);

        taskRequestUI.Setup(task);
        taskRequestUI.gameObject.SetActive(true);
    }
}