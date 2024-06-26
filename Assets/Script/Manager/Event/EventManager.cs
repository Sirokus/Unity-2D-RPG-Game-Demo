using System;
using System.Collections.Generic;

public static class EventTriggerExt
{
    public static void TriggerEvent(this object sender, string eventName)
    {
        EventManager.instance.TriggerEvent(eventName, sender);
    }

    public static void TriggerEvent(this object sender, string eventName, EventArgs args)
    {
        EventManager.instance.TriggerEvent(eventName, sender, args);
    }
}

public class EventManager : SingletonBase<EventManager>
{
    private Dictionary<string, EventHandler> handlerDic = new Dictionary<string, EventHandler>();

    public static void AddListener(string eventName, EventHandler handler)
    {
        if (instance.handlerDic.ContainsKey(eventName))
            instance.handlerDic[eventName] += handler;
        else
            instance.handlerDic.Add(eventName, handler);
    }

    public static void RemoveListener(string eventName, EventHandler handler)
    {
        if (instance.handlerDic.ContainsKey(eventName))
            instance.handlerDic[eventName] -= handler;
    }

    public void TriggerEvent(string eventName, object sender)
    {
        if (handlerDic.ContainsKey(eventName))
            handlerDic[eventName]?.Invoke(sender, EventArgs.Empty);
    }

    public void TriggerEvent(string eventName, object sender, EventArgs args)
    {
        if (handlerDic.ContainsKey(eventName))
            handlerDic[eventName]?.Invoke(sender, args);
    }

    public void RemoveAllEvent()
    {
        handlerDic.Clear();
    }
}
