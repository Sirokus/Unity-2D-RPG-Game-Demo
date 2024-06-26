using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    //单例
    public static SaveManager ins;

    //配置存储数据
    [SerializeField] private string SavefileName = "SaveSlot";
    [SerializeField] public List<GameData> gameDatas = new List<GameData>();
    public bool EncryptData = true;

    //存储接口和序列化
    public List<ISaveManager> saveTargets;
    private DataHandler dataHandler;

    //存储槽相关
    public int slotNum => dataHandler.mapTable.Count;
    public int curSlot = 0;

    //委托
    public System.Action onLoaded;

    private void Awake()
    {
        //单例
        if (ins != null)
        {
            Destroy(this);
            return;
        }

        ins = this;

        //读取与初始化数据
        dataHandler = new DataHandler("SaveSlots", SavefileName, EncryptData);

        for (int i = 0; i < slotNum; i++)
        {
            gameDatas.Add(dataHandler.Load(i));
        }

        saveTargets = findAllSaveTarget();
    }

    public void LoadGame(int slotIndex, bool bForceReloadScene = true)
    {
        StartCoroutine(LoadGameCoroutine(slotIndex, bForceReloadScene));
    }

    private IEnumerator LoadGameCoroutine(int slotIndex, bool bForceReloadScene = true)
    {
        //切换场景
        if (bForceReloadScene || SceneManager.GetActiveScene().name !=  gameDatas[slotIndex].levelName)
        {
            IEnumerable<IBeforeLoadNextLevel> objects = FindObjectsOfType<MonoBehaviour>(true).OfType<IBeforeLoadNextLevel>();
            foreach (var obj in objects)
            {
                obj.Execute(slotIndex);
            }

            AsyncOperation ao = SceneManager.LoadSceneAsync("LoadScene");
            yield return ao;

            LoadGame loadGame = FindAnyObjectByType<LoadGame>();
            ao = loadGame.ao;
            loadGame.Setup(gameDatas[slotIndex].levelName, slotIndex);
            yield return ao;
        }

        curSlot = slotIndex;
    }

    public void RefreshGame(int slotIndex)
    {
        Debug.Log("刷新读取！");

        saveTargets  = findAllSaveTarget();

        //调用场景上的读取接口
        foreach (var saveTarget in saveTargets)
        {
            saveTarget.LoadData(gameDatas[slotIndex]);
        }

        onLoaded?.Invoke();
    }

    public bool SaveGame(int slotIndex)
    {
        if (SceneManager.GetActiveScene().name == "StartScene" || SceneManager.GetActiveScene().name == "LoadScene")
            return false;

        foreach (var saveTarget in saveTargets)
        {
            saveTarget.SaveData(gameDatas[slotIndex]);
        }
        gameDatas[slotIndex].levelName = SceneManager.GetActiveScene().name;
        gameDatas[slotIndex].isNullSlot = false;
        dataHandler.Save(gameDatas[slotIndex], slotIndex);

        return true;
    }

    public void ChangeSaveSlotName(string name, int slotIndex)
    {
        gameDatas[slotIndex].saveName = name;
        dataHandler.Save(gameDatas[slotIndex], slotIndex, false);
    }

    //创建新存档，返回新存档的索引;
    public int CreateGame(bool savedOnCreate = true)
    {
        gameDatas.Add(new GameData(1500));
        if (gameDatas.Count == 1)
            gameDatas.Last().saveName = "自动保存";

        dataHandler.Save(gameDatas.Last(), slotNum, false);     //slotNum更新加一
        if (savedOnCreate)
            SaveGame(slotNum - 1);
        return slotNum - 1;
    }

    public void DeleteGame(int slotIndex)
    {
        gameDatas.RemoveAt(slotIndex);
        dataHandler.Delete(slotIndex);
    }

    private void OnApplicationQuit()
    {
        if (SceneManager.GetActiveScene().name != "StartScene")
            SaveGame(0);
    }

    public List<ISaveManager> findAllSaveTarget()
    {
        IEnumerable<ISaveManager> saveTargetss = FindObjectsOfType<MonoBehaviour>(true).OfType<ISaveManager>();
        return new List<ISaveManager>(saveTargetss);
    }

    public static void AutoSave()
    {
        ins.SaveGame(0);
    }
}
