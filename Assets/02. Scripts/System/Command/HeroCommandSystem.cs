using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections.Generic;

[RequireComponent(typeof(HeroSelectionSystem))]
public class HeroCommandSystem : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private MoveCommandMarker _moveCommandMarker;
    [SerializeField] private LayerMask _enemyLayerMask;
    [SerializeField] private LayerMask _interactableLayerMask = ~0;
    [SerializeField] private float _formationSpacing = 1.1f;

    private HeroSelectionSystem _selectionSystem;
    private readonly List<Vector2> _formationSlots = new List<Vector2>();
    private readonly List<int> _assignedSlotIndices = new List<int>();

    private void Awake()
    {
        _selectionSystem = GetComponent<HeroSelectionSystem>();

        if (_mainCamera == null)
            _mainCamera = Camera.main;

        if (_moveCommandMarker == null)
            _moveCommandMarker = FindFirstObjectByType<MoveCommandMarker>();

        if (_moveCommandMarker == null)
            _moveCommandMarker = MoveCommandMarker.CreateDefault();
    }

    private void Update()
    {
        if (Mouse.current == null)
            return;

        if (!Mouse.current.rightButton.wasPressedThisFrame)
            return;

        if (IsPointerOverUi())
            return;

        HandleRightClickCommand();
    }

    private void HandleRightClickCommand()
    {
        IReadOnlyList<HeroSelectable> selectedHeroes = _selectionSystem.SelectedHeroes;

        if (selectedHeroes.Count == 0)
            return;

        Transform enemyTarget = GetEnemyUnderPointer();

        if (enemyTarget != null)
        {
            CommandSelectedHeroesToAttack(selectedHeroes, enemyTarget);
            return;
        }

        IHeroInteractable interactable = GetInteractableUnderPointer();

        if (interactable != null)
        {
            CommandNearestHeroToInteract(selectedHeroes, interactable);
            return;
        }

        MoveSelectedHeroesToPointer(selectedHeroes);
    }

    private void MoveSelectedHeroesToPointer(IReadOnlyList<HeroSelectable> selectedHeroes)
    {
        if (selectedHeroes.Count == 0)
            return;

        Vector3 worldPosition = _mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 destination = new Vector2(worldPosition.x, worldPosition.y);

        _moveCommandMarker.ShowAt(destination);
        BuildFormationSlots(destination, selectedHeroes.Count);
        _assignedSlotIndices.Clear();

        for (int i = 0; i < selectedHeroes.Count; i++)
        {
            HeroSelectable hero = selectedHeroes[i];
            int slotIndex = FindNearestOpenSlot(hero.transform.position);
            _assignedSlotIndices.Add(slotIndex);
            hero.Controller.MoveTo(_formationSlots[slotIndex]);
        }
    }

    private void CommandSelectedHeroesToAttack(IReadOnlyList<HeroSelectable> selectedHeroes, Transform enemyTarget)
    {
        for (int i = 0; i < selectedHeroes.Count; i++)
            selectedHeroes[i].Controller.AttackTarget(enemyTarget);
    }

    private Transform GetEnemyUnderPointer()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, _enemyLayerMask);

        if (hit.collider == null)
            return null;

        IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();
        MonoBehaviour damageableBehaviour = damageable as MonoBehaviour;

        if (damageableBehaviour == null)
            return null;

        return damageableBehaviour.transform;
    }

    private IHeroInteractable GetInteractableUnderPointer()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, _interactableLayerMask);

        if (hit.collider == null)
            return null;

        return hit.collider.GetComponentInParent<IHeroInteractable>();
    }

    private void CommandNearestHeroToInteract(IReadOnlyList<HeroSelectable> selectedHeroes, IHeroInteractable interactable)
    {
        HeroSelectable nearestHero = null;
        float nearestDistance = float.MaxValue;

        for (int i = 0; i < selectedHeroes.Count; i++)
        {
            HeroSelectable hero = selectedHeroes[i];
            float distance = (hero.transform.position - interactable.Transform.position).sqrMagnitude;

            if (distance >= nearestDistance)
                continue;

            nearestDistance = distance;
            nearestHero = hero;
        }

        if (nearestHero != null)
            nearestHero.Controller.InteractWith(interactable);
    }

    private void BuildFormationSlots(Vector2 destination, int unitCount)
    {
        _formationSlots.Clear();

        if (unitCount <= 0)
            return;

        if (unitCount == 1)
        {
            _formationSlots.Add(destination);
            return;
        }

        if (unitCount <= 6)
        {
            AddRingSlots(destination, _formationSpacing, unitCount, -90f);
            return;
        }

        int ring = 1;

        while (_formationSlots.Count < unitCount)
        {
            int slotCount = Mathf.Max(6, ring * 6);
            float radius = _formationSpacing * ring;
            float angleOffset = ring % 2 == 0 ? 0f : 30f;
            int remainingCount = unitCount - _formationSlots.Count;
            int slotsToAdd = Mathf.Min(slotCount, remainingCount);

            AddRingSlots(destination, radius, slotsToAdd, angleOffset);

            ring++;
        }
    }

    private void AddRingSlots(Vector2 destination, float radius, int slotCount, float angleOffset)
    {
        for (int i = 0; i < slotCount; i++)
        {
            float angle = (360f / slotCount * i + angleOffset) * Mathf.Deg2Rad;
            Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            _formationSlots.Add(destination + offset);
        }
    }

    private int FindNearestOpenSlot(Vector2 heroPosition)
    {
        int bestIndex = 0;
        float bestDistance = float.MaxValue;

        for (int i = 0; i < _formationSlots.Count; i++)
        {
            if (_assignedSlotIndices.Contains(i))
                continue;

            float distance = (_formationSlots[i] - heroPosition).sqrMagnitude;

            if (distance >= bestDistance)
                continue;

            bestDistance = distance;
            bestIndex = i;
        }

        return bestIndex;
    }

    private bool IsPointerOverUi()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
}
