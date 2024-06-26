using UnityEngine;

public class Bullet : MonoBehaviour, IPoolable, IAttachable
{
    [Header("Component")]
    protected Rigidbody2D rb;

    [Header("Movement")]
    public Vector2 dir;
    public float speed = 1000;
    protected float initSpeed;
    protected string targetLayerName = "Player";
    public bool CanMove;

    [Header("Collision Execute")]
    protected Vector2 delta;
    protected float dis;
    protected Vector3 rcdPos;

    [Header("Gameplay")]
    protected CharacterStat owner;
    protected bool isHit = true;

    [Header("Flip")]
    public bool flipped;
    public bool canFlip = true;

    [Header("Pool")]
    Timer returnTimer;
    EPoolObjState IPoolable.state { get; set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public virtual void Setup(Vector2 dir, float speed, CharacterStat owner)
    {
        //数据赋值
        this.dir = dir;
        this.speed = speed;
        initSpeed = speed;
        this.owner = owner;

        //物理碰撞
        rb.velocity = dir * speed;
        rcdPos = transform.position;

        //旋转
        delta = rb.velocity.normalized;
        float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        //开始移动
        CanMove = true;
    }
    public virtual void OnGet()
    {
        targetLayerName = "Player";
        CanMove = false;

        flipped = false;

        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        GetComponent<Collider2D>().enabled = true;
        GetComponentInChildren<ParticleSystem>().Play();
    }
    public virtual void OnReturn()
    {
    }


    private void FixedUpdate()
    {
        if (!CanMove)
            return;

        //记录位移差值
        delta = transform.position - rcdPos;
        dis = delta.magnitude;

        //位移差值用于旋转
        delta = rb.velocity.normalized;
        float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);


        //位移差值用于保证不错过刚体
        if (dis > 0)
        {
            var hit = Physics2D.Raycast(rcdPos, delta, dis, LayerMask.GetMask("Ground") | LayerMask.GetMask(targetLayerName));
            if (hit)
            {
                if (hit.collider.TryGetComponent(out CharacterStat stat))
                {
                    if (!owner.DoDamage(stat))
                    {
                        targetLayerName = "";
                        rcdPos = transform.position;
                        return;
                    }
                }

                transform.position = hit.point;
                StuckInto(hit.collider);
            }
        }

        rcdPos = transform.position;
    }

    public virtual void AttachTo(Transform _parent)
    {
        Transform parent;
        if (_parent.TryGetComponent(out Entity entity))
        {
            parent = entity.attacher;
        }
        else
            parent = _parent;

        transform.parent = parent;
    }
    public virtual void DettachTo(Transform parent)
    {
        TimerManager.clearTimer(returnTimer);
        ObjectPool.Return(this);
    }
    private void StuckInto(Collider2D collision)
    {
        CanMove = false;
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        AttachTo(collision.transform);

        GetComponent<Collider2D>().enabled = false;
        GetComponentInChildren<ParticleSystem>().Stop();

        ObjectPool.Return(this, true);
        TimerManager.clearTimer(returnTimer);
        returnTimer = TimerManager.addTimer(Random.Range(3f, 5f), false, () => { ObjectPool.Return(this); returnTimer = null; });
    }

    public void Flip()
    {
        if (flipped)
            return;

        flipped = true;
        rb.velocity = dir * -speed;
        targetLayerName = "Enemy";
    }


}
