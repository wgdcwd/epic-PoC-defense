using UnityEngine;

[CreateAssetMenu(fileName = "ShotgunAttack", menuName = "Game/Data/Attack/Shotgun")]
public class ShotgunAttackDefinition : ProjectileHeroAttackDefinition
{
    [SerializeField] private float _spreadAngle = 45f;
    [SerializeField] private int _pelletCount = 5;
    [SerializeField] private float _pelletDamageMultiplier = 0.45f;

    public override void Execute(HeroAttackContext context)
    {
        Vector2 direction = (context.TargetPosition - context.Origin).normalized;

        if (direction.sqrMagnitude <= 0f)
            direction = Vector2.right;

        int pelletCount = Mathf.Max(1, _pelletCount);

        for (int i = 0; i < pelletCount; i++)
        {
            float t = pelletCount == 1 ? 0.5f : (float)i / (pelletCount - 1);
            float angle = Mathf.Lerp(-_spreadAngle * 0.5f, _spreadAngle * 0.5f, t);
            Vector2 pelletDirection = Quaternion.Euler(0f, 0f, angle) * direction;
            FireProjectile(context, pelletDirection, context.AttackPower * DamageMultiplier * _pelletDamageMultiplier);
        }
    }
}
