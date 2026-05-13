using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System;

public class HeroSelectionSystem : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private LayerMask _heroLayerMask = ~0;
    [SerializeField] private float _dragThreshold = 12f;
    [SerializeField] private SelectionDragView _selectionDragView;

    private readonly List<HeroSelectable> _selectedHeroes = new List<HeroSelectable>();
    private Vector2 _dragStartScreenPosition;
    private Vector2 _dragCurrentScreenPosition;
    private bool _isDragging;
    private bool _startedOverUi;

    public IReadOnlyList<HeroSelectable> SelectedHeroes => _selectedHeroes;
    public HeroSelectable SelectedHero => _selectedHeroes.Count > 0 ? _selectedHeroes[0] : null;
    public bool HasSelection => _selectedHeroes.Count > 0;

    public event Action<HeroSelectable> HeroSelected;
    public event Action SelectionCleared;
    public event Action<Rect, Vector2, bool> DragSelectionCompleted;

    private void Awake()
    {
        if (_mainCamera == null)
            _mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Mouse.current == null)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
            BeginSelectionInput();

        if (Mouse.current.leftButton.isPressed)
            UpdateDragSelection();

        if (Mouse.current.leftButton.wasReleasedThisFrame)
            EndSelectionInput();
    }

    public void ClearSelection()
    {
        for (int i = 0; i < _selectedHeroes.Count; i++)
            _selectedHeroes[i].SetSelected(false);

        _selectedHeroes.Clear();
        SelectionCleared?.Invoke();
    }

    public Vector2 GetSelectionCenter()
    {
        if (_selectedHeroes.Count == 0)
            return Vector2.zero;

        Vector2 center = Vector2.zero;

        for (int i = 0; i < _selectedHeroes.Count; i++)
            center += (Vector2)_selectedHeroes[i].transform.position;

        return center / _selectedHeroes.Count;
    }

    private void BeginSelectionInput()
    {
        _dragStartScreenPosition = Mouse.current.position.ReadValue();
        _dragCurrentScreenPosition = _dragStartScreenPosition;
        _isDragging = false;
        _startedOverUi = IsPointerOverUi();
    }

    private void UpdateDragSelection()
    {
        if (_startedOverUi)
            return;

        _dragCurrentScreenPosition = Mouse.current.position.ReadValue();

        if (!_isDragging &&
            (_dragCurrentScreenPosition - _dragStartScreenPosition).sqrMagnitude >= _dragThreshold * _dragThreshold)
        {
            _isDragging = true;
        }

        if (_isDragging && _selectionDragView != null)
            _selectionDragView.Show(_dragStartScreenPosition, _dragCurrentScreenPosition);
    }

    private void EndSelectionInput()
    {
        if (_startedOverUi)
            return;

        if (_isDragging)
            SelectHeroesInDragRect();
        else
            SelectHeroUnderPointer();

        _isDragging = false;

        if (_selectionDragView != null)
            _selectionDragView.Hide();
    }

    private void SelectHeroUnderPointer()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, _heroLayerMask);

        if (hit.collider == null)
        {
            ClearSelection();
            return;
        }

        HeroSelectable hero = hit.collider.GetComponentInParent<HeroSelectable>();

        if (hero == null)
        {
            ClearSelection();
            return;
        }

        SelectSingleHero(hero);
    }

    private void SelectSingleHero(HeroSelectable hero)
    {
        if (_selectedHeroes.Count == 1 && _selectedHeroes[0] == hero)
            return;

        ClearSelection();
        AddSelection(hero);
        HeroSelected?.Invoke(hero);
    }

    private void SelectHeroesInDragRect()
    {
        Rect selectionRect = GetScreenRect(_dragStartScreenPosition, _dragCurrentScreenPosition);
        HeroSelectable[] heroes = FindObjectsByType<HeroSelectable>(FindObjectsSortMode.None);

        ClearSelection();

        for (int i = 0; i < heroes.Length; i++)
        {
            HeroSelectable hero = heroes[i];

            if (!IsInHeroLayer(hero.gameObject))
                continue;

            Vector3 screenPosition = _mainCamera.WorldToScreenPoint(hero.transform.position);
            if (selectionRect.Contains(screenPosition))
                AddSelection(hero);
        }

        HeroSelectable representativeHero = GetRepresentativeSelectedHero(selectionRect);

        if (representativeHero != null)
            HeroSelected?.Invoke(representativeHero);

        DragSelectionCompleted?.Invoke(selectionRect, _dragStartScreenPosition, representativeHero != null);
    }

    private void AddSelection(HeroSelectable hero)
    {
        if (_selectedHeroes.Contains(hero))
            return;

        _selectedHeroes.Add(hero);
        hero.SetSelected(true);
    }

    private HeroSelectable GetRepresentativeSelectedHero(Rect selectionRect)
    {
        HeroSelectable representativeHero = null;
        float nearestDistance = float.MaxValue;
        Vector2 dragStartPosition = _dragStartScreenPosition;

        for (int i = 0; i < _selectedHeroes.Count; i++)
        {
            HeroSelectable hero = _selectedHeroes[i];

            if (hero == null)
                continue;

            Vector3 screenPosition = _mainCamera.WorldToScreenPoint(hero.transform.position);

            if (!selectionRect.Contains(screenPosition))
                continue;

            float distance = ((Vector2)screenPosition - dragStartPosition).sqrMagnitude;

            if (distance >= nearestDistance)
                continue;

            nearestDistance = distance;
            representativeHero = hero;
        }

        return representativeHero;
    }

    private bool IsInHeroLayer(GameObject target)
    {
        int layerMask = 1 << target.layer;
        return (_heroLayerMask.value & layerMask) != 0;
    }

    private Rect GetScreenRect(Vector2 startScreenPosition, Vector2 currentScreenPosition)
    {
        Vector2 bottomLeft = Vector2.Min(startScreenPosition, currentScreenPosition);
        Vector2 topRight = Vector2.Max(startScreenPosition, currentScreenPosition);

        return Rect.MinMaxRect(bottomLeft.x, bottomLeft.y, topRight.x, topRight.y);
    }

    private bool IsPointerOverUi()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
}
