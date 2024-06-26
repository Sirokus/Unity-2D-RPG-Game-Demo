using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    //����
    public static SaveManager ins;

    //���ô洢����
    [SerializeField] private string SavefileName = "SaveSlot";
    [SerializeField] public List<GameData> gameDatas = new List<GameData>();
    public bool EncryptData = true;

    //�洢�ӿں����л�
    public List<ISaveManager> saveTargets;
    private DataHandler dataHandler;

    //�洢�����
    public int slotNum => dataHandler.mapTable.Count;
    public int curSlot = 0;

    //ί��
    public System.Action onLoaded;

    private void Awake()
    {
        //����
        if (ins != null)
        {
            Destroy(this);
            return;
        }

        ins = this;

        //��ȡ���ʼ������
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
        //�л�����
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
        Debug.Log("ˢ�¶�ȡ��");

        saveTargets  = findAllSaveTarget();

        //���ó����ϵĶ�ȡ�ӿ�
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

    //�����´浵�������´浵������;
    public int CreateGame(bool savedOnCreate = true)
    {
        gameDatas.Add(new GameData(1500));
        if (gameDatas.Count == 1)
            gameDatas.Last().saveName = "�Զ�����";

        dataHandler.Save(gameDatas.Last(), slotNum, false);     //slotNum���¼�һ
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
