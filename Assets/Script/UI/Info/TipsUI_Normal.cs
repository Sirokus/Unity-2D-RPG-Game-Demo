using TMPro;
using UnityEngine;

public class TipsUI_Normal : MonoBehaviour
{
    public TextMeshProUGUI textUI;
    public Animator animator;

    public void Play(string text)
    {
        textUI.text = text;

        animator.SetTrigger("Play");
    }

}
