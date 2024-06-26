using System;
using System.Collections;
using UnityEngine;

public class Entity : MonoBehaviour, ISaveManager
{
    public string id;

    #region Components
    public Animator anim { get; private set; }
    public SpriteRenderer sr { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public EntityFX fx { get; private set; }
    public CharacterStat stats { get; private set; }
    public Collider2D cd { get; private set; }
    public Transform attacher { get; private set; }
    #endregion

    #region CollisionCheck
    [Header("KnockBack")]
    [SerializeField] protected Vector2 KnockBackDirection;
    [SerializeField] protected float KnockBackDuration = 0.07f;
    public bool isKnocked;

    [Header("GroundCheck")]
    [SerializeField] protected Transform GroundCheck;
    [SerializeField] protected float GroundCheckDistance;
    [SerializeField] protected LayerMask GroundMask;

    [Header("WallCheck")]
    [SerializeField] protected Transform WallCheck;
    [SerializeField] protected float WallCheckDistance;
    [SerializeField] protected float MaxBlockMoveTime = 3;
    public float BlockTimer;

    [Header("AttackCheck")]
    public Transform AttackCheck;
    public float AttackCheckRadius;
    #endregion

    public bool isBusy { get; private set; }
    public int dir { get; private set; } = 1;
    public bool isFacingRight { get; private set; } = true;

    public Action<bool> onFlipped;

    public bool shouldMakeSureReverseWhenTakeDamage = true;

    protected virtual void Awake()
    {
        attacher = transform.Find("Attacher");
    }
    protected virtual void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        fx = GetComponent<EntityFX>();
        stats = GetComponent<CharacterStat>();
        cd = GetComponent<Collider2D>();

        dir = transform.rotation.y == 0 ? 1 : -1;
        isFacingRight = dir == 1;

        BlockTimer = MaxBlockMoveTime;
    }
    protected virtual void Update()
    {
        if (Mathf.Approximately(rb.velocity.x, 0))
        {
            BlockTimer -= Time.deltaTime;
        }
        else
        {
            BlockTimer = MaxBlockMoveTime;
        }
    }
    protected virtual void FixedUpdate() { }

    public virtual void Flip(bool right)
    {
        if (isFacingRight == right) return;

        dir = right ? 1 : -1;
        isFacingRight = right;
        transform.rotation = new Quaternion(0, isFacingRight ? 0 : 180, 0, dir);

        if (onFlipped != null)
            onFlipped.Invoke(right);
    }

    public virtual void MakeSureReverseTo(Transform other)
    {
        if ((other.position.x > transform.position.x) != isFacingRight) Flip(!isFacingRight);
    }

    public IEnumerator BusyFor(float second)
    {
        isBusy = true;

        yield return new WaitForSeconds(second);

        isBusy = false;
    }

    public virtual bool IsGroundDetected() => Physics2D.Raycast(GroundCheck.position, Vector2.down, GroundCheckDistance, GroundMask) || isKnocked;
    public virtual bool IsWallDetected() => Physics2D.Raycast(WallCheck.position, Vector2.right * dir, WallCheckDistance, GroundMask) || BlockTimer < 0;

    public virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(GroundCheck.position, new Vector2(GroundCheck.position.x, GroundCheck.position.y - GroundCheckDistance));
        Gizmos.DrawLine(WallCheck.position, new Vector2(WallCheck.position.x + (WallCheckDistance * dir), WallCheck.position.y));
        Gizmos.DrawWireSphere(AttackCheck.position, AttackCheckRadius);
    }

    public virtual Vector2 GetVelocity() { return rb.velocity; }
    public virtual void SetVelocity(Vector2 velocity)
    {
        if (isKnocked)
            return;
        rb.velocity = velocity;
    }

    public virtual void Damage(Transform Instigator)
    {
        if (shouldMakeSureReverseWhenTakeDamage)
            MakeSureReverseTo(Instigator);

        fx?.StartCoroutine("FlashFX");
        StartCoroutine("HitKnockBack");
    }

    protected virtual IEnumerator HitKnockBack()
    {
        isKnocked = true;

        rb.velocity = new Vector2(KnockBackDirection.x * -dir, KnockBackDirection.y + rb.velocity.y);

        yield return new WaitForSeconds(KnockBackDuration);

        isKnocked = false;

        rb.velocity = new Vector2(0, rb.velocity.y);
    }

    public virtual void Die()
    {
        for (int i = 0; i < attacher.childCount; i++)
            if (attacher.GetChild(i).TryGetComponent(out IAttachable obj))
            {
                obj.DettachTo(attacher);
            }
    }

    public virtual void SlowEntityBy(float slowPercentage, float slowDuration)
    {

    }

    public virtual void ReturnDefaultSpeed()
    {
        anim.speed = 1;
    }

    public virtual void Attack()
    {

    }

    public virtual OldStateMachine GetStatMachine() => null;


    [ContextMenu("Generate Entity ID")]
    private void GenerateCheckPointID()
    {
        id = System.Guid.NewGuid().ToString();
    }

    public virtual void LoadData(GameData data)
    {
        if (data.haveValue(id))
        {
            transform.position = data.getValueV(id);
        }
    }

    public virtual void SaveData(GameData data)
    {

    }
}
