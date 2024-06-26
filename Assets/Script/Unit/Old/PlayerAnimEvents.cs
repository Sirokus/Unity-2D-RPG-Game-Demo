using UnityEngine;

public class PlayerAnimEvents : MonoBehaviour
{
    private Old.Player player;

    void Start()
    {
        player = GetComponentInParent<Old.Player>();
    }

    private void AnimationTrigger()
    {
        player.OnAttackOver();
    }


}
