using Dalamud.Game.ClientState.Objects.Types;
using System;
using System.Collections.Frozen;
using System.Linq;
using WrathCombo.Combos.PvE;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using static WrathCombo.Window.Functions.UserConfig;
namespace WrathCombo.Combos.PvP;

internal static class PvPCommon
{
    const uint RecuperateCost = 2500; // Recuperate MP Cost
    const uint RecuperateAmount = 15000; // Recuperate Base Heal

    public const uint
        Teleport = 5,
        Return = 6,
        StandardElixir = 29055,
        Recuperate = 29711,
        Purify = 29056,
        Guard = 29054,
        Sprint = 29057,
        PvPRoleAction = 43259;

    internal class Buffs
    {
        public const ushort
            RidingMecha = 1420, // Rival Wings
            Soaring = 1465,     // Rival Wings
            FlyingHigh = 1730,  // Rival Wings
            Sprint = 1342,
            Guard = 3054,
            Resilience = 3248,
            WeakenedGuard = 3673;
    }

    internal class Debuffs
    {
        public const ushort
            Silence = 1347,
            Bind = 1345,
            Stun = 1343,
            HalfAsleep = 3022,  // Unused
            Sleep = 1348,       // Unused
            DeepFreeze = 3219,
            Heavy = 1344,
            Unguarded = 3021,
            MiracleOfNature = 3085;
    }

    internal class Config
    {
        public static UserInt
            EmergencyHealThreshold = new("EmergencyHealThreshold"),
            EmergencyGuardThreshold = new("EmergencyGuardThreshold"),
            PurifyMPThreshold = new("PurifyMPThreshold");

        public static UserBoolArray
            QuickPurifyStatuses = new("QuickPurifyStatuses");

        internal static void Draw(Preset preset)
        {
            switch (preset)
            {
                case Preset.PvP_EmergencyHeals:
                    string baseMessage = $"Uses Recuperate when at or under the threshold.\n" +
                         $"Calculated from (MaxHP - {RecuperateAmount:N0}) to prevent overhealing.";

                    if (LocalPlayer is { } player && player.MaxHp > RecuperateAmount)
                    {
                        var adjustedMaxHP = player.MaxHp - RecuperateAmount;
                        var thresholdHP = adjustedMaxHP / 100f * EmergencyHealThreshold;

                        DrawSliderInt(1, 100, EmergencyHealThreshold, $"{baseMessage}\nThreshold: {thresholdHP:N0} HP");
                    }
                    else
                    {
                        DrawSliderInt(1, 100, EmergencyHealThreshold, baseMessage);
                    }
                    break;

                case Preset.PvP_EmergencyGuard:
                    DrawSliderInt(1, 100, EmergencyGuardThreshold, "Uses Guard when at or under:");
                    break;

                case Preset.PvP_QuickPurify:
                    DrawSliderInt(2500, 10000, PurifyMPThreshold, "Do not use Purify below set MP", sliderIncrement:100);
                    DrawPvPStatusMultiChoice(QuickPurifyStatuses);
                    break;
            }
        }
    }

    /// <summary> Checks if the target is immune to damage. Optionally, include buffs that provide significant damage reduction. </summary>
    /// <param name="includeReductions"> Includes buffs that provide significant damage reduction. </param>
    /// <param name="optionalTarget"> Optional target to check. </param>
    public static bool TargetImmuneToDamage(bool includeReductions = true, IGameObject? optionalTarget = null)
    {
        var t = optionalTarget ?? CurrentTarget;
        if (t is null || !InPvP()) return false;

        bool targetHasReductions = HasStatusEffect(Buffs.Guard, t, true) || HasStatusEffect(Buffs.WeakenedGuard, t, true) || HasStatusEffect(VPRPvP.Buffs.HardenedScales, t, true);
        bool targetHasImmunities = HasStatusEffect(DRKPvP.Buffs.UndeadRedemption, t, true) || HasStatusEffect(PLDPvP.Buffs.HallowedGround, t, true);

        return includeReductions
            ? targetHasReductions || targetHasImmunities
            : targetHasImmunities;
    }

    // Lists of Excluded Actions 
    internal static readonly FrozenSet<uint>
        CommonActions = [Teleport, Return, Guard, Recuperate, Purify, StandardElixir, Sprint, PvPRoleAction],
        MovementActions = [
            PLDPvP.Intervene, PLDPvP.Guardian,
            WARPvP.Onslaught, WARPvP.PrimalRend,
            DRKPvP.Plunge,
            GNBPvP.RoughDivide,
            WHMPvP.SeraphStrike,
            ASTPvP.Epicycle, ASTPvP.Retrograde,
            SGEPvP.Icarus,
            MNKPvP.Thunderclap,
            DRGPvP.ElusiveJump, DRGPvP.HighJump,
            NINPvP.Shukuchi, NINPvP.ForkedRaiju, NINPvP.FleetingRaiju,
            RPRPvP.HellsIngress, RPRPvP.Regress,
            VPRPvP.Slither,
            SAMPvP.Soten,
            DNCPvP.EnAvant,
            BRDPvP.RepellingShot,
            PCTPvP.Smudge,
            SMNPvP.CrimsonCyclone,
            BLMPvP.AetherialManipulation,
            RDMPvP.CorpsACorps, RDMPvP.Displacement];

