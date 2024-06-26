using System;

[Serializable]
public class InventoryItem
{
    public ItemData data;
    public int stackSize;

    public Action<int> onChanged;

    public InventoryItem(ItemData data)
    {
        this.data=data;
        stackSize = 1;
    }

    public void AddStack(int num = 1)
    {
        stackSize += num;
        onChanged?.Invoke(stackSize);
    }

    public void RemoveStack(int num = 1)
    {
        stackSize -= num;
        onChanged?.Invoke(stackSize);
    }
}
