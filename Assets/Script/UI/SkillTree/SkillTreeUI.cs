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
        //初始化所有UI状态
        foreach (SkillTreeNodeUI item in root)
        {
            item.ResetUI();
        }

        //更新初始可学习技能槽
        foreach (SkillTreeNodeUI item in root)
        {
            item.canUnlock = true;
            item.updateUI();
        }

        //载入存储的技能槽
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
