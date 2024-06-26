using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class AttributeEntryUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI Name, Value;
    [SerializeField] public EStat stat = EStat.none;
    PlayerStat playerStat;

    private void Awake()
    {
        Name.SetText(Utility.ToString(stat) + ":");
    }

    private void Start()
    {
        playerStat = PlayerManager.GetPlayer().GetComponent<PlayerStat>();
        if (stat == EStat.isIgnited)
        {
            Value.text = playerStat.isIgnited ? "有" : "无";
        }
        else if (stat == EStat.isChilled)
        {
            Value.text = playerStat.isChilled ? "有" : "无";
        }
        else if (stat == EStat.isShocked)
        {
            Value.text = playerStat.isShocked ? "有" : "无";
        }
        else
        {
            Stat targetStat = playerStat.getStatByType(stat);
            if (targetStat == null)
                return;

            targetStat.onModified += refreshValue;
            Value.text = targetStat.getValue().ToString();

            int baseValue = PlayerManager.playerStat.getStatByType(stat).getBaseValue();
            int value = int.Parse(Value.text);
            if (value < baseValue)
                Value.color = Color.red;
            else if (value > baseValue)
                Value.color = Color.green;
            else
                Value.color = Color.white;
        }
    }

    private void Update()
    {
        if (stat == EStat.isIgnited)
        {
            Value.text = playerStat.isIgnited ? "有" : "无";
        }
        else if (stat == EStat.isChilled)
        {
            Value.text = playerStat.isChilled ? "有" : "无";
        }
        else if (stat == EStat.isShocked)
        {
            Value.text = playerStat.isShocked ? "有" : "无";
        }
    }

    void refreshValue(int amount, bool isIncrease)
    {
        int baseValue = PlayerManager.playerStat.getStatByType(stat).getBaseValue();
        int value = int.Parse(Value.text);
        if (isIncrease)
            value += amount;
        else
            value -= amount;

        if (value < baseValue)
            Value.color = Color.red;
        else if (value > baseValue)
            Value.color = Color.green;
        else
            Value.color = Color.white;

        Value.text = value.ToString();
    }

    public void resetValue(EStat newStat)
    {
        playerStat = PlayerManager.playerStat;
        Name.SetText(Utility.ToString(newStat) + ":");

        if (newStat == EStat.isIgnited)
        {
            Value.text = playerStat.isIgnited ? "有" : "无";
        }
        else if (newStat == EStat.isChilled)
        {
            Value.text = playerStat.isChilled ? "有" : "无";
        }
        else if (newStat == EStat.isShocked)
        {
            Value.text = playerStat.isShocked ? "有" : "无";
        }
        else
        {
            //解绑上一个委托
            Stat lastStat = playerStat.getStatByType(stat);
            if (lastStat != null)
                lastStat.onModified -= refreshValue;

            //绑定这一个委托
            Stat targetStat = playerStat.getStatByType(newStat);
            targetStat.onModified += refreshValue;

            //刷新值
            Value.text = targetStat.getValue().ToString();
        }

        stat = newStat;
    }

    public void setupValue(string name, string value)
    {
        playerStat = PlayerManager.playerStat;
        Stat lastStat = playerStat.getStatByType(stat);
        if (lastStat != null)
            lastStat.onModified -= refreshValue;

        stat = EStat.none;

        Name.text = name + ":";
        Value.text = value;
    }

    Timer timer;
    public void OnPointerEnter(PointerEventData eventData)
    {
        Inventory.instance.infoUITracePointer = true;
        Inventory.instance.StatInfoUI.UpdateUI(stat);
        if (!Inventory.instance.StatInfoUI.haveMod)
            return;
        timer = TimerManager.addTimer(.1f, false, () =>
        {
            Inventory.instance.StatInfoUI.gameObject.SetActive(true);
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
        Inventory.instance.StatInfoUI.gameObject.SetActive(false);
        Inventory.instance.infoUITracePointer = false;
    }
}
