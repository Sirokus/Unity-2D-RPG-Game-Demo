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
        //���������ȡ��Ҫ�����Ķ���
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CircleCollider2D>();
    }

    //��Skill�е���,������Awake����г�ʼ��,�൱���Զ���Ĵ��εĹ���,������ʱ��������
    public void SetupSword(Vector2 _dir, float _gravityScale, Player _player, float _freezeTimeDuration)
    {
        player = _player;
        rb.velocity = _dir;
        rb.gravityScale = _gravityScale;
        spinDirection = Mathf.Clamp(rb.velocity.x, -1, 1);
        freezeTimeDuration = _freezeTimeDuration;
    }

    //�����߼���Skill��,�Ƿ����ȡ����Skill�е�ö��ֵ
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

    //����ʱ��Ϊ��groundState���ɵ�aimStateʱ�����н����ٻ�
    public void ReturnSword()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;//����
        transform.parent = null;//ȷ���޸���
        IsReturn = true;//����Ϊ����״̬
        anim.SetBool("IsRotation", true);//������ת����
    }

    private void Update()
    {
        //���������Ͷ������ʱ�䣬��ֱ�ӽ��дݻ�
        if (LifeDuration < 0)
            Destroy(player.sword);

        //��ת�߼�(�Ƕ��޸�)
        if (CanRotate)
        {
            transform.right = rb.velocity;
            if (!wasStopped)
                LifeDuration -= Time.deltaTime;
        }

        //�����߼�(λ���޸�)
        if (IsReturn)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, returnSpeed * Time.deltaTime);

            //�ڷ���״̬��С�ڸ�����������Ϊ���ץס��
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
            //����������Զ������Ҳ�Ϊֹͣ״̬�����ֹͣ
            if (Vector2.Distance(player.transform.position, transform.position) > maxTravelDistance && !wasStopped)
            {
                StopWhenSpinning();
            }

            //����ֹͣ״̬ʱ
            if (wasStopped)
            {
                //���м�ʱ
                spinTimer -= Time.deltaTime;

                transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x + spinDirection, transform.position.y), 1f * Time.deltaTime);

                //��ʱ��������뷵��״̬
                if (spinTimer < 0)
                {
                    isSpinning = false;
                    IsReturn = true;
                }

                //�����˺���ʱ����ʱ����ִ���˺��߼�
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

    //ֹͣ�߼�
    private void StopWhenSpinning()
    {
        wasStopped = true;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        spinTimer = spinDuration;
    }

    private void BounceLogic()
    {
        //ֻ���ڿ��Է������Ѿ�ӵ����׷������ʱ�ſ�ִ��
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

            //��������ĵ�ǰ�����ƶ�
            transform.position = Vector2.MoveTowards(transform.position, enemyTarget[targetIndex].position, bounceSpeed * Time.deltaTime);

            //���ӽ������������л�����һ������
            if (Vector2.Distance(transform.position, enemyTarget[targetIndex].position) < .1f)
            {
                PlayerManager.instance.player.stats.DoDamage(enemyTarget[targetIndex].GetComponent<CharacterStat>());

                //ѭ����������
                targetIndex = (targetIndex + 1) % enemyTarget.Count;

                //������󷴵��������ﵽ����תΪ����״̬,������ReturnSword����
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
        //����˺�
        if (collision.TryGetComponent(out Enemy enemy))
        {
            if (enemy.GetComponent<CharacterStat>().isAlive())
            {
                PlayerManager.instance.player.stats.DoDamage(enemy.GetComponent<CharacterStat>());
                Inventory.GetEquipmentByType(EquipmentType.Amulet)?.executeEffects(enemy.transform);
            }
        }

        //����״̬�����Ӵ�����Ӧ
        if (IsReturn) return;

        //����������ӵ��enemy�ű���������ɵ�����δӵ�е����������ʼ����������
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

    //��Ȼ�ú���������return״ִ̬��
    private void StuckSword(Collider2D collision)
    {
        //����״̬��ײ�Ὣ��������һ�������Ϊ���������߼�
        if (pierceAmount > 0 && collision.GetComponent<Enemy>())
        {
            pierceAmount--;
            return;
        }

        //��������ת״̬ʱ��ײҲ�����ֹͣ����
        if (isSpinning)
        {
            StopWhenSpinning();
            return;
        }

        CanRotate = false;
        cd.enabled = false;
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        //����Ǵ��ڷ���״̬���Ѿ���ʼ���������б���ֹͣ��ת
        if (isBouncing && enemyTarget.Count > 0) return;

        //���ø�����ֹͣ��ת
        transform.parent = collision.transform;
        anim.SetBool("IsRotation", false);
    }
}
