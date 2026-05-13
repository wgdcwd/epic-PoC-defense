using UnityEngine;

public class DefenseBase : MonoBehaviour, IDamageable
{
    [SerializeField] private float _maxHealth = 20f;
    [SerializeField] private float _currentHealth;
    [SerializeField] private bool _isDestroyed;

    public float CurrentHealth => _currentHealth;
    public float MaxHealth => _maxHealth;
    public bool IsDestroyed => _isDestroyed;

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (_isDestroyed)
            return;

        if (damage <= 0f)
            return;

        _currentHealth = Mathf.Max(0f, _currentHealth - damage);

        if (_currentHealth <= 0f)
            _isDestroyed = true;
    }
}
