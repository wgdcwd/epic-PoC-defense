using UnityEngine;

[RequireComponent(typeof(MonsterMovement))]
[RequireComponent(typeof(UnitRuntimeStats))]
[RequireComponent(typeof(MonsterCombat))]
[RequireComponent(typeof(UnitTargetSensor2D))]
public class MonsterController : MonoBehaviour
{
    [SerializeField] private string _currentState;
    [SerializeField] private string _currentTargetName;

    private MonsterMovement _movement;
    private UnitRuntimeStats _stats;
    private MonsterCombat _combat;
    private UnitTargetSensor2D _sensor;
    private Transform _targetHero;

    private void Awake()
    {
        _movement = GetComponent<MonsterMovement>();
        _stats = GetComponent<UnitRuntimeStats>();
        _combat = GetComponent<MonsterCombat>();
        _sensor = GetComponent<UnitTargetSensor2D>();
    }

    private void Update()
    {
        if (_stats.IsDead)
            return;

        UpdateTarget();

        if (_targetHero == null)
        {
            _movement.Stop();
            SetDebugState("Idle", null);
            return;
        }

        float attackRange = _stats.AttackRange;
        float sqrDistance = (_targetHero.position - transform.position).sqrMagnitude;

        if (sqrDistance > attackRange * attackRange)
        {
            _movement.MoveTo(_targetHero.position);
            SetDebugState("ChaseHero", _targetHero);
            return;
        }

        _movement.Stop();
        SetDebugState("AttackHero", _targetHero);
        _combat.Attack(_targetHero);
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
