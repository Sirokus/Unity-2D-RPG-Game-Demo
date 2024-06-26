using TMPro;
using UnityEngine;

public class TaskTargetSlotUI : MonoBehaviour
{
    public TextMeshProUGUI targetNameTxt, targetProgressTxt;
    int maxProgress;

    public void Setup(string targetName, int curProgress, int maxProgress)
    {
        this.maxProgress = maxProgress;

        targetNameTxt.text = targetName;
        targetProgressTxt.text = curProgress + "/" + maxProgress;
    }

    public void UpdateUI(int curProgress)
    {
        targetProgressTxt.text = curProgress + "/" + maxProgress;
    }
}