    internal class GlobalEmergencyHeals : CustomCombo
    {
        protected internal override Preset Preset => Preset.PvP_EmergencyHeals;

        protected override uint Invoke(uint actionID)
        {
            if ((HasStatusEffect(Buffs.Guard) || JustUsed(Guard)) && IsEnabled(Preset.PvP_MashCancel))
            {
                if (actionID is Guard)
                    return Guard;

                return All.SavageBlade;
            }

            if (Execute() && InPvP() &&
                !CommonActions.Contains(actionID) &&
                !MovementActions.Contains(actionID))
                return OriginalHook(Recuperate);

            return actionID;
        }

        public static bool Execute()
        {
            if (LocalPlayer is not { } player || player.IsDead || player.CurrentMp < RecuperateCost) return false;

            // Special States
            if (HasStatusEffect(DRGPvP.Buffs.SkyHigh) ||
                HasStatusEffect(VPRPvP.Buffs.HardenedScales) ||
                HasStatusEffect(DRKPvP.Buffs.UndeadRedemption) ||
                HasStatusEffect(Buffs.RidingMecha, anyOwner: true))
                return false;

            var adjustedCurrentHP = player.CurrentHp * 100;
            var adjustedMaxHP = player.MaxHp - RecuperateAmount;

            return adjustedCurrentHP <= Config.EmergencyHealThreshold * adjustedMaxHP;
        }
    }

    internal class GlobalEmergencyGuard : CustomCombo
    {
        protected internal override Preset Preset => Preset.PvP_EmergencyGuard;

        protected override uint Invoke(uint actionID)
        {
            if ((HasStatusEffect(Buffs.Guard) || JustUsed(Guard)) && IsEnabled(Preset.PvP_MashCancel))
            {
                if (actionID is Guard)
                {
                    var player = LocalPlayer;

                    if (IsEnabled(Preset.PvP_MashCancelRecup) && !JustUsed(Guard, 2f) &&
                        player.CurrentMp >= RecuperateCost && player.CurrentHp <= player.MaxHp - RecuperateAmount) 
                        return Recuperate;

                    return Guard;
                }

                return All.SavageBlade;
            }

            if (Execute() && InPvP() &&
                !CommonActions.Contains(actionID) &&
                !MovementActions.Contains(actionID))
                return OriginalHook(Guard);

            return actionID;
        }

        public static bool Execute()
        {
            if (LocalPlayer is not { } player || player.IsDead || IsOnCooldown(Guard)) return false;

            // Special States
            if (HasStatusEffect(DRGPvP.Buffs.SkyHigh) ||
                HasStatusEffect(WARPvP.Buffs.InnerRelease) ||
                HasStatusEffect(VPRPvP.Buffs.HardenedScales) ||
                HasStatusEffect(PLDPvP.Buffs.HallowedGround) ||
                HasStatusEffect(DRKPvP.Buffs.UndeadRedemption) ||
                HasStatusEffect(Debuffs.Unguarded, anyOwner: true) ||
                HasStatusEffect(Buffs.RidingMecha, anyOwner: true))
                return false;

            var adjustedCurrentHP = player.CurrentHp * 100;
            var adjustedThreshold = Config.EmergencyGuardThreshold * player.MaxHp;

            return adjustedCurrentHP <= adjustedThreshold;
        }
    }

    internal class QuickPurify : CustomCombo
    {
        protected internal override Preset Preset => Preset.PvP_QuickPurify;

        public static (ushort debuff, string label)[] Statuses =
        [
            (Debuffs.Stun, "Stun"),
            (Debuffs.DeepFreeze, "Deep Freeze"),
            (Debuffs.HalfAsleep, "Half Asleep"), // todo: remove, reset cfg
            (Debuffs.Sleep, "Sleep"), // todo: remove, reset cfg
            (Debuffs.Bind, "Bind"),
            (Debuffs.Heavy, "Heavy"),
            (Debuffs.Silence, "Silence"),
            (Debuffs.MiracleOfNature, "Miracle of Nature"),
        ];

        protected override uint Invoke(uint actionID)
        {
            if ((HasStatusEffect(Buffs.Guard) || JustUsed(Guard)) && IsEnabled(Preset.PvP_MashCancel))
            {
                if (actionID is Guard)
                    return Guard;

                return All.SavageBlade;
            }

            if (Execute() && InPvP() && IsOffCooldown(Purify) &&
                !CommonActions.Contains(actionID) && 
                LocalPlayer.CurrentMp >= Config.PurifyMPThreshold)
                return OriginalHook(Purify);

            return actionID;
        }

        public static bool Execute()
        {
            if (LocalPlayer is not { } player || player.IsDead || IsOnCooldown(Purify)) return false;

            bool[] selectedStatuses = Config.QuickPurifyStatuses;

            // Bail if nothing is enabled
            if (selectedStatuses.Length == 0) return false;

            // Make sure new statuses are supported
            Array.Resize(ref selectedStatuses, Statuses.Length);

            // Don't purify if under some buffs
            if (HasStatusEffect(DRGPvP.Buffs.SkyHigh) ||
                HasStatusEffect(VPRPvP.Buffs.HardenedScales) ||
                HasStatusEffect(Buffs.RidingMecha, anyOwner: true))
                return false;

            // Check if the status is present and one the user wants purified
            return selectedStatuses.Where((t, i) => t && HasStatusEffect(Statuses[i].debuff, anyOwner: true)).Any();
        }
    }
}