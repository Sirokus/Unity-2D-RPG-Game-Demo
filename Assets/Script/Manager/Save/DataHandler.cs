using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class DataHandler
{
    private string dataPath = "";
    private string dataFileName = "";
    private const string mapTableName = "saveSlotTable";
    public List<string> mapTable;

    private bool encryptData = false;
    private string codeWord = "man!ӭ����������������˴������������ָо��Ҵ�δ�й�";

    public DataHandler(string dataPath, string dataFileName, bool encryptData)
    {
        //��ʼ��·������������Json�ļ���
        this.dataPath = dataPath;
        this.dataFileName = dataFileName;
        this.encryptData=encryptData;

        //��ʼ����ź�Json���Ƶ�ӳ���
        string tablePath = Path.Combine(Application.dataPath, "Resources", dataPath, mapTableName) + ".json";
        if (File.Exists(tablePath))
        {
            mapTable = JsonConvert.DeserializeObject<List<string>>(Utility.ReadJson(mapTableName, dataPath));
        }
        else
        {
            mapTable = new List<string>();
        }
    }

    public void Save(GameData data, int slotIndex, bool recordTime = true)
    {
        try
        {
            if (recordTime)
                data.saveTime = Utility.GetCurTimeString();

            //string json = JsonUtility.ToJson(data, true);

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.Auto;
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            string json = JsonConvert.SerializeObject(data, settings);

            if (encryptData)
                json = EncryptDecrypt(json);

            string name = dataFileName + data.saveIndex;
            Utility.WriteJson(json, name, dataPath);

            //����ӳ���ļ�
            if (slotIndex >= mapTable.Count)
                mapTable.Add(name);
            else
                mapTable[slotIndex] = name;

            UpdateMapTable();
        }
        catch (Exception e)
        {
            Debug.LogError("Error to try access file path : " + Path.Combine(dataPath, dataFileName + data.saveIndex) + "\n" + e);
        }
    }

    public GameData Load(int slotIndex)
    {
        try
        {
            string json = Utility.ReadJson(mapTable[slotIndex], dataPath);

            if (encryptData)
                json = EncryptDecrypt(json);

            //return JsonUtility.FromJson<GameData>(json);
            return JsonConvert.DeserializeObject<GameData>(json);
        }
        catch (Exception e)
        {
            Debug.LogError("Error to try access file path : " + Path.Combine(dataPath, mapTable[slotIndex]) + "\n" + e);
        }

        return null;
    }

    public void Delete(int slotIndex)
    {
        string jsonName = mapTable[slotIndex];
        Utility.DeleteJson(jsonName, dataPath);

        mapTable.RemoveAt(slotIndex);
        UpdateMapTable();
    }

    public void UpdateMapTable()
    {
        string json = JsonConvert.SerializeObject(mapTable);
        Utility.WriteJson(json, mapTableName, dataPath);
    }

    private string EncryptDecrypt(string data)
    {
        StringBuilder modifiedData = new StringBuilder();

        for (int i = 0; i < data.Length; i++)
        {
            modifiedData.Append((char)(data[i] ^ codeWord[i % codeWord.Length]));
        }

        return modifiedData.ToString();
    }
}
