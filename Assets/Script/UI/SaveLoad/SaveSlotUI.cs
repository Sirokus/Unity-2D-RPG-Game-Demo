using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotUI : MonoBehaviour
{
    public TextMeshProUGUI index, slotName, slotTime;
    public Button saveBtn, loadBtn, deleteBtn, renameBtn;
    public TMP_InputField inputField;

    public int slotId;

    public void setup(int slotId)
    {
        this.slotId = slotId;
        index.text = slotId.ToString();
        slotTime.text = SaveManager.ins.gameDatas[this.slotId].saveTime;

        if (slotId == 0)
        {
            slotName.text = "自动保存";
            saveBtn.gameObject.SetActive(false);
            deleteBtn.gameObject.SetActive(false);
            renameBtn.enabled = false;
        }
        else
            slotName.text = SaveManager.ins.gameDatas[this.slotId].saveName;
    }

    private void Start()
    {
        saveBtn.onClick.AddListener(() =>
        {
            if (Save())
                TipsUI.AddTip("存档保存完成！", false);
        });

        loadBtn.onClick.AddListener(() =>
        {
            TipsUI.AddTip("读取存档成功！", false);
            SaveManager.ins.LoadGame(this.slotId);
        });

        deleteBtn.onClick.AddListener(() =>
        {
            TipsUI.AddTip("存档删除完成！", false);
            SaveManager.ins.DeleteGame(this.slotId);
            SaveLoadUI.removeSlotUI(this.slotId);
            Destroy(gameObject);
        });

        renameBtn.onClick.AddListener(() =>
        {
            inputField.gameObject.SetActive(true);
            inputField.text = slotName.text;
            KeyMgr.ins.pauseInput = true;
        });

        inputField.onEndEdit.AddListener((string s) =>
        {
            inputField.gameObject.SetActive(false);
            slotName.text = s;
            SaveManager.ins.ChangeSaveSlotName(s, this.slotId);
            KeyMgr.ins.pauseInput = false;
        });
    }

    public bool Save()
    {
        bool res = SaveManager.ins.SaveGame(this.slotId);
        slotTime.text = SaveManager.ins.gameDatas[this.slotId].saveTime;
        return res;
    }
}
