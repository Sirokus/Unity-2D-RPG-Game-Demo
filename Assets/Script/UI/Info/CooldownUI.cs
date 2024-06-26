using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CooldownUI : MonoBehaviour
{
    public Image img, mask;
    public TextMeshProUGUI nameUI;
    public float timer;
    public float count;
    private bool canStart;

    public void setup(Sprite sprite, string name, float timer)
    {
        img.sprite = mask.sprite = sprite;
        nameUI.text = name;
        this.timer = timer;
        count = timer;
        canStart = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (canStart)
        {
            timer -= Time.deltaTime;
            mask.fillAmount = timer / count;
            if (timer <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
