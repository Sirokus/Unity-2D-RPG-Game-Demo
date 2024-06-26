using UnityEngine;

namespace Old
{
    public class Enemy : Entity
    {
        [Header("Player detection")]
        [SerializeField] private float playerCheckDistance = 4;
        [SerializeField] private LayerMask PlayerMask;

        private RaycastHit2D isPlayerDetection;

        //private bool isAttacking = false;

        protected override void Start()
        {
            base.Start();
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();

            isPlayerDetection = Physics2D.Raycast(transform.position, Vector2.right*dir, playerCheckDistance, PlayerMask);

        }

        private void FixedUpdate()
        {
            if (!isPlayerDetection)
            {
                AddMoveInput(dir);
            }
            else
            {
                if (isPlayerDetection.distance > 1)
                {
                    AddMoveInput(dir * 2);
                }
                else
                {
                    //Attack...
                }

                return;
            }

            if (isJumping || isWallDetected)
            {
                bool isRight = dir == 1;
                FlipCharacter(!isRight);
            }
        }
    }
}

