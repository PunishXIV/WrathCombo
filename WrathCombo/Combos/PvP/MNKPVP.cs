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
            Meteodrive = 29485,
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
                    if (IsEnabled(CustomComboPreset.MNKPvP_Burst_Meteodrive) && PvPCommon.IsImmuneToDamage() && EnemyHealthCurrentHp() <= 20000 && IsLB1Ready) // LB options for when something is guarded and low on health. Meteo breaks thier shield and locks them in place
                        return Meteodrive;

                    if (!PvPCommon.IsImmuneToDamage())
                    {
                        if (IsEnabled(CustomComboPreset.MNKPvP_Burst_RisingPhoenix))
                        {
                            if (!HasEffect(Buffs.FireResonance) && GetRemainingCharges(RisingPhoenix) > 1 || WasLastWeaponskill(PouncingCoeurl) && GetRemainingCharges(RisingPhoenix) > 0) // Uses Rising Pheonix at 2, always retains one for use with phantom rush
                                return OriginalHook(RisingPhoenix);
                            if (HasEffect(Buffs.FireResonance) && WasLastWeaponskill(PouncingCoeurl))
                                return actionID; // makes sure it uses it on phantom rush
                        }

                        if (IsEnabled(CustomComboPreset.MNKPvP_Burst_RiddleOfEarth))
                        {
                            if (IsOffCooldown(RiddleOfEarth) && PlayerHealthPercentageHp() <= 95)  // Uses riddle of earth when you health starts to drop by being struck
                                return RiddleOfEarth;
                            if (HasEffect(Buffs.EarthResonance) && GetBuffRemainingTime(Buffs.EarthResonance) <= 2) // Uses earths reply when the buff is less than 2 secons remaining
                                return EarthsReply; 
                        }
                            

                        if (IsEnabled(CustomComboPreset.MNKPvP_Burst_Thunderclap) && GetRemainingCharges(ThunderClap) > 0 && !InMeleeRange()) // gap closer
                            return OriginalHook(ThunderClap);

                        if (IsEnabled(CustomComboPreset.MNKPvP_Burst_WindsReply) && InActionRange(WindsReply) && IsOffCooldown(WindsReply))
                            return WindsReply;

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
