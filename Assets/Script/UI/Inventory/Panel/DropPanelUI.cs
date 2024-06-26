using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DropPanelUI : MonoBehaviour
{
    public TMP_Text itemName, itemInfo;
    public TMP_Text numTxt;

    public Slider slider;
    public Button button;

    public InventoryItem inventoryItem;

    private void Awake()
    {
        slider.onValueChanged.AddListener(onSliderChanged);
        button.onClick.AddListener(onButtonDown);
    }

    public void Setup(InventoryItem item)
    {
        gameObject.SetActive(true);

        inventoryItem = item;

        slider.maxValue = item.stackSize;
        slider.value = 1;

        itemName.text = item.data.itemName;
        itemInfo.text = item.data.itemInfo;

        onSliderChanged(1);
    }

    public void onSliderChanged(float val)
    {
        numTxt.text = ((int)val).ToString();
    }

    public void onButtonDown()
    {
        //ºı…Ÿ≤÷ø‚
        Inventory.DropItem(inventoryItem, (int)slider.value);
        gameObject.SetActive(false);
    }
}
