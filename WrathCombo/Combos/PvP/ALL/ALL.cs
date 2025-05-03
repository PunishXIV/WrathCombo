using Dalamud.Game.ClientState.Objects.Types;
using ECommons.GameHelpers;
using System.Collections.Generic;
using WrathCombo.Combos.PvE;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using static WrathCombo.Window.Functions.UserConfig;

namespace WrathCombo.Combos.PvP
{
    internal static class PvPCommon
    {
        public const uint
            Teleport = 5,
            Return = 6,
            StandardElixir = 29055,
            Recuperate = 29711,
            Purify = 29056,
            Guard = 29054,
            Sprint = 29057;

        internal class Config
        {
            public static UserInt
                EmergencyHealThreshold = new("EmergencyHealThreshold"),
                EmergencyGuardThreshold = new("EmergencyGuardThreshold");
            public static UserBoolArray
                QuickPurifyStatuses = new("QuickPurifyStatuses");

            internal static void Draw(CustomComboPreset preset)
            {
                switch (preset)
                {
                    case CustomComboPreset.PvP_EmergencyHeals:
                        if (Player.Object != null)
                        {
                            uint maxHP = Player.Object.MaxHp <= 15000 ? 0 : Player.Object.MaxHp - 15000;

                            if (maxHP > 0)
                            {
                                int setting = EmergencyHealThreshold;
                                float hpThreshold = (float)maxHP / 100 * setting;

                                DrawSliderInt(1, 100, EmergencyHealThreshold, $"Set the percentage to be at or under for the feature to kick in.\n100% is considered to start at 15,000 less than your max HP to prevent wastage.\nHP Value to be at or under: {hpThreshold}");
                            }

                            else
                            {
                                DrawSliderInt(1, 100, EmergencyHealThreshold, "Set the percentage to be at or under for the feature to kick in.\n100% is considered to start at 15,000 less than your max HP to prevent wastage.");
                            }
                        }

                        else
                        {
                            DrawSliderInt(1, 100, EmergencyHealThreshold, "Set the percentage to be at or under for the feature to kick in.\n100% is considered to start at 15,000 less than your max HP to prevent wastage.");
                        }
                        
                        break;

                    case CustomComboPreset.PvP_EmergencyGuard:
                        DrawSliderInt(1, 100, EmergencyGuardThreshold, "Set the percentage to be at or under for the feature to kick in.");
                        break;

                    case CustomComboPreset.PvP_QuickPurify:
                        DrawPvPStatusMultiChoice(QuickPurifyStatuses);
                        break;
                }
            }
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
                Unguarded = 3021,
                MiracleOfNature = 3085;
        }

        internal class Buffs
        {
            public const ushort
                Sprint = 1342,
                Guard = 3054;
        }

        /// <summary> Checks if the target is immune to damage. Optionally, include buffs that provide significant damage reduction. </summary>
        /// <param name="includeReductions"> Includes buffs that provide significant damage reduction. </param>
        /// <param name="optionalTarget"> Optional target to check. </param>
        public static bool TargetImmuneToDamage(bool includeReductions = true, IGameObject? optionalTarget = null)
        {
            var t = optionalTarget ?? CurrentTarget;
            if (t is null || !InPvP()) return false;

            bool targetHasReductions = HasStatusEffect(Buffs.Guard, t, true) || HasStatusEffect(VPRPvP.Buffs.HardenedScales, t, true);
            bool targetHasImmunities = HasStatusEffect(DRKPvP.Buffs.UndeadRedemption, t, true) || HasStatusEffect(PLDPvP.Buffs.HallowedGround, t, true);

            return includeReductions
                ? targetHasReductions || targetHasImmunities
                : targetHasImmunities;
        }

        // Lists of Excluded skills 
        internal static readonly List<uint>
            MovmentSkills = [WARPvP.Onslaught, VPRPvP.Slither, NINPvP.Shukuchi, DNCPvP.EnAvant, MNKPvP.ThunderClap, RDMPvP.CorpsACorps, RDMPvP.Displacement, SGEPvP.Icarus, RPRPvP.HellsIngress, RPRPvP.Regress, BRDPvP.RepellingShot, BLMPvP.AetherialManipulation, DRGPvP.ElusiveJump, GNBPvP.RoughDivide],
            GlobalSkills = [Teleport, Guard, Recuperate, Purify, StandardElixir, Sprint];

        internal class GlobalEmergencyHeals : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PvP_EmergencyHeals;

            protected override uint Invoke(uint actionID)
            {
                if ((HasStatusEffect(Buffs.Guard) || JustUsed(Guard)) && IsEnabled(CustomComboPreset.PvP_MashCancel))
                {
                    if (actionID == Guard) return Guard;
                    return All.SavageBlade;
                }

                if (Execute() &&
                     InPvP() &&
                    !GlobalSkills.Contains(actionID) &&
                    !MovmentSkills.Contains(actionID))
                    return OriginalHook(Recuperate);

                return actionID;
            }

