using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class SelectionInfoPresenter : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private HeroSelectionSystem _heroSelectionSystem;
    [SerializeField] private SelectionInfoView _infoView;
    [SerializeField] private LayerMask _inspectLayerMask = ~0;

    private bool _isShowingHeroInfo;

    private void Awake()
    {
        if (_mainCamera == null)
            _mainCamera = Camera.main;

        if (_heroSelectionSystem == null)
            _heroSelectionSystem = FindFirstObjectByType<HeroSelectionSystem>();
    }

    private void OnEnable()
    {
        if (_heroSelectionSystem == null)
            return;

        _heroSelectionSystem.HeroSelected += OnHeroSelected;
        _heroSelectionSystem.SelectionCleared += OnSelectionCleared;
        _heroSelectionSystem.DragSelectionCompleted += OnDragSelectionCompleted;
    }

    private void OnDisable()
    {
        if (_heroSelectionSystem == null)
            return;

        _heroSelectionSystem.HeroSelected -= OnHeroSelected;
        _heroSelectionSystem.SelectionCleared -= OnSelectionCleared;
        _heroSelectionSystem.DragSelectionCompleted -= OnDragSelectionCompleted;
    }

    private void Update()
    {
        if (Mouse.current == null)
            return;

        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        ShowInfoUnderPointer();
    }

    private void ShowInfoUnderPointer()
    {
        if (_infoView == null)
            return;

        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, _inspectLayerMask);

        if (hit.collider == null)
        {
            _infoView.Hide();
            return;
        }

        HeroController hero = hit.collider.GetComponentInParent<HeroController>();

        if (hero != null)
        {
            _isShowingHeroInfo = true;
            _infoView.ShowHero(hero);
            return;
        }

        MonsterController monster = hit.collider.GetComponentInParent<MonsterController>();

        if (monster != null)
        {
            _isShowingHeroInfo = false;
            _infoView.ShowMonster(monster);
            return;
        }

        EquipmentPickup equipment = hit.collider.GetComponentInParent<EquipmentPickup>();

        if (equipment != null)
        {
            _isShowingHeroInfo = false;
            _infoView.ShowEquipment(equipment);
            return;
        }

        _isShowingHeroInfo = false;
        _infoView.Hide();
    }

    private void OnHeroSelected(HeroSelectable hero)
    {
        if (_infoView == null || hero == null)
            return;

        _isShowingHeroInfo = true;
        _infoView.ShowHero(hero.Controller);
    }

    private void OnSelectionCleared()
    {
        if (_infoView != null && _isShowingHeroInfo)
            _infoView.Hide();

        _isShowingHeroInfo = false;
    }

    private void OnDragSelectionCompleted(Rect selectionRect, Vector2 dragStartScreenPosition, bool hasHeroSelection)
    {
        if (hasHeroSelection || _infoView == null)
            return;

        MonsterController monster = FindFirstMonsterInRect(selectionRect, dragStartScreenPosition);

        if (monster != null)
        {
            _isShowingHeroInfo = false;
            _infoView.ShowMonster(monster);
            return;
        }

        EquipmentPickup equipment = FindFirstEquipmentInRect(selectionRect, dragStartScreenPosition);

        if (equipment != null)
        {
            _isShowingHeroInfo = false;
            _infoView.ShowEquipment(equipment);
            return;
        }

        _isShowingHeroInfo = false;
        _infoView.Hide();
    }

    private MonsterController FindFirstMonsterInRect(Rect selectionRect, Vector2 dragStartScreenPosition)
    {
        MonsterController[] monsters = FindObjectsByType<MonsterController>(FindObjectsSortMode.None);
        MonsterController nearestMonster = null;
        float nearestDistance = float.MaxValue;

        for (int i = 0; i < monsters.Length; i++)
        {
            MonsterController monster = monsters[i];

            if (monster == null || !monster.gameObject.activeInHierarchy)
                continue;

            Vector3 screenPosition = _mainCamera.WorldToScreenPoint(monster.transform.position);

            if (!selectionRect.Contains(screenPosition))
                continue;

            float distance = ((Vector2)screenPosition - dragStartScreenPosition).sqrMagnitude;

            if (distance >= nearestDistance)
                continue;

            nearestDistance = distance;
            nearestMonster = monster;
        }

        return nearestMonster;
    }

    private EquipmentPickup FindFirstEquipmentInRect(Rect selectionRect, Vector2 dragStartScreenPosition)
    {
        EquipmentPickup[] pickups = FindObjectsByType<EquipmentPickup>(FindObjectsSortMode.None);
        EquipmentPickup nearestPickup = null;
        float nearestDistance = float.MaxValue;

        for (int i = 0; i < pickups.Length; i++)
        {
            EquipmentPickup pickup = pickups[i];

            if (pickup == null || !pickup.gameObject.activeInHierarchy)
                continue;

            Vector3 screenPosition = _mainCamera.WorldToScreenPoint(pickup.transform.position);

            if (!selectionRect.Contains(screenPosition))
                continue;

            float distance = ((Vector2)screenPosition - dragStartScreenPosition).sqrMagnitude;

            if (distance >= nearestDistance)
                continue;

            nearestDistance = distance;
            nearestPickup = pickup;
        }

        return nearestPickup;
    }
}
