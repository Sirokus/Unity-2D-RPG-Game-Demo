using UnityEngine;

public class Crystal_Skill_Controller : MonoBehaviour
{
    private Animator anim => GetComponent<Animator>();
    private CircleCollider2D cd => GetComponent<CircleCollider2D>();
    private float duration;

    private bool canExplode;
    private bool canMove;
    private float moveSpeed;

    private bool canGrow;
    [SerializeField] private float growSpeed = 1.5f;

    private Transform closestTarget;
    [SerializeField] private LayerMask EnemyMask;
    public void SetupCrystal(float _duration, bool _canExplode, bool _canMove, float _moveSpeed, Transform _closestTarget)
    {
        duration = _duration;
        canExplode = _canExplode;
        canMove = _canMove;
        moveSpeed = _moveSpeed;
        closestTarget = _closestTarget;
    }

    public void ChooseRandomEnemy(float radius)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, EnemyMask);

        if (colliders.Length > 0)
        {
            closestTarget = colliders[Random.Range(0, colliders.Length)].transform;
        }
    }

    private void Update()
    {
        duration -= Time.deltaTime;

        if (duration < 0)
        {
            crystalDestroy();
        }

        if (canMove)
        {
            transform.position = Vector2.MoveTowards(transform.position, closestTarget.position, moveSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, closestTarget.position) < 1)
            {
                crystalDestroy();
                canMove = false;
            }
        }

        if (canGrow)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(3, 3), growSpeed * Time.deltaTime);
        }
    }

    public void crystalDestroy()
    {
        if (canExplode)
        {
            canGrow = true;
            anim.SetTrigger("Explode");
        }
        else
        {
            myDestroy();
        }

    }

    public void myDestroy()
    {
        Destroy(gameObject);
    }

    private void OnAnimationExplodeTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, cd.radius);

        foreach (var hit in colliders)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (!enemy) continue;

            PlayerManager.instance.player.stats.DoMagicalDamage(hit.GetComponent<CharacterStat>(), 0, 10, 0);

            Inventory.GetEquipmentByType(EquipmentType.Amulet)?.executeEffects(hit.transform);
        }
    }
}
