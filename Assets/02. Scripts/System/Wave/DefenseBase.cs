using System;
using UnityEngine;

public class DefenseBase : MonoBehaviour, IDamageable
{
    [SerializeField] private float _maxHealth = 20f;
    [SerializeField] private float _currentHealth;
    [SerializeField] private bool _isDestroyed;

    public float CurrentHealth => _currentHealth;
    public float MaxHealth => _maxHealth;
    public bool IsDestroyed => _isDestroyed;

    public event Action<DefenseBase> Destroyed;
    public event Action<DefenseBase> HealthChanged;

    private void Awake()
    {
        _currentHealth = _maxHealth;
        EnsureGameOverController();
        HealthChanged?.Invoke(this);
    }

    public void TakeDamage(float damage)
    {
        if (_isDestroyed)
            return;

        if (damage <= 0f)
            return;

        _currentHealth = Mathf.Max(0f, _currentHealth - damage);
        HealthChanged?.Invoke(this);

        if (_currentHealth <= 0f)
            HandleDestroyed();
    }

    private void HandleDestroyed()
    {
        if (_isDestroyed)
            return;

        _isDestroyed = true;
        Destroyed?.Invoke(this);
    }

    private void EnsureGameOverController()
    {
        GameOverController controller = FindFirstObjectByType<GameOverController>(FindObjectsInactive.Include);

        if (controller != null)
        {
            controller.SetDefenseBase(this);
            return;
        }

        controller = gameObject.AddComponent<GameOverController>();
        controller.SetDefenseBase(this);
    }

}
