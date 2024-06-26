using UnityEngine;

namespace Old
{
    public class Player : Entity
    {
        //引用
        private CapsuleCollider2D Capsule;

        //输入参数
        float xInput;

        //冲刺
        [Header("Dash Info")]
        [SerializeField] private float dashSpeed = 1000;
        [SerializeField] private float dashDuration = 0.2f;
        private float dashTime = 0;
        private bool isDashing = false;

        //三段攻击
        [Header("Attack Info")]
        [SerializeField] private int comboCounter = 0;
        [SerializeField] private float comboDuration = 1;
        private bool isAttacking = false;
        private float comboTime = 0;


        protected override void Start()
        {
            base.Start();

            Capsule = GetComponent<CapsuleCollider2D>();
        }

        protected override void Update()
        {
            base.Update();

            UpdateState();//更新状态参数

            AnimatorController();//Animator操作

            PlayerInput();//玩家控制输入
        }

        private void UpdateState()
        {
            isMoving = isMoving && xInput != 0;

            dashTime -= Time.deltaTime;//更新冲刺持续
            isDashing = dashTime > 0;//判断是否在冲刺

            //设置冲刺期间胶囊体大小
            if (isDashing)
            {
                Capsule.size = new Vector2(Capsule.size.x, 1.16f);
                Capsule.offset = new Vector2(Capsule.offset.x, -0.75f);
            }
            else
            {
                Capsule.size = new Vector2(Capsule.size.x, 2.1f);
                Capsule.offset = new Vector2(Capsule.offset.x, -0.28f);
            }

            comboTime -= Time.deltaTime;//更新连击延时时间
        }

        private void AnimatorController()
        {
            //翻转玩家
            if (xInput != 0) FlipCharacter(xInput > 0);

            //设置Animator参数
            anim.SetBool("IsMove", isMoving);
            anim.SetBool("IsJump", isJumping);
            anim.SetFloat("yVelocity", rb.velocity.y);
            anim.SetBool("IsDash", isDashing);
            anim.SetBool("IsAttack", isAttacking);
            anim.SetInteger("comboCounter", comboCounter);
        }

        private void PlayerInput()
        {
            //接收水平输入
            xInput = isAttacking ? 0 : Input.GetAxisRaw("Horizontal");

            //响应跳跃事件
            if (Input.GetButtonDown("Jump"))
            {
                Jump();
                isAttacking = false;
            }

            //响应冲刺事件
            if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing)
            {
                dashTime = dashDuration;
                isAttacking = false;
            }

            //响应攻击事件
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (!isJumping && !isDashing)
                {
                    if (comboTime < 0) comboCounter = 0;
                    isAttacking = true;
                    comboTime = comboDuration;
                }
            }


        }

        private void FixedUpdate()
        {
            //处理水平移动
            if (!isDashing || isAttacking)
            {
                AddMoveInput(xInput);
            }
            else
            {
                rb.velocity = new Vector2(dashSpeed * Time.deltaTime * dir, 0);
            }

        }

        public void OnAttackOver()
        {
            isAttacking = false;
            comboCounter = (comboCounter + 1) % 3;
        }

    }

}


