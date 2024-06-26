using System;
using System.Collections.Generic;
using UnityEngine;

public class TimerManager : MonoBehaviour, IBeforeLoadNextLevel
{
    public static TimerManager ins;
    public int TimerPoolSize = 10;
    public List<Timer> timers = new List<Timer>();
    public Queue<Timer> timerPool = new Queue<Timer>();

    private void Awake()
    {
        if (ins)
        {
            Destroy(this);
            return;
        }

        ins = this;

        for (int i = 0; i < TimerPoolSize; i++)
            timerPool.Enqueue(new Timer());
    }

    private void Update()
    {
        for (int i = 0; i <  timers.Count;)
        {
            if (timers[i].isComplete)
            {
                timers[i].completeCallback?.Invoke();
                clearTimer(timers[i]);
            }
            else
                timers[i++].Update();
        }
    }

    public static Timer addTimer(float delay, bool loop, Action callback, float startDelay = 0f, int loopTime = -1)
    {
        if (ins.timerPool.Count <= 0)
            ins.timerPool.Enqueue(new Timer());

        Timer timer = ins.timerPool.Dequeue();
        ins.timers.Add(timer);
        timer.Setup(delay, loop, callback, startDelay, loopTime);
        return timer;
    }

    public static void clearTimer(Timer timer)
    {
        if (timer == null)
            return;
        timer.callback = timer.completeCallback = null;
        ins.timers.Remove(timer);
        if (ins.timerPool.Count < ins.TimerPoolSize)
            ins.timerPool.Enqueue(timer);
    }

    public static void clearAllTimer()
    {
        foreach (Timer timer in ins.timers)
        {
            timer.callback = timer.completeCallback = null;
            if (ins.timerPool.Count < ins.TimerPoolSize)
                ins.timerPool.Enqueue(timer);
        }

        ins.timers.Clear();
    }

    public static void completeTimer(Timer timer)
    {
        timer?.callback?.Invoke();
        clearTimer(timer);
    }

    public void Execute(int slotIntex)
    {
        clearAllTimer();
    }
}
