using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SelectionInfoView : MonoBehaviour
{
    [SerializeField] private GameObject _root;
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _bodyText;
    [SerializeField] private Button _weaponButton;
    [SerializeField] private TMP_Text _weaponText;
    [SerializeField] private Button _armorButton;
    [SerializeField] private TMP_Text _armorText;
    [SerializeField] private Button _accessoryButton;
    [SerializeField] private TMP_Text _accessoryText;
    [SerializeField] private float _doubleClickSeconds = 0.3f;

    private enum InfoTargetType
    {
        None,
        Hero,
        Monster,
        Equipment
    }

    private InfoTargetType _currentTargetType;
    private HeroController _currentHero;
    private MonsterController _currentMonster;
    private EquipmentPickup _currentEquipmentPickup;
    private float _lastWeaponClickTime = -10f;
    private float _lastArmorClickTime = -10f;
    private float _lastAccessoryClickTime = -10f;

    private void Awake()
    {
        if (_weaponButton != null)
            _weaponButton.onClick.AddListener(OnWeaponClicked);

        if (_armorButton != null)
            _armorButton.onClick.AddListener(OnArmorClicked);

        if (_accessoryButton != null)
            _accessoryButton.onClick.AddListener(OnAccessoryClicked);

        Hide();
    }

    private void OnDestroy()
    {
        if (_weaponButton != null)
            _weaponButton.onClick.RemoveListener(OnWeaponClicked);

        if (_armorButton != null)
            _armorButton.onClick.RemoveListener(OnArmorClicked);

        if (_accessoryButton != null)
            _accessoryButton.onClick.RemoveListener(OnAccessoryClicked);
    }

    private void Update()
    {
        RefreshCurrentTarget();
    }

    public void ShowHero(HeroController hero)
    {
        _currentTargetType = InfoTargetType.Hero;
        _currentHero = hero;
        _currentMonster = null;
        _currentEquipmentPickup = null;

        if (hero == null)
        {
            Hide();
            return;
        }

        SetRootActive(true);
        RefreshHero(hero);
    }

    public void ShowMonster(MonsterController monster)
    {
        _currentTargetType = InfoTargetType.Monster;
        _currentHero = null;
        _currentMonster = monster;
        _currentEquipmentPickup = null;

        if (monster == null)
        {
            Hide();
            return;
        }

        SetRootActive(true);
        RefreshMonster(monster);
    }

    public void ShowEquipment(EquipmentPickup pickup)
    {
        _currentTargetType = InfoTargetType.Equipment;
        _currentHero = null;
        _currentMonster = null;
        _currentEquipmentPickup = pickup;

        if (pickup == null || pickup.Equipment == null)
        {
            Hide();
            return;
        }

        SetRootActive(true);
        RefreshEquipment(pickup);
    }

    public void Hide()
    {
        _currentTargetType = InfoTargetType.None;
        _currentHero = null;
        _currentMonster = null;
        _currentEquipmentPickup = null;
        SetRootActive(false);
    }

    private void RefreshCurrentTarget()
    {
        switch (_currentTargetType)
        {
            case InfoTargetType.Hero:
                RefreshHero(_currentHero);
                break;
            case InfoTargetType.Monster:
                RefreshMonster(_currentMonster);
                break;
            case InfoTargetType.Equipment:
                RefreshEquipment(_currentEquipmentPickup);
                break;
        }
    }

    private void RefreshHero(HeroController hero)
    {
        if (hero == null || !hero.gameObject.activeInHierarchy)
        {
            Hide();
            return;
        }

        UnitRuntimeStats stats = hero.Stats;
        HeroLevel level = hero.Level;

        SetTitle(hero.name);
        SetBody(
            "Hero\n" +
            "Level: " + level.Level + "\n" +
            "EXP: " + level.CurrentExperience + " / " + level.ExperienceToNextLevel + "\n" +
            "HP: " + Mathf.CeilToInt(stats.CurrentHealth) + " / " + Mathf.CeilToInt(stats.MaxHealth) + "\n" +
            "ATK: " + FormatFloat(stats.AttackPower) + "\n" +
            "DEF: " + FormatFloat(stats.Defense) + "\n" +
            "Range: " + FormatFloat(stats.AttackRange) + "\n" +
            "Cooldown: " + FormatFloat(stats.AttackCooldown));

        UpdateEquipmentSlots(hero.Equipment);
    }

    private void RefreshMonster(MonsterController monster)
    {
        if (monster == null || !monster.gameObject.activeInHierarchy)
        {
            Hide();
            return;
        }

        UnitRuntimeStats stats = monster.GetComponent<UnitRuntimeStats>();

        if (stats == null)
        {
            Hide();
            return;
        }

        SetTitle(monster.name);
        SetBody(
            "Monster\n" +
            "HP: " + Mathf.CeilToInt(stats.CurrentHealth) + " / " + Mathf.CeilToInt(stats.MaxHealth) + "\n" +
            "ATK: " + FormatFloat(stats.AttackPower) + "\n" +
            "DEF: " + FormatFloat(stats.Defense));

        ClearEquipmentSlots();
    }

    private void RefreshEquipment(EquipmentPickup pickup)
    {
        if (pickup == null || !pickup.gameObject.activeInHierarchy || pickup.Equipment == null)
        {
            Hide();
            return;
        }

        EquipmentDefinition equipment = pickup.Equipment;
        EquipmentStatModifier modifier = equipment.StatModifier;

        SetTitle(equipment.DisplayName);
        SetBody(
            "Equipment\n" +
            "Type: " + equipment.EquipmentType + "\n" +
            "HP: +" + modifier.MaxHealth + "\n" +
            "ATK: +" + modifier.AttackPower + "\n" +
            "DEF: +" + modifier.Defense + "\n" +
            "Range: +" + modifier.AttackRange + "\n" +
            "Cooldown: -" + modifier.AttackCooldownReduction);

        ClearEquipmentSlots();
    }

    private void OnWeaponClicked()
    {
        TryDropEquipment(EquipmentType.Weapon, ref _lastWeaponClickTime);
    }

    private void OnArmorClicked()
    {
        TryDropEquipment(EquipmentType.Armor, ref _lastArmorClickTime);
    }

    private void OnAccessoryClicked()
    {
        TryDropEquipment(EquipmentType.Accessory, ref _lastAccessoryClickTime);
    }

    private void TryDropEquipment(EquipmentType equipmentType, ref float lastClickTime)
    {
        if (Time.unscaledTime - lastClickTime > _doubleClickSeconds)
        {
            lastClickTime = Time.unscaledTime;
            return;
        }

        lastClickTime = -10f;

        if (_currentHero == null)
            return;

        _currentHero.Equipment.DropEquipped(equipmentType);
        ShowHero(_currentHero);
    }

    private void UpdateEquipmentSlots(HeroEquipment equipment)
    {
        SetEquipmentSlot(_weaponText, equipment.Weapon);
        SetEquipmentSlot(_armorText, equipment.Armor);
        SetEquipmentSlot(_accessoryText, equipment.Accessory);
    }

    private void ClearEquipmentSlots()
    {
        SetEquipmentSlot(_weaponText, null);
        SetEquipmentSlot(_armorText, null);
        SetEquipmentSlot(_accessoryText, null);
    }

    private void SetEquipmentSlot(TMP_Text text, EquipmentDefinition equipment)
    {
        if (text == null)
            return;

        text.text = equipment != null ? equipment.DisplayName : "-";
    }

    private void SetTitle(string text)
    {
        if (_titleText != null)
            _titleText.text = text;
    }

    private void SetBody(string text)
    {
        if (_bodyText != null)
            _bodyText.text = text;
    }

    private void SetRootActive(bool isActive)
    {
        if (_root != null)
            _root.SetActive(isActive);
    }

    private string FormatFloat(float value)
    {
        return value.ToString("0.##");
    }
}
