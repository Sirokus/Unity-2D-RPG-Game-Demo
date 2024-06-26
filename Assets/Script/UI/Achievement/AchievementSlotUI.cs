using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementSlotUI : MonoBehaviour, IBeforeLoadNextLevel
{
    public Image icon;
    public TMP_Text achiName, achiDesc;

    private Achievement achi;
    private bool isActived = false;

    private bool isEnable = false;

    public void Setup(Achievement achi, Sprite icon, string name, string desc)
    {
        this.icon.sprite = icon;
        achiName.text = name;
        achiDesc.text = desc;
        GetComponent<CanvasGroup>().alpha = achi.isActived ? 1 : 0.6f;
        isActived = achi.isActived;

        this.achi = achi;
        achi.onActive += onActive;
        isEnable = true;
    }

    public void onActive(bool needTips)
    {
        if (isActived)
            return;

        if (needTips)
            TipsUI.AddTipAchi(achiName.text, achiDesc.text);

        if (TryGetComponent(out CanvasGroup group))
        {
            group.alpha = 1;
        }

        achi.onActive -= onActive;
    }

    public void Execute(int slotIntex)
    {
        if (isEnable)
            achi.onActive -= onActive;
    }
}
