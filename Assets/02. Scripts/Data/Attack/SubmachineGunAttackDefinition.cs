using UnityEngine;

[CreateAssetMenu(fileName = "SubmachineGunAttack", menuName = "Game/Data/Attack/Submachine Gun")]
public class SubmachineGunAttackDefinition : ProjectileHeroAttackDefinition
{
    public override void Execute(HeroAttackContext context)
    {
        Vector2 direction = context.TargetPosition - context.Origin;
        FireProjectile(context, direction, context.AttackPower * DamageMultiplier);
    }
}
