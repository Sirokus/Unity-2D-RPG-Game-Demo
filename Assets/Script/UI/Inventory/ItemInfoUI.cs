using TMPro;
using UnityEngine;

public class ItemInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemInfo;
    [SerializeField] private GameObject AttributeEntryUIPrefab;
    [SerializeField] public Transform AttributeParent;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void UpdateUI(ItemData data)
    {
        itemName.SetText(data.itemName);
        itemInfo.SetText(data.itemInfo);

        for (int i = 0; i < AttributeParent.childCount; i++)
        {
            Destroy(AttributeParent.GetChild(i).gameObject);
        }

        if (data.itemType == EItemType.Equipment)
        {
            EquipmentData equipData = data as EquipmentData;
            foreach (var attribute in equipData.attributes)
            {
                GameObject ui = Instantiate(AttributeEntryUIPrefab);
                ui.GetComponent<AttributeEntryUI>().setupValue(Utility.ToString(attribute.stat), attribute.Modifier.ToString());
                ui.transform.SetParent(AttributeParent, false);
            }
        }
    }
}
