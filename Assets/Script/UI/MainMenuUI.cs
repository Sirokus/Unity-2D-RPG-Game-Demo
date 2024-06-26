using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public Button startBtn;
    public Button saveSlotBtn;
    public Button settingsBtn;
    public Button exitButton;

    public TextMeshProUGUI Title;

    public ScreenFadeUI screenFade;

    private void Start()
    {
        startBtn.onClick.AddListener(StartGame);
        exitButton.onClick.AddListener(ExitGame);

        if (SaveManager.ins.slotNum <= 0)
            startBtn.GetComponentInChildren<TextMeshProUGUI>().text = "新的开始";
        else
            startBtn.GetComponentInChildren<TextMeshProUGUI>().text = "继续游戏";
    }

    private void Update()
    {
        float t = Time.time / 6;
        float cycle = t - Mathf.Floor(t);
        float value = Mathf.Abs(2 * cycle - 1);
        //Title.color = Color.HSVToRGB(value, 1, 1);
    }

    public void StartGame()
    {
        StartCoroutine(startGameCoroutine());
    }

    IEnumerator startGameCoroutine()
    {
        screenFade.FadeOut();

        yield return new WaitForSeconds(0.8f);

        if (SaveManager.ins.slotNum <= 0)
            SaveManager.ins.CreateGame(false);
        SaveManager.ins.LoadGame(0);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
