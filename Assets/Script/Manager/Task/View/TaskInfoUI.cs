using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskInfoUI : MonoBehaviour
{
    public Task task;

    public TextMeshProUGUI nameTxt, infoTxt, currencyTxt;

    public Transform TaskTargetParent, TaskAwardParent;
    public GameObject taskTargetSlotPrefab, taskAwardSlotPrefab;

    //任务目标条目字典
    public Dictionary<string, TaskTargetSlotUI> taskTargets = new Dictionary<string, TaskTargetSlotUI>();
    //奖励列表
    public List<ItemSlotUI> itemUIs = new List<ItemSlotUI>();

    //提交任务按钮
    public Button getAwardBtn;

    public void Setup(Task task)
    {
        if (this.task != null && this.task != task)
        {
            this.task.onUpdate -= UpdateUI;
            this.task.onComplete -= UpdateUI;
        }
        this.task = task;

        //简介部分
        TaskCfgItem taskCfg = TaskCfg.ins.GetCfgItem(task.chainId, task.subId);
        nameTxt.text = taskCfg.name;
        infoTxt.text = taskCfg.desc;

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
        getAwardBtn.onClick.RemoveAllListeners();
        getAwardBtn.onClick.AddListener(() =>
        {
            TaskManager.ins.GetAward(task.chainId, task.subId, (code, award) =>
            {
                if (code != 0)
                    return;

                PlayerManager.IncreaseCurrency(award.currency);

                foreach (var ui in itemUIs)
                {
                    Inventory.AddItem(ui.item.data, ui.itemText.text == "" ? 1 : int.Parse(ui.itemText.text));
                }
            });
        });

        task.onUpdate += UpdateUI;
        task.onComplete += UpdateUI;
        UpdateUI(task);
    }

    public void UpdateUI(Task task)
    {
        //任务条目部分
        //清空残留的任务条目
        foreach (var ui in taskTargets.Values)
        {
            if (ui != null)
                Destroy(ui.gameObject);
        }
        taskTargets.Clear();
        //创建新的taskTargets
        foreach (var tuple in task.GetTaskTargetInfo())
        {
            AddOrUpdateTarget(tuple.Item1 + ": ", tuple.Item2, tuple.Item3);
        }

        getAwardBtn.gameObject.SetActive(task.isComplete());
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
