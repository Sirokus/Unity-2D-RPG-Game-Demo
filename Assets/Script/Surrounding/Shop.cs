using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public struct ShopItemInfo
{
    public int ID;
    public int Num;
    public ShopItemInfo(int id, int num)
    {
        ID = id;
        Num = num;
    }
}
[Serializable]
public class ShopItem
{
    public int ID;
    public ShopItemInfo ItemInfo;
    public int price;

    public ItemData data;
    public Action<int> onChanged;

    public ShopItem(int id, ShopItemInfo itemInfo, int price)
    {
        ID = id;
        ItemInfo = itemInfo;
        this.price = price;
    }
    public void AddStack(int num)
    {
        ItemInfo.Num += num;
        onChanged?.Invoke(ItemInfo.Num);
    }
    public void RemoveStack(int num)
    {
        ItemInfo.Num -= num;
        onChanged?.Invoke(ItemInfo.Num);
    }
}
[Serializable]
public struct ShopJsonHelper
{
    public List<ShopItem> shopItems;
}

public class Shop : MonoBehaviour, IInteractable, ISaveManager
{
    public List<ShopItem> shopItems;
    public bool isFirstVisited = true;

    public Action<ShopItem> onItemEnter, onItemOuter;
    public Action<ShopItem> onItemCreate;

    public string interactName { get => "ил╣Й"; }

    private void Awake()
    {
        string json = Utility.ReadJson("ShopItems", "Item");
        ShopJsonHelper jsonClass = JsonUtility.FromJson<ShopJsonHelper>(json);
        shopItems = jsonClass.shopItems;
        for (int i = shopItems.Count - 1; i >= 0; --i)
        {
            shopItems[i].data = Inventory.GetItemDataByID(shopItems[i].ItemInfo.ID);
            if (!shopItems[i].data)
                shopItems.RemoveAt(i);
        }
    }

    public void Interact()
    {
        if (isFirstVisited)
            UIManager.ins.dialogueManager.OpenDialogue("ShopDialogue");
        else
            UIManager.ins.dialogueManager.OpenDialogue("ShopDialogue_Short");

        isFirstVisited = false;

        SaveManager.AutoSave();
    }

    public void AddShopItem(ItemData data, int num = 1)
    {
        foreach (ShopItem item in shopItems)
        {
            if (item.data == data)
            {
                item.AddStack(num);
                num = 0;
                break;
            }
        }

        if (num != 0)
        {
            ShopItem item = new ShopItem(shopItems.Count, new ShopItemInfo(int.Parse(data.itemID), num), data.itemPrice);
            item.data = data;
            shopItems.Add(item);
            onItemCreate?.Invoke(item);
        }
    }

    public void RemoveShopItem(ShopItem item, int num = 1)
    {
        foreach (var shopItem in shopItems)
        {
            if (shopItem != item)
                continue;
            shopItem.RemoveStack(num);
            if (shopItem.ItemInfo.Num <= 0)
                shopItems.Remove(shopItem);
            break;
        }
    }

    public void LoadData(GameData data)
    {
        if (data.haveValue("isFirstVisited_Shop"))
            isFirstVisited = data.getValueb("isFirstVisited_Shop");
    }

    public void SaveData(GameData data)
    {
        data.addValue("isFirstVisited_Shop", isFirstVisited);
    }

}
