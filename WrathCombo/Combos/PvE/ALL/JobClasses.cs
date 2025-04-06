using WrathCombo.Combos.PvE.Content.Variant;

namespace WrathCombo.Combos.PvE;

//This defines a FFXIV job type, and maps specific Role and Variant actions to that job
//Examples
// GNB.Role.Interject would work, SGE.Role.Interject would not.
//This should help for future jobs and future random actions to quickly wireup job appropriate actions
internal class HealerJob
{
    public static IHealerVariant Variant { get; } = VariantHealer.Instance;
    public class Role : Healer;
    private HealerJob() { } // Prevent instantiation
}

internal class TankJob
{
    public static ITankVariant Variant { get; } = VariantTank.Instance;
    public class Role : Tank;
    private TankJob() { }
}

internal class MeleeJob
{
    public static IMeleeVariant Variant { get; } = VariantMelee.Instance;
    public class Role : Melee;
    private MeleeJob() { }
}

internal class PhysRangedJob
{
    public static IPhysRangedVariant Variant { get; } = VariantPhysRanged.Instance;
    public class Role : PhysRanged;
    private PhysRangedJob() { }
}

internal class CasterJob
{
    public static ICasterVariant Variant { get; } = VariantCaster.Instance;
    public class Role : Caster;
    private CasterJob() { }
}