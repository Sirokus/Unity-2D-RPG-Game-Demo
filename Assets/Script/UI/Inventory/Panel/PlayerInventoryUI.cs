using UnityEngine;

public class PlayerInventoryUI : InventoryUI
{
    public bool isShopInventory;

    protected override void Start()
    {
        base.Start();

        foreach (var item in Inventory.instance.inventory)
        {
            if (inventorySupportUI.Contains(item.data.itemType))
                onItemCreate(item);
        }

        Inventory.instance.onItemCreate += onItemCreate;
    }

    public void onItemCreate(InventoryItem item)
    {
        if (!inventorySupportUI.Contains(item.data.itemType))
            return;
        ItemData data = item.data;
        GameObject slot = Instantiate(ItemSlotPrefab);
        slot.transform.SetParent(inventorySlotParents[data.itemType], false);
        slot.GetComponent<ItemSlotUI>().SetUp(item);
        slot.GetComponent<ItemSlotUI>().isInShop = isShopInventory;
    }
}
