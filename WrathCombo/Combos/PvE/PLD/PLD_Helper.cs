using System.Collections.Generic;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Data;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Combos.PvE;

internal partial class PLD
{
    #region ID's

    public const byte ClassID = 1;
    public const byte JobID = 19;

    public const float CooldownThreshold = 0.5f;

    public const uint
        FastBlade = 9,
        RiotBlade = 15,
        ShieldBash = 16,
        Sentinel = 17,
        RageOfHalone = 21,
        Bulwark = 22,
        CircleOfScorn = 23,
        ShieldLob = 24,
        SpiritsWithin = 29,
        HallowedGround = 30,
        GoringBlade = 3538,
        DivineVeil = 3540,
        RoyalAuthority = 3539,
        Guardian = 36920,
        TotalEclipse = 7381,
        Intervention = 7382,
        Requiescat = 7383,
        Imperator = 36921,
        HolySpirit = 7384,
        Prominence = 16457,
        HolyCircle = 16458,
        Confiteor = 16459,
        Expiacion = 25747,
        BladeOfFaith = 25748,
        BladeOfTruth = 25749,
        BladeOfValor = 25750,
        FightOrFlight = 20,
        Atonement = 16460,
        Supplication = 36918, // Second Atonement
        Sepulchre = 36919, // Third Atonement
        Intervene = 16461,
        BladeOfHonor = 36922,
        Sheltron = 3542,
        Clemency = 3541;

    public static class Buffs
    {
        public const ushort
            IronWill = 79,
            Requiescat = 1368,
            AtonementReady = 1902, // First Atonement Buff
            SupplicationReady = 3827, // Second Atonement Buff
            SepulchreReady = 3828, // Third Atonement Buff
            GoringBladeReady = 3847,
            BladeOfHonor = 3831,
            FightOrFlight = 76,
            ConfiteorReady = 3019,
            DivineMight = 2673,
            HolySheltron = 2674,
            Sheltron = 1856;
    }

    public static class Debuffs
    {
        public const ushort
            BladeOfValor = 2721,
            GoringBlade = 725;
    }

    #endregion

    internal static PLDOpenerMaxLevel1 Opener1 = new();
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
        //Sheltron
        (OriginalHook(Sheltron), CustomComboPreset.PLD_Mit_Sheltron,
            () => Gauge.OathGauge >= 50),
        // Reprisal
        (All.Reprisal, CustomComboPreset.PLD_Mit_Reprisal,
            () => InActionRange(All.Reprisal)),
        //Divine Veil
        (DivineVeil, CustomComboPreset.PLD_Mit_DivineVeil,
            () => Config.PLD_Mit_DivineVeil_PartyRequirement ==
                  (int)Config.PartyRequirement.No ||
                  IsInParty()),
        //Rampart
        (All.Rampart, CustomComboPreset.PLD_Mit_Rampart,
            () => PlayerHealthPercentageHp() <= Config.PLD_Mit_Rampart_Health),
        //Sentinel
        (OriginalHook(Sentinel), CustomComboPreset.PLD_Mit_Sentinel,
            () => PlayerHealthPercentageHp() <= Config.PLD_Mit_Sentinel_Health),
        //Arm's Length
        (All.ArmsLength, CustomComboPreset.PLD_Mit_ArmsLength,
            () => CanCircleAoe(7) >= Config.PLD_Mit_ArmsLength_EnemyCount &&
                  (Config.PLD_Mit_ArmsLength_Boss == (int)Config.BossAvoidance.Off ||
                   InBossEncounter())),
        //Bulwark
        (Bulwark, CustomComboPreset.PLD_Mit_Bulwark,
            () => PlayerHealthPercentageHp() <= Config.PLD_Mit_Bulwark_Health),
        //Hallowed Ground
        (HallowedGround, CustomComboPreset.PLD_Mit_HallowedGround,
            () => PlayerHealthPercentageHp() <= Config.PLD_Mit_HallowedGround_Health &&
                  ContentCheck.IsInConfiguredContent(
                      Config.PLD_Mit_HallowedGround_Difficulty,
                      Config.PLD_Mit_HallowedGround_DifficultyListSet
                  )),
        //Clemency
        (Clemency, CustomComboPreset.PLD_Mit_Clemency,
            () => LocalPlayer.CurrentMp >= 2000 &&
                  PlayerHealthPercentageHp() <= Config.PLD_Mit_Clemency_Health),
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

    internal class PLDOpenerMaxLevel1 : WrathOpener
    {
        public override List<uint> OpenerActions { get; set; } =
        [
            HolySpirit,
            FastBlade,
            RiotBlade,
            RoyalAuthority,
            FightOrFlight,
            Imperator,
            Confiteor,
            CircleOfScorn,
            Expiacion,
            BladeOfFaith,
            Intervene,
            BladeOfTruth,
            Intervene,
            BladeOfValor,
            BladeOfHonor,
            GoringBlade,
            Atonement,
            Supplication,
            Sepulchre,
            HolySpirit
        ];

        public override int MinOpenerLevel => 100;
        public override int MaxOpenerLevel => 109;

        internal override UserData? ContentCheckConfig => Config.PLD_Balance_Content;

        public override bool HasCooldowns()
        {
            if (!IsOffCooldown(FightOrFlight)) return false;
            if (!IsOffCooldown(Imperator)) return false;
            if (!IsOffCooldown(CircleOfScorn)) return false;
            if (!IsOffCooldown(Expiacion)) return false;
            if (GetRemainingCharges(Intervene) < 2) return false;
            if (!IsOffCooldown(GoringBlade)) return false;

            return true;
        }
    }
}
