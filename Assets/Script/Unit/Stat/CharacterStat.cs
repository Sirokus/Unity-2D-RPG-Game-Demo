using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public enum Element
{
    Fire,
    Ice,
    Thunder
}

public class CharacterStat : MonoBehaviour
{
    private EntityFX fx;

    [Header("Major stats")]
    public Stat strength;   //�˺��ӳ�
    public Stat agility;    //���ܼӳ�
    public Stat intelligence;   //ħ���˺��ӳ�
    public Stat vitality;   //Ѫ���ӳ�

    [Header("Defencive stats")]
    public Stat maxHealth;  //���Ѫ��
    public Stat armor;      //����ֵ
    public Stat evasion;    //����ֵ

    [Header("Offensive stats")]
    public Stat damage;     //�˺�
    public Stat critChance; //��������
    public Stat critPower;  //��������

    [Header("Magic stats")]
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat lightningDamage;
    public Stat magicResistance;

    public bool isIgnited;  //����
    public bool isChilled;  //����
    public bool isShocked;  //����

    public bool isAnyEffect => isIgnited || isChilled || isShocked;

    private Timer ignitedTimer;
    private Timer chilledTimer;
    private Timer shockedTimer;

    private int igniteDamage;
    private int chilledDamage;
    private int shockDamage;

    [SerializeField] private GameObject shockStricke;

    public int currentHealth;

    public bool isInvincible;

    public UnityEvent<int> onHealHealth;
    public UnityEvent<int> onTakeDamage;
    public UnityEvent onDie;


    protected virtual void Start()
    {
        fx = GetComponent<EntityFX>();

        critPower.setValue(150);
        currentHealth = maxHealth.getValue();
    }

    public virtual bool DoDamage(CharacterStat _target, bool mustHit = false)
    {
        if (!_target.isAlive())
            return false;

        if (_target.isInvincible)
            return false;

        //��������
        if (TryEvasionAttack(_target) && !mustHit)
        {
            ObjectPool.Get<TextPopUI>()?.Setup(_target.transform.position, "MISS", EPopTextType.normal);
            return false;
        }

        bool isCrit = TryCirt();
        //�����˺�������������
        int totalDamage = damage.getValue() + strength.getValue();
        if (isCrit)
            totalDamage = Mathf.RoundToInt((critPower.getValue() + strength.getValue()) * .01f * totalDamage);

        //��������
        if (_target.isChilled)
            totalDamage -= Mathf.RoundToInt(_target.armor.getValue() * 0.8f);
        else
            totalDamage -= _target.armor.getValue();

        //��֤�˺���Χ�Ϸ�
        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);

        if (totalDamage > 0)
        {
            if (isCrit)
                ObjectPool.Get<TextPopUI>()?.Setup(_target.transform.position, totalDamage.ToString(), EPopTextType.critDamage);
            else
                ObjectPool.Get<TextPopUI>()?.Setup(_target.transform.position, totalDamage.ToString(), EPopTextType.damage);
        }

        //����˺�
        _target.TakeDamage(totalDamage, transform);

