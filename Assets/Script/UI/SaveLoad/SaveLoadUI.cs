using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveLoadUI : MonoBehaviour
{
    public static SaveLoadUI ins;
    public Transform SaveSlotUIParent;
    public GameObject SaveSlotUIObj;
    public List<SaveSlotUI> saveSlotUIs = new List<SaveSlotUI>();

    public Button createSaveSlotBtn;
    public bool canCreate = true;


    private void Awake()
    {
        ins = this;
    }

    private void Start()
    {
        for (int i = 0; i < SaveManager.ins.slotNum; i++)
        {
            GameObject obj = Instantiate(SaveSlotUIObj);
            obj.gameObject.SetActive(true);
            obj.transform.SetParent(SaveSlotUIParent, false);
            SaveSlotUI ui = obj.GetComponent<SaveSlotUI>();
            saveSlotUIs.Add(ui);
            ui.setup(i);
        }

        createSaveSlotBtn.transform.SetAsLastSibling();
        createSaveSlotBtn.onClick.AddListener(() =>
        {
            if (!canCreate)
                return;

            if (saveSlotUIs.Count > 100)
            {
                TipsUI.AddTip("存储槽已达上限！！！", false);
                return;
            }

            SaveManager.ins.CreateGame(false);

            GameObject obj = Instantiate(SaveSlotUIObj);
            obj.gameObject.SetActive(true);
            obj.transform.SetParent(SaveSlotUIParent, false);
            SaveSlotUI ui = obj.GetComponent<SaveSlotUI>();
            saveSlotUIs.Add(ui);
            ui.setup(SaveManager.ins.slotNum - 1);
            ui.Save();

            createSaveSlotBtn.transform.SetAsLastSibling();

            canCreate = false;
            TimerManager.addTimer(1, false, () => { canCreate = true; });
        });
    }

    public static void removeSlotUI(int index)
    {
        ins.saveSlotUIs.RemoveAt(index);
        for (int i = index; i < ins.saveSlotUIs.Count; ++i)
        {
            ins.saveSlotUIs[i].setup(i);
        }
    }
}
