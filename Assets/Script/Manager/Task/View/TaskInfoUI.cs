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

    //����Ŀ����Ŀ�ֵ�
    public Dictionary<string, TaskTargetSlotUI> taskTargets = new Dictionary<string, TaskTargetSlotUI>();
    //�����б�
    public List<ItemSlotUI> itemUIs = new List<ItemSlotUI>();

    //�ύ����ť
    public Button getAwardBtn;

    public void Setup(Task task)
    {
        if (this.task != null && this.task != task)
        {
            this.task.onUpdate -= UpdateUI;
            this.task.onComplete -= UpdateUI;
        }
        this.task = task;

        //��鲿��
        TaskCfgItem taskCfg = TaskCfg.ins.GetCfgItem(task.chainId, task.subId);
        nameTxt.text = taskCfg.name;
        infoTxt.text = taskCfg.desc;

        //��������
        TaskAward award = taskCfg.award;
        currencyTxt.text = "���: " + award.currency;
        //������Ʒ������
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

        //��ɰ�ť����
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
        //������Ŀ����
        //��ղ�����������Ŀ
        foreach (var ui in taskTargets.Values)
        {
            if (ui != null)
                Destroy(ui.gameObject);
        }
        taskTargets.Clear();
        //�����µ�taskTargets
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
