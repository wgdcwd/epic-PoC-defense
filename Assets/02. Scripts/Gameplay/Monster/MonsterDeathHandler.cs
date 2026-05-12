using UnityEngine;

[RequireComponent(typeof(MonsterMovement))]
public class MonsterDeathHandler : MonoBehaviour, IDeathHandler
{
    [SerializeField] private bool _isDead;

    private MonsterMovement _movement;
    private MonsterController _controller;
    private MonsterCombat _combat;

    private void Awake()
    {
        _movement = GetComponent<MonsterMovement>();
        _controller = GetComponent<MonsterController>();
        _combat = GetComponent<MonsterCombat>();
    }

    public void HandleDeath()
    {
        if (_isDead)
            return;

        _isDead = true;
        _movement.Stop();

        if (_controller != null)
            _controller.enabled = false;

        if (_combat != null)
            _combat.enabled = false;

        gameObject.SetActive(false);
    }
}
