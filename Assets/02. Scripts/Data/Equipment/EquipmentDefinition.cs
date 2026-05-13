using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentDefinition", menuName = "Game/Data/Equipment Definition")]
public class EquipmentDefinition : ScriptableObject
{
    [SerializeField] private string _displayName;
    [SerializeField] private EquipmentType _equipmentType;
    [SerializeField] private EquipmentStatModifier _statModifier;

    public string DisplayName => string.IsNullOrWhiteSpace(_displayName) ? name : _displayName;
    public EquipmentType EquipmentType => _equipmentType;
    public EquipmentStatModifier StatModifier => _statModifier;
}
