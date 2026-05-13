using UnityEngine;

[RequireComponent(typeof(HeroController))]
public class HeroInteractor : MonoBehaviour
{
    [SerializeField] private float _interactRange = 0.35f;
    [SerializeField] private string _currentInteractableName;

    private HeroController _hero;
    private IHeroInteractable _targetInteractable;

    private void Awake()
    {
        _hero = GetComponent<HeroController>();
    }

    private void Update()
    {
        if (_targetInteractable == null)
            return;

        if (!_targetInteractable.CanInteract(_hero))
        {
            ClearTarget();
            return;
        }

        Vector3 targetPosition = _targetInteractable.Transform.position;
        float sqrDistance = (targetPosition - transform.position).sqrMagnitude;

        if (sqrDistance > _interactRange * _interactRange)
        {
            _hero.MoveForInteraction(targetPosition);
            return;
        }

        IHeroInteractable interactable = _targetInteractable;

        _hero.StopForInteraction();
        interactable.Interact(_hero);
        ClearTarget();
    }

    public void SetTarget(IHeroInteractable interactable)
    {
        _targetInteractable = interactable;
        _currentInteractableName = interactable != null ? interactable.Transform.name : string.Empty;
    }

    public void ClearTarget()
    {
        _targetInteractable = null;
        _currentInteractableName = string.Empty;
    }
}
