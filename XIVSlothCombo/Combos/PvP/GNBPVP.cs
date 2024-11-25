using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.Data;

namespace XIVSlothCombo.Combos.PvP
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
                corundumThreshold = nameof(corundumThreshold);

        }
        internal class GNBPvP_Burst : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNBPvP_Burst;

            float GCD = GetCooldown(KeenEdge).CooldownTotal; // 2.4 base in PvP
            
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is KeenEdge or BrutalShell or SolidBarrel)
                {
                    int corundumThreshold = GetOptionValue(Config.corundumThreshold);
                    bool enemyGuard = TargetHasEffect(PvPCommon.Buffs.Guard); //Guard check
                    bool inGF = JustUsed(GnashingFang, 3f) || JustUsed(SavageClaw, 3f) || JustUsed(WickedTalon, 2f);

                    if (CanWeave(ActionWatching.LastWeaponskill) && IsEnabled(CustomComboPreset.GNBPvP_Corundum) && PlayerHealthPercentageHp() <= corundumThreshold && IsOffCooldown(HeartOfCorundum))
                        return HeartOfCorundum;

                    if (!PvPCommon.IsImmuneToDamage())
                    {

                        if (CanWeave(ActionWatching.LastWeaponskill))
                        {
                            //Continuation
                            if (IsEnabled(CustomComboPreset.GNBPvP_ST_Continuation) &&
                                HasEffect(Buffs.ReadyToBlast) || //Hypervelocity
                                HasEffect(Buffs.ReadyToRip) || //GunStep1
                                HasEffect(Buffs.ReadyToTear) || //GunStep2
                                HasEffect(Buffs.ReadyToGouge)) //GunStep reset to 0
                                return OriginalHook(Continuation);

                            //RoughDivide
                            if (IsEnabled(CustomComboPreset.GNBPvP_RoughDivide))
                            {
                                if (ActionReady(RoughDivide) && !HasEffect(Buffs.NoMercy) && //Mashing would be a waste
                                   (GetCooldownRemainingTime(FatedCircle) <= GCD || //Keeps 1 charge always for Fated Circle usage
                                    GetCooldownRemainingTime(GnashingFang) <= GCD)) //Keeps 1 charge always for GnashingFang usage
                                    return RoughDivide;

                            }

                            if (IsEnabled(CustomComboPreset.GNBPvP_BlastingZone) && IsOffCooldown(BlastingZone) && HasEffect(Buffs.NoMercy))
                                return BlastingZone;
                        }

                        //RoughDivide overcap protection
                        if (IsEnabled(CustomComboPreset.GNBPvP_RoughDivide))
                        {
                            if (HasCharges(RoughDivide) &&
                               (ActionReady(FatedCircle) && GetRemainingCharges(RoughDivide) > 1) || //force for Fated Circle
                                GetRemainingCharges(RoughDivide) == 2) //force if at 2
                                return RoughDivide;
                        }

                        if (IsEnabled(CustomComboPreset.GNBPvP_FatedCircle) &&
                            !HasEffect(Buffs.ReadyToBlast) &&
                            !HasEffect(Buffs.ReadyToRip) &&
                            !HasEffect(Buffs.ReadyToTear) &&
                            !HasEffect(Buffs.ReadyToGouge) &&
                            !WasLastWeaponskill(GnashingFang) &&
                            !WasLastWeaponskill(WickedTalon) &&
                            !WasLastWeaponskill(SavageClaw))
                        {
                            if (HasEffect(Buffs.ReadyToRaze))
                                return OriginalHook(FatedBrand);
                            if (IsOffCooldown(FatedCircle) && HasEffect(Buffs.NoMercy))
                                return FatedCircle;
                        }

                        //SavageClaw & WickedTalon
                        if (IsEnabled(CustomComboPreset.GNBPvP_ST_GnashingFang) &&
                            JustUsed(GnashingFang, 3f) || JustUsed(SavageClaw, 3f))
                            return OriginalHook(GnashingFang);

                        //BurstStrike
                        if (IsEnabled(CustomComboPreset.GNBPvP_BurstStrike) && HasEffect(Buffs.PowderBarrel) &&
                            (!HasEffect(Buffs.JugularRip) || !HasEffect(Buffs.AbdomenTear) || !HasEffect(Buffs.EyeGouge) && //Do not use when in GnashingFang combo
                            !HasEffect(Buffs.ReadyToBlast) || !HasEffect(Buffs.ReadyToRip) || !HasEffect(Buffs.ReadyToTear) || !HasEffect(Buffs.ReadyToGouge))) //Burst Strike has prio over GnashingFang, but not Fated Circle
                            return BurstStrike;

                        //GnashingFang
                        if (IsEnabled(CustomComboPreset.GNBPvP_ST_GnashingFang) && ActionReady(GnashingFang) && !HasEffect(Buffs.PowderBarrel)) //BurstStrike first to avoid losing the buff if applicable
                            return GnashingFang;

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