using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EquipmentPickup : MonoBehaviour, IHeroInteractable
{
    [SerializeField] private EquipmentDefinition _equipment;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    public EquipmentDefinition Equipment => _equipment;
    public Transform Transform => transform;

    private void Awake()
    {
        if (_spriteRenderer == null)
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        RefreshVisual();
    }

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
        pickupObject.transform.localScale = Vector3.one * 3f;

        SpriteRenderer spriteRenderer = pickupObject.AddComponent<SpriteRenderer>();

        if (equipment != null)
            spriteRenderer.sprite = equipment.WorldSprite;

        CircleCollider2D collider = pickupObject.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 0.25f;

        EquipmentPickup pickup = pickupObject.AddComponent<EquipmentPickup>();
        pickup.SetSpriteRenderer(spriteRenderer);
        pickup.SetEquipment(equipment);

        

        return pickup;
    }

    public void SetEquipment(EquipmentDefinition equipment)
    {
        _equipment = equipment;
        RefreshVisual();
    }

    public void SetSpriteRenderer(SpriteRenderer spriteRenderer)
    {
        _spriteRenderer = spriteRenderer;
        RefreshVisual();
    }

    private void RefreshVisual()
    {
        if (_spriteRenderer == null || _equipment == null)
            return;

        _spriteRenderer.sprite = _equipment.WorldSprite;
    }
}