        return true;
    }

    public virtual void DoMagicalDamage(CharacterStat target)
    {
        DoMagicalDamage(target, fireDamage.getValue(), iceDamage.getValue(), lightningDamage.getValue());
    }
    public virtual void DoMagicalDamage(CharacterStat _target, int fireDmg, int iceDmg, int lightningDmg)
    {
        if (!_target.isAlive() || _target.isInvincible)
            return;

        //��������
        if (TryEvasionAttack(_target))
            return;

        //����ʵ���˺�
        int totalDamage = fireDmg + iceDmg + lightningDmg + intelligence.getValue();
        totalDamage -= _target.magicResistance.getValue() + (_target.intelligence.getValue() * 3);

        //��֤��Χ�Ϸ�
        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);

        //����˺�
        _target.TakeDamage(totalDamage, transform);

        //û��Ԫ���˺���ֱ�ӷ���
        if (fireDmg == 0 && iceDmg == 0 && lightningDmg == 0)
            return;

        //���㵱ǰ��Ҫ��Ԫ���˺�
        bool canApplyIgnite = fireDmg > iceDmg && fireDmg > lightningDmg;
        bool canApplyChill = iceDmg > fireDmg && iceDmg > lightningDmg;
        bool canApplyShock = lightningDmg > fireDmg && lightningDmg > iceDmg;

        if (!canApplyIgnite && !canApplyShock && !canApplyChill)
        {
            int rand = Random.Range(0, 2);
            switch (rand)
            {
            case 0:
                canApplyIgnite = true; break;
            case 1:
                canApplyChill = true; break;
            case 2:
                canApplyShock = true; break;
            }
        }

        if (canApplyIgnite)
            _target.setupIgniteDamage(Mathf.RoundToInt(fireDmg * .2f));
        if (canApplyChill)
            _target.setupChillDamage(Mathf.RoundToInt(iceDmg * .2f));
        if (canApplyShock)
            _target.setupShockDamage(Mathf.RoundToInt(lightningDmg * .2f));

        //Ϊ��λӦ��Ԫ��Ч��
        _target.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
    }

    public void ApplyAilments(bool _ignite, bool _chill, bool _shock)
    {
        if (ignitedTimer != null)
        {
            TimerManager.completeTimer(ignitedTimer);
            ignitedTimer = null;
        }
        if (chilledTimer != null)
        {
            TimerManager.completeTimer(chilledTimer);
            chilledTimer = null;
            GetComponent<Entity>()?.ReturnDefaultSpeed();
        }
        if (shockedTimer != null)
        {
            TimerManager.completeTimer(shockedTimer);
            shockedTimer = null;
        }
        fx?.CancelColorChange();

        isIgnited = _ignite;
        isChilled = _chill;
        isShocked = _shock;

        //�����˺�Ч��
        if (_ignite)
        {
            TimerManager.addTimer(4, false, () =>
            {
                isIgnited = false;
                if (ignitedTimer != null)
                {
                    TimerManager.completeTimer(ignitedTimer);
                    ignitedTimer = null;
                }
            });
            ignitedTimer = TimerManager.addTimer(.5f, true, () =>
            {
                DecreseHealth(igniteDamage);
            });
            fx?.IgniteFXFor(4);
        }

        //��������Ч��
        if (_chill)
        {
            TimerManager.addTimer(4, false, () =>
            {
                isChilled = false;
                if (chilledTimer != null)
                {
                    TimerManager.completeTimer(chilledTimer);
                    chilledTimer = null;
                }
            });
            chilledTimer = TimerManager.addTimer(.5f, true, () =>
            {
                DecreseHealth(chilledDamage);
            });
            fx?.ChillFXFor(4);
            GetComponent<Entity>()?.SlowEntityBy(.5f, 4);
        }

        //��������Ч��
        if (_shock)
        {
            TimerManager.addTimer(2, false, () =>
            {
                isShocked = false;
                if (shockedTimer != null)
                {
                    TimerManager.completeTimer(shockedTimer);
                    shockedTimer = null;
                }
            });
            shockedTimer = TimerManager.addTimer(.5f, true, () =>
            {
                DecreseHealth(shockDamage);
            });
            fx?.ShockFXFor(2);

            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 5);
            foreach (var hit in colliders)
            {
                if (hit.TryGetComponent(out Enemy enemy))
                {
                    if (!enemy.GetComponent<CharacterStat>().isAlive() || enemy.GetComponent<CharacterStat>().isShocked)
                        continue;

                    GameObject shock = Instantiate(shockStricke, transform.position, Quaternion.identity);
                    shock.GetComponent<ShockStrike_Controller>().Setup(shockDamage, this, enemy.GetComponent<CharacterStat>());
                }
            }
        }
    }

    public void setupIgniteDamage(int _damage) => igniteDamage = _damage;
    public void setupChillDamage(int _damage) => chilledDamage = _damage;
    public void setupShockDamage(int _damage) => shockDamage = _damage;

    public virtual bool TakeDamage(int _damage, Transform _Instigator)
    {
        if (DecreseHealth(_damage))
        {
            _Instigator.GetComponent<Player>()?.TriggerEvent(EventName.OnPlayerKill, new PlayerKillArgs { enemyType = GetComponent<Enemy>().enemyType.ToString() });
            return true;
        }

        return false;
    }

    public virtual void IncreaseStatBy(Stat statToModify, int modifier, float duration)
    {
        StartCoroutine(StartModifyCoroutine(statToModify, modifier, duration));
    }
    IEnumerator StartModifyCoroutine(Stat statToModify, int modifier, float duration)
    {
        statToModify.addModifier(modifier);
        yield return new WaitForSeconds(duration);
        statToModify.removeModifier(modifier);
    }

    public virtual bool IncreaseHealth(int amount)
    {
        if (currentHealth == getMaxHealthValue())
            return false;

        currentHealth += amount;

        if (currentHealth >= getMaxHealthValue())
            currentHealth = getMaxHealthValue();

        onHealHealth?.Invoke(amount);
        return true;
    }

    public virtual bool DecreseHealth(int _damage)
    {
        if (isInvincible)
            return false;

        if (currentHealth <= 0)
            return true;

        currentHealth -= _damage;

        onTakeDamage?.Invoke(_damage);

        if (currentHealth <= 0)
        {
            Die();
            return true;
        }
        return false;
    }

    protected virtual void Die()
    {
        if (ignitedTimer != null)
        {
            TimerManager.completeTimer(ignitedTimer);
            ignitedTimer = null;
        }
        if (chilledTimer != null)
        {
            TimerManager.completeTimer(chilledTimer);
            chilledTimer = null;
        }
        if (shockedTimer != null)
        {
            TimerManager.completeTimer(shockedTimer);
            shockedTimer = null;
        }
        GetComponent<Entity>()?.ReturnDefaultSpeed();

        fx?.Invoke("CancelColorChange", .1f);
    }

    public virtual bool isAlive()
    {
        return currentHealth > 0;
    }

    public bool TryEvasionAttack(CharacterStat _target)
    {
        int totalEvasion = _target.evasion.getValue() + _target.agility.getValue();

        if (isShocked)
            totalEvasion += 20;

        if (Random.Range(0, 100) < totalEvasion)
        {
            Debug.Log("Attack Missing!");
            return true;
        }
        return false;
    }

    private bool TryCirt()
    {
        int totalCriticalChance = critChance.getValue() + agility.getValue();

        if (Random.Range(0, 100) < totalCriticalChance)
        {
            Debug.Log("Critical Attack!");
            return true;
        }
        return false;
    }

    public int getMaxHealthValue()
    {
        return maxHealth.getValue() + vitality.getValue() * 5;
    }

    public Stat getStatByType(EStat type)
    {
        if (type == EStat.none)
            return null;
        return GetType().GetField(type.ToString()).GetValue(this) as Stat;
    }
}
