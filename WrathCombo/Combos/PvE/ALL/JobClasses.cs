using WrathCombo.Combos.PvE.Content;

namespace WrathCombo.Combos.PvE;

//This defines a FFXIV job type, and maps specific Role and Variant actions to that job
//Examples
// GNB.Role.Interject would work, SGE.Role.Interject would not.
//THis should help for future jobs and future random actions to quickly wireup job appropriate actions
class HealerJob
{
    public class Variant : VariantHealer;
    public static IHealer Role { get; } = Healer.Instance;
}

class TankJob
{
    public class Variant : VariantTank;
    public static ITank Role { get; } = Tank.Instance;
}

class MeleeJob
{
    public class Variant : VariantPDPS;
    public static IMelee Role { get; } = Melee.Instance;
}

class PhysRangedJob
{
    public class Variant : VariantPDPS;
    public static IPhysRanged Role { get; } = PhysRanged.Instance;
}

class CasterJob
{
    public class Variant : VariantMDPS;
    public static ICaster Role { get; } = Caster.Instance;
}