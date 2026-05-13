using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroHudSlotView : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private Image _portraitImage;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _levelText;
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private Slider _experienceSlider;

    private HeroController _hero;
    private HeroSelectionSystem _selectionSystem;

    private void Awake()
    {
        if (_button == null)
            _button = GetComponent<Button>();

        if (_button != null)
            _button.onClick.AddListener(OnClicked);
    }

    private void OnDestroy()
    {
        if (_button != null)
            _button.onClick.RemoveListener(OnClicked);
    }

    private void Update()
    {
        Refresh();
    }

    public void Initialize(HeroController hero, HeroSelectionSystem selectionSystem)
    {
        _hero = hero;
        _selectionSystem = selectionSystem;
        RefreshStaticInfo();
        Refresh();
    }

    private void RefreshStaticInfo()
    {
        if (_hero == null)
            return;

        if (_portraitImage != null)
        {
            _portraitImage.sprite = _hero.PortraitSprite;
            _portraitImage.enabled = _hero.PortraitSprite != null;
        }

        if (_nameText != null)
            _nameText.text = _hero.DisplayName;
    }

    private void Refresh()
    {
        if (_hero == null)
            return;

        UnitRuntimeStats stats = _hero.Stats;
        HeroLevel level = _hero.Level;

        if (_levelText != null)
            _levelText.text = "Lv." + level.Level;

        if (_healthSlider != null)
        {
            _healthSlider.minValue = 0f;
            _healthSlider.maxValue = Mathf.Max(1f, stats.MaxHealth);
            _healthSlider.value = Mathf.Clamp(stats.CurrentHealth, 0f, _healthSlider.maxValue);
        }

        if (_experienceSlider != null)
        {
            _experienceSlider.minValue = 0f;
            _experienceSlider.maxValue = Mathf.Max(1, level.ExperienceToNextLevel);
            _experienceSlider.value = Mathf.Clamp(level.CurrentExperience, 0, level.ExperienceToNextLevel);
        }
    }

    private void OnClicked()
    {
        if (_selectionSystem == null || _hero == null)
            return;

        _selectionSystem.SelectHero(_hero);
    }
}
