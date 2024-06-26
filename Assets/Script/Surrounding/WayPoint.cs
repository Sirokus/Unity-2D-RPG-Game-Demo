using UnityEngine;

public class WayPoint : MonoBehaviour, ISaveManager
{
    public string WayPointName;
    public bool canCycleTrigger = false;
    public bool used = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!used || canCycleTrigger)
        {
            if (collision.GetComponent<Player>())
            {
                used = true;
                this.TriggerEvent(EventName.OnPlayerMoveToLoc, new PlayerMoveToLocArgs { wayPointName = WayPointName });
            }
        }
    }

    public void LoadData(GameData data)
    {
        used = data.getValueb(WayPointName + "used");
    }

    public void SaveData(GameData data)
    {
        data.addValue(WayPointName + "used", used);
    }
}
