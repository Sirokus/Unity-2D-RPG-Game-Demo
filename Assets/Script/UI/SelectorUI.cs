using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectorUI : MonoBehaviour
{
    public Button[] buttons;
    public Transform[] Canvas;

    private void Awake()
    {
        for (int i = 0; i <  buttons.Length; i++)
        {
            int tmp = i;
            buttons[i].onClick.AddListener(() => select(tmp));
        }

        select(1);
    }

    void select(int index)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i == index)
            {
                Canvas[i].gameObject.SetActive(true);
                buttons[i].GetComponent<Image>().color = Color.white;
                buttons[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
            }
            else
            {
                Canvas[i].gameObject.SetActive(false);
                buttons[i].GetComponent<Image>().color = Color.gray;
                buttons[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.gray;
            }
        }
    }
}
