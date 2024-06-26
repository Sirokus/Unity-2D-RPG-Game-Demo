using UnityEngine;

namespace Old
{
    public class Entity : MonoBehaviour
    {
        //基础引用
        protected Rigidbody2D rb;
        protected Animator anim;

        [SerializeField] protected Transform groundCheck;

        //移动属性
        [Header("Movement")]
        [SerializeField] protected float speed = 200;
        [SerializeField] protected float JumpForce = 1000;
        protected bool isMoving = false;
        protected bool isJumping = false;
        protected int dir;

        //地面检查
        [Header("Ground Check")]
        [SerializeField] protected float groundCheckDistance = 1.4f;
        [SerializeField] protected LayerMask GroundMask;

        //墙面检查
        [Header("Wall Check")]
        [SerializeField] protected float wallCheckDistance = 0.7f;
        protected bool isWallDetected;

        // Start is called before the first frame update
        protected virtual void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponentInChildren<Animator>();

            dir = transform.rotation.y ==0 ? 1 : -1;
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            isMoving = !Mathf.Approximately(rb.velocity.x, 0); //判断是否在移动
            isJumping = !Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, GroundMask); ;//判断是否在跳跃
            isWallDetected = Physics2D.Raycast(transform.position, Vector2.right * dir, wallCheckDistance, GroundMask);
        }

        protected void FlipCharacter(bool right)
        {
            if (isMoving)
            {
                dir = right ? 1 : -1;

                Quaternion Rot = new Quaternion();
                Rot.y = right ? 0 : 180;
                transform.rotation = Rot;
            }
        }

        protected void AddMoveInput(float Input)
        {
            rb.velocity = new Vector2(Input * speed * Time.deltaTime, rb.velocity.y);
        }

        protected void Jump()
        {
            if (!isJumping)
            {
                rb.velocity = new Vector2(rb.velocity.x, JumpForce);
            }
        }

        protected virtual void OnDrawGizmos()
        {
            Vector3 Start = groundCheck.position;
            Vector3 End = groundCheck.position;
            End.y -= groundCheckDistance;
            Gizmos.DrawLine(Start, End);

            Start = transform.position;
            End = transform.position;
            End.x += wallCheckDistance * dir;
            Gizmos.DrawLine(Start, End);
        }
    }

}

