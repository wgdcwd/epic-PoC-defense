using UnityEngine;

[RequireComponent(typeof(HeroMovement))]
[RequireComponent(typeof(UnitRuntimeStats))]
[RequireComponent(typeof(HeroCombat))]
[RequireComponent(typeof(HeroLevel))]
[RequireComponent(typeof(HeroEquipment))]
[RequireComponent(typeof(HeroInteractor))]
public class HeroController : MonoBehaviour
{
    [SerializeField] private HeroDefinition _definition;
    [SerializeField] private bool _startsRecruited = true;
    [SerializeField] private bool _isRecruited;

    private HeroMovement _movement;
    private UnitRuntimeStats _stats;
    private HeroCombat _combat;
    private HeroLevel _level;
    private HeroEquipment _equipment;
    private HeroInteractor _interactor;
    private HeroSelectable _selectable;

    public HeroMovement Movement => _movement;
    public UnitRuntimeStats Stats => _stats;
    public HeroCombat Combat => _combat;
    public HeroLevel Level => _level;
    public HeroEquipment Equipment => _equipment;
    public HeroDefinition Definition => GetHeroDefinition();
    public string DisplayName => Definition != null ? Definition.DisplayName : name;
    public Sprite PortraitSprite => Definition != null ? Definition.PortraitSprite : null;
    public bool IsDead => _stats.IsDead;
    public bool IsRecruited => _isRecruited;

    private void Awake()
    {
        _movement = GetComponent<HeroMovement>();
        _stats = GetComponent<UnitRuntimeStats>();
        _combat = GetComponent<HeroCombat>();
        _level = GetComponent<HeroLevel>();
        _equipment = GetComponent<HeroEquipment>();
        _interactor = GetComponent<HeroInteractor>();
        _selectable = GetComponent<HeroSelectable>();
        _isRecruited = _startsRecruited;

        HeroDefinition definition = GetHeroDefinition();

        if (definition != null)
        {
            _stats.InitializeFromDefinition(definition);
            _combat.SetAttackDefinition(definition.DefaultAttackDefinition);
        }

        ApplyRecruitmentState();
    }

    private HeroDefinition GetHeroDefinition()
    {
        if (_definition != null)
            return _definition;

        return _stats != null ? _stats.Definition as HeroDefinition : null;
    }

    private void OnEnable()
    {
        HeroRoster roster = HeroRoster.Instance;

        if (roster != null && _isRecruited)
            roster.Register(this);
    }

    private void OnDisable()
    {
        HeroRoster roster = HeroRoster.Instance;

        if (roster != null)
            roster.Unregister(this);
    }

    public void MoveTo(Vector2 destination)
    {
        if (!_isRecruited)
            return;

        _interactor.ClearTarget();
        _combat.ClearChaseTarget();
        _movement.MoveTo(destination);
    }

    public void AttackTarget(Transform target)
    {
        if (!_isRecruited)
            return;

        _interactor.ClearTarget();
        _combat.SetChaseTarget(target);
    }

    public void InteractWith(IHeroInteractable interactable)
    {
        if (!_isRecruited)
            return;

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

    public void Recruit()
    {
        if (_isRecruited)
            return;

        _isRecruited = true;
        ApplyRecruitmentState();

        HeroRoster roster = HeroRoster.Instance;

        if (roster != null)
            roster.Register(this);
    }

    public void SetUnrecruited()
    {
        if (!_isRecruited)
            return;

        _isRecruited = false;
        Stop();
        ApplyRecruitmentState();

        HeroRoster roster = HeroRoster.Instance;

        if (roster != null)
            roster.Unregister(this);
    }

    private void ApplyRecruitmentState()
    {
        if (_selectable != null)
        {
            _selectable.SetSelected(false);
            _selectable.enabled = _isRecruited;
        }

        if (_combat != null)
        {
            _combat.ClearChaseTarget();
            _combat.enabled = _isRecruited;
        }

        if (_interactor != null)
        {
            _interactor.ClearTarget();
            _interactor.enabled = _isRecruited;
        }
    }
}
