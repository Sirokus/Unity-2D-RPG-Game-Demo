using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat
{
    [SerializeField] private int baseValue;

    public Action<int, bool> onModified;
    public List<int> modifiers;

    public int getValue()
    {
        int finalValue = baseValue;

        foreach (int modifier in modifiers)
        {
            finalValue += modifier;
        }

        return finalValue;
    }

    public int getBaseValue() => baseValue;

    public void setValue(int value)
    {
        baseValue = value;
    }

    public void addModifier(int _modifier)
    {
        modifiers.Add(_modifier);
        onModified?.Invoke(_modifier, true);
    }

    public void removeModifier(int _modifier)
    {
        modifiers.Remove(_modifier);
        onModified?.Invoke(_modifier, false);
    }

}
