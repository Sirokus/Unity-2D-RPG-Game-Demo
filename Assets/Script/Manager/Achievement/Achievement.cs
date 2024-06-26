using UnityEngine;

public class Achievement : MonoBehaviour
{
    public string achievementID;
    public bool isActived = false;
    public System.Action<bool> onActive;    //bool�б��Ƿ��Ǵ浵����ʱ����ĳɾ�

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
