using UnityEngine;

public class StatInfoUI : MonoBehaviour
{
    [SerializeField] private GameObject AttributeEntryUIPrefab;
    [SerializeField] public Transform AttributeParent;
    public bool haveMod = false;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void UpdateUI(EStat type)
    {
        haveMod = false;
        for (int i = 0; i < AttributeParent.childCount; i++)
            Destroy(AttributeParent.GetChild(i).gameObject);

        EquipmentType[] equipmentTypes = System.Enum.GetValues(typeof(EStat)) as EquipmentType[];
        var equipments = Inventory.instance.equipmentDictionary;
        foreach (EquipmentType equipmentType in equipmentTypes)
        {
            if (!equipments.ContainsKey(equipmentType)) continue;

            foreach (var mod in ((EquipmentData)equipments[equipmentType]).attributes)
            {
                if (mod.stat != type) continue;
                GameObject ui = Instantiate(AttributeEntryUIPrefab);
                ui.GetComponent<AttributeEntryUI>().setupValue(equipments[equipmentType].itemName, mod.Modifier.ToString());
                ui.transform.SetParent(AttributeParent, false);
                haveMod = true;
            }
        }
    }
}
