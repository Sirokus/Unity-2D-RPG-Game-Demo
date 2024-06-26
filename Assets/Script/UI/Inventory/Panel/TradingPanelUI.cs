using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TradingPanelUI : MonoBehaviour
{
    public TMP_Text itemName, itemInfo;
    public TMP_Text numTxt, priceTxt;

    public Slider slider;
    public Button button;

    public bool isBuy = true;
    public TMP_Text buyTxt;

    public ShopItem item;
    public InventoryItem inventoryItem;

    public Shop shop;
    public float playerSellMultiplier = 0.5f;

    private void Awake()
    {
        slider.onValueChanged.AddListener(onSliderChanged);
        button.onClick.AddListener(onButtonDown);
    }

    public void Setup(ShopItem item, Shop shop)
    {
        gameObject.SetActive(true);

        this.item = item;
        this.shop = shop;

        slider.maxValue = item.ItemInfo.Num;
        slider.value = 1;

        itemName.text = item.data.itemName;
        itemInfo.text = item.data.itemInfo;

        isBuy = true;
        buyTxt.text = "花费:";
        button.GetComponentInChildren<TextMeshProUGUI>().text = "购买";

        onSliderChanged(1);
    }

    public void Setup(InventoryItem item)
    {
        gameObject.SetActive(true);

        inventoryItem = item;
        shop = UIManager.ins.shopUI.shop;

        slider.maxValue = item.stackSize;
        slider.value = 1;

        itemName.text = item.data.itemName;
        itemInfo.text = item.data.itemInfo;

        isBuy = false;
        buyTxt.text = "获得:";
        button.GetComponentInChildren<TextMeshProUGUI>().text = "出售";

        onSliderChanged(1);
    }

    public void onSliderChanged(float val)
    {
        numTxt.text = ((int)val).ToString();
        priceTxt.text = ((int)((int)val * (isBuy ? item.price : inventoryItem.data.itemPrice * playerSellMultiplier))).ToString();
    }

    public void onButtonDown()
    {
        if (isBuy)
        {
            if (PlayerManager.instance.Currency <= slider.value * item.price)
            {
                TipsUI.AddTip("现金不足，无法购买！", false);
                return;
            }

            if (PlayerManager.instance.Currency >= item.price)
            {
                //, false
                TipsUI.AddTip("购买 [" + item.data.itemName + "] 物品成功！", false);
                //扣除金额
                PlayerManager.DecreaseCurrency((int)slider.value * item.price);
                //增加仓库
                Inventory.AddItem(item.data, (int)slider.value);
                //减少商品
                shop.RemoveShopItem(item, (int)slider.value);
            }
        }
        else
        {
            //添加提示
            TipsUI.AddTip("出售 [" + inventoryItem.data.itemName + "] 物品成功！", false);
            //增加金额
            PlayerManager.IncreaseCurrency((int)((int)slider.value * inventoryItem.data.itemPrice * playerSellMultiplier));
            //减少仓库
            Inventory.RemoveItem(inventoryItem, (int)slider.value);
            //增加商品
            shop.AddShopItem(inventoryItem.data, (int)slider.value);
        }

        gameObject.SetActive(false);
    }

    public void Sell(InventoryItem item)
    {
        slider.value = item.stackSize;
        //添加提示
        TipsUI.AddTip("出售 [" + item.data.itemName + "] 物品成功！", false);
        //增加金额
        PlayerManager.IncreaseCurrency((int)((int)slider.value * item.data.itemPrice * playerSellMultiplier));
        //减少仓库
        Inventory.RemoveItem(item, (int)slider.value);
        //增加商品
        shop.AddShopItem(item.data, (int)slider.value);
    }
}
