using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image itemImage;
    [SerializeField] public TextMeshProUGUI itemText;

    public InventoryItem item;

    public bool OnlyShow;   //used when in the craft panel
    public bool isInShop;   //used when in the shop panel

    private void Awake()
    {
        itemText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetUp(InventoryItem item)
    {
        this.item = item;
        itemImage.sprite = item.data.itemIcon;
        item.onChanged += UpdateUI;
        UpdateUI(item.stackSize);
    }

    public void UpdateUI(int stackSize)
    {
        if (stackSize > 1)
            itemText.text = stackSize.ToString();
        else if (stackSize == 1)
            itemText.text = "";
        else
        {
            item.onChanged -= UpdateUI;
            Destroy(gameObject);
        }
    }

    public void UpdateUI(ItemData data, int num, bool onlyShow)
    {
        InventoryItem item = new InventoryItem(data);
        SetUp(item);
        this.OnlyShow = onlyShow;

        if (num > 1)
            itemText.text = num.ToString();
        else if (num == 1)
            itemText.text = "";
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (OnlyShow)
            return;

        if (!isInShop)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                switch (item.data.itemType)
                {
                case EItemType.Material:
                    return;
                case EItemType.Equipment:
                    Inventory.EquipItem(item.data);
                    break;
                }
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                if (item.stackSize == 1)
                {
                    Inventory.DropItem(item);
                }
                else
                    UIManager.ins.dropUI.Setup(item);
            }
        }
        else
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                UIManager.ins.tradingUI.Setup(item);
            else if (eventData.button == PointerEventData.InputButton.Right)
                UIManager.ins.tradingUI.Sell(item);
        }
        OnPointerExit(null);
    }

    Timer timer;
    public void OnPointerEnter(PointerEventData eventData)
    {
        Inventory.instance.infoUITracePointer = true;
        Inventory.instance.itemInfoUI.UpdateUI(item.data);
        timer = TimerManager.addTimer(.1f, false, () =>
        {
            Inventory.instance.itemInfoUI.gameObject.SetActive(true);
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
        Inventory.instance.itemInfoUI.gameObject.SetActive(false);
        Inventory.instance.infoUITracePointer = false;
    }
}
