using UnityEngine;

[CreateAssetMenu(fileName = "New Item Effect", menuName = "Data/Effect/IceAndFire")]
public class Effect_IceAndFire : ItemEffect
{
    [SerializeField] private GameObject IceAndFirePrefab;
    [SerializeField] private Vector2 velocity;

    public override void execute(Transform target)
    {
        Transform player = PlayerManager.GetPlayer().transform;

        bool isThirdAttack = player.GetComponent<Player>().primaryAttack.comboCounter == 2;
        if (!isThirdAttack)
            return;

        GameObject projectile = Instantiate(IceAndFirePrefab, target.position, player.rotation);
        projectile.GetComponent<Rigidbody2D>().velocity = new Vector2(velocity.x * (PlayerManager.isFacingRight ? 1 : -1), velocity.y);
    }
}
