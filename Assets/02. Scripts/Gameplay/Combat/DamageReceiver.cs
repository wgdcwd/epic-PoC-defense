using UnityEngine;

[RequireComponent(typeof(UnitRuntimeStats))]
public class DamageReceiver : MonoBehaviour, IDamageable
{
    private UnitRuntimeStats _stats;
    private IDeathHandler _deathHandler;

    private void Awake()
    {
        _stats = GetComponent<UnitRuntimeStats>();
        _deathHandler = GetComponent<IDeathHandler>();
    }

    public void TakeDamage(float damage)
    {
        if (_stats.IsDead)
            return;

        if (damage <= 0f)
            return;

        _stats.ReduceHealth(damage);

        if (_stats.IsDead)
            _deathHandler?.HandleDeath();
    }
}
