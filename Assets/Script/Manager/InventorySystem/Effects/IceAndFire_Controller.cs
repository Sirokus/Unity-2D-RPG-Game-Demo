using UnityEngine;

public class IceAndFire_Controller : ThunderStrike_Controller
{
    public int damage = 10;

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out EnemyStat enemy))
        {
            if (!enemy.isAnyEffect)
            {
                if (Random.value <= 0.5)
                {
                    playerStat.DoMagicalDamage(enemy, damage, 0, 0);
                }
                else
                {
                    playerStat.DoMagicalDamage(enemy, 0, damage, 0);
                }
            }
        }
        Invoke("DestroySelf", 1f);
    }
}
