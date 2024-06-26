using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
public class TaskSlotUI : MonoBehaviour, IPointerDownHandler
{
    [HideInInspector] public Task task;

    public TextMeshProUGUI taskNameTxt, processTxt;
    public Scrollbar scrollbar;

    public CanvasGroup canvas;

    public void Init(Task task)
    {
        this.task = task;

        TaskCfgItem cfg = TaskCfg.ins.GetCfgItem(task.chainId, task.subId);
        taskNameTxt.text = task.chainId.ToString() + "-" + task.subId.ToString() + " " + cfg.name;

        onUpdate(task);

        task.onUpdate += onUpdate;
        task.onComplete += onComplete;
    }

    public void onUpdate(Task task)
    {
        scrollbar.size = task.progress;
        processTxt.text = (task.progress * 100).ToString() + "%";
    }

    public void onComplete(Task task)
    {
        onUpdate(task);

        task.onUpdate -= onUpdate;
        task.onComplete -= onComplete;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        UIManager.ins.taskUI.SelectTask(task);
    }

    public void Select(bool bSelect)
    {
        canvas.alpha = bSelect ? 1 : 0.6f;
    }
}