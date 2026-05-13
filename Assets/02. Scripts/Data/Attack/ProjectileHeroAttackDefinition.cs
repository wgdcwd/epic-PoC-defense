using UnityEngine;

public abstract class ProjectileHeroAttackDefinition : HeroAttackDefinition
{
    [SerializeField] private HeroProjectile _projectilePrefab;
    [SerializeField] private Sprite _fallbackProjectileSprite;
    [SerializeField] private float _projectileSpeed = 12f;
    [SerializeField] private float _projectileLifeTime = 2f;
    [SerializeField] private float _spawnOffset = 0.35f;

    protected void FireProjectile(HeroAttackContext context, Vector2 direction, float damage)
    {
        Vector2 normalizedDirection = direction.sqrMagnitude > 0f ? direction.normalized : Vector2.right;
        Vector2 spawnPosition = context.Origin + normalizedDirection * _spawnOffset;
        HeroProjectile projectile = CreateProjectile(spawnPosition);
        Transform owner = context.Hero != null ? context.Hero.transform : null;

        projectile.Initialize(
            owner,
            normalizedDirection,
            damage,
            _projectileSpeed,
            _projectileLifeTime,
            context.TargetLayerMask);
    }

    private HeroProjectile CreateProjectile(Vector2 spawnPosition)
    {
        if (_projectilePrefab != null)
            return Instantiate(_projectilePrefab, spawnPosition, Quaternion.identity);

        GameObject projectileObject = new GameObject(DisplayName + " Projectile");
        projectileObject.transform.position = spawnPosition;

        if (_fallbackProjectileSprite != null)
        {
            SpriteRenderer spriteRenderer = projectileObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = _fallbackProjectileSprite;
        }
        else
        {
            projectileObject.AddComponent<LineRenderer>();
            projectileObject.AddComponent<ProjectileLineVisual>();
        }

        CircleCollider2D collider = projectileObject.AddComponent<CircleCollider2D>();
        collider.radius = 0.08f;
        collider.isTrigger = true;

        Rigidbody2D rigidbody = projectileObject.AddComponent<Rigidbody2D>();
        rigidbody.gravityScale = 0f;
        rigidbody.bodyType = RigidbodyType2D.Kinematic;

        return projectileObject.AddComponent<HeroProjectile>();
    }
}
