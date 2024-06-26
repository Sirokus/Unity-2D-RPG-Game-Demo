using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftEquipmentUI : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private TextMeshProUGUI Name, Rare;
    [SerializeField] private Image image;
    public Image Bg;

    public EquipmentData equipment;

    public void setupUI(EquipmentData equipment)
    {
        this.equipment = equipment;
        Name.text = equipment.itemName;
        Rare.text = Utility.ToString(equipment.rare);
        image.sprite = equipment.itemIcon;

        UpdateCanMake();
    }

    public bool canMake()
    {
        foreach (var material in equipment.materials)
        {
            if (Inventory.instance.inventoryDictionary.TryGetValue(material.itemData, out List<InventoryItem> items))
            {
                int allNum = 0;
                foreach (var item in items)
                {
                    allNum += item.stackSize;
                    if (allNum >= material.num)
                        break;
                }
                if (allNum < material.num)
                    return false;
            }
            else
                return false;
        }
        return true;
    }

    public void UpdateCanMake()
    {
        if (canMake())
        {
            Name.color = Color.white;
            Rare.color = Color.white;
            image.color = Color.white;
            image.transform.parent.GetComponent<Image>().color = Color.white;
        }
        else
        {
            Name.color = Color.gray;
            Rare.color = Color.gray;
            image.color = Color.gray;
            image.transform.parent.GetComponent<Image>().color = Color.gray;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (CraftSelectorUI.ins.curActiveEquipmentUI != null)
            CraftSelectorUI.ins.curActiveEquipmentUI.Bg.color = new Color(0, 0, 0, 0.3f);
        Bg.color = new Color(1, 1, 1, 0.3f);
        CraftSelectorUI.ins.curActiveEquipmentUI = this;
        CraftItemInfoUI.ins.setupItem(this);
    }
}
