using UnityEngine;

[RequireComponent(typeof(HeroController))]
public class HeroSelectable : MonoBehaviour
{
    [SerializeField] private GameObject _selectionIndicator;

    private HeroController _controller;

    [SerializeField] private bool _isSelected;

    public HeroController Controller => _controller;
    public HeroMovement Movement => _controller.Movement;
    public bool IsSelected => _isSelected;

    private void Awake()
    {
        _controller = GetComponent<HeroController>();
        SetSelected(false);
    }

    public void SetSelected(bool isSelected)
    {
        _isSelected = isSelected;

        if (_selectionIndicator != null)
            _selectionIndicator.SetActive(isSelected);
    }
}
