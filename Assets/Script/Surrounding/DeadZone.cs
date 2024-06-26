using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out CharacterStat stat))
        {
            stat.DecreseHealth(99999999);
        }
        else if (collision.TryGetComponent(out ItemObj item))
        {
            TimerManager.addTimer(3, false, () => Destroy(item.gameObject));
        }
        else if (collision.TryGetComponent(out Bullet bullet))
        {
            ObjectPool.Return(bullet);
        }

    }
}
