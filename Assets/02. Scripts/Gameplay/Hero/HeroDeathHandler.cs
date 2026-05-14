using System.Collections;
using UnityEngine;

[RequireComponent(typeof(HeroMovement))]
[RequireComponent(typeof(UnitRuntimeStats))]
public class HeroDeathHandler : MonoBehaviour, IDeathHandler
{
    [SerializeField] private float _respawnDelay = 20f;
    [SerializeField] private Transform _respawnPoint;
    [SerializeField] private Vector2 _respawnOffset;
    [SerializeField] private bool _isDead;

    private HeroMovement _movement;
    private HeroCombat _combat;
    private HeroInteractor _interactor;
    private HeroSelectable _selectable;
    private HeroController _hero;
    private UnitRuntimeStats _stats;
    private Renderer[] _renderers;
    private Collider2D[] _colliders;
    private bool[] _rendererEnabledStates;
    private bool[] _colliderEnabledStates;
    private Coroutine _respawnRoutine;

    private void Awake()
    {
        _movement = GetComponent<HeroMovement>();
        _combat = GetComponent<HeroCombat>();
        _interactor = GetComponent<HeroInteractor>();
        _selectable = GetComponent<HeroSelectable>();
        _hero = GetComponent<HeroController>();
        _stats = GetComponent<UnitRuntimeStats>();
        _renderers = GetComponentsInChildren<Renderer>(true);
        _colliders = GetComponentsInChildren<Collider2D>(true);
        _rendererEnabledStates = new bool[_renderers.Length];
        _colliderEnabledStates = new bool[_colliders.Length];

        for (int i = 0; i < _renderers.Length; i++)
            _rendererEnabledStates[i] = _renderers[i].enabled;

        for (int i = 0; i < _colliders.Length; i++)
            _colliderEnabledStates[i] = _colliders[i].enabled;
    }

    public void HandleDeath()
    {
        if (_isDead)
            return;

        _isDead = true;
        _movement.Stop();
        SetInactiveWhileDead(true);

        if (_respawnRoutine != null)
            StopCoroutine(_respawnRoutine);

        _respawnRoutine = StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(_respawnDelay);

        Respawn();
        _respawnRoutine = null;
    }

    private void Respawn()
    {
        transform.position = GetRespawnPosition();
        _stats.RestoreHealthToFull();
        _isDead = false;
        SetInactiveWhileDead(false);
    }

    private Vector3 GetRespawnPosition()
    {
        if (_respawnPoint != null)
            return _respawnPoint.position + (Vector3)_respawnOffset;

        DefenseBase defenseBase = FindFirstObjectByType<DefenseBase>();

        if (defenseBase != null)
            return defenseBase.transform.position + (Vector3)_respawnOffset;

        return transform.position;
    }

    private void SetInactiveWhileDead(bool isInactive)
    {
        bool canControl = !isInactive && (_hero == null || _hero.IsRecruited);

        if (_combat != null)
            _combat.enabled = canControl;

        if (_interactor != null)
        {
            _interactor.ClearTarget();
            _interactor.enabled = canControl;
        }

        if (_selectable != null)
        {
            _selectable.SetSelected(false);
            _selectable.enabled = canControl;
        }

        for (int i = 0; i < _renderers.Length; i++)
            _renderers[i].enabled = !isInactive && _rendererEnabledStates[i];

        for (int i = 0; i < _colliders.Length; i++)
            _colliders[i].enabled = !isInactive && _colliderEnabledStates[i];
    }
}
