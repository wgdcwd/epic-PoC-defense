using UnityEngine;

[RequireComponent(typeof(MonsterMovement))]
[RequireComponent(typeof(UnitRuntimeStats))]
public class MonsterController : MonoBehaviour, IDeathHandler
{
    [SerializeField] private LayerMask _heroLayerMask;
    [SerializeField] private float _detectionRange = 5f;
    [SerializeField] private string _currentState;
    [SerializeField] private string _currentTargetName;

    private MonsterMovement _movement;
    private UnitRuntimeStats _stats;
    private Transform _targetHero;
    private float _attackTimer;
    private bool _isDead;

    private void Awake()
    {
        _movement = GetComponent<MonsterMovement>();
        _stats = GetComponent<UnitRuntimeStats>();
    }

    private void Update()
    {
        if (_isDead || _stats.IsDead)
            return;

        _attackTimer -= Time.deltaTime;
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
        TryAttack(_targetHero);
    }

    public void HandleDeath()
    {
        if (_isDead)
            return;

        _isDead = true;
        _movement.Stop();
        gameObject.SetActive(false);
    }

    private void UpdateTarget()
    {
        if (!IsInvalidTarget(_targetHero) && IsTargetInsideDetectionRange(_targetHero))
            return;

        _targetHero = FindNearestHero();
    }

    private Transform FindNearestHero()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _detectionRange, _heroLayerMask);
        Transform nearestHero = null;
        float nearestDistance = float.MaxValue;

        for (int i = 0; i < hits.Length; i++)
        {
            Transform hero = GetStatsTransform(hits[i]);

            if (IsInvalidTarget(hero))
                continue;

            float distance = (hero.position - transform.position).sqrMagnitude;

            if (distance >= nearestDistance)
                continue;

            nearestDistance = distance;
            nearestHero = hero;
        }

        return nearestHero;
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

    private bool IsTargetInsideDetectionRange(Transform target)
    {
        float sqrDistance = (target.position - transform.position).sqrMagnitude;
        return sqrDistance <= _detectionRange * _detectionRange;
    }

    private void SetDebugState(string state, Transform target)
    {
        _currentState = state;
        _currentTargetName = target != null ? target.name : string.Empty;
    }

    private Transform GetStatsTransform(Collider2D hit)
    {
        UnitRuntimeStats stats = hit.GetComponentInParent<UnitRuntimeStats>();

        return stats != null ? stats.transform : hit.transform;
    }
}
