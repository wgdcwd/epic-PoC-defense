using UnityEngine;

[RequireComponent(typeof(UnitRuntimeStats))]
public class HeroEquipment : MonoBehaviour
{
    [SerializeField] private EquipmentDefinition _weapon;
    [SerializeField] private EquipmentDefinition _armor;
    [SerializeField] private EquipmentDefinition _accessory;

    private UnitRuntimeStats _stats;

    public EquipmentDefinition Weapon => _weapon;
    public EquipmentDefinition Armor => _armor;
    public EquipmentDefinition Accessory => _accessory;

    private void Awake()
    {
        _stats = GetComponent<UnitRuntimeStats>();
        ApplyEquipment(_weapon);
        ApplyEquipment(_armor);
        ApplyEquipment(_accessory);
    }

    public void Equip(EquipmentDefinition equipment)
    {
        if (equipment == null)
            return;

        EquipmentDefinition previousEquipment = GetEquipped(equipment.EquipmentType);

        if (previousEquipment != null)
        {
            RemoveEquipment(previousEquipment);
            DropEquipment(previousEquipment);
        }

        SetEquipped(equipment.EquipmentType, equipment);
        ApplyEquipment(equipment);
    }

    public void DropEquipped(EquipmentType equipmentType)
    {
        EquipmentDefinition equipment = GetEquipped(equipmentType);

        if (equipment == null)
            return;

        RemoveEquipment(equipment);
        SetEquipped(equipmentType, null);
        DropEquipment(equipment);
    }

    public EquipmentDefinition GetEquipped(EquipmentType equipmentType)
    {
        switch (equipmentType)
        {
            case EquipmentType.Weapon:
                return _weapon;
            case EquipmentType.Armor:
                return _armor;
            case EquipmentType.Accessory:
                return _accessory;
            default:
                return null;
        }
    }

    private void SetEquipped(EquipmentType equipmentType, EquipmentDefinition equipment)
    {
        switch (equipmentType)
        {
            case EquipmentType.Weapon:
                _weapon = equipment;
                break;
            case EquipmentType.Armor:
                _armor = equipment;
                break;
            case EquipmentType.Accessory:
                _accessory = equipment;
                break;
        }
    }

    private void ApplyEquipment(EquipmentDefinition equipment)
    {
        if (equipment == null)
            return;

        _stats.AddStatModifier(equipment.StatModifier);
    }

    private void RemoveEquipment(EquipmentDefinition equipment)
    {
        if (equipment == null)
            return;

        _stats.RemoveStatModifier(equipment.StatModifier);
    }

    private void DropEquipment(EquipmentDefinition equipment)
    {
        Vector3 dropPosition = transform.position + Vector3.right * 0.5f;
        EquipmentPickup.CreateDropped(equipment, dropPosition);
    }
}
