using TMPro;
using UnityEngine;

public class SkillInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemInfo;
    [SerializeField] private TextMeshProUGUI itemSpend;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void UpdateUI(SkillTreeNodeUI node)
    {
        itemName.SetText(node.skillName);
        itemInfo.SetText(node.skillDescription);
        itemSpend.SetText("»¨·Ñ: " + node.spend);
    }
}
