using UnityEngine;

[RequireComponent(typeof(MonsterMovement))]
[RequireComponent(typeof(UnitRuntimeStats))]
[RequireComponent(typeof(MonsterCombat))]
[RequireComponent(typeof(UnitTargetSensor2D))]
[RequireComponent(typeof(MonsterPathFollower))]
public class MonsterController : MonoBehaviour
{
    [SerializeField] private DefenseBase _defenseBase;
    [SerializeField] private string _currentState;
    [SerializeField] private string _currentTargetName;

    private MonsterMovement _movement;
    private UnitRuntimeStats _stats;
    private MonsterCombat _combat;
    private UnitTargetSensor2D _sensor;
    private MonsterPathFollower _pathFollower;
    private Transform _targetHero;

    private void Awake()
    {
        _movement = GetComponent<MonsterMovement>();
        _stats = GetComponent<UnitRuntimeStats>();
        _combat = GetComponent<MonsterCombat>();
        _sensor = GetComponent<UnitTargetSensor2D>();
        _pathFollower = GetComponent<MonsterPathFollower>();

        if (_defenseBase == null)
            _defenseBase = FindFirstObjectByType<DefenseBase>();
    }

    private void Update()
    {
        if (_stats.IsDead)
            return;

        UpdateTarget();

        if (_targetHero == null)
        {
            TickBaseOrPath();
            return;
        }

        TickTarget("ChaseHero", "AttackHero", _targetHero);
    }

    private void TickBaseOrPath()
    {
        if (_pathFollower.IsPathComplete || !_pathFollower.HasPath)
        {
            if (_defenseBase == null || _defenseBase.IsDestroyed)
            {
                _movement.Stop();
                SetDebugState("Idle", null);
                return;
            }

            TickTarget("MoveToBase", "AttackBase", _defenseBase.transform);
            return;
        }

        _pathFollower.TickPath();
        SetDebugState("FollowPath", null);
    }

    private void TickTarget(string moveState, string attackState, Transform target)
    {
        float attackRange = _stats.AttackRange;
        float sqrDistance = (target.position - transform.position).sqrMagnitude;

        if (sqrDistance > attackRange * attackRange)
        {
            if (!IsDefenseBaseTarget(target))
                _pathFollower.ReleasePath();

            _movement.MoveTo(target.position);
            SetDebugState(moveState, target);
            return;
        }

        if (!IsDefenseBaseTarget(target))
            _pathFollower.ReleasePath();

        _movement.Stop();
        SetDebugState(attackState, target);
        _combat.Attack(target);
    }

    private bool IsDefenseBaseTarget(Transform target)
    {
        return _defenseBase != null && target == _defenseBase.transform;
    }

    private void UpdateTarget()
    {
        if (_sensor.IsTargetInsideRange(_targetHero))
            return;

        _targetHero = _sensor.FindNearestTarget();
    }

    private void SetDebugState(string state, Transform target)
    {
        _currentState = state;
        _currentTargetName = target != null ? target.name : string.Empty;
    }
}