            public static bool Execute()
            {
                var jobMaxHp = LocalPlayer.MaxHp;
                int threshold = Config.EmergencyHealThreshold;
                var maxHPThreshold = jobMaxHp - 15000;
                var remainingPercentage = (float)LocalPlayer.CurrentHp / (float)maxHPThreshold;


                if (HasStatusEffect(3180)) return false; //DRG LB buff
                if (HasStatusEffect(1420, anyOwner: true)) return false; //Rival Wings Mounted
                if (HasStatusEffect(4096)) return false; //VPR Snakesbane
                if (HasStatusEffect(DRKPvP.Buffs.UndeadRedemption)) return false;
                if (LocalPlayer.CurrentMp < 2500) return false;
                if (remainingPercentage * 100 > threshold) return false;

                return true;

            }
        }

        internal class GlobalEmergencyGuard : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PvP_EmergencyGuard;

            protected override uint Invoke(uint actionID)
            {
                if ((HasStatusEffect(Buffs.Guard) || JustUsed(Guard)) && IsEnabled(CustomComboPreset.PvP_MashCancel))
                {
                    if (actionID == Guard)
                    {
                        if (IsEnabled(CustomComboPreset.PvP_MashCancelRecup) && !JustUsed(Guard, 2f) && LocalPlayer.CurrentMp >= 2500 && LocalPlayer.CurrentHp <= LocalPlayer.MaxHp - 15000) 
                            return Recuperate;
                        return Guard;
                    }
                    return All.SavageBlade;
                }

                if (Execute() &&
                    InPvP() &&
                    !GlobalSkills.Contains(actionID) &&
                    !MovmentSkills.Contains(actionID))
                    return OriginalHook(Guard);

                return actionID;
            }

            public static bool Execute()
            {
                var jobMaxHp = LocalPlayer.MaxHp;
                var threshold = Config.EmergencyGuardThreshold;
                var remainingPercentage = (float)LocalPlayer.CurrentHp / (float)jobMaxHp;

                if (HasStatusEffect(3180)) return false; //DRG LB buff
                if (HasStatusEffect(4096)) return false; //VPR Snakesbane
                if (HasStatusEffect(1420, anyOwner: true)) return false; //Rival Wings Mounted
                if (HasStatusEffect(DRKPvP.Buffs.UndeadRedemption)) return false;
                if (HasStatusEffect(Debuffs.Unguarded, anyOwner: true) || HasStatusEffect(WARPvP.Buffs.InnerRelease)) return false;
                if (GetCooldown(Guard).IsCooldown) return false;
                if (remainingPercentage * 100 > threshold) return false;

                return true;

            }
        }

        internal class QuickPurify : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PvP_QuickPurify;

            protected override uint Invoke(uint actionID)
            {
                if ((HasStatusEffect(Buffs.Guard) || JustUsed(Guard)) && IsEnabled(CustomComboPreset.PvP_MashCancel))
                {
                    if (actionID == Guard) return Guard;
                    return All.SavageBlade;
                }

                if (Execute() &&
                    InPvP() &&
                    !GlobalSkills.Contains(actionID))
                    return OriginalHook(Purify);

                return actionID;
            }

            public static bool Execute()
            {
                bool[] selectedStatuses = Config.QuickPurifyStatuses;

                if (HasStatusEffect(3180)) return false; //DRG LB buff
                if (HasStatusEffect(4096)) return false; //VPR Snakesbane
                if (HasStatusEffect(1420, anyOwner: true)) return false; //Rival Wings Mounted

                if (selectedStatuses.Length == 0) return false;
                if (GetCooldown(Purify).IsCooldown) return false;
                if (HasStatusEffect(Debuffs.Stun, anyOwner: true) && selectedStatuses[0]) return true;
                if (HasStatusEffect(Debuffs.DeepFreeze, anyOwner: true) && selectedStatuses[1]) return true;
                if (HasStatusEffect(Debuffs.HalfAsleep, anyOwner: true) && selectedStatuses[2]) return true;
                if (HasStatusEffect(Debuffs.Sleep, anyOwner: true) && selectedStatuses[3]) return true;
                if (HasStatusEffect(Debuffs.Bind, anyOwner: true) && selectedStatuses[4]) return true;
                if (HasStatusEffect(Debuffs.Heavy, anyOwner: true) && selectedStatuses[5]) return true;
                if (HasStatusEffect(Debuffs.Silence, anyOwner: true) && selectedStatuses[6]) return true;
                if (HasStatusEffect(Debuffs.MiracleOfNature, anyOwner: true) && selectedStatuses[7]) return true;

                return false;

            }
        }

    }

}
