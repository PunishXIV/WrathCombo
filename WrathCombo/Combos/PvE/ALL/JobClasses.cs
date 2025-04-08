using WrathCombo.Combos.PvE.Content;

namespace WrathCombo.Combos.PvE;

//This defines a FFXIV job type, and maps specific Role and Variant actions to that job
//Examples
// GNB.Role.Interject would work, SGE.Role.Interject would not.
//THis should help for future jobs and future random actions to quickly wireup job appropriate actions
class HealerJob
{
    public class Variant : VariantHealer;
    public class Role : Healer;
}

class TankJob
{
    public class Variant : VariantTank;
    public class Role : Tank;
}

class MeleeJob
{
    public class Variant : VariantPDPS;
    public class Role : Melee;
}

class PhysRangedJob
{
    public class Variant : VariantPDPS;
    public class Role : PhysRanged;
}

class CasterJob
{
    public class Variant : VariantMDPS;
    public class Role : Caster;
}