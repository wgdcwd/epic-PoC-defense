using UnityEngine;

[CreateAssetMenu(fileName = "SniperAttack", menuName = "Game/Data/Attack/Sniper")]
public class SniperAttackDefinition : ProjectileHeroAttackDefinition
{
    public override void Execute(HeroAttackContext context)
    {
        Vector2 direction = context.TargetPosition - context.Origin;
        FireProjectile(context, direction, context.AttackPower * DamageMultiplier);
    }
}
