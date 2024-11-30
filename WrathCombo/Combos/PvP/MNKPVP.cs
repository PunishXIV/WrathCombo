using WrathCombo.CustomComboNS;

namespace WrathCombo.Combos.PvP
{
    internal static class MNKPvP
    {
        public const byte ClassID = 2;
        public const byte JobID = 20;

        public const uint
            PhantomRushCombo = 55,
            DragonKick = 29475,
            TwinSnakes = 29476,
            Demolish = 29477,
            PhantomRush = 29478,
            RisingPhoenix = 29481,
            RiddleOfEarth = 29482,
            ThunderClap = 29484,
            EarthsReply = 29483,
            Meteordrive = 29485,
            WindsReply = 41509,
            FlintsReply = 41447,
            LeapingOpo = 41444,
            RisingRaptor = 41445,
            PouncingCoeurl = 41446;

        public static class Buffs
        {
            public const ushort
                FiresRumination = 4301,
                FireResonance = 3170,
                EarthResonance = 3171;

        }

        public static class Debuffs
        {
            public const ushort
                PressurePoint = 3172;
        }

        internal class MNKPvP_Burst : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MNKPvP_Burst;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is DragonKick or TwinSnakes or Demolish or LeapingOpo or RisingRaptor or PouncingCoeurl or PhantomRush)
                {

                    if (!PvPCommon.IsImmuneToDamage())
                    {
                        if (IsEnabled(CustomComboPreset.MNKPvP_Burst_RisingPhoenix))
                        {
                            if (!HasEffect(Buffs.FireResonance) && GetRemainingCharges(RisingPhoenix) > 1 || WasLastWeaponskill(PouncingCoeurl) && GetRemainingCharges(RisingPhoenix) > 0)
                                return OriginalHook(RisingPhoenix);
                            if (HasEffect(Buffs.FireResonance) && WasLastWeaponskill(PouncingCoeurl))
                                return actionID;
                        }

                        if (IsEnabled(CustomComboPreset.MNKPvP_Burst_RiddleOfEarth) && IsOffCooldown(RiddleOfEarth) && PlayerHealthPercentageHp() <= 95)
                            return OriginalHook(RiddleOfEarth);

                        if (IsEnabled(CustomComboPreset.MNKPvP_Burst_Thunderclap) && GetRemainingCharges(ThunderClap) > 0 && !InMeleeRange())
                            return OriginalHook(ThunderClap);

                        if (IsEnabled(CustomComboPreset.MNKPvP_Burst_WindsReply) && InActionRange(WindsReply) && IsOffCooldown(WindsReply))
                            return WindsReply;

                        if (CanWeave(actionID))
                        {
                                if (IsEnabled(CustomComboPreset.MNKPvP_Burst_RiddleOfEarth) && HasEffect(Buffs.EarthResonance) && GetBuffRemainingTime(Buffs.EarthResonance) < 6)
                                return OriginalHook(EarthsReply);
                        }

                        if (IsEnabled(CustomComboPreset.MNKPvP_Burst_FlintsReply))
                        {
                            if (GetRemainingCharges(FlintsReply) > 0 && (!WasLastAction(LeapingOpo) || !WasLastAction(RisingRaptor) || !WasLastAction(PouncingCoeurl)) || HasEffect(Buffs.FiresRumination) && !WasLastAction(PouncingCoeurl))
                                return OriginalHook(FlintsReply);
                        }
                    }
                }

                return actionID;
            }
        }
    }
}
