using UnityEngine;
using UnityEngine.Playables;

public class BossRoomCollider : MonoBehaviour, ISaveManager
{
    public PlayableDirector director;
    public Enemy_Boss boss;


    public Transform bossInitPosition;
    private bool isTriggered;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isTriggered)
            return;

        if (collision.GetComponent<Player>())
        {
            isTriggered = true;
            director.Play();
            boss.enabled = false;
        }
    }

    public void LoadData(GameData data)
    {
        isTriggered = data.getValueb(this.GetType().Name + nameof(isTriggered));
        if (isTriggered)
        {
            boss.enabled = true;
            boss.transform.position = bossInitPosition.position;
        }
    }

    public void SaveData(GameData data)
    {
        data.addValue(this.GetType().Name + nameof(isTriggered), isTriggered);
    }
}
