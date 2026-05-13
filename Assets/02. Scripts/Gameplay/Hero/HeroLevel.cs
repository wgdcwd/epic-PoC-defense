using UnityEngine;

[RequireComponent(typeof(UnitRuntimeStats))]
public class HeroLevel : MonoBehaviour
{
    [SerializeField] private int _level = 1;
    [SerializeField] private int _currentExperience;
    [SerializeField] private int _baseExperienceToLevelUp = 100;
    [SerializeField] private int _experienceIncreasePerLevel = 50;

    [Header("Level Up Stats")]
    [SerializeField] private float _maxHealthPerLevel = 10f;
    [SerializeField] private float _attackPowerPerLevel = 2f;
    [SerializeField] private float _defensePerLevel = 0.5f;
    [SerializeField] private float _attackRangePerLevel;
    [SerializeField] private float _attackCooldownReductionPerLevel = 0.02f;

    private UnitRuntimeStats _stats;

    public int Level => _level;
    public int CurrentExperience => _currentExperience;
    public int ExperienceToNextLevel => GetExperienceToLevelUp();

    private void Awake()
    {
        _stats = GetComponent<UnitRuntimeStats>();
    }

    public void AddExperience(int experience)
    {
        if (experience <= 0)
            return;

        _currentExperience += experience;

        while (_currentExperience >= GetExperienceToLevelUp())
        {
            _currentExperience -= GetExperienceToLevelUp();
            LevelUp();
        }
    }

    private void LevelUp()
    {
        _level++;
        _stats.AddPermanentStats(
            _maxHealthPerLevel,
            _attackPowerPerLevel,
            _defensePerLevel,
            _attackRangePerLevel,
            _attackCooldownReductionPerLevel);
    }

    private int GetExperienceToLevelUp()
    {
        return Mathf.Max(1, _baseExperienceToLevelUp + (_level - 1) * _experienceIncreasePerLevel);
    }
}
