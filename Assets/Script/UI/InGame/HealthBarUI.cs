using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public bool needFlip = true;
    private Entity entity => GetComponentInParent<Entity>();
    public CharacterStat stat;

    private RectTransform rect;

    private Slider slider;
    private TextMeshProUGUI healthText;

    private void Awake()
    {
        if (stat == null)
            stat = GetComponentInParent<CharacterStat>();

        rect = GetComponent<RectTransform>();
        slider = GetComponentInChildren<Slider>();
        healthText = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        UpdateHealthUI();
    }

    private void onFlipped(bool right)
    {
        //rect.localScale = new Vector3(right ? 0.005f : -0.005f, 0.005f, 0.005f);
        rect.rotation = Quaternion.Euler(0, right ? 0 : 1, 0);
    }

    private void UpdateHealthUI(int notUse = 0)
    {
        slider.maxValue = stat.getMaxHealthValue();
        slider.value = Mathf.Clamp(stat.currentHealth, 0, slider.maxValue);

        healthText.SetText(slider.value + "/" + slider.maxValue);
    }

    private void OnEnable()
    {
        if (needFlip)
        {
            entity.onFlipped += onFlipped;
            onFlipped(entity.isFacingRight);
        }

        stat.onTakeDamage.AddListener(UpdateHealthUI);
        stat.onHealHealth.AddListener(UpdateHealthUI);

        UpdateHealthUI();
    }

    private void OnDisable()
    {
        if (needFlip && entity)
            entity.onFlipped -= onFlipped;
        stat.onTakeDamage.RemoveListener(UpdateHealthUI);
        stat.onHealHealth.RemoveListener(UpdateHealthUI);
    }
}
