using System;
using UnityEngine;

public class NPC : MonoBehaviour, IInteractable, ISaveManager
{
    public string interactName { get => "Â½ÈÊ¼Ö"; }

    public int dialoguePart = 0;

    public void Interact()
    {
        string dialogueName = "NPC_1_Dialogue_First";
        switch (dialoguePart)
        {
        case 1:
            dialogueName = "NPC_1_Dialogue_First_BeforeComplete";
            break;
        case 2:
            dialogueName = "NPC_1_Dialogue_Last";
            break;
        }

        if (dialoguePart != -1)
            UIManager.ins.dialogueManager.OpenDialogue(dialogueName);

        Vector3 scale = Vector3.one;
        scale.x = PlayerManager.playerPos.x - transform.position.x < 0 ? 1 : -1;
        transform.localScale = scale;
    }

    // Start is called before the first frame update 
    void Start()
    {
        EventManager.AddListener(EventName.OnTaskComplete, OnTaskComplete);
        EventManager.AddListener(EventName.OnTaskAdd, OnTaskAdd);
    }

    void OnTaskComplete(object sender, EventArgs _args)
    {
        TaskCompleteArgs args = _args as TaskCompleteArgs;
        if (args != null && args.isFirstComplete)
        {
            if (dialoguePart == 1 && args.task.chainId == 1 && args.task.subId == 1)
            {
                dialoguePart = -1;
            }
        }
    }

    void OnTaskAdd(object sender, EventArgs _args)
    {
        TaskAddArgs args = _args as TaskAddArgs;
        if (args != null)
        {
            if (args.task.chainId == 3 && args.task.subId == 1)
            {
                dialoguePart = 2;
            }
            if (dialoguePart == 0 && args.task.chainId == 1 && args.task.subId == 1)
            {
                dialoguePart = 1;
            }
        }
    }

    public void LoadData(GameData data)
    {
        dialoguePart = (int)data.getValuef(interactName);
    }

    public void SaveData(GameData data)
    {
        data.addValue(interactName, dialoguePart);
    }
}
