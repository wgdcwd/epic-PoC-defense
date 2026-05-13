using UnityEngine;

public abstract class HeroAttackDefinition : ScriptableObject
{
    [SerializeField] private string _displayName;
    [SerializeField] private float _rangeMultiplier = 1f;
    [SerializeField] private float _cooldownMultiplier = 1f;
    [SerializeField] private float _damageMultiplier = 1f;

    public string DisplayName => string.IsNullOrWhiteSpace(_displayName) ? name : _displayName;
    public float RangeMultiplier => Mathf.Max(0.1f, _rangeMultiplier);
    public float CooldownMultiplier => Mathf.Max(0.05f, _cooldownMultiplier);
    public float DamageMultiplier => Mathf.Max(0f, _damageMultiplier);

    public float GetRange(UnitRuntimeStats stats)
    {
        return stats.AttackRange * RangeMultiplier;
    }

    public float GetCooldown(UnitRuntimeStats stats)
    {
        return stats.AttackCooldown * CooldownMultiplier;
    }

    public abstract void Execute(HeroAttackContext context);
}
