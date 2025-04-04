using WrathCombo.Combos.PvE.Content;

namespace WrathCombo.Combos.PvE;

//This defines a FFXIV job type, and maps specific Role and Variant actions to that job
//Examples
// GNB.Role.Interject would work, SGE.Role.Interject would not.
//This should help for future jobs and future random actions to quickly wireup job appropriate actions
internal class HealerJob
{
    public static IHealerVariant Variant { get; } = new VariantHealer();
    public class Role : Healer;
}

internal class TankJob
{
    public static ITankVariant Variant { get; } = new VariantTank();
    public class Role : Tank;
}

internal class MeleeJob
{
    public static IMDPSVariant Variant { get; } = new VariantMDPS();
    public class Role : Melee;
}

internal class PhysRangedJob
{
    public static IPDPSVariant Variant { get; } = new VariantPDPS();
    public class Role : PhysRanged;
}

internal class CasterJob
{
    public static ICasterVariant Variant { get; } = new VariantCaster();
    public class Role : Caster;
}