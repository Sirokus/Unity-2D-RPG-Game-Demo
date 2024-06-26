using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffUI : MonoBehaviour
{
    public Image img, mask;
    public TextMeshProUGUI nameUI, amountUI;
    public float timer;
    public float count;
    private bool canStart;

    public void setup(Sprite sprite, string name, float timer, int amount)
    {
        img.sprite = mask.sprite = sprite;
        nameUI.text = name;
        amountUI.text = (amount >= 0 ? "+" : "-") + amount.ToString();
        if (amount >= 0)
            amountUI.color = Color.green;
        else
            amountUI.color = Color.red;

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
