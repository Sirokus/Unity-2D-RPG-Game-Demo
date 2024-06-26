using UnityEngine;

public class BossSpellController : MonoBehaviour
{
    [SerializeField] Transform check;
    [SerializeField] Vector2 boxSize;
    [SerializeField] LayerMask playerLayer;

    private CharacterStat myStat;

    public void SetupSpell(CharacterStat stat)
    {
        myStat = stat;
    }

    private void AnimationTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(check.position, boxSize, playerLayer);

        foreach (var hit in colliders)
        {
            if (hit.TryGetComponent(out PlayerStat stat))
            {
                myStat.DoDamage(hit.GetComponent<CharacterStat>());
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(check.position, boxSize);
    }

    private void SelfDestroy()
    {
        Timer timer = null;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        timer = TimerManager.addTimer(.02f, true, () =>
        {
            Color c = sr.color;
            c.a *= 0.9f;
            sr.color = c;

            if (c.a < 0.1f)
            {
                Destroy(gameObject);
                TimerManager.clearTimer(timer);
            }
        });
    }
}
