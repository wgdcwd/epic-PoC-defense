using System.Collections.Generic;
using UnityEngine;

public class HeroRosterHudView : MonoBehaviour
{
    [SerializeField] private HeroRoster _roster;
    [SerializeField] private HeroSelectionSystem _selectionSystem;
    [SerializeField] private Transform _slotRoot;
    [SerializeField] private HeroHudSlotView _slotPrefab;

    private readonly List<HeroHudSlotView> _slots = new List<HeroHudSlotView>();

    private void Awake()
    {
        if (_roster == null)
            _roster = HeroRoster.Instance;

        if (_selectionSystem == null)
            _selectionSystem = FindFirstObjectByType<HeroSelectionSystem>();

        if (_slotRoot == null)
            _slotRoot = transform;
    }

    private void OnEnable()
    {
        SubscribeRoster();
        Rebuild();
    }

    private void Start()
    {
        SubscribeRoster();
        Rebuild();
    }

    private void OnDisable()
    {
        if (_roster != null)
            _roster.Changed -= Rebuild;
    }

    public void Rebuild()
    {
        ClearSlots();

        if (_roster == null)
            _roster = HeroRoster.Instance;

        if (_roster == null || _slotPrefab == null)
            return;

        IReadOnlyList<HeroController> heroes = _roster.Heroes;

        for (int i = 0; i < heroes.Count; i++)
        {
            HeroHudSlotView slot = Instantiate(_slotPrefab, _slotRoot);
            slot.Initialize(heroes[i], _selectionSystem);
            _slots.Add(slot);
        }
    }

    private void ClearSlots()
    {
        for (int i = 0; i < _slots.Count; i++)
        {
            if (_slots[i] != null)
                Destroy(_slots[i].gameObject);
        }

        _slots.Clear();
    }

    private void SubscribeRoster()
    {
        if (_roster == null)
            _roster = HeroRoster.Instance;

        if (_roster == null)
            return;

        _roster.Changed -= Rebuild;
        _roster.Changed += Rebuild;
    }
}
