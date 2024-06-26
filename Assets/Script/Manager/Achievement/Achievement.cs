using UnityEngine;

public class Achievement : MonoBehaviour
{
    public string achievementID;
    public bool isActived = false;
    public System.Action<bool> onActive;    //bool判别是否是存档加载时激活的成就

    [ContextMenu("Generate checkPoint ID")]
    private void GenerateCheckPointID()
    {
        achievementID = System.Guid.NewGuid().ToString();
    }

    protected virtual void Start()
    {

    }

    protected virtual void OnDestroy()
    {

    }

    public void Active(bool needTips = true)
    {
        onActive?.Invoke(needTips);
        isActived = true;
    }
}
