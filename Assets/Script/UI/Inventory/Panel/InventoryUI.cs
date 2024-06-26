using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("Inventory UI")]
    public List<EItemType> inventorySupportUI = new List<EItemType>() { EItemType.Material, EItemType.Equipment };
    public Dictionary<EItemType, Transform> inventorySlotParents = new Dictionary<EItemType, Transform>();
    public Transform inventorySlotParentTemplate;
    public GameObject ItemSlotPrefab;


    protected virtual void Awake()
    {
        foreach (EItemType item in inventorySupportUI)
        {
            inventorySlotParents.Add(item, Instantiate(inventorySlotParentTemplate).transform);
            inventorySlotParents.Last().Value.SetParent(inventorySlotParentTemplate.parent, false);
        }
    }

    protected virtual void Start()
    {
    }
}
