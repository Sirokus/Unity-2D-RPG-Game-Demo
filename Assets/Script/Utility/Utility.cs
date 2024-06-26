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
            return "材料";
        case EItemType.Equipment:
            return "装备";
        }
        return "无";
    }
    public static string ToString(EStat type)
    {
        switch (type)
        {
        case EStat.strength:
            return "力量";
        case EStat.agility:
            return "活力";
        case EStat.intelligence:
            return "智力";
        case EStat.vitality:
            return "血量加成";
        case EStat.maxHealth:
            return "最大血量";
        case EStat.armor:
            return "护甲";
        case EStat.evasion:
            return "闪避";
        case EStat.damage:
            return "伤害";
        case EStat.critChance:
            return "暴击概率";
        case EStat.critPower:
            return "暴击倍率";
        case EStat.fireDamage:
            return "火焰伤害";
        case EStat.iceDamage:
            return "冰冻伤害";
        case EStat.lightningDamage:
            return "雷电伤害";
        case EStat.magicResistance:
            return "魔法抗性";
        case EStat.isIgnited:
            return "火";
        case EStat.isChilled:
            return "冰";
        case EStat.isShocked:
            return "电";
        default:
            return "错误属性";
        }
    }
    public static string ToString(ERare rare)
    {
        switch (rare)
        {
        case ERare.White:
            return "白色";
        case ERare.Green:
            return "绿色";
        case ERare.Blue:
            return "蓝色";
        case ERare.Purple:
            return "紫色";
        case ERare.Gold:
            return "金色";
        case ERare.Red:
            return "红色";
        default:
            return "无属性";
        }
    }
    public static string ToString(GameAction action)
    {
        switch (action)
        {
        case GameAction.MoveRight:
            return "向右移动";
        case GameAction.MoveLeft:
            return "向左移动";
        case GameAction.Jump:
            return "跳跃";
        case GameAction.Attack:
            return "攻击";
        case GameAction.Counter:
            return "格挡";
        case GameAction.Dash:
            return "冲刺";
        case GameAction.Sword:
            return "投掷武器";
        case GameAction.Crystal:
            return "释放水晶";
        case GameAction.BlackHole:
            return "黑洞技能";
        case GameAction.FlaskUse:
            return "使用药剂";
        case GameAction.Interact:
            return "互动";
        }
        return "未识别游戏动作";
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
        //删除Json
        string jsonPath = Path.Combine(Application.dataPath, "Resources", path, JsonName) + ".json";
        if (File.Exists(jsonPath))
            File.Delete(jsonPath);
        else
            Debug.LogError("Delete Json failed！Not find the target file！");

        //删除Json的meta文件
        jsonPath += ".meta";
        if (File.Exists(jsonPath))
            File.Delete(jsonPath);
    }

    //输入Csv名称，相对于Resources文件夹的路径，返回该Csv文件的行列组成的数组
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
