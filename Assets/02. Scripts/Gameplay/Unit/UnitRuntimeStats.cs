using UnityEngine;

public class UnitRuntimeStats : MonoBehaviour
{
    [SerializeField] private UnitDefinition _definition;
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _attackPower = 10f;
    [SerializeField] private float _defense = 0f;
    [SerializeField] private float _attackRange = 1.5f;
    [SerializeField] private float _attackCooldown = 0.8f;

    [SerializeField] private float _currentHealth;

    public float MaxHealth => _maxHealth;
    public float CurrentHealth => _currentHealth;
    public float AttackPower => _attackPower;
    public float Defense => _defense;
    public float AttackRange => _attackRange;
    public float AttackCooldown => _attackCooldown;
    public UnitDefinition Definition => _definition;
    public bool IsDead => _currentHealth <= 0f;

    private void Awake()
    {
        if (_definition != null)
            ApplyDefinition(_definition);

        _currentHealth = _maxHealth;
    }

    public void InitializeFromDefinition(UnitDefinition definition)
    {
        if (definition == null)
            return;

        _definition = definition;
        ApplyDefinition(_definition);
        _currentHealth = _maxHealth;
    }

    public void ReduceHealth(float damage)
    {
        if (damage <= 0f)
            return;

        float finalDamage = Mathf.Max(1f, damage - _defense);
        _currentHealth = Mathf.Max(0f, _currentHealth - finalDamage);
    }

    public void AddPermanentStats(
        float maxHealthBonus,
        float attackPowerBonus,
        float defenseBonus,
        float attackRangeBonus,
        float attackCooldownReduction)
    {
        _maxHealth = Mathf.Max(1f, _maxHealth + maxHealthBonus);
        _attackPower = Mathf.Max(0f, _attackPower + attackPowerBonus);
        _defense = Mathf.Max(0f, _defense + defenseBonus);
        _attackRange = Mathf.Max(0.1f, _attackRange + attackRangeBonus);
        _attackCooldown = Mathf.Max(0.1f, _attackCooldown - attackCooldownReduction);

        if (maxHealthBonus > 0f && !IsDead)
            _currentHealth = Mathf.Min(_maxHealth, _currentHealth + maxHealthBonus);
    }

    public void AddStatModifier(EquipmentStatModifier modifier)
    {
        ApplyStatModifier(modifier, 1f);
    }

    public void RemoveStatModifier(EquipmentStatModifier modifier)
    {
        ApplyStatModifier(modifier, -1f);
    }

    public void Heal(float amount)
    {
        if (amount <= 0f)
            return;

        _currentHealth = Mathf.Min(_maxHealth, _currentHealth + amount);
    }

    public void RestoreHealthToFull()
    {
        _currentHealth = _maxHealth;
    }

    private void ApplyStatModifier(EquipmentStatModifier modifier, float sign)
    {
        float maxHealthDelta = modifier.MaxHealth * sign;

        _maxHealth = Mathf.Max(1f, _maxHealth + maxHealthDelta);
        _attackPower = Mathf.Max(0f, _attackPower + modifier.AttackPower * sign);
        _defense = Mathf.Max(0f, _defense + modifier.Defense * sign);
        _attackRange = Mathf.Max(0.1f, _attackRange + modifier.AttackRange * sign);
        _attackCooldown = Mathf.Max(0.1f, _attackCooldown - modifier.AttackCooldownReduction * sign);

        if (maxHealthDelta > 0f && !IsDead)
            _currentHealth = Mathf.Min(_maxHealth, _currentHealth + maxHealthDelta);
        else
            _currentHealth = Mathf.Min(_currentHealth, _maxHealth);
    }

    private void ApplyDefinition(UnitDefinition definition)
    {
        _maxHealth = definition.MaxHealth;
        _attackPower = definition.AttackPower;
        _defense = definition.Defense;
        _attackRange = definition.AttackRange;
        _attackCooldown = definition.AttackCooldown;
    }
}
