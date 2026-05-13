using UnityEngine;

[CreateAssetMenu(fileName = "BaseballBatAttack", menuName = "Game/Data/Attack/Baseball Bat")]
public class BaseballBatAttackDefinition : HeroAttackDefinition
{
    [SerializeField] private MeleeArcHitbox _hitboxPrefab;
    [SerializeField] private Sprite _fallbackSwingSprite;
    [SerializeField] private float _arcAngle = 120f;
    [SerializeField] private int _maxTargets = 3;
    [SerializeField] private float _activeTime = 0.15f;

    public override void Execute(HeroAttackContext context)
    {
        Vector2 direction = (context.TargetPosition - context.Origin).normalized;

        if (direction.sqrMagnitude <= 0f)
            direction = Vector2.right;

        float range = GetRange(context.Stats);
        MeleeArcHitbox hitbox = CreateHitbox(context.Origin);
        hitbox.Initialize(
            context.Origin,
            direction,
            range,
            _arcAngle,
            _maxTargets,
            context.AttackPower * DamageMultiplier,
            _activeTime,
            context.TargetLayerMask);

        MeleeArcVisual spriteVisual = hitbox.GetComponent<MeleeArcVisual>();

        if (spriteVisual != null)
            spriteVisual.Initialize(_activeTime);

        MeleeArcLineVisual lineVisual = hitbox.GetComponent<MeleeArcLineVisual>();

        if (lineVisual != null)
            lineVisual.Initialize(range, _arcAngle, _activeTime);
    }

    private MeleeArcHitbox CreateHitbox(Vector2 origin)
    {
        if (_hitboxPrefab != null)
            return Instantiate(_hitboxPrefab, origin, Quaternion.identity);

        GameObject hitboxObject = new GameObject(DisplayName + " Swing");
        hitboxObject.transform.position = origin;

        if (_fallbackSwingSprite != null)
        {
            SpriteRenderer spriteRenderer = hitboxObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = _fallbackSwingSprite;
            hitboxObject.AddComponent<MeleeArcVisual>();
        }
        else
        {
            hitboxObject.AddComponent<LineRenderer>();
            hitboxObject.AddComponent<MeleeArcLineVisual>();
        }

        return hitboxObject.AddComponent<MeleeArcHitbox>();
    }
}
