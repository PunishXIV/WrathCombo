using System.Collections.Generic;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Combos.PvE;

internal partial class WAR
{
    #region ID's

    public const byte ClassID = 3; //Marauder (MRD) 
    public const byte JobID = 21; //Warrior (WAR)
    public const uint //Actions

    #region Offensive
    HeavySwing = 31, //Lv1, instant, GCD, range 3, single-target, targets=Hostile
    Maim = 37, //Lv4, instant, GCD, range 3, single-target, targets=Hostile
    Berserk = 38, //Lv6, instant, 60.0s CD (group 10), range 0, single-target, targets=Self
    Overpower = 41, //Lv10, instant, GCD, range 0, AOE 5 circle, targets=Self
    Tomahawk = 46, //Lv15, instant, GCD, range 20, single-target, targets=Hostile
    StormsPath = 42, //Lv26, instant, GCD, range 3, single-target, targets=Hostile
    InnerBeast = 49, //Lv35, instant, GCD, range 3, single-target, targets=Hostile
    MythrilTempest = 16462, //Lv40, instant, GCD, range 0, AOE 5 circle, targets=Self
    SteelCyclone = 51, //Lv45, instant, GCD, range 0, AOE 5 circle, targets=Self
    StormsEye = 45, //Lv50, instant, GCD, range 3, single-target, targets=Hostile
    Infuriate = 52, //Lv50, instant, 60.0s CD (group 19/70) (2 charges), range 0, single-target, targets=Self
    FellCleave = 3549, //Lv54, instant, GCD, range 3, single-target, targets=Hostile
    Decimate = 3550, //Lv60, instant, GCD, range 0, AOE 5 circle, targets=Self
    Onslaught = 7386, //Lv62, instant, 30.0s CD (group 7/71) (2-3 charges), range 20, single-target, targets=Hostile
    Upheaval = 7387, //Lv64, instant, 30.0s CD (group 8), range 3, single-target, targets=Hostile
    InnerRelease = 7389, //Lv70, instant, 60.0s CD (group 11), range 0, single-target, targets=Self
    ChaoticCyclone = 16463, //Lv72, instant, GCD, range 0, AOE 5 circle, targets=Self
    InnerChaos = 16465, //Lv80, instant, GCD, range 3, single-target, targets=Hostile
    Orogeny = 25752, //Lv86, instant, 30.0s CD (group 8), range 0, AOE 5 circle, targets=Self
    PrimalRend = 25753, //Lv90, instant, GCD, range 20, AOE 5 circle, targets=Hostile, animLock=1.150
    PrimalWrath = 36924, //Lv96, instant, 1.0s CD (group 0), range 0, AOE 5 circle, targets=Self
    PrimalRuination = 36925, //Lv100, instant, GCD, range 3, AOE 5 circle, targets=Hostile
    #endregion

    #region Defensive
    Defiance = 48, //Lv10, instant, 2.0s CD (group 1), range 0, single-target, targets=Self
    ReleaseDefiance = 32066, //Lv10, instant, 1.0s CD (group 1), range 0, single-target, targets=Self
    ThrillOfBattle = 40, //Lv30, instant, 90.0s CD (group 15), range 0, single-target, targets=Self
    Vengeance = 44, //Lv38, instant, 120.0s CD (group 21), range 0, single-target, targets=Self
    Holmgang = 43, //Lv42, instant, 240.0s CD (group 24), range 6, single-target, targets=Self/Hostile
    RawIntuition = 3551, //Lv56, instant, 25.0s CD (group 6), range 0, single-target, targets=Self
    Equilibrium = 3552, //Lv58, instant, 60.0s CD (group 13), range 0, single-target, targets=Self
    ShakeItOff = 7388, //Lv68, instant, 90.0s CD (group 14), range 0, AOE 30 circle, targets=Self
    NascentFlash = 16464, //Lv76, instant, 25.0s CD (group 6), range 30, single-target, targets=Party
    Bloodwhetting = 25751, //Lv82, instant, 25.0s CD (group 6), range 0, single-target, targets=Self
    Damnation = 36923, //Lv92, instant, 120.0s CD (group 21), range 0, single-target, targets=Self
    #endregion

    //Limit Break
    LandWaker = 4240; //LB3, instant, range 0, AOE 50 circle, targets=Self, animLock=3.860

    public static class Buffs
    {
        public const ushort
        #region Offensive
            SurgingTempest = 2677, //applied by Storm's Eye, Mythril Tempest to self, damage buff
            NascentChaos = 1897, //applied by Infuriate to self, converts next FC to IC
            Berserk = 86, //applied by Berserk to self, next 3 GCDs are crit dhit
            InnerReleaseStacks = 1177, //applied by Inner Release to self, next 3 GCDs should be free FCs
            InnerReleaseBuff = 1303, //applied by Inner Release to self, 15s buff
            PrimalRendReady = 2624, //applied by Inner Release to self, allows casting PR
            InnerStrength = 2663, //applied by Inner Release to self, immunes
            BurgeoningFury = 3833, //applied by Fell Cleave to self, 3 stacks turns into wrathful
            Wrathful = 3901, //3rd stack of Burgeoning Fury turns into this, allows Primal Wrath
            PrimalRuinationReady = 3834, //applied by Primal Rend to self
        #endregion

        #region Defensive
            VengeanceRetaliation = 89, //applied by Vengeance to self, retaliation for physical attacks
            VengeanceDefense = 912, //applied by Vengeance to self, -30% damage taken
            Damnation = 3832, //applied by Damnation to self, -40% damage taken and retaliation for physical attacks
            PrimevalImpulse = 3900, //hot applied after hit under Damnation
            ThrillOfBattle = 87, //applied by Thrill of Battle to self
            Holmgang = 409, //applied by Holmgang to self
            EquilibriumRegen = 2681, //applied by Equilibrium to self, hp regen
            ShakeItOff = 1457, //applied by Shake It Off to self/target, damage shield
            ShakeItOffHOT = 2108, //applied by Shake It Off to self/target
            RawIntuition = 735, //applied by Raw Intuition to self
            NascentFlashSelf = 1857, //applied by Nascent Flash to self, heal on hit
            NascentFlashTarget = 1858, //applied by Nascent Flash to target, -10% damage taken + heal on hit
            BloodwhettingDefenseLong = 2678, //applied by Bloodwhetting to self, -10% damage taken + heal on hit for 8 sec
            BloodwhettingDefenseShort = 2679, //applied by Bloodwhetting, Nascent Flash to self/target, -10% damage taken for 4 sec
            BloodwhettingShield = 2680, //applied by Bloodwhetting, Nascent Flash to self/target, damage shield
            ArmsLength = 1209, //applied by Arm's Length to self
            Defiance = 91, //applied by Defiance to self, tank stance
            ShieldWall = 194, //applied by Shield Wall to self/target
            Stronghold = 195, //applied by Stronghold to self/target
            LandWaker = 863; //applied by Land Waker to self/target
        #endregion
    }

    public static class Debuffs
    {
        public const ushort
            Placeholder = 1;
    }

    #endregion

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
            () => (FindEffect(Buffs.ShakeItOff) is null &&
                  Config.WAR_Mit_ShakeItOff_PartyRequirement ==
                  (int)Config.PartyRequirement.No) ||
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
            if (!ActionReady(InnerRelease))
                return false;

            if (!ActionReady(Upheaval))
                return false;

            if (GetRemainingCharges(Infuriate) < 2)
                return false;

            if (GetRemainingCharges(Onslaught) < 3)
                return false;

            return true;
        }
    }
}
