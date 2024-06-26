using TMPro;
using UnityEngine;

public class TipsUI_Achievement : MonoBehaviour
{
    public TextMeshProUGUI nameTxt, descTxt;
    public Animator anim;

    public void Play(string achiName, string achiDesc)
    {
        nameTxt.text = achiName;
        descTxt.text = achiDesc;

        anim.SetTrigger("Play");
    }
}
