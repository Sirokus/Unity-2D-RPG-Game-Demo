using System.Collections.Generic;
using UnityEngine;

public class Sword_Skill_Controller : MonoBehaviour
{
    [SerializeField] private float returnSpeed = 12;
    [SerializeField] private float bounceSpeed = 20;

    private Animator anim;
    private Rigidbody2D rb;
    private CircleCollider2D cd;
    private Player player;

    [SerializeField] private bool CanRotate = true;
    private bool IsReturn = false;

    [SerializeField] private float LifeDuration = 5;

    [Header("Bounce info")]
    public bool isBouncing = false;
    public int amountOfBounce;
    public List<Transform> enemyTarget;
    public int targetIndex;

    [Header("Pierce info")]
    [SerializeField] private float pierceAmount;

    [Header("Spin info")]
    private float maxTravelDistance;
    private float spinDuration;
    private float spinTimer;
    private bool wasStopped;
    private bool isSpinning;

    private float hitTimer;
    private float hitCoolDown;

    private float spinDirection;

    private float freezeTimeDuration;

    private void Awake()
    {
        //在最初即获取需要操作的对象
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CircleCollider2D>();
    }

    //在Skill中调用,将会在Awake后进行初始化,相当于自定义的带参的构造,但调用时机更往后
    public void SetupSword(Vector2 _dir, float _gravityScale, Player _player, float _freezeTimeDuration)
    {
        player = _player;
        rb.velocity = _dir;
        rb.gravityScale = _gravityScale;
        spinDirection = Mathf.Clamp(rb.velocity.x, -1, 1);
        freezeTimeDuration = _freezeTimeDuration;
    }

    //创建逻辑在Skill中,是否调用取决于Skill中的枚举值
    public void SetupBounce(bool _isBouncing, int _amountOfBounce)
    {
        isBouncing = _isBouncing;
        amountOfBounce = _amountOfBounce;

        anim.SetBool("IsRotation", true);
    }
    public void SetupPierce(int _pierceAmount)
    {
        pierceAmount = _pierceAmount;

        anim.SetBool("IsRotation", false);
    }
    public void SetupSpin(bool _isSpinning, float _maxTravelDistance, float _spinDuration, float _hitCoolDown)
    {
        isSpinning = _isSpinning;
        maxTravelDistance = _maxTravelDistance;
        spinDuration = _spinDuration;
        hitCoolDown = _hitCoolDown;

        anim.SetBool("IsRotation", true);
    }

    //调用时机为在groundState过渡到aimState时，若有剑则召回
    public void ReturnSword()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;//冻结
        transform.parent = null;//确保无父级
        IsReturn = true;//设置为返回状态
        anim.SetBool("IsRotation", true);//开启旋转动画
    }

    private void Update()
    {
        //若超出最大投掷飞行时间，则直接进行摧毁
        if (LifeDuration < 0)
            Destroy(player.sword);

        //旋转逻辑(角度修改)
        if (CanRotate)
        {
            transform.right = rb.velocity;
            if (!wasStopped)
                LifeDuration -= Time.deltaTime;
        }

        //返回逻辑(位移修改)
        if (IsReturn)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, returnSpeed * Time.deltaTime);

            //在返回状态下小于给定距离则视为玩家抓住剑
            if (Vector2.Distance(transform.position, player.transform.position) < 0.5f)
            {
                player.CatchSword();
            }
        }

        BounceLogic();

        SpinLogic();
    }

    private void SpinLogic()
    {
        if (isSpinning)
        {
            //距离大于最大远离距离且不为停止状态则调用停止
            if (Vector2.Distance(player.transform.position, transform.position) > maxTravelDistance && !wasStopped)
            {
                StopWhenSpinning();
            }

            //处于停止状态时
            if (wasStopped)
            {
                //进行计时
                spinTimer -= Time.deltaTime;

                transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x + spinDirection, transform.position.y), 1f * Time.deltaTime);

                //计时结束则进入返回状态
                if (spinTimer < 0)
                {
                    isSpinning = false;
                    IsReturn = true;
                }

                //进行伤害定时器计时，并执行伤害逻辑
                hitTimer -= Time.deltaTime;
                if (hitTimer < 0)
                {
                    hitTimer = hitCoolDown;
                    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 2);
                    foreach (var hit in colliders)
                    {
                        if (hit.TryGetComponent(out Enemy enemy))
                            PlayerManager.instance.player.stats.DoDamage(enemy.GetComponent<CharacterStat>());
                    }
                }
            }
        }
    }

    //停止逻辑
    private void StopWhenSpinning()
    {
        wasStopped = true;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        spinTimer = spinDuration;
    }

    private void BounceLogic()
    {
        //只有在可以反弹且已经拥有了追踪数组时才可执行
        if (isBouncing && enemyTarget.Count > 1)
        {
            if (!enemyTarget[targetIndex].GetComponent<CharacterStat>().isAlive())
            {
                enemyTarget.RemoveAt(targetIndex);
                if (enemyTarget.Count > 1)
                    targetIndex = targetIndex % enemyTarget.Count;
                else
                {
                    isBouncing = false;
                    IsReturn = true;
                    return;
                }
            }

            //向遍历到的当前敌人移动
            transform.position = Vector2.MoveTowards(transform.position, enemyTarget[targetIndex].position, bounceSpeed * Time.deltaTime);

            //若接近给定距离则切换到下一个敌人
            if (Vector2.Distance(transform.position, enemyTarget[targetIndex].position) < .1f)
            {
                PlayerManager.instance.player.stats.DoDamage(enemyTarget[targetIndex].GetComponent<CharacterStat>());

                //循环敌人索引
                targetIndex = (targetIndex + 1) % enemyTarget.Count;

                //限制最大反弹次数，达到后则转为返回状态,不经过ReturnSword函数
                amountOfBounce--;

                if (amountOfBounce < 0)
                {
                    isBouncing = false;
                    IsReturn = true;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //添加伤害
        if (collision.TryGetComponent(out Enemy enemy))
        {
            if (enemy.GetComponent<CharacterStat>().isAlive())
            {
                PlayerManager.instance.player.stats.DoDamage(enemy.GetComponent<CharacterStat>());
                Inventory.GetEquipmentByType(EquipmentType.Amulet)?.executeEffects(enemy.transform);
            }
        }

        //返回状态则无视触发响应
        if (IsReturn) return;

        //若击中物体拥有enemy脚本，且自身可弹跳，未拥有敌人数组则初始化敌人数组
        if (collision.GetComponent<Enemy>() && collision.GetComponent<CharacterStat>().isAlive() && isBouncing && enemyTarget.Count <= 0)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10);
            foreach (var hit in colliders)
            {
                if (!hit.GetComponent<Enemy>() || !hit.GetComponent<CharacterStat>().isAlive())
                    continue;
                enemyTarget.Add(hit.transform);
            }
        }

        StuckSword(collision);
    }

    //显然该函数不会在return状态执行
    private void StuckSword(Collider2D collision)
    {
        //穿刺状态碰撞会将计数器减一，减完后即为正常击中逻辑
        if (pierceAmount > 0 && collision.GetComponent<Enemy>())
        {
            pierceAmount--;
            return;
        }

        //若处于旋转状态时碰撞也会调用停止函数
        if (isSpinning)
        {
            StopWhenSpinning();
            return;
        }

        CanRotate = false;
        cd.enabled = false;
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        //如果是处于反弹状态且已经初始化过敌人列表，则不停止旋转
        if (isBouncing && enemyTarget.Count > 0) return;

        //设置父级，停止旋转
        transform.parent = collision.transform;
        anim.SetBool("IsRotation", false);
    }
}
