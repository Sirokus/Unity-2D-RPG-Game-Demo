using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ShopSlotUI : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI priceTxt, numTxt;

    public ShopItem item;
    public Shop shop;

    public void Setup(ShopItem item, Shop shop)
    {
        this.item = item;
        this.shop = shop;
        itemImage.sprite = item.data.itemIcon;
        priceTxt.text = '$' + item.price.ToString();
        numTxt.text = item.ItemInfo.Num.ToString();
        item.onChanged += UpdateUI;

        UpdateUI(item.ItemInfo.Num);
    }

    public void UpdateUI(int num)
    {
        if (num > 1)
        {
            numTxt.text = num.ToString();
        }
        else if (num == 1)
        {
            numTxt.text = "";
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            UIManager.ins.tradingUI.Setup(item, shop);
        }
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
