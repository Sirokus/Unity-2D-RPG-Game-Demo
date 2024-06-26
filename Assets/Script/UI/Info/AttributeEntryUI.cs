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
            Value.text = playerStat.isIgnited ? "��" : "��";
        }
        else if (stat == EStat.isChilled)
        {
            Value.text = playerStat.isChilled ? "��" : "��";
        }
        else if (stat == EStat.isShocked)
        {
            Value.text = playerStat.isShocked ? "��" : "��";
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
            Value.text = playerStat.isIgnited ? "��" : "��";
        }
        else if (stat == EStat.isChilled)
        {
            Value.text = playerStat.isChilled ? "��" : "��";
        }
        else if (stat == EStat.isShocked)
        {
            Value.text = playerStat.isShocked ? "��" : "��";
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
            Value.text = playerStat.isIgnited ? "��" : "��";
        }
        else if (newStat == EStat.isChilled)
        {
            Value.text = playerStat.isChilled ? "��" : "��";
        }
        else if (newStat == EStat.isShocked)
        {
            Value.text = playerStat.isShocked ? "��" : "��";
        }
        else
        {
            //�����һ��ί��
            Stat lastStat = playerStat.getStatByType(stat);
            if (lastStat != null)
                lastStat.onModified -= refreshValue;

            //����һ��ί��
            Stat targetStat = playerStat.getStatByType(newStat);
            targetStat.onModified += refreshValue;

            //ˢ��ֵ
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
