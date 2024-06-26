using System.Collections.Generic;
using UnityEngine;

public class EnemyStat : CharacterStat
{
    private Enemy enemy;

    [Header("Level details")]
    [SerializeField] private int level;

    [SerializeField] private Stat currency;
    [SerializeField] public int DropNum = 3;
    [System.Serializable]
    public struct dropItem
    {
        public float possible;
        public ItemData data;
    }
    [SerializeField] private List<dropItem> possibleDropItemDatas;

    protected override void Start()
    {
        Modify(damage);
        Modify(maxHealth);
        Modify(currency);

        base.Start();

        enemy = GetComponent<Enemy>();

    }

    private void Modify(Stat stat)
    {
        for (int i = 1; i <= level; i++)
        {
            float modifier = stat.getBaseValue() * Random.Range(0f, 0.2f);

            stat.addModifier(Mathf.RoundToInt(modifier));
        }
    }

    public override bool TakeDamage(int _damage, Transform _Instigator)
    {
        base.TakeDamage(_damage, _Instigator);

        if (isInvincible)
            return false;

        enemy.Damage(_Instigator);

        return false;
    }

    protected override void Die()
    {
        base.Die();

        enemy.Die();

        float sum = 0;
        foreach (var item in possibleDropItemDatas)
        {
            sum += item.possible;
        }

        for (int i = 0; i < DropNum; i++)
        {
            float rand = Random.Range(0, 100);
            float percent = 0;
            foreach (var item in possibleDropItemDatas)
            {
                percent += (item.possible / sum) * 100;
                if (percent >= rand)
                {
                    Inventory.SpawnItem(item.data, enemy.transform.position, new Vector2(Random.Range(-200f, 200f), Random.Range(100f, 200f)));
                    break;
                }
            }
        }

        PlayerManager.IncreaseCurrency(currency.getValue());
    }
}
