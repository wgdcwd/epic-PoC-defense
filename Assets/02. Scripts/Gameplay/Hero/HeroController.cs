using UnityEngine;

[RequireComponent(typeof(HeroMovement))]
[RequireComponent(typeof(UnitRuntimeStats))]
[RequireComponent(typeof(HeroCombat))]
[RequireComponent(typeof(HeroLevel))]
[RequireComponent(typeof(HeroEquipment))]
[RequireComponent(typeof(HeroInteractor))]
public class HeroController : MonoBehaviour
{
    private HeroMovement _movement;
    private UnitRuntimeStats _stats;
    private HeroCombat _combat;
    private HeroLevel _level;
    private HeroEquipment _equipment;
    private HeroInteractor _interactor;

    public HeroMovement Movement => _movement;
    public UnitRuntimeStats Stats => _stats;
    public HeroCombat Combat => _combat;
    public HeroLevel Level => _level;
    public HeroEquipment Equipment => _equipment;
    public bool IsDead => _stats.IsDead;

    private void Awake()
    {
        _movement = GetComponent<HeroMovement>();
        _stats = GetComponent<UnitRuntimeStats>();
        _combat = GetComponent<HeroCombat>();
        _level = GetComponent<HeroLevel>();
        _equipment = GetComponent<HeroEquipment>();
        _interactor = GetComponent<HeroInteractor>();
    }

    public void MoveTo(Vector2 destination)
    {
        _interactor.ClearTarget();
        _combat.ClearChaseTarget();
        _movement.MoveTo(destination);
    }

    public void AttackTarget(Transform target)
    {
        _interactor.ClearTarget();
        _combat.SetChaseTarget(target);
    }

    public void InteractWith(IHeroInteractable interactable)
    {
        _combat.ClearChaseTarget();
        _interactor.SetTarget(interactable);
    }

    public void MoveForInteraction(Vector2 destination)
    {
        _combat.ClearChaseTarget();
        _movement.MoveTo(destination);
    }

    public void StopForInteraction()
    {
        _combat.ClearChaseTarget();
        _movement.Stop();
    }

    public void Stop()
    {
        _interactor.ClearTarget();
        _combat.ClearChaseTarget();
        _movement.Stop();
    }
}
