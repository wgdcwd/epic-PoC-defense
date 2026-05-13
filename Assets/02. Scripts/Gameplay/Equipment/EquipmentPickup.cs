using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EquipmentPickup : MonoBehaviour, IHeroInteractable
{
    [SerializeField] private EquipmentDefinition _equipment;

    public EquipmentDefinition Equipment => _equipment;
    public Transform Transform => transform;

    public bool CanInteract(HeroController hero)
    {
        return hero != null && _equipment != null && gameObject.activeInHierarchy;
    }

    public void Interact(HeroController hero)
    {
        if (!CanInteract(hero))
            return;

        HeroEquipment equipment = hero.GetComponent<HeroEquipment>();

        if (equipment == null)
            return;

        equipment.Equip(_equipment);
        gameObject.SetActive(false);
    }

    public static EquipmentPickup CreateDropped(EquipmentDefinition equipment, Vector3 position)
    {
        GameObject pickupObject = new GameObject(equipment != null ? equipment.DisplayName : "Dropped Equipment");
        pickupObject.transform.position = position;

        CircleCollider2D collider = pickupObject.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 0.25f;

        EquipmentPickup pickup = pickupObject.AddComponent<EquipmentPickup>();
        pickup.SetEquipment(equipment);

        return pickup;
    }

    public void SetEquipment(EquipmentDefinition equipment)
    {
        _equipment = equipment;
    }
}
