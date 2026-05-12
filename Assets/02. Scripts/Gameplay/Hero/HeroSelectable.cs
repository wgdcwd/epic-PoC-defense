using UnityEngine;

[RequireComponent(typeof(HeroMovement))]
public class HeroSelectable : MonoBehaviour
{
    [SerializeField] private GameObject _selectionIndicator;

    private HeroMovement _movement;

    [SerializeField] private bool _isSelected;

    public HeroMovement Movement => _movement;
    public bool IsSelected => _isSelected;

    private void Awake()
    {
        _movement = GetComponent<HeroMovement>();
        SetSelected(false);
    }

    public void SetSelected(bool isSelected)
    {
        _isSelected = isSelected;

        if (_selectionIndicator != null)
            _selectionIndicator.SetActive(isSelected);
    }
}
