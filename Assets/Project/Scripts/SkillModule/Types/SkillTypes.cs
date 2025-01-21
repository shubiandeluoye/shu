namespace SkillModule.Types
{
    public enum SkillType
    {
        None = 0,
        Attack = 1,
        Heal = 2,
        Barrier = 3,
        Box = 4,
        Shoot = 5,
        Buff = 6
    }

    public enum EffectType
    {
        None = 0,
        Damage = 1,
        Heal = 2,
        Shield = 3,
        Speed = 4,
        Stun = 5,
        Buff = 6
    }

    public enum TargetType
    {
        None = 0,
        Self = 1,
        Single = 2,
        Area = 3,
        Direction = 4
    }
} 