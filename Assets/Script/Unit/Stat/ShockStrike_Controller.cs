using UnityEngine;

public class ShockStrike_Controller : MonoBehaviour
{
    [SerializeField] private CharacterStat owner;
    [SerializeField] private CharacterStat targetStat;
    [SerializeField] private float speed = 5;
    private int damage;

    private Animator anim;
    private bool triggered;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    public void Setup(int damage, CharacterStat owner, CharacterStat targetStat)
    {
        this.damage = damage;
        this.owner = owner;
        this.targetStat = targetStat;
    }

    // Update is called once per frame
    void Update()
    {
        if (triggered || !targetStat)
            return;

        transform.position = Vector2.MoveTowards(transform.position, targetStat.transform.position, speed * Time.deltaTime);
        transform.right = transform.position - targetStat.transform.position;

        if (Vector2.Distance(transform.position, targetStat.transform.position) < .1f)
        {
            anim.transform.localRotation = Quaternion.identity;
            anim.transform.localPosition = new Vector3(0, .5f);

            transform.localRotation = Quaternion.identity;
            transform.localScale = new Vector3(3, 3, 3);

            anim.SetTrigger("Hit");
            triggered = true;

            Invoke("DamageAndSelfDestroy", .2f);
        }
    }

    private void DamageAndSelfDestroy()
    {
        owner.DoMagicalDamage(targetStat, 0, 0, 10);
        Destroy(gameObject, .4f);
    }
}
