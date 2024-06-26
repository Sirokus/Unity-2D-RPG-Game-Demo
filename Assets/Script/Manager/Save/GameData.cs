using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
    //存档信息
    public string saveIndex = Utility.GetCurTimeStringFile();
    public string saveName = "新建存档";
    public string saveTime = Utility.GetCurTimeString();
    public string levelName = "MainScene";
    public bool isNullSlot = true;

    //现金
    public int currency;

    //库存
    public List<KeyValuePair<string, int>> inventory = new List<KeyValuePair<string, int>>();
    public List<string> equipment = new List<string>();
    public List<Tuple<string, int, SerVector3>> dropItems = new List<Tuple<string, int, SerVector3>>();

    //检查点
    public string lastCheckPointID;

    //存储集合
    public Dictionary<string, bool> boolDictionary = new Dictionary<string, bool>();
    public Dictionary<string, float> floatDictionary = new Dictionary<string, float>();
    public Dictionary<string, SerVector3> vectorDictionary = new Dictionary<string, SerVector3>();

    //任务系统
    public List<SerTask> taskDataList = new List<SerTask>();

    public GameData(int currency)
    {
        this.currency=currency;
    }

    public void addValue(string key, bool value)
    {
        if (boolDictionary.ContainsKey(key))
            boolDictionary[key] = value;
        else
            boolDictionary.Add(key, value);
    }
    public void addValue(string key, float value)
    {
        if (floatDictionary.ContainsKey(key))
            floatDictionary[key] = value;
        else
            floatDictionary.Add(key, value);
    }
    public void addValue(string key, Vector3 value)
    {
        if (vectorDictionary.ContainsKey(key))
            vectorDictionary[key] = new SerVector3(value);
        else
            vectorDictionary.Add(key, new SerVector3(value));
    }

    public bool haveValue(string key)
    {
        return boolDictionary.ContainsKey(key) || floatDictionary.ContainsKey(key) || vectorDictionary.ContainsKey(key);
    }
    public bool haveValueb(string key)
    {
        return boolDictionary.ContainsKey(key);
    }
    public bool haveValuef(string key)
    {
        return floatDictionary.ContainsKey(key);
    }
    public bool haveValueV(string key)
    {
        return vectorDictionary.ContainsKey(key);
    }

    public bool getValueb(string key)
    {
        return boolDictionary[key];
    }
    public float getValuef(string key)
    {
        return floatDictionary[key];
    }
    public Vector3 getValueV(string key)
    {
        return vectorDictionary[key].Get();
    }
}
