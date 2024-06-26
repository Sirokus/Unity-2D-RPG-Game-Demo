using UnityEngine;

public class PlayerManager : MonoBehaviour, ISaveManager
{
    public static PlayerManager instance;
    public Player player;
    public int Currency = 0;

    public static Player GetPlayer() => instance.player;
    public static PlayerStat playerStat => instance.player?.GetComponent<PlayerStat>();
    public static Vector3 playerPos => instance.player.transform.position;
    public static bool isFacingRight => instance.player.isFacingRight;

    private void Awake()
    {
        if (instance)
        {
            Destroy(instance.gameObject);
        }

        instance = this;
    }

    private void Start()
    {
        TimerManager.addTimer(2, true, () => IncreaseCurrency(10));
    }

    public static bool CheckCurrencyEnough(int need)
    {
        return instance.Currency >= need;
    }

    public static bool DecreaseCurrency(int spend)
    {
        if (!CheckCurrencyEnough(spend))
            return false;

        PlayerUI.ModifyMoney(-spend);
        return true;
    }

    public static bool IncreaseCurrency(int amount)
    {
        if (instance.Currency >= 99999999)
            return false;

        PlayerUI.ModifyMoney(amount);
        return true;
    }

    public void LoadData(GameData data)
    {
        Currency = data.currency;
        PlayerUI.updateMoneyText();
    }

    public void SaveData(GameData data)
    {
        data.currency = Currency;
    }
}
