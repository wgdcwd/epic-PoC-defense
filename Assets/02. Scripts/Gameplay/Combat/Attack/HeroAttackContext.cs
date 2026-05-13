using UnityEngine;

public struct HeroAttackContext
{
    public HeroController Hero;
    public UnitRuntimeStats Stats;
    public Transform Target;
    public LayerMask TargetLayerMask;

    public Vector2 Origin => Hero != null ? Hero.transform.position : Vector2.zero;
    public Vector2 TargetPosition => Target != null ? Target.position : Origin;
    public float AttackPower => Stats != null ? Stats.AttackPower : 0f;
}
