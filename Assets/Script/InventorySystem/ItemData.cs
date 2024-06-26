using System;
using System.Collections.Generic;
using UnityEngine;

public enum EItemType
{
    Material,
    Equipment
}

public enum ERare
{
    White,
    Green,
    Blue,
    Purple,
    Gold,
    Red
}

[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Item")]
public class ItemData : ScriptableObject
{
    public string itemID;

    public string itemName = "Item";
    public string itemInfo = "This is a Item.";
    public Sprite itemIcon;
    public EItemType itemType;
    public ERare rare;

    public int itemPrice = 500;

    protected virtual void OnValidate()
    {
#if UNITY_EDITOR
        //string path = AssetDatabase.GetAssetPath(this);
        //itemID = AssetDatabase.AssetPathToGUID(path);

        List<string[]> items = Utility.ReadCsv("Items", "Item");
        if (items == null)
            return;
        string[] words = items[int.Parse(itemID)];
        itemName = words[2];
        itemInfo = words[5];
        itemType = (EItemType)Enum.Parse(typeof(EItemType), words[3]);
        rare = (ERare)Enum.Parse(typeof(ERare), words[4]);
#endif
    }
}
