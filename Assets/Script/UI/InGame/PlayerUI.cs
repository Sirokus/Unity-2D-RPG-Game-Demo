using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    public static PlayerUI ins;

    [Header("Cooldown UI")]
    [SerializeField] private Transform cooldownsParent;
    [SerializeField] private GameObject cooldownUIPrefab;

    [Header("Buff UI")]
    [SerializeField] private Transform buffsParent;
    [SerializeField] private GameObject buffsUIPrefab;

    [SerializeField] private TextMeshProUGUI MoneyUI;

    private void Awake()
    {
        ins = this;
    }

    private void Start()
    {
        MoneyUI.text = PlayerManager.instance.Currency.ToString("#,#");
    }

    public static CooldownUI AddCooldownUI(Sprite sprite, string name, float cooldown)
    {
        GameObject obj = Instantiate(ins.cooldownUIPrefab);
        obj.transform.SetParent(ins.cooldownsParent, false);
        CooldownUI ui = obj.GetComponent<CooldownUI>();
        ui.setup(sprite, name, cooldown);
        return ui;
    }

    public static BuffUI AddBuffUI(Sprite sprite, string name, float cooldown, int amount)
    {
        GameObject obj = Instantiate(ins.buffsUIPrefab);
        obj.transform.SetParent(ins.buffsParent, false);
        BuffUI ui = obj.GetComponent<BuffUI>();
        ui.setup(sprite, name, cooldown, amount);
        return ui;
    }

    public static void updateMoneyText()
    {
        if (PlayerManager.instance.Currency > 0)
            ins.MoneyUI.text = PlayerManager.instance.Currency.ToString("#,#");
        else
            ins.MoneyUI.text = "0";
    }

    public static void ModifyMoney(int delta)
    {
        ins.StartCoroutine(ins.ModifyMoneyCoroutine(delta));
    }

    IEnumerator ModifyMoneyCoroutine(int delta)
    {
        int cur = PlayerManager.instance.Currency;
        int target = cur + delta;
        PlayerManager.instance.Currency = target;

        int step = Mathf.Abs(delta / 100);
        if (step  == 0)
            step = 1;

        while (cur != target)
        {
            if (cur > target)
            {
                if (cur - step < target)
                    cur = target;
                else
                    cur -= step;
            }
            else
            {
                if (cur + step > target)
                    cur = target;
                else
                    cur += step;
            }

            MoneyUI.text = cur.ToString("#,#");
            if (cur <= 0)
                MoneyUI.text = "0";

            yield return new WaitForEndOfFrame();
        }
    }
}
