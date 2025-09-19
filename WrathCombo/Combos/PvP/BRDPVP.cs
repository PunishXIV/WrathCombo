using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.Window.Functions.UserConfig;
using static WrathCombo.Combos.PvP.BRDPvP.Config;

namespace WrathCombo.Combos.PvP;

internal static class BRDPvP
{
    #region IDs
    internal class Role : PvPPhysRanged;

    public const uint
        PowerfulShot = 29391,
        ApexArrow = 29393,
        SilentNocturne = 29395,
        RepellingShot = 29399,
        WardensPaean = 29400,
        PitchPerfect = 29392,
        BlastArrow = 29394,
        HarmonicArrow = 41464,
        FinalFantasia = 29401;

    public static class Buffs
    {
        public const ushort
            FrontlinersMarch = 3138,
            FrontlinersForte = 3140,
            Repertoire = 3137,
            BlastArrowReady = 3142,
            EncoreofLightReady = 4312,
            FrontlineMarch = 3139;
    }
    #endregion

    #region Config
    public static class Config
    {
        public static UserInt
            BRDPvP_HarmonicArrowCharges = new("BRDPvP_HarmonicArrowCharges"),
            BRDPvP_EagleThreshold = new("BRDPvP_EagleThreshold");

        internal static void Draw(Preset preset)
        {
            switch (preset)
            {
                case Preset.BRDPvP_HarmonicArrow:
                    DrawSliderInt(1, 4, BRDPvP_HarmonicArrowCharges, "How many Charges to use it at \n 1 charge 8000 damage \n 2 charge 12000 damage \n 3 charge 15000 damage \n 4 charge 17000 damage");
                    break;

                case Preset.BRDPvP_Eagle:
                    DrawSliderInt(0, 100, BRDPvP_EagleThreshold, "Target HP percent threshold to use Eagle Eye Shot Below.");
                    break;
            }
        }
    }
    #endregion

    internal class BRDPvP_BurstMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.BRDPvP_BurstMode;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not PowerfulShot) 
                return actionID;
            
            var canWeave = CanWeave(0.5f);
            uint harmonicCharges = GetRemainingCharges(HarmonicArrow);

            if (IsEnabled(Preset.BRDPvP_Eagle) && PvPPhysRanged.CanEagleEyeShot() && (PvPCommon.TargetImmuneToDamage() || GetTargetHPPercent() <= BRDPvP_EagleThreshold))
                return PvPPhysRanged.EagleEyeShot;

            if (!PvPCommon.TargetImmuneToDamage())
            {
                if (IsEnabled(Preset.BRDPvP_Wardens) && InPvP() &&  //Autowardens set up only for soft ccs, it cant be used while cced like purify
                    (HasStatusEffect(PvPCommon.Debuffs.Bind, anyOwner: true) || HasStatusEffect(PvPCommon.Debuffs.Heavy, anyOwner: true) || HasStatusEffect(PvPCommon.Debuffs.HalfAsleep, anyOwner: true)))
                    return OriginalHook(WardensPaean);

                if (canWeave)
                {
                    // Silence shot that gives PP, set up to not happen right after apex to tighten burst and silence after the bigger damage. Apex > Harmonic> Silent > Burst > PP or Apex > Burst > Silent >  PP
                    if (IsEnabled(Preset.BRDPvP_SilentNocturne) && !GetCooldown(SilentNocturne).IsCooldown && !WasLastAction(ApexArrow) && !HasStatusEffect(Buffs.Repertoire)) 
                        return OriginalHook(SilentNocturne);

                    if (IsEnabled(Preset.BRDPvP_EncoreOfLight) && HasStatusEffect(Buffs.EncoreofLightReady)) // LB finisher shot
                        return OriginalHook(FinalFantasia);
                }

                if (IsEnabled(Preset.BRDPvP_ApexArrow) && ActionReady(ApexArrow) && !HasStatusEffect(Buffs.BlastArrowReady)) // Use on cd to keep up buff
                    return OriginalHook(ApexArrow);

                if (HasStatusEffect(Buffs.FrontlineMarch))
                {
                    if (IsEnabled(Preset.BRDPvP_HarmonicArrow) &&    //Harmonic Logic. Slider plus execute ranges
                        (harmonicCharges >= BRDPvP_HarmonicArrowCharges ||
                         harmonicCharges == 1 && GetTargetCurrentHP() <= 8000 ||
                         harmonicCharges == 2 && GetTargetCurrentHP() <= 12000 ||
                         harmonicCharges == 3 && GetTargetCurrentHP() <= 15000))
                        return OriginalHook(HarmonicArrow);

                    if (IsEnabled(Preset.BRDPvP_BlastArrow) && HasStatusEffect(Buffs.BlastArrowReady)) // Blast arrow when ready
                        return OriginalHook(BlastArrow);
                }
                return OriginalHook(PowerfulShot); // Main shot but also Pitch Perfect
            }
            return actionID;
        }                       
    }
}