using TMPro;
using UnityEngine;

public class CraftItemInfoUI : MonoBehaviour
{
    public static CraftItemInfoUI ins;

    [SerializeField] private TextMeshProUGUI ItemName;
    [SerializeField] private TextMeshProUGUI ItemDescription;
    [SerializeField] private Transform AttributeParent;
    [SerializeField] private Transform NeedListParent;

    [SerializeField] private GameObject AttributeEntryPrefab;
    [SerializeField] private GameObject ItemSlotPrefab;

    private CraftEquipmentUI ItemUI;

    private void Awake()
    {
        ins = this;
    }

    private void Start()
    {
        CraftEquipmentUI firstUI = CraftSelectorUI.ins.CraftParent.GetComponentInChildren<CraftEquipmentUI>();
        firstUI.Bg.color = new Color(1, 1, 1, 0.3f);
        CraftSelectorUI.ins.curActiveEquipmentUI = firstUI;
        setupItem(firstUI);
    }

    public void setupItem(CraftEquipmentUI itemUI)
    {
        //������Ϣ����
        ItemUI = itemUI;
        EquipmentData item = itemUI.equipment;
        ItemName.text = item.itemName;
        ItemDescription.text = item.itemInfo;

        //��ʼ����������
        for (int i = 0; i < AttributeParent.childCount; i++)
            Destroy(AttributeParent.GetChild(i).gameObject);
        for (int i = 0; i < NeedListParent.childCount; i++)
            Destroy(NeedListParent.GetChild(i).gameObject);

        //���������Ŀ
        foreach (var attribute in item.attributes)
        {
            GameObject ui = Instantiate(AttributeEntryPrefab);
            ui.GetComponent<AttributeEntryUI>().setupValue(Utility.ToString(attribute.stat), attribute.Modifier.ToString());
            ui.transform.SetParent(AttributeParent, false);
        }

        //���������Ʒ��Ŀ
        foreach (var material in item.materials)
        {
            GameObject ui = Instantiate(ItemSlotPrefab);
            ui.GetComponent<ItemSlotUI>().UpdateUI(material.itemData, material.num, true);
            ui.transform.SetParent(NeedListParent, false);
        }
    }

    public void MakeItem()
    {
        if (ItemUI == null || !ItemUI.canMake())
        {
            TipsUI.AddTip("���ϲ��㣬�޷����죡", false);
            return;
        }

        foreach (var material in ItemUI.equipment.materials)
        {
            Inventory.RemoveItem(material.itemData);
        }
        Inventory.AddItem(ItemUI.equipment);
        TipsUI.AddTip("[" + ItemUI.equipment.itemName + "] ����ɹ���", false);
    }
}
