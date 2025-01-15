using FFXIVClientStructs.FFXIV.Client.Game;
using System.Collections.Generic;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Combos.PvE;

internal partial class GNB
{
    public static GNBOpenerMaxLevel1 Opener1 = new();
    public static GNBOpenerMaxLevel2 Opener2 = new();

    public static WrathOpener Opener()
    {
        var gcd = ActionManager.GetAdjustedRecastTime(ActionType.Action, KeenEdge) / 1000f;

        if (gcd <= 2.47f && Opener1.LevelChecked)
            return Opener1;

        if (Opener2.LevelChecked)
            return Opener2;

        return WrathOpener.Dummy;
    }

    #region Mitigation Priority

    /// <summary>
    ///     The list of Mitigations to use in the One-Button Mitigation combo.<br />
    ///     The order of the list needs to match the order in
    ///     <see cref="CustomComboPreset" />.
    /// </summary>
    /// <value>
    ///     <c>Action</c> is the action to use.<br />
    ///     <c>Preset</c> is the preset to check if the action is enabled.<br />
    ///     <c>Logic</c> is the logic for whether to use the action.
    /// </value>
    /// <remarks>
    ///     Each logic check is already combined with checking if the preset
    ///     <see cref="IsEnabled(uint)">is enabled</see>
    ///     and if the action is <see cref="ActionReady(uint)">ready</see> and
    ///     <see cref="LevelChecked(uint)">level-checked</see>.<br />
    ///     Do not add any of these checks to <c>Logic</c>.
    /// </remarks>
    private static (uint Action, CustomComboPreset Preset, System.Func<bool> Logic)[]
        PrioritizedMitigation =>
    [
        //Heart of Corundum
        (OriginalHook(HeartOfStone), CustomComboPreset.GNB_Mit_Corundum,
            () => FindEffect(Buffs.HeartOfCorundum) is null &&
                  FindEffect(Buffs.HeartOfStone) is null &&
                  PlayerHealthPercentageHp() <= Config.GNB_Mit_Corundum_Health),
        //Aurora
        (Aurora, CustomComboPreset.GNB_Mit_Aurora,
            () => (!((HasFriendlyTarget() && TargetHasEffectAny(Buffs.Aurora)) ||
                     (!HasFriendlyTarget() && HasEffectAny(Buffs.Aurora)))) &&
                  GetRemainingCharges(Aurora) > Config.GNB_Mit_Aurora_Charges &&
                  PlayerHealthPercentageHp() <= Config.GNB_Mit_Aurora_Health),
        //Camouflage
        (Camouflage, CustomComboPreset.GNB_Mit_Camouflage, () => true),
        // Reprisal
        (All.Reprisal, CustomComboPreset.GNB_Mit_Reprisal,
            () => InActionRange(All.Reprisal)),
        //Heart of Light
        (HeartOfLight, CustomComboPreset.GNB_Mit_HeartOfLight,
            () => Config.GNB_Mit_HeartOfLight_PartyRequirement ==
                  (int)Config.PartyRequirement.No ||
                  IsInParty()),
        //Rampart
        (All.Rampart, CustomComboPreset.GNB_Mit_Rampart,
            () => PlayerHealthPercentageHp() <= Config.GNB_Mit_Rampart_Health),
        //Arm's Length
        (All.ArmsLength, CustomComboPreset.GNB_Mit_ArmsLength,
            () => CanCircleAoe(7) >= Config.GNB_Mit_ArmsLength_EnemyCount &&
                  (Config.GNB_Mit_ArmsLength_Boss == (int)Config.BossAvoidance.Off ||
                   InBossEncounter())),
        //Nebula
        (OriginalHook(Nebula), CustomComboPreset.GNB_Mit_Nebula,
            () => PlayerHealthPercentageHp() <= Config.GNB_Mit_Nebula_Health),
    ];

    /// <summary>
    ///     Given the index of a mitigation in <see cref="PrioritizedMitigation" />,
    ///     checks if the mitigation is ready and meets the provided requirements.
    /// </summary>
    /// <param name="index">
    ///     The index of the mitigation in <see cref="PrioritizedMitigation" />,
    ///     which is the order of the mitigation in <see cref="CustomComboPreset" />.
    /// </param>
    /// <param name="action">
    ///     The variable to set to the action to, if the mitigation is set to be
    ///     used.
    /// </param>
    /// <returns>
    ///     Whether the mitigation is ready, enabled, and passes the provided logic
    ///     check.
    /// </returns>
    private static bool CheckMitigationConfigMeetsRequirements
        (int index, out uint action)
    {
        action = PrioritizedMitigation[index].Action;
        return ActionReady(action) && LevelChecked(action) &&
               PrioritizedMitigation[index].Logic() &&
               IsEnabled(PrioritizedMitigation[index].Preset);
    }

    #endregion

    internal class GNBOpenerMaxLevel1 : WrathOpener
    {
        //2.47 GCD or lower
        public override List<uint> OpenerActions { get; set; } =
        [
            LightningShot,
            Bloodfest,
            KeenEdge,
            BrutalShell,
            NoMercy,
            GnashingFang,
            JugularRip,
            BowShock,
            DoubleDown,
            BlastingZone,
            SavageClaw,
            AbdomenTear,
            WickedTalon,
            EyeGouge,
            ReignOfBeasts,
            NobleBlood,
            LionHeart,
            BurstStrike,
            Hypervelocity,
            SonicBreak

        ];
        public override int MinOpenerLevel => 100;
        public override int MaxOpenerLevel => 109;

        public override List<int> DelayedWeaveSteps { get; set; } =
        [
            2,
            5,
        ];
        internal override UserData? ContentCheckConfig => Config.GNB_ST_Balance_Content;

        public override bool HasCooldowns()
        {
            if (!IsOffCooldown(Bloodfest))
                return false;

            if (!IsOffCooldown(NoMercy))
                return false;

            if (!IsOffCooldown(Hypervelocity))
                return false;

            if (!IsOffCooldown(SonicBreak))
                return false;

            if (!IsOffCooldown(DoubleDown))
                return false;

            if (!IsOffCooldown(BowShock))
                return false;

            return true;
        }
    }

    internal class GNBOpenerMaxLevel2 : WrathOpener
    {
        //Above 2.47 GCD
        public override List<uint> OpenerActions { get; set; } =
        [
            LightningShot,
            Bloodfest,
            KeenEdge,
            BurstStrike,
            NoMercy,
            Hypervelocity,
            GnashingFang,
            JugularRip,
            BowShock,
            DoubleDown,
            BlastingZone,
            SonicBreak,
            SavageClaw,
            AbdomenTear,
            WickedTalon,
            EyeGouge,
            ReignOfBeasts,
            NobleBlood,
            LionHeart

        ];
        public override int MinOpenerLevel => 100;
        public override int MaxOpenerLevel => 109;

        public override List<int> DelayedWeaveSteps { get; set; } =
        [
            2,
        ];

        internal override UserData? ContentCheckConfig => Config.GNB_ST_Balance_Content;
        public override bool HasCooldowns()
        {
            if (!IsOffCooldown(Bloodfest))
                return false;

            if (!IsOffCooldown(NoMercy))
                return false;

            if (!IsOffCooldown(Hypervelocity))
                return false;

            if (!IsOffCooldown(SonicBreak))
                return false;

            if (!IsOffCooldown(DoubleDown))
                return false;

            if (!IsOffCooldown(BowShock))
                return false;

            return true;
        }
    }
}

