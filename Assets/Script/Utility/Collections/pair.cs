using System;

[Serializable]
public class pair<TKey, TValue>
{
    public TKey key;
    public TValue value;

    public pair(TKey key, TValue value)
    {
        this.key = key;
        this.value = value;
    }
}