using UnityEngine;

public class UnitRuntimeStats : MonoBehaviour
{
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
    public bool IsDead => _currentHealth <= 0f;

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }

    public void ReduceHealth(float damage)
    {
        if (damage <= 0f)
            return;

        float finalDamage = Mathf.Max(1f, damage - _defense);
        _currentHealth = Mathf.Max(0f, _currentHealth - finalDamage);
    }
}
