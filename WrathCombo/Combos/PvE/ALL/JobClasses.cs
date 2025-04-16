using static WrathCombo.Combos.PvE.Content.Variant;
using static WrathCombo.Combos.PvE.RoleActions;

namespace WrathCombo.Combos.PvE;

//This defines a FFXIV job type, and maps specific Role and Variant actions to that job
//Examples
// GNB.Role.Interject would work, SGE.Role.Interject would not.
//This should help for future jobs and future random actions to quickly wireup job appropriate actions
internal class Healer
{
    public static IHealerVariant Variant { get; } = VariantRoles.Healer.Instance;
    public static IHealer Role { get; } = Roles.Healer.Instance;
    protected Healer() { } // Prevent instantiation
}

internal class Tank
{
    public static ITankVariant Variant { get; } = VariantRoles.Tank.Instance;
    public static ITank Role { get; } = Roles.Tank.Instance;
    protected Tank() { }
}

internal class Melee
{
    public static IMeleeVariant Variant { get; } = VariantRoles.Melee.Instance;
    public static IMelee Role { get; } = Roles.Melee.Instance;
    protected Melee() { }
}

internal class PhysicalRanged
{
    public static IPhysicalRangedVariant Variant { get; } = VariantRoles.PhysicalRanged.Instance;
    public static IPhysicalRanged Role { get; } = Roles.PhysicalRanged.Instance;
    protected PhysicalRanged() { }
}

internal class Caster
{
    public static ICasterVariant Variant { get; } = VariantRoles.Caster.Instance;
    public static ICaster Role { get; } = Roles.Caster.Instance;
    protected Caster() { }
}