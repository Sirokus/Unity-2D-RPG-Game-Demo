using UnityEngine;

public class SkillTreeUI : MonoBehaviour, ISaveManager
{
    public static SkillTreeUI ins;
    public SkillTreeNodeUI[] root;

    [Header("Tiny Info UI")]
    [SerializeField] public SkillInfoUI infoUI;
    public bool infoUITracePointer = false;

    private void Awake()
    {
        ins = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (SkillTreeNodeUI item in root)
        {
            item.canUnlock = true;
            item.Init();
        }
    }

    private void FixedUpdate()
    {
        if (infoUITracePointer)
        {
            Vector3 mousePos = Input.mousePosition;
            infoUI.transform.position = new Vector3(mousePos.x, mousePos.y, infoUI.transform.position.z);
        }
    }

    public void LoadData(GameData data)
    {
        //��ʼ������UI״̬
        foreach (SkillTreeNodeUI item in root)
        {
            item.ResetUI();
        }

        //���³�ʼ��ѧϰ���ܲ�
        foreach (SkillTreeNodeUI item in root)
        {
            item.canUnlock = true;
            item.updateUI();
        }

        //����洢�ļ��ܲ�
        foreach (SkillTreeNodeUI item in root)
        {
            item.LoadFrom(data);
        }
    }

    public void SaveData(GameData data)
    {
        foreach (SkillTreeNodeUI item in root)
        {
            item.SaveTo(data);
        }
    }
}
