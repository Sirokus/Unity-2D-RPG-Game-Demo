using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TMP_Text nameTxt;
    [SerializeField] private TMP_Text dialogueTxt;
    [SerializeField] private Image srL;
    [SerializeField] private Image srR;


    [Header("Dialogue")]
    [SerializeField] private List<string[]> dialogue = new List<string[]>();
    private int curIndex;
    public float typeSpeed = .01f;

    [Header("Controllers")]
    [SerializeField] private Button Next;
    [SerializeField] private Transform selectorBtnParent;
    private Button[] selectorBtns;
    private List<bool> selectState = new List<bool>();


    [Serializable]
    public struct AtlasNameMapping
    {
        public string name;
        public string atlasName;
    }
    [SerializeField] private AtlasNameMapping[] atlasNameMappings;
    public SpriteAtlas atlas;


    private void Awake()
    {
        Next.onClick.AddListener(() => ShowDialogueRow());
        selectorBtns = selectorBtnParent.GetComponentsInChildren<Button>(true);

        transform.gameObject.SetActive(false);
    }

    private string getAtlasNameByName(string name)
    {
        foreach (var mapping in atlasNameMappings)
        {
            if (mapping.name == name)
                return mapping.atlasName;
        }
        return null;
    }

    private void UpdateSprite(string name = "", bool isLeft = true)
    {
        Sprite sprite = atlas.GetSprite(getAtlasNameByName(name));
        if (sprite)
        {
            if (isLeft)
                srL.sprite = sprite;
            else
                srR.sprite = sprite;
        }
        srL.enabled = isLeft;
        srR.enabled = !isLeft;
    }

    TweenerCore<string, string, StringOptions> tweener;
    private void UpdateText(string _name, string _dialogue)
    {
        nameTxt.text = _name;
        tweener?.Kill();
        tweener = DOTween.To(() => string.Empty, value => dialogueTxt.text = value, _dialogue, _dialogue.Length * typeSpeed).SetUpdate(true).SetEase(Ease.Linear);
    }
    public void ShowDialogueRow()
    {
        string[] cell = dialogue[curIndex];

        //对话的特殊操作
        if (cell[6] != "")
        {
            string[] word = cell[6].Split('@');
            if (word[0] == "InitSelect")
            {
                selectState.Clear();
                for (int i = 0; i < int.Parse(word[1]); i++)
                    selectState.Add(false);
            }
        }

        if (cell[0] == "#")
        {
            for (int i = 0; i < selectorBtns.Length; ++i)
                selectorBtns[i].gameObject.SetActive(false);

            UpdateText(cell[2], cell[4]);
            UpdateSprite(cell[2], cell[3] == "左");
            curIndex = int.Parse(cell[5]);
            ReadCommand(cell[7]);
        }
        else if (cell[0] == "&")
        {
            int selectionCount = 0;
            int tmpIndex = curIndex;
            while (tmpIndex < dialogue.Count && dialogue[tmpIndex][0] == "&")
            {
                int tmp = tmpIndex;
                int selectIndex = tmp - curIndex;
                selectorBtns[selectionCount].onClick.RemoveAllListeners();
                selectorBtns[selectionCount].onClick.AddListener(() =>
                {
                    curIndex = int.Parse(dialogue[tmp][5]);
                    selectState[selectIndex] = true;
                    ShowDialogueRow();
                    ReadCommand(dialogue[tmp][7]);
                });

                bool canShow = true;
                if (dialogue[tmpIndex][6] == "last")
                {
                    for (int i = 0; i < selectState.Count; i++)
                        if (i != selectionCount)
                            canShow &= selectState[i];
                }
                selectorBtns[selectionCount].interactable = !selectState[selectIndex];
                selectorBtns[selectionCount].gameObject.SetActive(canShow);
                selectorBtns[selectionCount].GetComponentInChildren<TextMeshProUGUI>().text = dialogue[tmpIndex][4];


                tmpIndex++;
                selectionCount++;
            }

            for (int i = selectionCount; i < selectorBtns.Length; ++i)
                selectorBtns[i].gameObject.SetActive(false);
        }
        else if (cell[0] == "END")
        {
            Time.timeScale = 1;
            KeyMgr.ins.pauseInput = false;
            gameObject.SetActive(false);
            ReadCommand(cell[7]);
        }
    }

    public void OpenDialogue(string dialogueName)
    {
        transform.gameObject.SetActive(true);
        dialogue = Utility.ReadCsv(dialogueName, "Dialogue");
        curIndex = 0;

        Time.timeScale = 0;
        KeyMgr.ins.pauseInput = true;
        ShowDialogueRow();
    }

    public void ReadCommand(string command)
    {
        string[] word = command.Split('@');
        if (word[0] == "打开")
        {
            GameObject ui = null;
            switch (word[1].Substring(0, word[1].Length - 1))
            {
            case "商店界面":
                ui = UIManager.ins.shopUI.gameObject; break;
            }
            ui.SetActive(true);
        }
        else if (word[0] == "关闭")
        {
            GameObject ui = null;
            switch (word[1])
            {
            case "商店界面":
                ui = UIManager.ins.shopUI.gameObject; break;
            }
            ui.SetActive(false);
        }
        else if (word[0] == "请求任务")
        {
            string[] taskIDs = word[1].Split("|");
            int chainID = int.Parse(taskIDs[0]);
            int subID = int.Parse(taskIDs[1]);
            UIManager.ins.taskUI.requestTask(chainID, subID);
        }
        else if (word[0] == "退出游戏")
        {
            TimerManager.addTimer(3f, false, () => UIManager.ins.ExitGame());
        }
    }
}
