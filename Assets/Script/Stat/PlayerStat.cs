using UnityEngine;

public class PlayerStat : CharacterStat
{
    private Player player;

    [SerializeField] private float percentageLossItem = 0.3f;

    protected override void Start()
    {
        base.Start();

        player = GetComponent<Player>();
    }

    public override bool TakeDamage(int _damage, Transform _Instigator)
    {
        if (isInvincible)
            return false;

        base.TakeDamage(_damage, _Instigator);

        player.Damage(_Instigator);

        Inventory.GetEquipmentByType(EquipmentType.Armor)?.executeEffects(_Instigator);

        AudioManager.ins.PlaySFX(Random.Range(34, 35));

        return false;
    }

    protected override void Die()
    {
        base.Die();

        //��ɫ�߼�����
        player.Die();

        //��Ʒ�߼�����
        PlayerManager.DecreaseCurrency(PlayerManager.instance.Currency / 5);
        for (int i = 0; i < 10; i++)
        {
            if (Random.value <= percentageLossItem)
            {
                Inventory.DropItem(Inventory.GetRandomItem(), PlayerManager.playerPos, new Vector2(Random.Range(-200f, 200f), Random.Range(100f, 200f)));
            }
        }

        //UI�߼�����
        TimerManager.addTimer(1.5f, false, () => UIManager.ins.PlayDeadUI());

        //GameManager��¼
        GameManager.ins.isDead = true;

        //�����¼�
        this.TriggerEvent(EventName.OnPlayerDead);
    }
}
