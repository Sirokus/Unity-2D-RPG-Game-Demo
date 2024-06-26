using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour, ISaveManager
{
    [Header("Global")]
    public static UIManager ins;
    public Transform canvas;
    public GameObject[] needUnVisibleWhenStart;

    [Header("Dead UI")]
    public ScreenFadeUI screenFade;
    public Button restartBtn;

    [Header("SettingsUI")]
    public VolumeSliderUI[] volumeSliders;
    public Toggle showPlayerHealthBar;

    [Header("DialogueUI")]
    public DialogueManager dialogueManager;

    [Header("ShopUI")]
    public ShopInventoryUI shopUI;
    public TradingPanelUI tradingUI;

    [Header("InventoryUI")]
    public DropPanelUI dropUI;

    [Header("Task UI")]
    public TaskUI taskUI;

    [Header("TextPopUI")]
    public List<TextPopSO> popTextList;
    public Transform popTextParent;

    private void Awake()
    {
        ins = this;
    }

    void Start()
    {
        foreach (GameObject go in needUnVisibleWhenStart)
        {
            go.SetActive(false);
        }

        screenFade.gameObject.SetActive(true);

        restartBtn.onClick.AddListener(() => GameManager.RestartGame());
    }

    public void PlayDeadUI()
    {
        screenFade.FadeOut();
    }
    public void ExitGame()
    {
        SaveManager.AutoSave();
        TimerManager.clearAllTimer();
        SceneManager.LoadScene("StartScene");
    }

    public TextPopSO GetTextPopSOByType(EPopTextType type)
    {
        foreach (var so in popTextList)
        {
            if (so.popTextType == type)
            {
                return so;
            }
        }
        return null;
    }

    public void LoadData(GameData data)
    {
        foreach (VolumeSliderUI ui in volumeSliders)
        {
            if (data.haveValue(ui.parameter))
            {
                ui.SliderValue(data.getValuef(ui.parameter));
                ui.slider.value = data.getValuef(ui.parameter);
            }
        }

        if (data.haveValueb("showPlayerHealthBar"))
            showPlayerHealthBar.isOn = data.getValueb("showPlayerHealthBar");
        PlayerManager.GetPlayer().setShowHealthBarUI(showPlayerHealthBar.isOn);
    }
    public void SaveData(GameData data)
    {
        foreach (VolumeSliderUI ui in volumeSliders)
        {
            data.addValue(ui.parameter, ui.slider.value);
        }

        data.addValue("showPlayerHealthBar", showPlayerHealthBar.isOn);
    }
}
