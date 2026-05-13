using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class SelectionInfoPresenter : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private SelectionInfoView _infoView;
    [SerializeField] private LayerMask _inspectLayerMask = ~0;

    private void Awake()
    {
        if (_mainCamera == null)
            _mainCamera = Camera.main;
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
            _infoView.ShowHero(hero);
            return;
        }

        MonsterController monster = hit.collider.GetComponentInParent<MonsterController>();

        if (monster != null)
        {
            _infoView.ShowMonster(monster);
            return;
        }

        EquipmentPickup equipment = hit.collider.GetComponentInParent<EquipmentPickup>();

        if (equipment != null)
        {
            _infoView.ShowEquipment(equipment);
            return;
        }

        _infoView.Hide();
    }
}
