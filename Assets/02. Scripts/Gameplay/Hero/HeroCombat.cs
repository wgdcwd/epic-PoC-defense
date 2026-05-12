using UnityEngine;

[RequireComponent(typeof(HeroMovement))]
[RequireComponent(typeof(UnitRuntimeStats))]
[RequireComponent(typeof(UnitTargetSensor2D))]
public class HeroCombat : MonoBehaviour
{
    [SerializeField] private string _currentMode;
    [SerializeField] private string _currentTargetName;

    private HeroMovement _movement;
    private UnitRuntimeStats _stats;
    private UnitTargetSensor2D _sensor;
    private Transform _chaseTarget;
    private float _attackTimer;

    private void Awake()
    {
        _movement = GetComponent<HeroMovement>();
        _stats = GetComponent<UnitRuntimeStats>();
        _sensor = GetComponent<UnitTargetSensor2D>();
    }

    private void Update()
    {
        if (_stats.IsDead)
            return;

        _attackTimer -= Time.deltaTime;

        if (_chaseTarget != null)
        {
            TickChaseTarget();
            return;
        }

        if (_movement.IsMoving)
        {
            SetDebugTarget("Move", null);
            return;
        }

        Transform autoTarget = _sensor.FindNearestTarget(_stats.AttackRange);

        if (autoTarget == null)
        {
            SetDebugTarget("Hold", null);
            return;
        }

        SetDebugTarget("AutoAttack", autoTarget);
        TryAttack(autoTarget);
    }

    public void SetChaseTarget(Transform target)
    {
        _chaseTarget = target;
        SetDebugTarget("Chase", target);
    }

    public void ClearChaseTarget()
    {
        _chaseTarget = null;
    }

    private void TickChaseTarget()
    {
        if (IsInvalidTarget(_chaseTarget))
        {
            ClearChaseTarget();
            return;
        }

        float attackRange = _stats.AttackRange;
        float sqrDistance = (_chaseTarget.position - transform.position).sqrMagnitude;

        if (sqrDistance > attackRange * attackRange)
        {
            _movement.MoveTo(_chaseTarget.position);
            SetDebugTarget("Chase", _chaseTarget);
            return;
        }

        _movement.Stop();
        SetDebugTarget("ChaseAttack", _chaseTarget);
        TryAttack(_chaseTarget);
    }

    private void TryAttack(Transform target)
    {
        if (_attackTimer > 0f)
            return;

        IDamageable damageable = target.GetComponentInParent<IDamageable>();

        if (damageable == null)
            return;

        damageable.TakeDamage(_stats.AttackPower);
        _attackTimer = _stats.AttackCooldown;
    }

    private bool IsInvalidTarget(Transform target)
    {
        if (target == null)
            return true;

        UnitRuntimeStats targetStats = target.GetComponentInParent<UnitRuntimeStats>();

        return targetStats != null && targetStats.IsDead;
    }

    private void SetDebugTarget(string mode, Transform target)
    {
        _currentMode = mode;
        _currentTargetName = target != null ? target.name : string.Empty;
    }
}
