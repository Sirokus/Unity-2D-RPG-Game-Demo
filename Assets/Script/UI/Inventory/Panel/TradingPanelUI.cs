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
        buyTxt.text = "����:";
        button.GetComponentInChildren<TextMeshProUGUI>().text = "����";

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
        buyTxt.text = "���:";
        button.GetComponentInChildren<TextMeshProUGUI>().text = "����";

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
                TipsUI.AddTip("�ֽ��㣬�޷�����", false);
                return;
            }

            if (PlayerManager.instance.Currency >= item.price)
            {
                //, false
                TipsUI.AddTip("���� [" + item.data.itemName + "] ��Ʒ�ɹ���", false);
                //�۳����
                PlayerManager.DecreaseCurrency((int)slider.value * item.price);
                //���Ӳֿ�
                Inventory.AddItem(item.data, (int)slider.value);
                //������Ʒ
                shop.RemoveShopItem(item, (int)slider.value);
            }
        }
        else
        {
            //�����ʾ
            TipsUI.AddTip("���� [" + inventoryItem.data.itemName + "] ��Ʒ�ɹ���", false);
            //���ӽ��
            PlayerManager.IncreaseCurrency((int)((int)slider.value * inventoryItem.data.itemPrice * playerSellMultiplier));
            //���ٲֿ�
            Inventory.RemoveItem(inventoryItem, (int)slider.value);
            //������Ʒ
            shop.AddShopItem(inventoryItem.data, (int)slider.value);
        }

        gameObject.SetActive(false);
    }

    public void Sell(InventoryItem item)
    {
        slider.value = item.stackSize;
        //�����ʾ
        TipsUI.AddTip("���� [" + item.data.itemName + "] ��Ʒ�ɹ���", false);
        //���ӽ��
        PlayerManager.IncreaseCurrency((int)((int)slider.value * item.data.itemPrice * playerSellMultiplier));
        //���ٲֿ�
        Inventory.RemoveItem(item, (int)slider.value);
        //������Ʒ
        shop.AddShopItem(item.data, (int)slider.value);
    }
}
