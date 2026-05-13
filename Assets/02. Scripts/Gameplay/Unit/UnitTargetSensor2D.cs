using UnityEngine;

public class UnitTargetSensor2D : MonoBehaviour
{
    [SerializeField] private LayerMask _targetLayerMask;
    [SerializeField] private float _detectionRange = 5f;

    public float DetectionRange => _detectionRange;

    public Transform FindNearestTarget()
    {
        return FindNearestTarget(_detectionRange);
    }

    public Transform FindNearestTarget(float range)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range, _targetLayerMask);
        Transform nearestTarget = null;
        float nearestDistance = float.MaxValue;

        for (int i = 0; i < hits.Length; i++)
        {
            Transform target = GetTargetRoot(hits[i]);

            if (IsInvalidTarget(target))
                continue;

            float distance = (target.position - transform.position).sqrMagnitude;

            if (distance >= nearestDistance)
                continue;

            nearestDistance = distance;
            nearestTarget = target;
        }

        return nearestTarget;
    }

    public bool IsTargetInsideRange(Transform target)
    {
        return IsTargetInsideRange(target, _detectionRange);
    }

    public bool IsTargetInsideRange(Transform target, float range)
    {
        if (IsInvalidTarget(target))
            return false;

        float sqrDistance = (target.position - transform.position).sqrMagnitude;
        return sqrDistance <= range * range;
    }

    private Transform GetTargetRoot(Collider2D hit)
    {
        UnitRuntimeStats stats = hit.GetComponentInParent<UnitRuntimeStats>();

        if (stats != null)
            return stats.transform;

        IDamageable damageable = hit.GetComponentInParent<IDamageable>();
        MonoBehaviour damageableBehaviour = damageable as MonoBehaviour;

        return damageableBehaviour != null ? damageableBehaviour.transform : hit.transform;
    }

    private bool IsInvalidTarget(Transform target)
    {
        if (target == null)
            return true;

        UnitRuntimeStats stats = target.GetComponentInParent<UnitRuntimeStats>();

        if (stats != null && stats.IsDead)
            return true;

        HeroController hero = target.GetComponentInParent<HeroController>();

        return hero != null && !hero.IsRecruited;
    }
}
