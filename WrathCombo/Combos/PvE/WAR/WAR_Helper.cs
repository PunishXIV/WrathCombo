using System.Collections.Generic;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Combos.PvE;

internal partial class WAR
{
    internal static WAROpenerMaxLevel1 Opener1 = new();
    internal static WrathOpener Opener()
    {
        if (Opener1.LevelChecked)
            return Opener1;
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
        //Bloodwhetting
        (OriginalHook(RawIntuition), CustomComboPreset.WAR_Mit_Bloodwhetting,
            () => FindEffect(Buffs.RawIntuition) is null &&
                  FindEffect(Buffs.BloodwhettingDefenseLong) is null &&
                  PlayerHealthPercentageHp() <= Config.WAR_Mit_Bloodwhetting_Health),
        //Equilibrium
        (Equilibrium, CustomComboPreset.WAR_Mit_Equilibrium,
            () => PlayerHealthPercentageHp() <= Config.WAR_Mit_Equilibrium_Health),
        // Reprisal
        (All.Reprisal, CustomComboPreset.WAR_Mit_Reprisal,
            () => InActionRange(All.Reprisal)),
        //Thrill of Battle
        (ThrillOfBattle, CustomComboPreset.WAR_Mit_ThrillOfBattle,
            () => PlayerHealthPercentageHp() <= Config.WAR_Mit_ThrillOfBattle_Health),
        //Rampart
        (All.Rampart, CustomComboPreset.WAR_Mit_Rampart,
            () => PlayerHealthPercentageHp() <= Config.WAR_Mit_Rampart_Health),
        //Shake it Off
        (ShakeItOff, CustomComboPreset.WAR_Mit_ShakeItOff,
            () => FindEffect(Buffs.ShakeItOff) is null &&
                  Config.WAR_Mit_ShakeItOff_PartyRequirement ==
                  (int)Config.PartyRequirement.No ||
                  IsInParty()),
        //Arm's Length
        (All.ArmsLength, CustomComboPreset.WAR_Mit_ArmsLength,
            () => CanCircleAoe(7) >= Config.WAR_Mit_ArmsLength_EnemyCount &&
                  (Config.WAR_Mit_ArmsLength_Boss == (int)Config.BossAvoidance.Off ||
                   InBossEncounter())),
        //Vengeance
        (OriginalHook(Vengeance), CustomComboPreset.WAR_Mit_Vengeance,
            () => PlayerHealthPercentageHp() <= Config.WAR_Mit_Vengeance_Health),
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

    internal class WAROpenerMaxLevel1 : WrathOpener
    {
        public override List<uint> OpenerActions { get; set; } =
        [
            Tomahawk,
           Infuriate,
            HeavySwing,
            Maim,
            StormsEye,
            InnerRelease,
            InnerChaos,
            Upheaval,
            Onslaught,
            PrimalRend,
            Onslaught,
            PrimalRuination,
            Onslaught,
            FellCleave,
            FellCleave,
            FellCleave,
            PrimalWrath,
            Infuriate,
            InnerChaos,
            HeavySwing,
            Maim,
            StormsPath,
            FellCleave,
            Infuriate,
            InnerChaos
        ];

        public override int MinOpenerLevel => 100;
        public override int MaxOpenerLevel => 109;

        internal override UserData? ContentCheckConfig => Config.WAR_BalanceOpener_Content;

        public override bool HasCooldowns()
        {
            if (!IsOffCooldown(InnerRelease))
                return false;
            if (!IsOffCooldown(Upheaval))
                return false;
            if (GetRemainingCharges(Infuriate) < 2)
                return false;
            if (GetRemainingCharges(Onslaught) < 3)
                return false;

            return true;
        }
    }
}
