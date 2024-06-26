using System;
using UnityEngine;

[Serializable]
public class Timer
{
    public Action callback;
    public Action completeCallback;

    public float coolDown;
    public float timer;
    public bool loop = false;
    public int loopTime = -1;

    public void Setup(float coolDown, bool loop, Action callback, float startDelay, int loopTime)
    {
        this.coolDown = coolDown;
        this.timer = coolDown + startDelay;
        this.loop = loop;
        this.callback = callback;
        this.loopTime = loopTime;
    }

    public void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            callback?.Invoke();

            if (loop)
            {
                if (loopTime != -1)
                    loopTime--;
                if (loopTime == -1 || loopTime > 0)
                    timer = coolDown;
                else if (loopTime == 0)
                    loop = false;
            }
        }
    }

    public float remainTime => timer;
    public bool isComplete => timer <= 0 && !loop;

    public Timer OnComplete(Action onComplete)
    {
        completeCallback = onComplete;
        return this;
    }
}
