using System;
using System.Collections.Generic;
using UnityEngine;

public enum EquipmentType
{
    Weapon,
    Armor,
    Amulet,
    Flask
}

[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Equipment")]
public class EquipmentData : ItemData
{
    public EquipmentType equipmentType;

    [Serializable]
    public class Attribute
    {
        public EStat stat;
        public int Modifier;
    }
    public Attribute[] attributes;
    public ItemEffect[] effects;
    public float useCoolDown;
    public bool dontShowTips;

    //工艺部分
    [Serializable]
    public class Material
    {
        public ItemData itemData;
        public int num;
    }
    public Material[] materials;

    protected override void OnValidate()
    {
#if UNITY_EDITOR
        base.OnValidate();
        List<string[]> items = Utility.ReadCsv("Items", "Item");
        if (items == null)
            return;
        string[] words = items[int.Parse(itemID)];
        equipmentType = (EquipmentType)Enum.Parse(typeof(EquipmentType), words[6]);
#endif
    }

    public void AddModifiers()
    {
        PlayerStat playerStat = PlayerManager.playerStat;

        foreach (var attribute in attributes)
        {
            playerStat.getStatByType(attribute.stat).addModifier(attribute.Modifier);

            if (attribute.stat == EStat.maxHealth)
            {
                playerStat.IncreaseHealth(attribute.Modifier);
            }
        }
    }

    public void RemoveModifiers()
    {
        PlayerStat playerStat = PlayerManager.playerStat;

        foreach (var attribute in attributes)
        {
            playerStat.getStatByType(attribute.stat).removeModifier(attribute.Modifier);

            if (attribute.stat == EStat.maxHealth)
            {
                if (playerStat.currentHealth > playerStat.getMaxHealthValue())
                {
                    playerStat.DecreseHealth(playerStat.currentHealth - playerStat.getMaxHealthValue());
                }
                else
                {
                    playerStat.DecreseHealth(0);
                }
            }
        }
    }

    public bool executeEffects(Transform target)
    {
        foreach (var effect in effects)
        {
            effect.dontShowTips = dontShowTips;
            if (!effect.conditionCheck())
                return false;
        }

        foreach (var effect in effects)
        {
            effect.execute(target);
        }

        if (useCoolDown != 0)
        {
            PlayerUI.AddCooldownUI(itemIcon, itemName + "冷却", useCoolDown);
        }

        return true;
    }
}
