using System.Collections.Generic;
using UnityEngine;

public class MeleeArcHitbox : MonoBehaviour
{
    [SerializeField] private float _lifeTime = 0.15f;

    private readonly List<Transform> _hitTargets = new List<Transform>();

    private Vector2 _origin;
    private Vector2 _direction;
    private float _range;
    private float _arcAngle;
    private int _maxTargets;
    private float _damage;
    private LayerMask _targetLayerMask;
    private float _remainingLifeTime;
    private bool _hasHit;

    private void Update()
    {
        _remainingLifeTime -= Time.deltaTime;

        if (!_hasHit)
        {
            HitTargets();
            _hasHit = true;
        }

        if (_remainingLifeTime <= 0f)
            Destroy(gameObject);
    }

    public void Initialize(
        Vector2 origin,
        Vector2 direction,
        float range,
        float arcAngle,
        int maxTargets,
        float damage,
        float lifeTime,
        LayerMask targetLayerMask)
    {
        _origin = origin;
        _direction = direction.sqrMagnitude > 0f ? direction.normalized : Vector2.right;
        _range = range;
        _arcAngle = arcAngle;
        _maxTargets = Mathf.Max(1, maxTargets);
        _damage = damage;
        _lifeTime = lifeTime;
        _remainingLifeTime = _lifeTime;
        _targetLayerMask = targetLayerMask;

        transform.position = origin;
        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void HitTargets()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(_origin, _range, _targetLayerMask);
        int hitCount = 0;

        for (int i = 0; i < hits.Length; i++)
        {
            UnitRuntimeStats targetStats = hits[i].GetComponentInParent<UnitRuntimeStats>();
            Transform target = targetStats != null ? targetStats.transform : hits[i].transform;

            if (_hitTargets.Contains(target))
                continue;

            Vector2 toTarget = ((Vector2)target.position - _origin).normalized;

            if (Vector2.Angle(_direction, toTarget) > _arcAngle * 0.5f)
                continue;

            IDamageable damageable = target.GetComponentInParent<IDamageable>();

            if (damageable == null)
                continue;

            damageable.TakeDamage(_damage);
            _hitTargets.Add(target);
            hitCount++;

            if (hitCount >= _maxTargets)
                return;
        }
    }
}
