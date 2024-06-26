using UnityEngine;

public class ScreenFadeUI : MonoBehaviour
{
    public Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void FadeOut() => anim.SetTrigger("FadeOut");
    public void FadeIn() => anim.SetTrigger("FadeIn");
}
