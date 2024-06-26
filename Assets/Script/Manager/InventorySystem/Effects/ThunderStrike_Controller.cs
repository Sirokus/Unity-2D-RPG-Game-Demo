using UnityEngine;

public class ThunderStrike_Controller : MonoBehaviour
{
    protected PlayerStat playerStat;

    private void Start()
    {
        playerStat = PlayerManager.GetPlayer().GetComponent<PlayerStat>();
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out EnemyStat enemy))
        {
            playerStat.DoMagicalDamage(enemy);
        }
        Invoke("DestroySelf", .5f);
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}
