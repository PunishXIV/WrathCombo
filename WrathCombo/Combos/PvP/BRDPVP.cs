using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Window.Functions;

namespace WrathCombo.Combos.PvP
{
    internal static class BRDPvP
    {
        #region IDs

        public const byte ClassID = 5;
        public const byte JobID = 23;

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
        internal class Debuffs
        {
            public const ushort
                Silence = 1347,
                Bind = 1345,
                Stun = 1343,
                HalfAsleep = 3022,
                Sleep = 1348,
                DeepFreeze = 3219,
                Heavy = 1344,
                Unguarded = 3021;
        }
        #endregion

        #region Config
        public static class Config
        {
            public static UserInt
                BRDPvP_HarmonicArrowCharges = new("BRDPvP_HarmonicArrowCharges"),
                BRDPvP_EagleThreshold = new("BRDPvP_EagleThreshold");

            internal static void Draw(CustomComboPreset preset)
            {
                switch (preset)
                {

                    case CustomComboPreset.BRDPvP_HarmonicArrow:
                        UserConfig.DrawSliderInt(1, 4, BRDPvP_HarmonicArrowCharges, "How many Charges to use it at \n 1 charge 8000 damage \n 2 charge 12000 damage \n 3 charge 15000 damage \n 4 charge 17000 damage");

                        break;

                    case CustomComboPreset.BRDPvP_Eagle:
                        UserConfig.DrawSliderInt(0, 100, BRDPvP_EagleThreshold,
                            "Target HP percent threshold to use Eagle Eye Shot Below.");

                        break;
                }
            }

        }
        #endregion

        internal class BRDPvP_BurstMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRDPvP_BurstMode;

            protected override uint Invoke(uint actionID)

            {

                if (actionID == PowerfulShot)
                {
                    var canWeave = CanWeave(0.5);
                    uint harmonicCharges = GetRemainingCharges(HarmonicArrow);

                    if (IsEnabled(CustomComboPreset.BRDPvP_Eagle) && PvPPhysRanged.CanEagleEyeShot() && (PvPCommon.TargetImmuneToDamage() || GetTargetHPPercent() <= Config.BRDPvP_EagleThreshold))
                        return PvPPhysRanged.EagleEyeShot;

                    if (!PvPCommon.TargetImmuneToDamage())
                    {
                        if (IsEnabled(CustomComboPreset.BRDPvP_Wardens) && InPvP() &&  //Autowardens set up only for soft ccs, it cant be used while cced like purify
                            (HasEffectAny(Debuffs.Bind) || HasEffectAny(Debuffs.Heavy) || HasEffectAny(Debuffs.HalfAsleep)))
                            return OriginalHook(WardensPaean);

                        if (canWeave)
                        {
                            // Silence shot that gives PP, set up to not happen right after apex to tighten burst and silence after the bigger damage. Apex > Harmonic> Silent > Burst > PP or Apex > Burst > Silent >  PP
                            if (IsEnabled(CustomComboPreset.BRDPvP_SilentNocturne) && !GetCooldown(SilentNocturne).IsCooldown && !WasLastAction(ApexArrow) && !HasEffect(Buffs.Repertoire)) 
                                return OriginalHook(SilentNocturne);

                            if (IsEnabled(CustomComboPreset.BRDPvP_EncoreOfLight) && HasEffect(Buffs.EncoreofLightReady)) // LB finisher shot
                                return OriginalHook(FinalFantasia);
                        }

                        if (IsEnabled(CustomComboPreset.BRDPvP_ApexArrow) && ActionReady(ApexArrow)) // Use on cd to keep up buff
                            return OriginalHook(ApexArrow);

                        if (HasEffect(Buffs.FrontlineMarch))
                        {
                            if (IsEnabled(CustomComboPreset.BRDPvP_HarmonicArrow) &&    //Harmonic Logic. Slider plus execute ranges
                               (harmonicCharges >= Config.BRDPvP_HarmonicArrowCharges ||
                               harmonicCharges == 1 && EnemyHealthCurrentHp() <= 8000 ||
                               harmonicCharges == 2 && EnemyHealthCurrentHp() <= 12000 ||
                               harmonicCharges == 3 && EnemyHealthCurrentHp() <= 15000))
                                return OriginalHook(HarmonicArrow);

                            if (IsEnabled(CustomComboPreset.BRDPvP_BlastArrow) && HasEffect(Buffs.BlastArrowReady)) // Blast arrow when ready
                                return OriginalHook(BlastArrow);
                        }

                        return OriginalHook(PowerfulShot); // Main shot but also Pitch Perfect
                    }

                }

                return actionID;
            }                       
        }
    }
    
}
