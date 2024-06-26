using UnityEngine;

public class Clone_Skill_Controller : MonoBehaviour
{
    private SpriteRenderer sr;
    private Animator anim;

    [SerializeField] private float InitializeAlpha = 0.5f;
    [SerializeField] private float AlphaLosingSpeed = 1;
    private float CloneTimer;

    [SerializeField] private Transform AttackCheck;
    [SerializeField] private float AttackCheckRadius = .8f;

    private Transform MinEnemy;
    private int dir = 1;

    private bool canDuplicateClone;
    private float DuplicateChance = 0.6f;

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        CloneTimer -= Time.deltaTime;

        if (CloneTimer < 0)
        {
            sr.color = new Color(1, 1, 1, sr.color.a - (Time.deltaTime * AlphaLosingSpeed));

            if (sr.color.a <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    public void SetupClone(Transform trans, float CloneDuration, bool CanAttack, Transform _minEnemy, bool _canDuplicateClone, float _DuplicateChance)
    {
        transform.position = trans.position;
        transform.rotation = trans.rotation;

        if (CanAttack)
        {
            anim.SetInteger("ComboCounter", Random.Range(1, 4));
        }

        CloneTimer = CloneDuration;
        MinEnemy = _minEnemy;
        canDuplicateClone = _canDuplicateClone;
        DuplicateChance = _DuplicateChance;

        sr.color = new Color(1, 1, 1, InitializeAlpha);

        FaceDirection();
    }

    public void SetupClone(Transform trans, float CloneDuration, bool CanAttack, Vector3 _offset, Transform _minEnemy, bool _canDuplicateClone, float _DuplicateChance)
    {
        transform.position = trans.position + _offset;
        transform.rotation = trans.rotation;

        if (CanAttack)
        {
            anim.SetInteger("ComboCounter", Random.Range(1, 4));
        }

        CloneTimer = CloneDuration;
        MinEnemy = _minEnemy;
        canDuplicateClone = _canDuplicateClone;
        DuplicateChance = _DuplicateChance;

        sr.color = new Color(1, 1, 1, InitializeAlpha);

        FaceDirection();
    }

    private void FaceDirection()
    {
        if (!MinEnemy)
        {
            Destroy(gameObject);
            return;
        }

        bool right = MinEnemy.position.x > transform.position.x;
        transform.rotation = new Quaternion(0, right ? 0 : 180, 0, 0);

        dir = right ? 1 : -1;
    }

    private void PlayerAnimationTrigger()
    {
        CloneTimer = -0.1f;
    }

    private void PlayerAttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(AttackCheck.position, AttackCheckRadius);

        bool isAttacked = false;
        foreach (var hit in colliders)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (!enemy || !enemy.GetComponent<CharacterStat>().isAlive())
                continue;

            EnemyStat target = hit.GetComponent<EnemyStat>();
            PlayerManager.instance.player.stats.DoDamage(target);
            Inventory.GetEquipmentByType(EquipmentType.Amulet)?.executeEffects(hit.transform);

            if (canDuplicateClone && !isAttacked)
            {
                if (Random.Range(0f, 1f) < DuplicateChance)
                {
                    SkillManager.instance.clone.CreateClone(hit.transform, new Vector3(1.5f * dir, 0));
                }

                isAttacked = true;
            }
        }
    }
}
