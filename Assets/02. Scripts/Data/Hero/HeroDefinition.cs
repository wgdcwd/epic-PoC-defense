using UnityEngine;

[CreateAssetMenu(fileName = "HeroDefinition", menuName = "Game/Data/Hero Definition")]
public class HeroDefinition : UnitDefinition
{
    [SerializeField] private string _displayName;
    [SerializeField] private Sprite _portraitSprite;
    [SerializeField] private HeroAttackDefinition _defaultAttackDefinition;

    public string DisplayName => string.IsNullOrWhiteSpace(_displayName) ? name : _displayName;
    public Sprite PortraitSprite => _portraitSprite;
    public HeroAttackDefinition DefaultAttackDefinition => _defaultAttackDefinition;
}
