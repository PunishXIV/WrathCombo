using WrathCombo.CustomComboNS;

namespace WrathCombo.Combos.PvP
{
    internal static class VPRPvP
    {
        public const byte JobID = 41;

        internal const uint
            SteelFangs = 39157,
            HuntersSting = 39159,
            BarbarousSlice = 39161,
            PiercingFangs = 39158,
            SwiftskinsSting = 39160,
            RavenousBite = 39163,
            Bloodcoil = 39166,
            UncoiledFury = 39168,
            SerpentsTail = 39183,
            FirstGeneration = 39169,
            SecondGeneration = 39170,
            ThirdGeneration = 39171,
            FourthGeneration = 39172,
            Ouroboros = 39173,
            Slither = 39184,
            Snakescales = 39185,
            Backlash = 39186,
            RattlingCoil = 39189;

        internal class Buffs
        {
            internal const ushort
                HardenedScales = 4096;
        }

        internal class VPRPvP_Burst : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.VPRPvP_Burst;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                #region Variables
                bool canWeave = CanWeave(OriginalHook(actionID));
                bool isSerpentsTailPrimed = OriginalHook(SerpentsTail) != SerpentsTail;
                bool inGenerationsCombo = OriginalHook(actionID) is FirstGeneration or SecondGeneration or ThirdGeneration or FourthGeneration;
                bool inMeleeRange = GetTargetDistance() <= 5;
                #endregion

                if (actionID is SteelFangs or HuntersSting or BarbarousSlice or PiercingFangs or SwiftskinsSting or RavenousBite)
                {
                    // Serpent's Tail every ability bypasses guard. Canweave removed because gcd of 2 seconds is too fast for the needed double weave Twinfang/Twinblood
                    if (isSerpentsTailPrimed && IsEnabled(CustomComboPreset.VPRPvP_Burst_Serpent))
                        return OriginalHook(SerpentsTail);

                    // gap closer
                    if (ActionReady(Slither) && IsEnabled(CustomComboPreset.VPRPvP_Burst_Slither) && !inMeleeRange)
                        return OriginalHook(Slither);

                    // Snakescales backlash finisher, will not initiate the snakescales guard stance
                    if (OriginalHook(Snakescales) == Backlash && IsEnabled(CustomComboPreset.VPRPvP_Burst_Backlash))
                        return OriginalHook(Snakescales);

                    // Rattling coil reseting cds on snakescales and uncoiled fury
                    if (IsOnCooldown(UncoiledFury) && IsOnCooldown(Snakescales) && ActionReady(RattlingCoil) && IsEnabled(CustomComboPreset.VPRPvP_Burst_Rattling))
                        return OriginalHook(RattlingCoil);

                    if (!PvPCommon.IsImmuneToDamage() && HasTarget())
                    {
                        if (inMeleeRange)
                        {
                            // Ouroboros lb stuff
                            if (OriginalHook(Bloodcoil) == Ouroboros && !inGenerationsCombo)
                                return OriginalHook(Bloodcoil);

                            // Reawakened lb stuff
                            if (inGenerationsCombo)
                                return OriginalHook(actionID);
                        }

                        // Uncoiled Fury If capped, ranged, or execute
                        if (IsEnabled(CustomComboPreset.VPRPvP_Burst_Uncoiled))
                        {
                            if (GetRemainingCharges(UncoiledFury) > 1 || !inMeleeRange && GetRemainingCharges(UncoiledFury) > 0 || EnemyHealthCurrentHp() < 10000 && GetRemainingCharges(UncoiledFury) > 0)
                            return OriginalHook(UncoiledFury);
                        }

                        // Bloodcoil, its a self heal but it is also, with its weave, 11k damage. So didnt tie it to self health
                        if (IsEnabled(CustomComboPreset.VPRPvP_Burst_Bloodcoil) && (ActionReady(Bloodcoil) || OriginalHook(Bloodcoil) != Bloodcoil))
                            return OriginalHook(Bloodcoil);
                    }
                }

                return actionID;
            }
        }
    }
}
