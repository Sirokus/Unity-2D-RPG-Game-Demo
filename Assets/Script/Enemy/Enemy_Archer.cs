using UnityEngine;

public class Enemy_Archer : Enemy_Skeleton
{
    [Header("Arrow")]
    public GameObject ArrowPrefab;
    public float arrowSpeed = 20;
    public Vector2 arrowSpeedRandomRange = new Vector2(-5, 5);
    public Vector2 arrowRandomAngle = new Vector2(-.2f, .2f);
    public int arrowNum = 2;
    public float arrowShootSpaceTime = 1f;

    Timer arrowTimer;
    public override void Attack()
    {
        if (arrowTimer != null)
            return;
        anim.speed = 0;
        TimerManager.clearTimer(arrowTimer);
        arrowTimer = TimerManager.addTimer(arrowShootSpaceTime, true, () =>
        {
            int facingDir = target.position.x > transform.position.x ? 1 : -1;
            Flip(facingDir > 0);

            Bullet newArrow = ObjectPool.Get<Bullet>();
            if (!newArrow)
                return;

            newArrow.transform.position = AttackCheck.position;
            newArrow.Setup(new Vector2(dir, Random.Range(arrowRandomAngle.x, arrowRandomAngle.y)), arrowSpeed + Random.Range(arrowSpeedRandomRange.x, arrowSpeedRandomRange.y), stats);

        }, -arrowShootSpaceTime, arrowNum).OnComplete(() =>
        {
            anim.speed = 1;
            arrowTimer = null;
        });
    }

    public override void Die()
    {
        base.Die();

        TimerManager.clearTimer(arrowTimer);
    }
}
