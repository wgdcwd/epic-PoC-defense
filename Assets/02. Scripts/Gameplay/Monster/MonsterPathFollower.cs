using UnityEngine;

[RequireComponent(typeof(MonsterMovement))]
public class MonsterPathFollower : MonoBehaviour
{
    [SerializeField] private MonsterPath _path;
    [SerializeField] private float _waypointReachDistance = 0.2f;
    [SerializeField] private bool _stopAtEnd = true;
    [SerializeField] private int _currentWaypointIndex;
    [SerializeField] private bool _isPathComplete;
    [SerializeField] private bool _hasSelectedWaypoint;

    private MonsterMovement _movement;

    public bool HasPath => _path != null && _path.HasWaypoints;
    public bool IsPathComplete => _isPathComplete;

    private void Awake()
    {
        _movement = GetComponent<MonsterMovement>();
    }

    public void SetPath(MonsterPath path)
    {
        _path = path;
        _currentWaypointIndex = 0;
        _isPathComplete = false;
        _hasSelectedWaypoint = false;
    }

    public void ReleasePath()
    {
        _hasSelectedWaypoint = false;
    }

    public void TickPath()
    {
        if (_isPathComplete)
            return;

        if (_path == null || !_path.HasWaypoints)
        {
            _movement.Stop();
            return;
        }

        if (!_hasSelectedWaypoint)
            SelectNearestRemainingWaypoint();

        Transform waypoint = _path.GetWaypoint(_currentWaypointIndex);

        if (waypoint == null)
        {
            AdvanceWaypoint();
            return;
        }

        float sqrDistance = (waypoint.position - transform.position).sqrMagnitude;

        if (sqrDistance <= _waypointReachDistance * _waypointReachDistance)
        {
            AdvanceWaypoint();
            return;
        }

        _movement.MoveTo(waypoint.position);
    }

    private void AdvanceWaypoint()
    {
        _currentWaypointIndex++;
        _hasSelectedWaypoint = true;

        if (_path != null && _currentWaypointIndex < _path.WaypointCount)
            return;

        CompletePath();
    }

    private void SelectNearestRemainingWaypoint()
    {
        if (_path == null || !_path.HasWaypoints)
            return;

        int nearestIndex = -1;
        float nearestDistance = float.MaxValue;

        for (int i = _currentWaypointIndex; i < _path.WaypointCount; i++)
        {
            Transform waypoint = _path.GetWaypoint(i);

            if (waypoint == null)
                continue;

            float distance = (waypoint.position - transform.position).sqrMagnitude;

            if (distance >= nearestDistance)
                continue;

            nearestDistance = distance;
            nearestIndex = i;
        }

        if (nearestIndex < 0)
        {
            CompletePath();
            return;
        }

        _currentWaypointIndex = nearestIndex;
        _hasSelectedWaypoint = true;
    }

    private void CompletePath()
    {
        if (_stopAtEnd)
        {
            _isPathComplete = true;
            _movement.Stop();
            return;
        }

        _currentWaypointIndex = 0;
        _hasSelectedWaypoint = false;
    }
}
