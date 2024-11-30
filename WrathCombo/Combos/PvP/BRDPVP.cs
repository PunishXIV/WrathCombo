using WrathCombo.CustomComboNS;

namespace WrathCombo.Combos.PvP
{
    internal static class BRDPvP
    {
        public const byte ClassID = 5;
        public const byte JobID = 23;

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

        public static class Config
        {
            public const string
                BRDPvP_HarmonicArrowCharges = "BRDPvP_HarmonicArrowCharges";

        }

        internal class BRDPvP_BurstMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRDPvP_BurstMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {

                if (actionID == PowerfulShot)
                {
                    var canWeave = CanWeave(actionID, 0.5);

                    if (!PvPCommon.IsImmuneToDamage())
                    {
                        if (canWeave)
                        {
                            if (IsEnabled(CustomComboPreset.BRDPvP_SilentNocturne) && !GetCooldown(SilentNocturne).IsCooldown)
                                return OriginalHook(SilentNocturne);

                            if (IsEnabled(CustomComboPreset.BRDPvP_EncoreOfLight) && HasEffect(Buffs.EncoreofLightReady))
                                return OriginalHook(FinalFantasia);
                        }

                        if (IsEnabled(CustomComboPreset.BRDPvP_ApexArrow) && !GetCooldown(ApexArrow).IsCooldown)
                            return OriginalHook(ApexArrow);

                        if (HasEffect(Buffs.FrontlineMarch))
                        {
                            if (IsEnabled(CustomComboPreset.BRDPvP_HarmonicArrow) && 
                               (GetRemainingCharges(HarmonicArrow) >= GetOptionValue(Config.BRDPvP_HarmonicArrowCharges) || GetRemainingCharges(HarmonicArrow) > 0 && EnemyHealthCurrentHp() <= 12000))
                                return OriginalHook(HarmonicArrow);

                            if (IsEnabled(CustomComboPreset.BRDPvP_BlastArrow) && HasEffect(Buffs.BlastArrowReady))
                                return OriginalHook(BlastArrow);

                            if (HasEffect(Buffs.Repertoire))
                                return OriginalHook(PowerfulShot);

                        }

                        return OriginalHook(PowerfulShot);
                    }

                }

                return actionID;
            }
        }
    }
}
