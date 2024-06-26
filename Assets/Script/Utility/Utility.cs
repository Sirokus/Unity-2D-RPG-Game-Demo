using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class Utility
{
    public static string ToString(EItemType type)
    {
        switch (type)
        {
        case EItemType.Material:
            return "����";
        case EItemType.Equipment:
            return "װ��";
        }
        return "��";
    }
    public static string ToString(EStat type)
    {
        switch (type)
        {
        case EStat.strength:
            return "����";
        case EStat.agility:
            return "����";
        case EStat.intelligence:
            return "����";
        case EStat.vitality:
            return "Ѫ���ӳ�";
        case EStat.maxHealth:
            return "���Ѫ��";
        case EStat.armor:
            return "����";
        case EStat.evasion:
            return "����";
        case EStat.damage:
            return "�˺�";
        case EStat.critChance:
            return "��������";
        case EStat.critPower:
            return "��������";
        case EStat.fireDamage:
            return "�����˺�";
        case EStat.iceDamage:
            return "�����˺�";
        case EStat.lightningDamage:
            return "�׵��˺�";
        case EStat.magicResistance:
            return "ħ������";
        case EStat.isIgnited:
            return "��";
        case EStat.isChilled:
            return "��";
        case EStat.isShocked:
            return "��";
        default:
            return "��������";
        }
    }
    public static string ToString(ERare rare)
    {
        switch (rare)
        {
        case ERare.White:
            return "��ɫ";
        case ERare.Green:
            return "��ɫ";
        case ERare.Blue:
            return "��ɫ";
        case ERare.Purple:
            return "��ɫ";
        case ERare.Gold:
            return "��ɫ";
        case ERare.Red:
            return "��ɫ";
        default:
            return "������";
        }
    }
    public static string ToString(GameAction action)
    {
        switch (action)
        {
        case GameAction.MoveRight:
            return "�����ƶ�";
        case GameAction.MoveLeft:
            return "�����ƶ�";
        case GameAction.Jump:
            return "��Ծ";
        case GameAction.Attack:
            return "����";
        case GameAction.Counter:
            return "��";
        case GameAction.Dash:
            return "���";
        case GameAction.Sword:
            return "Ͷ������";
        case GameAction.Crystal:
            return "�ͷ�ˮ��";
        case GameAction.BlackHole:
            return "�ڶ�����";
        case GameAction.FlaskUse:
            return "ʹ��ҩ��";
        case GameAction.Interact:
            return "����";
        }
        return "δʶ����Ϸ����";
    }
    public static string GetCurTimeString()
    {
        return DateTime.Now.ToLocalTime().ToString("u");
    }
    public static string GetCurTimeStringFile()
    {
        return DateTime.Now.ToLocalTime().ToString("yy-MM-d-HH.mm.ss");
    }

    public static string ReadJson(string JsonName, string path = "")
    {
        string jsonPath = Path.Combine(Application.dataPath, "Resources", path, JsonName) + ".json";
        using (StreamReader sr = File.OpenText(jsonPath))
        {
            string json = sr.ReadToEnd();
            return json;
        }
    }
    public static void WriteJson(string JsonContent, string JsonName, string path = "")
    {
        string jsonPath = Path.Combine(Application.dataPath, "Resources", path, JsonName) + ".json";
        using (StreamWriter sr = new StreamWriter(jsonPath))
        {
            sr.Write(JsonContent);
        }
    }
    public static void DeleteJson(string JsonName, string path = "")
    {
        //ɾ��Json
        string jsonPath = Path.Combine(Application.dataPath, "Resources", path, JsonName) + ".json";
        if (File.Exists(jsonPath))
            File.Delete(jsonPath);
        else
            Debug.LogError("Delete Json failed��Not find the target file��");

        //ɾ��Json��meta�ļ�
        jsonPath += ".meta";
        if (File.Exists(jsonPath))
            File.Delete(jsonPath);
    }

    //����Csv���ƣ������Resources�ļ��е�·�������ظ�Csv�ļ���������ɵ�����
    public static List<string[]> ReadCsv(string CsvName, string path = "")
    {
        string csvPath = Path.Combine(Application.dataPath, "Resources", path, CsvName) + ".csv";
        using (StreamReader sr = File.OpenText(csvPath))
        {
            List<string[]> res = new List<string[]>();
            string[] text = sr.ReadToEnd().Split("\n");
            for (int i = 1; i < text.Length; i++)
                res.Add(text[i].Split(","));
            return res;
        }
    }
}
