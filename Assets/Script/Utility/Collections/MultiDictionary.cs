using System;
using System.Collections.Generic;

[Serializable]
public class MultiDictionary<TKey, TValue> : Dictionary<TKey, List<TValue>>
{
    public void Add(TKey key, TValue value)
    {
        if (this.TryGetValue(key, out List<TValue> list))
        {
            list.Add(value);
        }
        else
        {
            base.Add(key, new List<TValue> { value });
        }
    }

    public new void Remove(TKey key)
    {
        if (this.TryGetValue(key, out List<TValue> list))
        {
            if (list.Count > 1)
                list.RemoveAt(0);
            else
                base.Remove(key);
        }
        else
            throw new KeyNotFoundException($"The key '{key}' was not found in the dictionary.");
    }

    public void Remove(TKey key, TValue value)
    {
        if (this.TryGetValue(key, out List<TValue> list))
        {
            if (list.Count > 1)
                list.Remove(value);
            else
                base.Remove(key);
        }
        else
            throw new KeyNotFoundException($"The key '{key}' was not found in the dictionary.");
    }
}
