using UnityEngine;

[RequireComponent(typeof(HeroMovement))]
[RequireComponent(typeof(UnitRuntimeStats))]
[RequireComponent(typeof(UnitTargetSensor2D))]
public class HeroCombat : MonoBehaviour
{
    [SerializeField] private HeroAttackDefinition _attackDefinition;
    [SerializeField] private LayerMask _targetLayerMask;
    [SerializeField] private string _currentMode;
    [SerializeField] private string _currentTargetName;

    private HeroMovement _movement;
    private UnitRuntimeStats _stats;
    private UnitTargetSensor2D _sensor;
    private HeroController _hero;
    private Transform _chaseTarget;
    private float _attackTimer;
    private bool _missingAttackDefinitionReported;

    private void Awake()
    {
        _movement = GetComponent<HeroMovement>();
        _stats = GetComponent<UnitRuntimeStats>();
        _sensor = GetComponent<UnitTargetSensor2D>();
        _hero = GetComponent<HeroController>();
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

        Transform autoTarget = _sensor.FindNearestTarget(GetAttackRange());

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

    public void SetAttackDefinition(HeroAttackDefinition attackDefinition)
    {
        _attackDefinition = attackDefinition;
        _attackTimer = 0f;
        _missingAttackDefinitionReported = false;
    }

    private void TickChaseTarget()
    {
        if (IsInvalidTarget(_chaseTarget))
        {
            ClearChaseTarget();
            return;
        }

        float attackRange = GetAttackRange();
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

        if (_attackDefinition == null)
        {
            ReportMissingAttackDefinition();
            return;
        }

        HeroAttackContext context = new HeroAttackContext
        {
            Hero = _hero,
            Stats = _stats,
            Target = target,
            TargetLayerMask = _targetLayerMask
        };

        _attackDefinition.Execute(context);
        _attackTimer = _attackDefinition.GetCooldown(_stats);
    }

    private float GetAttackRange()
    {
        if (_attackDefinition == null)
            return _stats.AttackRange;

        return _attackDefinition.GetRange(_stats);
    }

    private void ReportMissingAttackDefinition()
    {
        if (_missingAttackDefinitionReported)
            return;

        _missingAttackDefinitionReported = true;
        Debug.LogError($"{nameof(HeroCombat)} on {name} requires a {nameof(HeroAttackDefinition)}.", this);
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
