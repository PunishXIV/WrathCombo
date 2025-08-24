using static WrathCombo.Combos.PvE.RoleActions;
namespace WrathCombo.Combos.PvE;

//This defines a FFXIV job type, and maps specific Role and Variant actions to that job
//Examples
// GNB.Role.Interject would work, SGE.Role.Interject would not.
//This should help for future jobs and future random actions to quickly wireup job appropriate actions
internal class Healer
{
    public static IHealer Role => Roles.Healer.Instance;
    protected Healer() { } // Prevent instantiation
}

internal class Tank
{
    public static ITank Role => Roles.Tank.Instance;
    protected Tank() { }
}

internal class Melee
{
    public static IMelee Role => Roles.Melee.Instance;
    protected Melee() { }
}

internal class PhysicalRanged
{
    public static IPhysicalRanged Role => Roles.PhysicalRanged.Instance;
    protected PhysicalRanged() { }
}

internal class Caster
{
    public static ICaster Role => Roles.Caster.Instance;
    protected Caster() { }
}