using UnityEngine;

public class GameManager : MonoBehaviour, ISaveManager
{
    public static GameManager ins;

    [Header("SaveManager")]
    public bool refreshSaveWhenStart = true;

    [Header("Check Point")]
    [SerializeField] private CheckPoint[] checkPoints;
    public CheckPoint lastCheckPoint;

    [Header("Player")]
    public bool isDead;

    [Header("Camera")]
    public ParallaxBackground[] bgs;

    private void Awake()
    {
        if (ins != null)
        {
            Destroy(this);
            return;
        }
        ins = this;

        bgs = FindObjectsByType<ParallaxBackground>(FindObjectsSortMode.None);
    }

    private void Start()
    {
        checkPoints = FindObjectsOfType<CheckPoint>();

        SaveManager.ins.onLoaded += onLoaded;

        if (refreshSaveWhenStart)
        {
            if (SaveManager.ins.slotNum <= 0)
                SaveManager.ins.CreateGame();

            //ÁÙÊ±¼ÓµÄ
            if (SaveManager.ins.gameDatas[SaveManager.ins.curSlot].isNullSlot)
            {
                SaveManager.ins.saveTargets  = SaveManager.ins.findAllSaveTarget();
                SaveManager.ins.SaveGame(SaveManager.ins.curSlot);
            }
            else
                SaveManager.ins.RefreshGame(SaveManager.ins.curSlot);
        }
    }

    public static void RestartGame()
    {
        SaveManager.AutoSave();
        SaveManager.ins.LoadGame(0);
    }

    public void onLoaded()
    {
        if (isDead && lastCheckPoint)
            PlayerManager.GetPlayer().transform.position = lastCheckPoint.transform.position;
        isDead = false;
    }

    public void LoadData(GameData data)
    {
        foreach (CheckPoint checkPoint in checkPoints)
        {
            if (data.haveValue(checkPoint.ID) && data.getValueb(checkPoint.ID) == true)
            {
                checkPoint.ActivateCheckPoint();
            }
            if (checkPoint.ID == data.lastCheckPointID)
            {
                if (!data.haveValueV("Player"))
                    PlayerManager.GetPlayer().transform.position = checkPoint.transform.position;
                lastCheckPoint = checkPoint;
            }
        }
    }

    public void SaveData(GameData data)
    {
        data.lastCheckPointID = lastCheckPoint?.ID;
        foreach (CheckPoint checkPoint in checkPoints)
        {
            data.addValue(checkPoint.ID, checkPoint.activated);
        }
    }

    public void PauseGame(bool bPause)
    {
        Time.timeScale = bPause ? 0 : 1;
        KeyMgr.ins.pauseInput = bPause;
    }

    public void ChangeCam(Transform cam)
    {
        foreach (var bg in bgs)
            bg.ChangeCamera(cam);
    }
}
