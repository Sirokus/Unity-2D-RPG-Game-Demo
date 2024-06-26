using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour, ISaveManager
{
    public static ObjectPool ins;
    private Dictionary<Type, Queue<GameObject>> avaiablePools = new Dictionary<Type, Queue<GameObject>>();

    [Header("Shadow Pool")]
    public GameObject shadowPrefab;
    public int shadowCount;

    [Header("Arrow Pool")]
    public GameObject arrowPrefab;
    public int arrowCount;

    [Header("Item Pool")]
    public GameObject itemObjPrefab;
    public int itemCount;

    [Header("TextPop Pool")]
    public GameObject textPopUIPrefab;
    public int textPopCount;

    public List<int> poolInfo = new List<int>();

    private void Awake()
    {
        if (ins != null)
            Destroy(this);
        else
            ins = this;

        AddPool<ShadowSprite_OP>(shadowPrefab, shadowCount);
        AddPool<Bullet>(arrowPrefab, arrowCount);
        AddPool<ItemObj>(itemObjPrefab, itemCount);
        AddPool<TextPopUI>(textPopUIPrefab, textPopCount);

        foreach (var pair in avaiablePools)
            poolInfo.Add(pair.Value.Count);
    }

    private void Update()
    {
        int i = 0;
        foreach (var pair in avaiablePools)
        {
            poolInfo[i++] = pair.Value.Count;
        }
    }

    public static void AddPool<T>(GameObject template, int poolSize) where T : MonoBehaviour, IPoolable
    {
        if (template.GetComponent<T>() == null)
        {
            Debug.Log("创建对象池失败！给定模板对象未包含指定组件！");
            return;
        }
        Type type = typeof(T);

        //创建新的空父对象
        ins.avaiablePools.Add(type, new Queue<GameObject>());
        GameObject poolParent = new GameObject(type.Name);
        poolParent.transform.parent = ins.transform;

        //向该对象放入物体
        for (int i = 0; i <  poolSize; i++)
        {
            GameObject obj = Instantiate(template);
            obj.transform.parent = poolParent.transform;
            obj.SetActive(false);
            obj.GetComponent<T>().state = EPoolObjState.Avaiable;
            ins.avaiablePools[typeof(T)].Enqueue(obj);
        }
    }

    public static T Get<T>() where T : MonoBehaviour, IPoolable
    {
        if (ins.avaiablePools.TryGetValue(typeof(T), out Queue<GameObject> queue) && queue.Count > 0)
        {
            T obj = queue.Dequeue().GetComponent<T>();
            obj.OnGet();
            obj.gameObject.SetActive(true);
            obj.state = EPoolObjState.Using;
            return obj;
        }
        return null;
    }

    //如果物品仍需留在原地，但随时允许回收，则可将readyToReturn设为true
    public static void Return<T>(T obj, bool readyToReturn = false) where T : MonoBehaviour, IPoolable
    {
        if (obj.state == EPoolObjState.Avaiable)
            return;

        if (ins.avaiablePools.TryGetValue(typeof(T), out Queue<GameObject> queue))
        {
            if (obj.state == EPoolObjState.Using)
            {
                queue.Enqueue(obj.gameObject);
            }

            if (readyToReturn)
            {
                obj.state = EPoolObjState.Ready;
            }
            else
            {
                obj.state = EPoolObjState.Avaiable;
                ReturnObj(obj);
            }
        }
    }

    private static void ReturnObj<T>(T obj) where T : MonoBehaviour, IPoolable
    {
        obj.OnReturn();
        obj.transform.parent = ins.transform.Find(typeof(T).Name);
        obj.gameObject.SetActive(false);
    }

    public void LoadData(GameData data)
    {
        foreach (var (id, num, pos) in data.dropItems)
        {
            ItemData itemData = Inventory.GetItemDataByID(int.Parse(id));
            ItemObj obj = Get<ItemObj>();
            obj.transform.position = pos.Get();
            obj.Setup(itemData, num);
        }
    }

    public void SaveData(GameData data)
    {
        Transform itemsParent = ins.transform.Find(typeof(ItemObj).Name);

        data.dropItems.Clear();
        foreach (var item in itemsParent.GetComponentsInChildren<ItemObj>())
        {
            if (item.gameObject.activeSelf)
            {
                data.dropItems.Add(Tuple.Create(item.data.itemID, item.stackSize, new SerVector3(item.gameObject.transform.position)));
            }
        }
    }
}
