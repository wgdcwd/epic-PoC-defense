using UnityEngine;

[CreateAssetMenu(fileName = "UnitDefinition", menuName = "Game/Data/Unit Definition")]
public class UnitDefinition : ScriptableObject
{
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _attackPower = 10f;
    [SerializeField] private float _defense;
    [SerializeField] private float _attackRange = 1.5f;
    [SerializeField] private float _attackCooldown = 0.8f;

    public float MaxHealth => _maxHealth;
    public float AttackPower => _attackPower;
    public float Defense => _defense;
    public float AttackRange => _attackRange;
    public float AttackCooldown => _attackCooldown;
}
