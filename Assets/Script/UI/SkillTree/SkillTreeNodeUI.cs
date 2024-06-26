using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
public class SkillTreeNodeUI : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    //状态修改所需
    public List<SkillTreeNodeUI> parents = new List<SkillTreeNodeUI>();
    [SerializeField] public List<SkillTreeNodeUI> childs = new List<SkillTreeNodeUI>();
    public int spend = 500;
    public bool canUnlock = false;
    public bool isLock = true;

    public Action<bool> onLockChanged;

    //外观
    [SerializeField] private Image skillIcon;
    [SerializeField] private Image bg;
    [SerializeField] private GameObject linePrefab;

    [SerializeField] public string skillName, skillDescription;

    // Start is called before the first frame update
    public void Init()
    {
        foreach (var child in childs)
        {
            child.parents.Add(this);
            child.canUnlock = child.CanUnlock();
            child.updateUI();

            SkillTreeLine line = Instantiate(linePrefab).GetComponent<SkillTreeLine>();
            line.transform.SetParent(transform.parent, false);
            line.setupLine(GetComponent<RectTransform>(), child.GetComponent<RectTransform>());

            child.Init();
        }
    }

    public bool CanUnlock()
    {
        foreach (var p in parents)
        {
            if (p.isLock)
                return false;
        }
        return true;
    }

    public void Unlock()
    {
        if (!isLock)
            return;

        if (!CanUnlock())
        {
            TipsUI.AddTip("请先解锁前置技能！", false);
            return;
        }

        if (!PlayerManager.CheckCurrencyEnough(spend))
        {
            TipsUI.AddTip("技能点数不足，无法解锁！", false);
            return;
        }

        PlayerManager.DecreaseCurrency(spend);

        SetUnlock(true);
    }

    public void SetUnlock(bool unlock)
    {
        isLock = !unlock;
        onLockChanged?.Invoke(unlock);
        updateUI();

        foreach (var child in childs)
        {
            child.canUnlock = child.CanUnlock();
            child.updateUI();
        }
    }

    public void updateUI()
    {
        if (!isLock)
        {
            skillIcon.color = Color.white;
            bg.color = Color.green;
        }
        else if (canUnlock)
        {
            skillIcon.color = Color.white;
            bg.color = Color.white;
        }
        else
        {
            skillIcon.color = Color.gray;
            bg.color = Color.gray;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Unlock();
    }

    Timer timer;
    public void OnPointerEnter(PointerEventData eventData)
    {
        SkillTreeUI.ins.infoUITracePointer = true;
        SkillTreeUI.ins.infoUI.UpdateUI(this);
        timer = TimerManager.addTimer(.1f, false, () =>
        {
            SkillTreeUI.ins.infoUI.gameObject.SetActive(true);
            timer = null;
        });
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (timer != null)
        {
            TimerManager.clearTimer(timer);
            timer = null;
        }
        SkillTreeUI.ins.infoUI.gameObject.SetActive(false);
        SkillTreeUI.ins.infoUITracePointer = false;
    }

    public void ResetUI()
    {
        foreach (var child in childs)
        {
            canUnlock = false;
            SetUnlock(false);
            child.ResetUI();
        }
    }

    public void LoadFrom(GameData data)
    {
        if (data.haveValue(skillName))
        {
            SetUnlock(data.getValueb(skillName));
        }
        foreach (var child in childs)
            child.LoadFrom(data);
    }

    public void SaveTo(GameData data)
    {
        data.addValue(skillName, !isLock);
        foreach (var child in childs)
            child.SaveTo(data);
    }
}
