using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskRequestUI : MonoBehaviour
{
    public TextMeshProUGUI nameTxt, infoTxt, currencyTxt;

    public Transform TaskTargetParent, TaskAwardParent;
    public GameObject taskTargetSlotPrefab, taskAwardSlotPrefab;

    //任务目标条目字典
    public Dictionary<string, TaskTargetSlotUI> taskTargets = new Dictionary<string, TaskTargetSlotUI>();
    //奖励列表
    public List<ItemSlotUI> itemUIs = new List<ItemSlotUI>();

    //提交任务按钮
    public Button taskRequestBtn;

    public void Setup(Task task)
    {
        //简介部分
        TaskCfgItem taskCfg = TaskCfg.ins.GetCfgItem(task.chainId, task.subId);
        nameTxt.text = taskCfg.name;
        infoTxt.text = taskCfg.desc;

        //任务条目部分
        //清空残留的任务条目
        foreach (var ui in taskTargets.Values)
        {
            Destroy(ui.gameObject);
        }
        taskTargets.Clear();
        //创建新的taskTargets
        foreach (var tuple in task.GetTaskTargetInfo())
        {
            AddOrUpdateTarget(tuple.Item1 + ": ", tuple.Item2, tuple.Item3);
        }

        //奖励部分
        TaskAward award = taskCfg.award;
        currencyTxt.text = "灵魂: " + award.currency;
        //奖励物品栏部分
        foreach (var ui in itemUIs)
        {
            Destroy(ui.gameObject);
        }
        itemUIs.Clear();
        foreach (var item in award.items)
        {
            ItemData data = Inventory.GetItemDataByID(int.Parse(item.Key));
            ItemSlotUI ui = Instantiate(taskAwardSlotPrefab).GetComponent<ItemSlotUI>();
            ui.transform.SetParent(TaskAwardParent, false);
            ui.gameObject.SetActive(true);
            ui.UpdateUI(data, item.Value, true);
            itemUIs.Add(ui);
        }

        //完成按钮部分
        taskRequestBtn.onClick.RemoveAllListeners();
        taskRequestBtn.onClick.AddListener(() =>
        {
            TaskManager.ins.AddTask(task.chainId, task.subId);
            gameObject.SetActive(false);
        });
    }

    public void AddOrUpdateTarget(string targetName, int curProgress, int maxProgress)
    {
        if (taskTargets.TryGetValue(targetName, out TaskTargetSlotUI ui))
        {
            ui.Setup(targetName, curProgress, maxProgress);
        }
        else
        {
            TaskTargetSlotUI newUI = Instantiate(taskTargetSlotPrefab).GetComponent<TaskTargetSlotUI>();
            newUI.Setup(targetName, curProgress, maxProgress);
            newUI.gameObject.SetActive(true);
            newUI.transform.SetParent(TaskTargetParent, false);
            taskTargets.Add(targetName, newUI);
        }
    }
}
