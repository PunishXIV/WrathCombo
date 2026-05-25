using static WrathCombo.Combos.PvE.RoleActions;
namespace WrathCombo.Combos.PvE;

//This defines a FFXIV job type, and maps specific Role and Variant actions to that job
//Examples
// GNB.Role.Interject would work, SGE.Role.Interject would not.
//This should help for future jobs and future random actions to quickly wireup job appropriate actions
internal class Healer
{
    protected Healer() { } // Prevent instantiation
    public static IHealer Role => Roles.Healer.Instance;
}

internal class Tank
{
    protected Tank() { }
    public static ITank Role => Roles.Tank.Instance;
}

internal class Melee
{
    protected Melee() { }
    public static IMelee Role => Roles.Melee.Instance;
}

internal class PhysicalRanged
{
    protected PhysicalRanged() { }
    public static IPhysicalRanged Role => Roles.PhysicalRanged.Instance;
}

internal class Caster
{
    protected Caster() { }
    public static ICaster Role => Roles.Caster.Instance;
}
