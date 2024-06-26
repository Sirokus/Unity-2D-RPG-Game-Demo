using TMPro;
using UnityEngine;

public class Blackhole_Hotkey_Controller : MonoBehaviour
{
    private SpriteRenderer sr;
    private KeyCode myHotKey;
    private TextMeshProUGUI myText;

    private Transform enemy;
    private Blackhole_Skill_Controller blackhole;

    public void SetupHotKey(KeyCode _hotKey, Transform _enemy, Blackhole_Skill_Controller _blackhole)
    {
        sr = GetComponent<SpriteRenderer>();
        myText = GetComponentInChildren<TextMeshProUGUI>();

        enemy = _enemy;
        blackhole = _blackhole;

        myHotKey = _hotKey;
        myText.text = myHotKey.ToString();
    }

    private void Update()
    {
        if (Input.GetKeyDown(myHotKey))
        {
            blackhole.targets.Add(enemy);

            myText.color = Color.clear;
            sr.color = Color.clear;
        }
    }
}
