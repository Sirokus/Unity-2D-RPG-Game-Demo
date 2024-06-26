using UnityEngine;

[CreateAssetMenu(fileName = "New Item Effect", menuName = "Data/Effect/ThunderStrike")]
public class Effect_ThunderStrike : ItemEffect
{
    [SerializeField] private GameObject ThunderStrikePrefab;
    public override void execute(Transform target)
    {
        Instantiate(ThunderStrikePrefab, target.position, Quaternion.identity);
    }
}
