using UnityEngine;

public class ShopInventoryUI : InventoryUI
{
    public Shop shop;

    protected override void Start()
    {
        base.Start();

        foreach (var shopItem in shop.shopItems)
        {
            onItemCreate(shopItem);
        }

        shop.onItemCreate += onItemCreate;
    }

    public void onItemCreate(ShopItem item)
    {
        ItemData data = item.data;

        if (!inventorySupportUI.Contains(data.itemType))
            return;

        GameObject slot = Instantiate(ItemSlotPrefab);
        slot.transform.SetParent(inventorySlotParents[data.itemType], false);
        slot.GetComponent<ShopSlotUI>().Setup(item, shop);
    }
}
