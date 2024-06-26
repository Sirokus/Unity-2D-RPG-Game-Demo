using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySelectorUI : MonoBehaviour
{
    public InventoryUI inventoryUI;
    public GameObject button;

    public Dictionary<EItemType, Button> buttons = new Dictionary<EItemType, Button>();
    public EItemType currentBtn;

    // Start is called before the first frame update
    void Start()
    {
        foreach (EItemType type in inventoryUI.inventorySupportUI)
        {
            EItemType tmp = type;
            buttons.Add(tmp, Instantiate(button).GetComponent<Button>());
            buttons.Last().Value.GetComponentInChildren<TextMeshProUGUI>().SetText(Utility.ToString(tmp));
            buttons.Last().Value.transform.SetParent(transform, false);
            buttons.Last().Value.gameObject.SetActive(true);
            buttons.Last().Value.onClick.AddListener(() => onSelected(tmp));
        }

        Init(inventoryUI.inventorySupportUI[0]);
    }

    private void Init(EItemType type)
    {
        currentBtn = type;
        inventoryUI.inventorySlotParents[currentBtn].gameObject.SetActive(true);
        buttons[currentBtn].GetComponent<Image>().color = Color.white;
        buttons[currentBtn].GetComponentInChildren<TextMeshProUGUI>().color = Color.white;

        foreach (var btn in buttons)
        {
            if (btn.Key != type)
            {
                buttons[btn.Key].GetComponent<Image>().color = Color.gray;
                buttons[btn.Key].GetComponentInChildren<TextMeshProUGUI>().color = Color.gray;
            }
        }
    }

    void onSelected(EItemType type)
    {
        if (currentBtn == type)
            return;

        inventoryUI.inventorySlotParents[currentBtn].gameObject.SetActive(false);
        buttons[currentBtn].GetComponent<Image>().color = Color.gray;
        buttons[currentBtn].GetComponentInChildren<TextMeshProUGUI>().color = Color.gray;

        inventoryUI.inventorySlotParents[type].gameObject.SetActive(true);
        buttons[type].GetComponent<Image>().color = Color.white;
        buttons[type].GetComponentInChildren<TextMeshProUGUI>().color = Color.white;

        currentBtn = type;
    }
}
