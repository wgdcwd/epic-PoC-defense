using UnityEngine;

[RequireComponent(typeof(HeroMovement))]
public class HeroDeathHandler : MonoBehaviour, IDeathHandler
{
    [SerializeField] private bool _isDead;

    private HeroMovement _movement;
    private HeroCombat _combat;

    private void Awake()
    {
        _movement = GetComponent<HeroMovement>();
        _combat = GetComponent<HeroCombat>();
    }

    public void HandleDeath()
    {
        if (_isDead)
            return;

        _isDead = true;
        _movement.Stop();

        if (_combat != null)
            _combat.enabled = false;
    }
}
