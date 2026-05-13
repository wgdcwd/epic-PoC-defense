using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class UnitMovement2D : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 4f;
    [SerializeField] private float _stopDistance = 0.12f;
    [SerializeField] private float _blockedStopDistance = 0.35f;
    [SerializeField] private float _blockedTimeLimit = 0.3f;
    [SerializeField] private float _minProgressDistance = 0.005f;

    private Rigidbody2D _rigidbody;

    [SerializeField] private Vector2 _destination;
    [SerializeField] private bool _hasDestination;
    [SerializeField] private float _blockedTimer;

    private Vector2 _lastPosition;

    public float MoveSpeed => _moveSpeed;
    public bool IsMoving => _hasDestination;
    public Vector2 Destination => _destination;

    protected virtual void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _destination = _rigidbody.position;
        _lastPosition = _rigidbody.position;
    }

    private void FixedUpdate()
    {
        if (!_hasDestination)
            return;

        Vector2 currentPosition = _rigidbody.position;
        Vector2 toDestination = _destination - currentPosition;

        if (toDestination.sqrMagnitude <= _stopDistance * _stopDistance)
        {
            Stop();
            return;
        }

        UpdateBlockedTimer(currentPosition);

        if (_blockedTimer >= _blockedTimeLimit &&
            toDestination.sqrMagnitude <= _blockedStopDistance * _blockedStopDistance)
        {
            Stop();
            return;
        }

        Vector2 nextPosition = Vector2.MoveTowards(
            currentPosition,
            _destination,
            _moveSpeed * Time.fixedDeltaTime);

        _rigidbody.MovePosition(nextPosition);
        _lastPosition = currentPosition;
    }

    public void MoveTo(Vector2 destination)
    {
        if (_hasDestination && (_destination - destination).sqrMagnitude <= 0.0001f)
            return;

        _destination = destination;
        _hasDestination = true;
        _blockedTimer = 0f;
        _lastPosition = _rigidbody.position;
    }

    public void Stop()
    {
        _hasDestination = false;
        _blockedTimer = 0f;
        _rigidbody.linearVelocity = Vector2.zero;
    }

    private void UpdateBlockedTimer(Vector2 currentPosition)
    {
        float progressDistance = (currentPosition - _lastPosition).magnitude;

        if (progressDistance <= _minProgressDistance)
        {
            _blockedTimer += Time.fixedDeltaTime;
            return;
        }

        _blockedTimer = 0f;
    }
}
