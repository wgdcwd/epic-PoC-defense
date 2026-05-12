using UnityEngine;

[RequireComponent(typeof(HeroMovement))]
[RequireComponent(typeof(UnitRuntimeStats))]
[RequireComponent(typeof(HeroCombat))]
public class HeroController : MonoBehaviour
{
    private HeroMovement _movement;
    private UnitRuntimeStats _stats;
    private HeroCombat _combat;

    public HeroMovement Movement => _movement;
    public UnitRuntimeStats Stats => _stats;
    public HeroCombat Combat => _combat;
    public bool IsDead => _stats.IsDead;

    private void Awake()
    {
        _movement = GetComponent<HeroMovement>();
        _stats = GetComponent<UnitRuntimeStats>();
        _combat = GetComponent<HeroCombat>();
    }

    public void MoveTo(Vector2 destination)
    {
        _combat.ClearChaseTarget();
        _movement.MoveTo(destination);
    }

    public void AttackTarget(Transform target)
    {
        _combat.SetChaseTarget(target);
    }

    public void Stop()
    {
        _combat.ClearChaseTarget();
        _movement.Stop();
    }
}
