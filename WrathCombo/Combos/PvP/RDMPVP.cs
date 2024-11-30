using WrathCombo.CustomComboNS;

namespace WrathCombo.Combos.PvP
{
    internal static class RDMPvP
    {
        public const byte JobID = 35;

        public const uint
            Verstone = 29683,
            EnchantedRiposte = 41488,
            Resolution = 41492,
            MagickBarrier = 29697,
            CorpsACorps = 29699,
            Displacement = 29700,
            EnchantedZwerchhau = 41489,
            EnchantedRedoublement = 41490,
            Frazzle = 29698,
            SouthernCross = 29704,
            Scorch = 41491,
            Embolden = 41494,
            Forte = 41496,
            Jolt3 = 41486,
            ViceofThorns = 41493,
            Prefulgence = 41495;

        public static class Buffs
        {
            public const ushort
                WhiteShift = 3245,
                BlackShift = 3246,
                Dualcast = 1393,
                EnchantedRiposte = 3234,
                EnchantedRedoublement = 3236,
                EnchantedZwerchhau = 3235,
                VermilionRadiance = 3233,
                MagickBarrier = 3240,
                Displacement = 3243,
                Embolden = 2282,
                Forte = 4320,
                PrefulgenceReady = 4322,
                ThornedFlourish = 0;
        }

        public static class Debuffs
        {
            public const ushort
                Monomachy = 3242;
        }
        public static class Config
        {
            public const string
                RDMPvP_Burst_CorpsACorps = "RDMPvP_Burst_CorpsACorps",
                RDMPvP_Burst_Displacement = "RDMPvP_Burst_Displacement";

        }
        internal class RDMPvP_BurstMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDMPvP_BurstMode;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (actionID is Jolt3)
                {
                    if (ActionReady(Forte) && CanWeave(actionID))
                        return Forte;

                    if (IsEnabled(CustomComboPreset.RDMPvP_Burst_Resolution) && ActionReady(Resolution) && !PvPCommon.IsImmuneToDamage())
                        return OriginalHook(Resolution);

                    if (IsEnabled(CustomComboPreset.RDMPvP_Burst_CorpsACorps) && GetRemainingCharges(CorpsACorps) > GetOptionValue(Config.RDMPvP_Burst_CorpsACorps) && ActionReady(EnchantedRiposte) && !InMeleeRange())
                        return OriginalHook(CorpsACorps);

                    if (InMeleeRange())
                    {
                        if (IsEnabled(CustomComboPreset.RDMPvP_Burst_Embolden))
                        {
                            if (ActionReady(Embolden) && OriginalHook(Embolden) == Embolden && (WasLastAbility(CorpsACorps) || TargetHasEffect(Debuffs.Monomachy)))
                                return OriginalHook(Embolden);
                        }

                        if (IsEnabled(CustomComboPreset.RDMPvP_Burst_Displacement) && GetRemainingCharges(Displacement) > GetOptionValue(Config.RDMPvP_Burst_Displacement) && !ActionReady(EnchantedRiposte) && OriginalHook(EnchantedRiposte) == Scorch)
                            return OriginalHook(Displacement);

                        if (IsEnabled(CustomComboPreset.RDMPvP_Burst_EnchantedRiposte))
                        {
                            if (ActionReady(EnchantedRiposte) || OriginalHook(EnchantedRiposte) != EnchantedRiposte)
                                return OriginalHook(EnchantedRiposte);
                        }                                                
                    }
                    if (OriginalHook(EnchantedRiposte) == Scorch)
                        return OriginalHook(EnchantedRiposte);

                    if (IsEnabled(CustomComboPreset.RDMPvP_Burst_Embolden) && OriginalHook(Embolden) == Prefulgence)
                        return OriginalHook(Embolden);

                }

                return actionID;
            }
        }
    }
}
