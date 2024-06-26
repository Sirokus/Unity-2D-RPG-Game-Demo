using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AchievementManager : MonoBehaviour, ISaveManager
{
    public static AchievementManager ins;

    public Achievement[] Achievements;

    private void Awake()
    {
        if (ins != null)
        {
            Destroy(gameObject);
            return;
        }

        ins = this;

        DontDestroyOnLoad(gameObject);
    }

    public void LoadData(GameData data)
    {
        foreach (var achievement in Achievements)
        {
            if (data.haveValueb(achievement.achievementID) && data.getValueb(achievement.achievementID))
            {
                achievement.Active(false);
            }
        }
    }

    public void SaveData(GameData data)
    {
        foreach (var achievement in Achievements)
        {
            data.addValue(achievement.achievementID, achievement.isActived);
        }
    }

#if UNITY_EDITOR
    [ContextMenu("CreateAchievementData")]
    private void CreateAchievementData()
    {
        //获取所有挂载的成就组件
        Achievements = GetComponents<Achievement>();

        //获取成就配置表
        List<string[]> configs = Utility.ReadCsv("AchievementConfig", "Achievement");

        //获取Icons表
        Sprite[] sprites = Resources.LoadAll<Sprite>("Art/#2 - Transparent Icons & Drop Shadow");

        //按配置表为对应的成就创建Scriptable Object
        foreach (var achievement in Achievements)
        {
            foreach (var config in configs)
            {
                if (config[0] != achievement.GetType().Name)
                    continue;

                //创建配置好的SO实例
                AchievementData so = ScriptableObject.CreateInstance<AchievementData>();
                so.AchiId = config[0];
                so.AchiName = config[1];
                so.AchiDesc = config[2];
                so.AchiIcon = sprites[int.Parse(config[3]) + 1];

                //创建路径上的资源实例
                AssetDatabase.CreateAsset(so, @"Assets/Resources/ScriptableObject/Achievement/" + config[0] + ".asset");
                AssetDatabase.SaveAssets();
            }
        }
        AssetDatabase.Refresh();
    }
#endif
}
