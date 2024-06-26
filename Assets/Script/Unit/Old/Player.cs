using UnityEngine;

namespace Old
{
    public class Player : Entity
    {
        //����
        private CapsuleCollider2D Capsule;

        //�������
        float xInput;

        //���
        [Header("Dash Info")]
        [SerializeField] private float dashSpeed = 1000;
        [SerializeField] private float dashDuration = 0.2f;
        private float dashTime = 0;
        private bool isDashing = false;

        //���ι���
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

            UpdateState();//����״̬����

            AnimatorController();//Animator����

            PlayerInput();//��ҿ�������
        }

        private void UpdateState()
        {
            isMoving = isMoving && xInput != 0;

            dashTime -= Time.deltaTime;//���³�̳���
            isDashing = dashTime > 0;//�ж��Ƿ��ڳ��

            //���ó���ڼ佺�����С
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

            comboTime -= Time.deltaTime;//����������ʱʱ��
        }

        private void AnimatorController()
        {
            //��ת���
            if (xInput != 0) FlipCharacter(xInput > 0);

            //����Animator����
            anim.SetBool("IsMove", isMoving);
            anim.SetBool("IsJump", isJumping);
            anim.SetFloat("yVelocity", rb.velocity.y);
            anim.SetBool("IsDash", isDashing);
            anim.SetBool("IsAttack", isAttacking);
            anim.SetInteger("comboCounter", comboCounter);
        }

        private void PlayerInput()
        {
            //����ˮƽ����
            xInput = isAttacking ? 0 : Input.GetAxisRaw("Horizontal");

            //��Ӧ��Ծ�¼�
            if (Input.GetButtonDown("Jump"))
            {
                Jump();
                isAttacking = false;
            }

            //��Ӧ����¼�
            if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing)
            {
                dashTime = dashDuration;
                isAttacking = false;
            }

            //��Ӧ�����¼�
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
            //����ˮƽ�ƶ�
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


