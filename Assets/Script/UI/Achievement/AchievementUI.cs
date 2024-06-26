using System.Collections.Generic;
using UnityEngine;

public class AchievementUI : MonoBehaviour
{
    public Transform achievementParent;
    public GameObject achievementPrefab;

    // Start is called before the first frame update
    private void Start()
    {
        //获取成就配置表
        List<string[]> configs = Utility.ReadCsv("AchievementConfig", "Achievement");

        //获取Icons表
        Sprite[] sprites = Resources.LoadAll<Sprite>("Art/#2 - Transparent Icons & Drop Shadow");

        //按配置表为对应的成就创建Scriptable Object
        foreach (var achievement in AchievementManager.ins.Achievements)
        {
            foreach (var config in configs)
            {
                if (config[0] != achievement.GetType().Name)
                    continue;
                createAchiSlot(achievement, config[0], sprites[int.Parse(config[3])], config[1], config[2]);
                break;
            }
        }
    }

    public void createAchiSlot(Achievement achievement, string id, Sprite sprite, string name, string desc)
    {
        //创建和配置UI
        AchievementSlotUI ui = Instantiate(achievementPrefab).GetComponent<AchievementSlotUI>();
        ui.transform.SetParent(achievementParent, false);
        ui.gameObject.SetActive(true);
        ui.Setup(achievement, sprite, name, desc);
    }
}
