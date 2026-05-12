using UnityEngine;

[RequireComponent(typeof(UnitRuntimeStats))]
public class MonsterCombat : MonoBehaviour
{
    private UnitRuntimeStats _stats;
    private float _attackTimer;

    private void Awake()
    {
        _stats = GetComponent<UnitRuntimeStats>();
    }

    private void Update()
    {
        if (_attackTimer > 0f)
            _attackTimer -= Time.deltaTime;
    }

    public bool CanAttack(Transform target)
    {
        if (target == null)
            return false;

        if (_stats.IsDead)
            return false;

        if (_attackTimer > 0f)
            return false;

        float attackRange = _stats.AttackRange;
        float sqrDistance = (target.position - transform.position).sqrMagnitude;

        return sqrDistance <= attackRange * attackRange;
    }

    public void Attack(Transform target)
    {
        if (!CanAttack(target))
            return;

        IDamageable damageable = target.GetComponentInParent<IDamageable>();

        if (damageable == null)
            return;

        damageable.TakeDamage(_stats.AttackPower);
        _attackTimer = _stats.AttackCooldown;
    }
}
