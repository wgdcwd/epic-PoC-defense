using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class HeroProjectile : MonoBehaviour
{
    [SerializeField] private float _speed = 12f;
    [SerializeField] private float _lifeTime = 2f;
    [SerializeField] private LayerMask _targetLayerMask;

    private Rigidbody2D _rigidbody;
    private Vector2 _direction;
    private float _damage;
    private float _remainingLifeTime;
    private Transform _owner;
    private bool _isInitialized;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _rigidbody.gravityScale = 0f;
        _rigidbody.bodyType = RigidbodyType2D.Kinematic;

        Collider2D projectileCollider = GetComponent<Collider2D>();
        projectileCollider.isTrigger = true;
    }

    private void Update()
    {
        if (!_isInitialized)
            return;

        _remainingLifeTime -= Time.deltaTime;

        if (_remainingLifeTime <= 0f)
            Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        if (!_isInitialized)
            return;

        _rigidbody.MovePosition(_rigidbody.position + _direction * _speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        int layerMask = 1 << other.gameObject.layer;

        if ((_targetLayerMask.value & layerMask) == 0)
            return;

        if (_owner != null && other.transform.IsChildOf(_owner))
            return;

        IDamageable damageable = other.GetComponentInParent<IDamageable>();

        if (damageable == null)
            return;

        damageable.TakeDamage(_damage);
        Destroy(gameObject);
    }

    public void Initialize(
        Transform owner,
        Vector2 direction,
        float damage,
        float speed,
        float lifeTime,
        LayerMask targetLayerMask)
    {
        _owner = owner;
        _direction = direction.sqrMagnitude > 0f ? direction.normalized : Vector2.right;
        _damage = damage;
        _speed = speed;
        _lifeTime = lifeTime;
        _remainingLifeTime = _lifeTime;
        _targetLayerMask = targetLayerMask;
        _isInitialized = true;

        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
