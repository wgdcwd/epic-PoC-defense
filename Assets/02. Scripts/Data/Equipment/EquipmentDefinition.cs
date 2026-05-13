using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentDefinition", menuName = "Game/Data/Equipment Definition")]
public class EquipmentDefinition : ScriptableObject
{
    [SerializeField] private string _displayName;
    [SerializeField] private EquipmentType _equipmentType;
    [SerializeField] private Sprite _iconSprite;
    [SerializeField] private Sprite _worldSprite;
    [SerializeField] private EquipmentStatModifier _statModifier;

    public string DisplayName => string.IsNullOrWhiteSpace(_displayName) ? name : _displayName;
    public EquipmentType EquipmentType => _equipmentType;
    public Sprite IconSprite => _iconSprite != null ? _iconSprite : _worldSprite;
    public Sprite WorldSprite => _worldSprite != null ? _worldSprite : _iconSprite;
    public EquipmentStatModifier StatModifier => _statModifier;
}
