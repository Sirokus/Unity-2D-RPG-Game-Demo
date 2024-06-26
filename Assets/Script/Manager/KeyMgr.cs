using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum GameAction
{
    MoveRight,
    MoveLeft,
    Jump,
    Attack,
    Counter,
    Dash,
    Sword,
    Crystal,
    BlackHole,
    FlaskUse,
    Interact
}

[Serializable]
public class InputAction
{
    public GameAction action;
    public KeyCode key;

    public InputAction(GameAction action, KeyCode key)
    {
        this.action = action;
        this.key = key;
    }
}
[Serializable]
public class JsonData
{
    [Serializable]
    public class data
    {
        public string action;
        public string key;
    }
    public data[] Keys;
}
public class KeyMgr : MonoBehaviour
{
    public static KeyMgr ins;
    public Dictionary<GameAction, KeyCode> Keys = new Dictionary<GameAction, KeyCode>();

    public bool pauseInput = false;

    public GameObject InputModifyUIPrefab;
    public Transform InputModifyUIParent;

    private void Awake()
    {
        ins = this;

        string json = Utility.ReadJson("InputConfig");
        JsonData jsonData = JsonUtility.FromJson<JsonData>(json);

        foreach (var data in jsonData.Keys)
        {
            Keys.Add((GameAction)Enum.Parse(typeof(GameAction), data.action),
                                  (KeyCode)Enum.Parse(typeof(KeyCode), data.key));
            GameObject obj = Instantiate(InputModifyUIPrefab);
            obj.transform.SetParent(InputModifyUIParent, false);
            obj.GetComponent<InputModifyUI>().setup(Keys.Last().Key, Keys.Last().Value);
        }
    }

    public static bool set(GameAction action, KeyCode key)
    {
        foreach (var val in ins.Keys)
        {
            if (val.Value == key && val.Key != action)
            {
                TipsUI.AddTip("键位冲突！设置新按键失败！", false);
                return false;
            }
        }

        ins.Keys[action] = key;

        //读取数据
        string json = Utility.ReadJson("InputConfig");
        //反序列化为数据
        JsonData jsonData = JsonUtility.FromJson<JsonData>(json);
        //修改对应值
        foreach (var data in jsonData.Keys)
        {
            if (data.action == action.ToString())
                data.key = key.ToString();
        }
        //序列化为Json
        json = JsonUtility.ToJson(jsonData);
        //写入数据
        Utility.WriteJson(json, "InputConfig");

        return true;
    }

    public static KeyCode get(GameAction action)
    {
        if (ins.pauseInput)
            return KeyCode.None;
        return ins.Keys[action];
    }

    public static bool getKeyDown(GameAction action)
    {
        if (ins.pauseInput)
            return false;
        if (Input.GetKeyDown(get(action)))
        {
            ins.TriggerEvent(EventName.OnPlayerInput, new PlayerInputArgs { action = action });
            return true;
        }
        else
            return false;
    }
    public static bool getKey(GameAction action)
    {
        if (ins.pauseInput)
            return false;
        return Input.GetKey(get(action));
    }

    public static float getAxisX()
    {
        float r = getKey(GameAction.MoveRight) ? 1 : 0;
        float l = getKey(GameAction.MoveLeft) ? -1 : 0;
        return r + l;
    }
}
