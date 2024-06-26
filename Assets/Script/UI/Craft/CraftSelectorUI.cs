using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftSelectorUI : MonoBehaviour
{
    public static CraftSelectorUI ins;
    public Button[] buttons;
    public Transform CraftParent;
    public GameObject CraftEntryUIPrefab;

    public int curActiveIndex = 0;

    public CraftEquipmentUI curActiveEquipmentUI;

    [Serializable]
    public class Craft
    {
        public EquipmentType type;
        public List<EquipmentData> equipments;
    }
    public Craft[] crafts;

    private void Awake()
    {
        ins = this;
        for (int i = 0; i <  buttons.Length; i++)
        {
            int tmp = i;
            buttons[i].onClick.AddListener(() => select(tmp));
        }
    }

    private void Start()
    {
        select(0);

        Inventory.instance.onItemEnter += onItemChanged;
        Inventory.instance.onItemOuter += onItemChanged;
    }

    void select(int index)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i == index)
            {
                buttons[i].GetComponent<Image>().color = Color.white;
                buttons[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.white;

                for (int j = 0; j < CraftParent.childCount; j++)
                    Destroy(CraftParent.GetChild(j).gameObject);

                foreach (var data in crafts[i].equipments)
                {
                    GameObject ui = Instantiate(CraftEntryUIPrefab);
                    ui.GetComponent<CraftEquipmentUI>().setupUI(data);
                    ui.transform.SetParent(CraftParent, false);
                }

                curActiveIndex = index;
            }
            else
            {
                buttons[i].GetComponent<Image>().color = Color.gray;
                buttons[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.gray;
            }
        }
    }

    void onItemChanged(InventoryItem item)
    {
        foreach (var child in CraftParent.GetComponentsInChildren<CraftEquipmentUI>())
            child.UpdateCanMake();
    }
}
