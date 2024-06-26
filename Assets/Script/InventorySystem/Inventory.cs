using Mono.Cecil;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Inventory : MonoBehaviour, ISaveManager
{
    public static Inventory instance;

    //初始物品
    public ItemData[] starterItems;

    //库存物品
    public List<InventoryItem> inventory = new List<InventoryItem>();
    public MultiDictionary<ItemData, InventoryItem> inventoryDictionary = new MultiDictionary<ItemData, InventoryItem>();
    //装备物品
    public Dictionary<EquipmentType, ItemData> equipmentDictionary = new Dictionary<EquipmentType, ItemData>();


    [Header("Usage Item")]
    public bool canUseFlask = true;

    [Header("Data Base")]
    public List<ItemData> loadedItems = new List<ItemData>();


    [Header("Tiny Info UI")]
    [SerializeField] public ItemInfoUI itemInfoUI;
    [SerializeField] public StatInfoUI StatInfoUI;
    public bool infoUITracePointer = false;
    public EquipSlotUI[] equipSlots;
    [SerializeField] private GameObject ItemObjPrefab;

    //委托
    public System.Action<InventoryItem> onItemCreate;
    public System.Action<InventoryItem> onItemEnter;
    public System.Action<InventoryItem> onItemOuter;

    private void Awake()
    {
        if (instance)
            Destroy(instance);
        instance = this;
    }

    private void Start()
    {
        foreach (var item in starterItems)
        {
            AddItem(item);
            if (item.itemType == EItemType.Equipment)
                EquipItem(item);
        }
    }

    private void FixedUpdate()
    {
        if (infoUITracePointer)
        {
            Vector3 mousePos = Input.mousePosition;
            itemInfoUI.transform.position = new Vector3(mousePos.x, mousePos.y, itemInfoUI.transform.position.z);
            StatInfoUI.transform.position = new Vector3(mousePos.x, mousePos.y, StatInfoUI.transform.position.z);
        }
    }

    public static void AddItem(ItemData data, int num = 1)
    {
        if (instance.inventoryDictionary.TryGetValue(data, out List<InventoryItem> list))
        {
            foreach (var value in list)
            {
                if (num > 0 && value.stackSize < 32 && value.data.itemType != EItemType.Equipment)
                {
                    int amount = Mathf.Min(num, 32 - value.stackSize);
                    value.AddStack(amount);
                    instance.onItemEnter?.Invoke(value);
                    num -= amount;
                    if (num <= 0)
                        break;
                }
            }
        }

        if (num > 0)
        {
            while (num > 0)
            {
                InventoryItem item = new InventoryItem(data);
                instance.inventory.Add(item);
                instance.inventoryDictionary.Add(data, item);
                num--;

                int amount = Mathf.Min(num, data.itemType == EItemType.Equipment ? 0 : 31);
                num -= amount;
                item.AddStack(amount);

                instance.onItemCreate?.Invoke(item);
                instance.onItemEnter?.Invoke(item);
            }
        }
    }

    //Remove的指定个数
    public static void RemoveItem(ItemData data, int num = 1)
    {
        if (instance.inventoryDictionary.TryGetValue(data, out List<InventoryItem> list))
        {
            foreach (var value in list)
            {
                int removeAmount = Mathf.Min(num, value.stackSize);
                num -= removeAmount;
                value.RemoveStack(removeAmount);
                instance.onItemOuter?.Invoke(value);

                if (value.stackSize <= 0)
                {
                    instance.inventory.Remove(value);
                    instance.inventoryDictionary.Remove(data);
                }

                if (num <= 0) break;
            }
        }
    }

    public static void RemoveItem(InventoryItem item, int num = 1)
    {
        if (instance.inventoryDictionary.TryGetValue(item.data, out List<InventoryItem> list))
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                var value = list[i];
                if (value == item)
                {
                    int removeAmount = Mathf.Min(num, value.stackSize);
                    num -= removeAmount;
                    value.RemoveStack(removeAmount);
                    instance.onItemOuter?.Invoke(value);

                    if (value.stackSize <= 0)
                    {
                        instance.inventory.Remove(value);
                        instance.inventoryDictionary.Remove(item.data, item);
                    }

                    if (num <= 0) break;
                }
            }
        }
    }

    public static void EquipItem(ItemData item, bool removeInInventory = true)
    {
        if (item.itemType != EItemType.Equipment)
            return;

        EquipmentData equipmentData = (EquipmentData)item;

        //遍历寻找合适的槽
        foreach (var slot in instance.equipSlots)
        {
            if (slot.type == equipmentData.equipmentType)
            {
                //如果存在装备则先取下原有装备
                if (instance.equipmentDictionary.ContainsKey(equipmentData.equipmentType))
                {
                    slot.UnEquip();
                }

                //装备新装备
                if (removeInInventory)
                    RemoveItem(item);
                instance.equipmentDictionary.Add(equipmentData.equipmentType, item);
                slot.Equip(item);
                equipmentData.AddModifiers();
            }
        }
    }

    public static void DropItem(InventoryItem item, int num = 1)
    {
        float force = Random.Range(1f, 2f) * 100;
        DropItem(item, PlayerManager.playerPos,
                            PlayerManager.isFacingRight ? Vector2.right * force : Vector2.left * force, num);
    }

    public static void DropItem(InventoryItem item, Vector2 position, Vector2 velocity, int num = 1)
    {
        if (item == null) return;
        RemoveItem(item, num);
        SpawnItem(item.data, position, velocity, num);
    }

    public static void SpawnItem(ItemData data, Vector2 position, Vector2 veloctiy, int num = 1)
    {
        ItemObj itemObj = ObjectPool.Get<ItemObj>();
        if (!itemObj)
            return;
        itemObj.transform.position = position;
        itemObj.Setup(data, num);
        itemObj.GetComponent<Rigidbody2D>().AddForce(veloctiy);
    }

    public static InventoryItem GetRandomItem()
    {
        if (instance.inventory.Any())
        {
            return instance.inventory[Random.Range(0, instance.inventory.Count - 1)];
        }
        return null;
    }

    public static EquipmentData GetEquipmentByType(EquipmentType type)
    {
        if (instance.equipmentDictionary.ContainsKey(type))
            return instance.equipmentDictionary[type] as EquipmentData;
        return null;
    }

    public void UseFlask()
    {
        if (!canUseFlask)
        {
            TipsUI.AddTip("药剂正在冷却时间当中。");
            return;
        }

        EquipmentData currentFlask = GetEquipmentByType(EquipmentType.Flask);
        if (!currentFlask)
        {
            TipsUI.AddTip("尚未装备药剂以供使用。");
            return;
        }

        if (!currentFlask.executeEffects(PlayerManager.GetPlayer().transform))
            return;

        canUseFlask = false;
        TimerManager.addTimer(currentFlask.useCoolDown, false, () =>
        {
            canUseFlask = true;
        });
    }

    //清空库存中的所有数据！！！
    public void resetInventory()
    {
        //脱下装备
        foreach (var slot in equipSlots)
            slot.UnEquip();
        //清空库存
        while (inventory.Any())
            RemoveItem(inventory.First().data, inventory.First().stackSize);
    }


    public void SaveData(GameData data)
    {
        //存储仓库
        data.inventory.Clear();
        foreach (var item in inventory)
        {
            data.inventory.Add(new KeyValuePair<string, int>(item.data.itemID, item.stackSize));
        }
        //存储装备
        data.equipment.Clear();
        foreach (var itemData in equipmentDictionary.Values)
        {
            data.equipment.Add(itemData.itemID);
        }
    }

    public void LoadData(GameData data)
    {
        //清空原先数据
        resetInventory();

        //从文件获取所有物品
        loadedItems = GetItemDataBase();

        //还原仓库
        foreach (var (k, v) in data.inventory)
        {
            var val = v;
            foreach (var item in loadedItems)
            {
                if (item != null && item.itemID == k)
                {
                    AddItem(item, val);
                }
            }
        }
        //还原装备
        foreach (var equipment in data.equipment)
        {
            foreach (var item in loadedItems)
            {
                if (item != null && item.itemID == equipment)
                {
                    EquipItem(item, false);
                }
            }
        }
    }

    public List<ItemData> GetItemDataBase()
    {
        List<ItemData> res = new List<ItemData>();

        ItemData[] datas = Resources.FindObjectsOfTypeAll<ItemData>();
        foreach(var data in datas)
        {
            res.Add(data);
        }

        return res;
    }

    public static ItemData GetItemDataByID(int id)
    {
        if (instance.loadedItems.Count == 0)
            instance.loadedItems = instance.GetItemDataBase();

        foreach (var item in instance.loadedItems)
            if (item.itemID == id.ToString())
                return item;
        return null;
    }
}
