using UnityEngine;

public class MonsterPath : MonoBehaviour
{
    [SerializeField] private Transform[] _waypoints;

    public int WaypointCount => _waypoints != null ? _waypoints.Length : 0;

    public bool HasWaypoints => WaypointCount > 0;

    public Transform GetWaypoint(int index)
    {
        if (!HasWaypoints)
            return null;

        if (index < 0 || index >= _waypoints.Length)
            return null;

        return _waypoints[index];
    }

    private void OnDrawGizmos()
    {
        if (_waypoints == null || _waypoints.Length == 0)
            return;

        Gizmos.color = Color.red;

        for (int i = 0; i < _waypoints.Length; i++)
        {
            if (_waypoints[i] == null)
                continue;

            Gizmos.DrawSphere(_waypoints[i].position, 0.12f);

            if (i + 1 >= _waypoints.Length || _waypoints[i + 1] == null)
                continue;

            Gizmos.DrawLine(_waypoints[i].position, _waypoints[i + 1].position);
        }
    }
}
