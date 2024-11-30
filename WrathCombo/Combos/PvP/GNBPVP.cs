using WrathCombo.CustomComboNS;
using WrathCombo.Data;

namespace WrathCombo.Combos.PvP
{
    internal static class GNBPvP
    {

        public const uint
            KeenEdge = 29098,
            BrutalShell = 29099,
            SolidBarrel = 29100,
            BurstStrike = 29101,
            GnashingFang = 29102,
            SavageClaw = 29103,
            WickedTalon = 29104,
            Continuation = 29106,
            JugularRip = 29108,
            AbdomenTear = 29109,
            EyeGouge = 29110,
            RoughDivide = 29123,
            RelentlessRush = 29130,
            TerminalTrigger = 29131,
            FatedCircle = 41511,
            FatedBrand = 41442,
            BlastingZone = 29128,
            HeartOfCorundum = 41443;


        internal class Debuffs
        {
            internal const ushort
                Stun = 1343;
        }

        internal class Buffs
        {
            internal const ushort
                ReadyToRip = 2002,
                ReadyToTear = 2003,
                ReadyToGouge = 2004,
                ReadyToBlast = 3041,
                NoMercy = 3042,
                PowderBarrel = 3043,
                JugularRip = 3048,
                AbdomenTear = 3049,
                EyeGouge = 3050,
                ReadyToRaze = 4293;

        }
        public class Config
        {
            public const string
                corundumThreshold = nameof(corundumThreshold),
                blastingZoneThreshold = nameof(blastingZoneThreshold);

        }
        internal class GNBPvP_Burst : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNBPvP_Burst;

            float GCD = GetCooldown(KeenEdge).CooldownTotal; // 2.4 base in PvP
            
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is KeenEdge or BrutalShell or SolidBarrel or BurstStrike)
                {
                    int corundumThreshold = GetOptionValue(Config.corundumThreshold);
                    int blastingZoneThreshold = GetOptionValue(Config.blastingZoneThreshold); 

                    if (CanWeave(ActionWatching.LastWeaponskill) && IsEnabled(CustomComboPreset.GNBPvP_Corundum) && PlayerHealthPercentageHp() <= corundumThreshold && IsOffCooldown(HeartOfCorundum))
                        return HeartOfCorundum;

                    if (!PvPCommon.IsImmuneToDamage())
                    {
                        if (CanWeave(ActionWatching.LastWeaponskill)) //Weave section
                        {
                            //Continuation
                            if (IsEnabled(CustomComboPreset.GNBPvP_ST_Continuation) && OriginalHook(Continuation) != Continuation) // Weaving followup button, whenever it changes to something useable, it will fire
                                return OriginalHook(Continuation);

                            if (IsEnabled(CustomComboPreset.GNBPvP_BlastingZone) && IsOffCooldown(BlastingZone) && GetTargetHPPercent() < blastingZoneThreshold)  // Removed nomercy requirement bc of hp threshold.
                                return BlastingZone;
                        }

                        //RoughDivide overcap protection
                        if (IsEnabled(CustomComboPreset.GNBPvP_RoughDivide))
                        {
                            if (HasCharges(RoughDivide) && !HasEffect(Buffs.NoMercy) && !JustUsed(RoughDivide, 3f) &&
                               (ActionReady(FatedCircle)|| ActionReady(GnashingFang) || GetRemainingCharges(RoughDivide) == 2)) // Will RD for for no mercy when at 2 charges, or before the fated circle or gnashing fang combo
                                return RoughDivide;
                        }

                        //Fated Circle and Followup
                        if (IsEnabled(CustomComboPreset.GNBPvP_FatedCircle))
                        {
                            if (ActionReady(FatedCircle) && HasEffect(Buffs.NoMercy) && OriginalHook(Continuation) == Continuation)
                                return FatedCircle;
                        }

                        //GnashingFang
                        if (IsEnabled(CustomComboPreset.GNBPvP_ST_GnashingFang) && (ActionReady(GnashingFang) || OriginalHook(GnashingFang) != GnashingFang))
                            return OriginalHook(GnashingFang);

                    }
                }

                return actionID;
            }
        }

        internal class GNBPvP_GnashingFang : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNBPvP_GnashingFang;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) =>
                actionID is GnashingFang &&
                    CanWeave(actionID) && (HasEffect(Buffs.ReadyToRip) || HasEffect(Buffs.ReadyToTear) || HasEffect(Buffs.ReadyToGouge))
                    ? OriginalHook(Continuation)
                    : actionID;
        }

    }
}
