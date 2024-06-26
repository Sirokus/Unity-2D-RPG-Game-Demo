using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TipsUI : MonoBehaviour
{
    public static TipsUI instance;

    public TextMeshProUGUI textUI;
    public CanvasGroup canvasUI;

    [Header("tip stat")]
    private Coroutine curTask;
    public float UpSpeed = 1;
    bool isDecreassing = false;

    [Header("OtherTips")]
    public TipsUI_Normal tipsUI_Normal;
    public TipsUI_Achievement tipsUI_Achievement;

    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);

        EventManager.AddListener(EventName.OnTaskAdd, AddTipTask);
        EventManager.AddListener(EventName.OnTaskComplete, AddTipTaskComplete);
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener(EventName.OnTaskAdd, AddTipTask);
        EventManager.RemoveListener(EventName.OnTaskComplete, AddTipTaskComplete);
    }

    private void Update()
    {
        var pos = Camera.main.WorldToScreenPoint(PlayerManager.playerPos + Vector3.up * 1.5f);
        if (isDecreassing)
            pos.y = transform.position.y;
        transform.position = pos;
    }

    public static void AddTip(string text, bool isTiny = true)
    {
        if (isTiny)
        {
            instance.gameObject.SetActive(true);

            instance.textUI.text = text;
            instance.canvasUI.alpha = 1;

            instance.transform.position = Camera.main.WorldToScreenPoint(PlayerManager.playerPos + Vector3.up * 1.8f);

            if (instance.curTask != null)
                instance.StopCoroutine(instance.curTask);
            instance.isDecreassing = false;
            instance.curTask = instance.StartCoroutine(instance.letItGo());
        }
        else
            instance.tipsUI_Normal.Play(text);
    }

    public static void AddTipAchi(string name, string desc)
    {
        instance.tipsUI_Achievement.Play(name, desc);
    }

    public static void AddTipTask(object sender, EventArgs _args)
    {
        TaskAddArgs args = _args as TaskAddArgs;
        if (args != null && args.isLoadAdded != true)
        {
            TaskCfgItem cfg = TaskCfg.ins.GetCfgItem(args.task.chainId, args.task.subId);
            instance.tipsUI_Achievement.Play("任务 - [" + cfg.name + "] 获取", cfg.desc);
        }
    }

    private static void AddTipTaskComplete(object sender, EventArgs _args)
    {
        TaskCompleteArgs args = _args as TaskCompleteArgs;
        if (args != null && args.isFirstComplete)
        {
            TaskCfgItem cfg = TaskCfg.ins.GetCfgItem(args.task.chainId, args.task.subId);
            instance.tipsUI_Achievement.Play("完成任务 - [" + cfg.name + "]", cfg.desc);
        }
    }

    IEnumerator letItGo()
    {
        yield return new WaitForSeconds(1f);
        isDecreassing = true;
        while (canvasUI.alpha > 0)
        {
            canvasUI.alpha -= 0.03f;

            Vector2 pos = transform.position;
            pos.y += UpSpeed * Time.deltaTime;
            transform.position = pos;

            yield return null;
        }

        isDecreassing = false;
        gameObject.SetActive(false);
    }
}
