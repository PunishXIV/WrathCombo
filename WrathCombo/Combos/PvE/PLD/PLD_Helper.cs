using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Data;
using static WrathCombo.Combos.PvE.PLD.Config;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using PartyRequirement = WrathCombo.Combos.PvE.All.Enums.PartyRequirement;
namespace WrathCombo.Combos.PvE;

internal partial class PLD
{
    #region Variables

    private static PLDGauge Gauge => GetJobGauge<PLDGauge>();

    private static float DurationFightOrFlight =>
        GetStatusEffectRemainingTime(Buffs.FightOrFlight);

    private static float CooldownFightOrFlight =>
        GetCooldownRemainingTime(FightOrFlight);

    private static float CooldownRequiescat =>
        GetCooldownRemainingTime(Requiescat);

    private static bool CanFightOrFlight =>
        OriginalHook(FightOrFlight) is FightOrFlight && ActionReady(FightOrFlight);

    private static bool HasRequiescat =>
        HasStatusEffect(Buffs.Requiescat);

    private static bool HasDivineMight =>
        HasStatusEffect(Buffs.DivineMight);

    private static bool HasFightOrFlight =>
        HasStatusEffect(Buffs.FightOrFlight);

    private static bool HasDivineMagicMP =>
        LocalPlayer.CurrentMp >= GetResourceCost(HolySpirit);

    private static bool HasRequiescatMPSimple =>
        LocalPlayer.CurrentMp >= GetResourceCost(HolySpirit) * 3.6;

    private static bool HasRequiescatMPAdv =>
        IsNotEnabled(Preset.PLD_ST_AdvancedMode_MP_Reserve) && LocalPlayer.CurrentMp >= GetResourceCost(HolySpirit) * 3.6 ||
        IsEnabled(Preset.PLD_ST_AdvancedMode_MP_Reserve) && LocalPlayer.CurrentMp >= GetResourceCost(HolySpirit) * 3.6 + PLD_ST_MP_Reserve;

    private static bool HasRequiescatMPAdvAoE =>
        IsNotEnabled(Preset.PLD_AoE_AdvancedMode_MP_Reserve) && LocalPlayer.CurrentMp >= GetResourceCost(HolySpirit) * 3.6 ||
        IsEnabled(Preset.PLD_AoE_AdvancedMode_MP_Reserve) && LocalPlayer.CurrentMp >= GetResourceCost(HolySpirit) * 3.6 + PLD_AoE_MP_Reserve;

    private static bool InBurstWindow =>
        JustUsed(FightOrFlight, 30f);

    private static bool InAtonementStarter =>
        HasStatusEffect(Buffs.AtonementReady);

    private static bool InAtonementFinisher =>
        HasStatusEffect(Buffs.SepulchreReady);

    private static bool InAtonementPhase =>
        HasStatusEffect(Buffs.AtonementReady) || HasStatusEffect(Buffs.SupplicationReady) || HasStatusEffect(Buffs.SepulchreReady);

    private static bool IsDivineMightExpiring =>
        GetStatusEffectRemainingTime(Buffs.DivineMight) < 6;

    private static bool IsAtonementExpiring =>
        HasStatusEffect(Buffs.AtonementReady) && GetStatusEffectRemainingTime(Buffs.AtonementReady) < 6 ||
        HasStatusEffect(Buffs.SupplicationReady) && GetStatusEffectRemainingTime(Buffs.SupplicationReady) < 6 ||
        HasStatusEffect(Buffs.SepulchreReady) && GetStatusEffectRemainingTime(Buffs.SepulchreReady) < 6;

    private static bool JustMitted =>
        JustUsed(OriginalHook(Bulwark)) ||
        JustUsed(OriginalHook(Sentinel)) ||
        JustUsed(OriginalHook(Sheltron)) ||
        JustUsed(Role.ArmsLength) ||
        JustUsed(Role.Rampart) ||
        JustUsed(HallowedGround);

    private static bool IsAboveMPReserveAoE =>
        IsNotEnabled(Preset.PLD_AoE_AdvancedMode_MP_Reserve) ||
        IsEnabled(Preset.PLD_AoE_AdvancedMode_MP_Reserve) && LocalPlayer.CurrentMp >= GetResourceCost(HolySpirit) + PLD_AoE_MP_Reserve;

    private static bool IsAboveMPReserveST =>
        IsNotEnabled(Preset.PLD_ST_AdvancedMode_MP_Reserve) ||
        IsEnabled(Preset.PLD_ST_AdvancedMode_MP_Reserve) && LocalPlayer.CurrentMp >= GetResourceCost(HolySpirit) + PLD_ST_MP_Reserve;

    private static int HPThresholdFoF =>
        PLD_ST_FoF_BossOption == 1 ||
        !TargetIsBoss() ? PLD_ST_FoF_HPOption : 0;
    
    private static bool MitigationRunning =>
        HasStatusEffect(Role.Buffs.ArmsLength) ||
        HasStatusEffect(Role.Buffs.Rampart) || 
        HasStatusEffect(Buffs.HallowedGround) ||
        HasStatusEffect(Buffs.Bulwark) ||
        HasStatusEffect(Buffs.Sentinel) || 
        HasStatusEffect(Buffs.Guardian);

    private static int RoyalAuthorityCount =>
        ActionWatching.CombatActions.Count(x => x == OriginalHook(RageOfHalone));

    #endregion

    #region One Button Mitigation Priority

    /// <summary>
    ///     The list of Mitigations to use in the One-Button Mitigation combo.<br />
    ///     The order of the list needs to match the order in
    ///     <see cref="Preset" />.
    /// </summary>
    /// <value>
    ///     <c>Action</c> is the action to use.<br />
    ///     <c>Preset</c> is the preset to check if the action is enabled.<br />
    ///     <c>Logic</c> is the logic for whether to use the action.
    /// </value>
    /// <remarks>
    ///     Each logic check is already combined with checking if the preset
    ///     <see cref="IsEnabled(Preset)">is enabled</see>
    ///     and if the action is <see cref="ActionReady(uint,bool,bool)">ready</see> and
    ///     <see cref="LevelChecked(uint)">level-checked</see>.<br />
    ///     Do not add any of these checks to <c>Logic</c>.
    /// </remarks>
    private static (uint Action, Preset Preset, Func<bool> Logic)[]
        PrioritizedMitigation =>
    [
        //Sheltron
        (OriginalHook(Sheltron), Preset.PLD_Mit_Sheltron,
            () => Gauge.OathGauge >= 50),

        // Reprisal
        (Role.Reprisal, Preset.PLD_Mit_Reprisal,
            () => Role.CanReprisal()),

        //Divine Veil
        (DivineVeil, Preset.PLD_Mit_DivineVeil,
            () => PLD_Mit_DivineVeil_PartyRequirement ==
                  (int)PartyRequirement.No ||
                  IsInParty()),

        //Rampart
        (Role.Rampart, Preset.PLD_Mit_Rampart,
            () => Role.CanRampart()),

        //Bulwark
        (Bulwark, Preset.PLD_Mit_Bulwark,
            () => true),

        //Arm's Length
        (Role.ArmsLength, Preset.PLD_Mit_ArmsLength,
            () => Role.CanArmsLength(PLD_Mit_ArmsLength_EnemyCount,
                PLD_Mit_ArmsLength_Boss)),

        //Sentinel
        (OriginalHook(Sentinel), Preset.PLD_Mit_Sentinel,
            () => true),

        //Clemency
        (Clemency, Preset.PLD_Mit_Clemency,
            () => LocalPlayer.CurrentMp >= 2000 &&
                  PlayerHealthPercentageHp() <= PLD_Mit_Clemency_Health)
    ];

    /// <summary>
    ///     Given the index of a mitigation in <see cref="PrioritizedMitigation" />,
    ///     checks if the mitigation is ready and meets the provided requirements.
    /// </summary>
    /// <param name="index">
    ///     The index of the mitigation in <see cref="PrioritizedMitigation" />,
    ///     which is the order of the mitigation in <see cref="Preset" />.
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

    #region Auto Mitigation System
    
    [Flags]
    private enum RotationMode{
        simple = 1 << 0,
        advanced = 1 << 1
    }
    
    private static bool TryUseMits(RotationMode rotationFlags, ref uint actionID) => CanUseNonBossMits(rotationFlags, ref actionID) || CanUseBossMits(rotationFlags, ref actionID);
    
    private static bool CanUseNonBossMits(RotationMode rotationFlags, ref uint actionID)
    {
        #region Initial Bailout
        if (!InCombat() || !CanWeave() || InBossEncounter() || JustMitted || !IsEnabled(Preset.PLD_Mitigation_NonBoss))  
            return false;
        #endregion
        
        #region Sheltron Use Always
        if (IsEnabled(Preset.PLD_Mitigation_NonBoss_Sheltron) && ActionReady(OriginalHook(Sheltron)) && 
            !IsMoving() && CanWeave() && 
            !HasStatusEffect(Buffs.Sheltron) && !HasStatusEffect(Buffs.HallowedGround) && 
            Gauge.OathGauge >= 50)
        {
            actionID = OriginalHook(Sheltron);
            return true;
        }
        #endregion

        #region Mitigation Threshold / Weave Bailout
        float mitigationThreshold = rotationFlags.HasFlag(RotationMode.simple) 
            ? 10 
            : PLD_Mitigation_NonBoss_MitigationThreshold;
        
        if (GetAvgEnemyHPPercentInRange(5f) <= mitigationThreshold) return false;
        #endregion
        
        #region Divine Veil Health Checked
        var divineVeilThreshold = rotationFlags.HasFlag(RotationMode.simple) 
            ? 80 
            : PLD_Mitigation_NonBoss_DivineVeil_Health;
        
        if (IsEnabled(Preset.PLD_Mitigation_NonBoss_DivineVeil) && ActionReady(DivineVeil) && 
            PlayerHealthPercentageHp() <= divineVeilThreshold)
        {
            actionID = DivineVeil;
            return true;
        }
        #endregion
        
        var numberOfEnemies = NumberOfEnemiesInRange(Role.Reprisal);
        
        #region Reprisal 5+ Overlaps
        if (IsEnabled(Preset.PLD_Mitigation_NonBoss_Reprisal) && ActionReady(Role.Reprisal) &&
            numberOfEnemies >= 5 &&
            !HasStatusEffect(Buffs.HallowedGround))
        {
            actionID = Role.Reprisal;
            return true;
        }
        #endregion
        
        if (MitigationRunning || numberOfEnemies <= 2) return false; //Bail if already Mitted or too few enemies
        
        #region Mitigation 5+
        if (numberOfEnemies >= 5)
        {
            if (ActionReady(HallowedGround) && IsEnabled(Preset.PLD_Mitigation_NonBoss_HallowedGround))
            {
                actionID = HallowedGround;
                return true;
            }
            if (ActionReady(OriginalHook(Sentinel)) && IsEnabled(Preset.PLD_Mitigation_NonBoss_Sentinel))
            {
                actionID = OriginalHook(Sentinel);
                return true;
            }
            if (ActionReady(Role.ArmsLength) && IsEnabled(Preset.PLD_Mitigation_NonBoss_ArmsLength))
            {
                actionID = Role.ArmsLength;
                return true;
            }
        }
        #endregion
        
        #region Mitigation 3+
        if (Role.CanRampart() && IsEnabled(Preset.PLD_Mitigation_NonBoss_Rampart))
        {
            actionID = Role.Rampart;
            return true;
        }
        if (ActionReady(Bulwark) && IsEnabled(Preset.PLD_Mitigation_NonBoss_Bulwark))
        {
            actionID = Bulwark;
            return true;
        }
        #endregion
        
        return false;

        bool IsEnabled(Preset preset)
        {
            if (rotationFlags.HasFlag(RotationMode.simple))
                return true;
            
            return CustomComboFunctions.IsEnabled(preset);
        }
    }
    
    private static bool CanUseBossMits(RotationMode rotationFlags, ref uint actionID)
    {
        #region Initial Bailout
        if (!InCombat() || !CanWeave() || !InBossEncounter() || !IsEnabled(Preset.PLD_Mitigation_Boss)) return false;
        #endregion
        
        #region Sentinel
        var sentinelFirst = rotationFlags.HasFlag(RotationMode.simple)
            ? false
            : PLD_Mitigation_Boss_Sentinel_First;
        
        var sentinelInMitigationContent = rotationFlags.HasFlag(RotationMode.simple) || 
                                         ContentCheck.IsInConfiguredContent(PLD_Mitigation_Boss_Sentinel_Difficulty, PLD_Boss_Mit_DifficultyListSet);
        
        if (IsEnabled(Preset.PLD_Mitigation_Boss_Sentinel) && 
            ActionReady(OriginalHook(Sentinel)) && sentinelInMitigationContent && HasIncomingTankBusterEffect() && 
            !JustUsed(Role.Rampart, 20f) && // Prevent double big mits
            (!ActionReady(Role.Rampart) || sentinelFirst)) //Sentinel First or don't use unless rampart is on cd.
        {
            actionID = OriginalHook(Sentinel);
            return true;
        }
        #endregion
        
        #region Rampart
        var rampartInMitigationContent = rotationFlags.HasFlag(RotationMode.simple) || 
                                         ContentCheck.IsInConfiguredContent(PLD_Mitigation_Boss_Rampart_Difficulty, PLD_Boss_Mit_DifficultyListSet);
        
        if (IsEnabled(Preset.PLD_Mitigation_Boss_Rampart) && 
            ActionReady(Role.Rampart) && rampartInMitigationContent && HasIncomingTankBusterEffect() 
            && !JustUsed(OriginalHook(Sentinel), 15f)) // Prevent double big mits
        {
            actionID = Role.Rampart;
            return true;
        }
        #endregion
        
        #region Sheltron
        var sheltronThreshold = rotationFlags.HasFlag(RotationMode.simple)
            ? 95
            : PLD_Mitigation_Boss_SheltronOvercap_Threshold;
        
        var SheltronInMitigationContent = rotationFlags.HasFlag(RotationMode.simple) || 
                                         ContentCheck.IsInConfiguredContent(PLD_Mitigation_Boss_SheltronTankbuster_Difficulty, PLD_Boss_Mit_DifficultyListSet);

        bool sheltronOvercap = IsEnabled(Preset.PLD_Mitigation_Boss_SheltronOvercap) && Gauge.OathGauge >= sheltronThreshold && IsPlayerTargeted();
        bool sheltronTankbuster = IsEnabled(Preset.PLD_Mitigation_Boss_SheltronTankbuster) && Gauge.OathGauge >= 50 && HasIncomingTankBusterEffect() && SheltronInMitigationContent;
        
        if (ActionReady(OriginalHook(Sheltron)) &&  
            !HasStatusEffect(Buffs.Sheltron) &&
            (sheltronOvercap || sheltronTankbuster))
        {
            actionID = OriginalHook(Sheltron);
            return true;
        }
        #endregion
        
        #region Bulwark
        float emergencyBulwarkThreshold = rotationFlags.HasFlag(RotationMode.simple)
            ? 80
            : PLD_Mitigation_Boss_Bulwark_Threshold;
        
        var alignBulwark = rotationFlags.HasFlag(RotationMode.simple)
            ? true
            : PLD_Mitigation_Boss_Bulwark_Align;
        
        var BulwarkInMitigationContent = rotationFlags.HasFlag(RotationMode.simple) || 
                                         ContentCheck.IsInConfiguredContent(PLD_Mitigation_Boss_Bulwark_Difficulty, PLD_Boss_Mit_DifficultyListSet);

        bool emergencyBulwark = PlayerHealthPercentageHp() <= emergencyBulwarkThreshold;
        bool noOtherMitsToUse = !ActionReady(OriginalHook(Sentinel)) && !JustUsed(OriginalHook(Sentinel), 13f) &&
                           !ActionReady(Role.Rampart) && !JustUsed(Role.Rampart, 18f);
        bool alignBulwarkWithRampart = JustUsed(Role.Rampart, 20f) && alignBulwark;
        
        
        if (IsEnabled(Preset.PLD_Mitigation_Boss_Bulwark) && ActionReady(Bulwark) && HasIncomingTankBusterEffect() && BulwarkInMitigationContent &&
            (emergencyBulwark || noOtherMitsToUse || alignBulwarkWithRampart))
        {
            actionID = Bulwark;
            return true;
        }
        #endregion
        
        #region Reprisal
        
        var ReprisalInMitigationContent = rotationFlags.HasFlag(RotationMode.simple) ||
                                          ContentCheck.IsInConfiguredContent(PLD_Mitigation_Boss_Reprisal_Difficulty, PLD_Boss_Mit_DifficultyListSet);
        
        if (IsEnabled(Preset.PLD_Mitigation_Boss_Reprisal) && 
            Role.CanReprisal(enemyCount:1) && ReprisalInMitigationContent && GroupDamageIncoming() &&
            !JustUsed(DivineVeil, 10f))
        {
            actionID = Role.Reprisal;
            return true;
        }
        #endregion 
        
        #region Divine Veil
        var DivineVeilInMitigationContent = rotationFlags.HasFlag(RotationMode.simple) || 
                                            ContentCheck.IsInConfiguredContent(PLD_Mitigation_Boss_DivineVeil_Difficulty, PLD_Boss_Mit_DifficultyListSet);
        
        if (IsEnabled(Preset.PLD_Mitigation_Boss_DivineVeil) && 
            DivineVeilInMitigationContent && ActionReady(DivineVeil) && GroupDamageIncoming() &&
            !JustUsed(Role.Reprisal, 10f))
        {
            actionID = DivineVeil;
            return true;
        }
        #endregion
        
        return false;
        
        bool IsEnabled(Preset preset)
        {
            if (rotationFlags.HasFlag(RotationMode.simple))
                return true;
            
            return CustomComboFunctions.IsEnabled(preset);
        }
    }
    
    #endregion

    #region Openers

    internal static PLDLvl100StandardOpener Lvl100StandardOpener = new();

    internal static WrathOpener Opener()
    {
        if (Lvl100StandardOpener.LevelChecked)
            return Lvl100StandardOpener;

        return WrathOpener.Dummy;
    }

    internal class PLDLvl100StandardOpener : WrathOpener
    {
        public override int MinOpenerLevel => 100;
        public override int MaxOpenerLevel => 109;

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
            Intervene, //11
            BladeOfTruth,
            Intervene, //13
            BladeOfValor,
            BladeOfHonor,
            GoringBlade,
            Atonement,
            Supplication,
            Sepulchre,
            HolySpirit
        ];

        public override List<(int[] Steps, Func<bool> Condition)> SkipSteps { get; set; } =
        [
            ([11, 13], () => !HasCharges(Intervene))
        ];

        public override Preset Preset => Preset.PLD_ST_AdvancedMode_BalanceOpener;
        internal override UserData ContentCheckConfig => PLD_Balance_Content;

        public override bool HasCooldowns() =>
            IsOffCooldown(FightOrFlight) &&
            IsOffCooldown(Imperator) &&
            IsOffCooldown(CircleOfScorn) &&
            IsOffCooldown(Expiacion) &&
            GetRemainingCharges(Intervene) >= 2 &&
            IsOffCooldown(GoringBlade);
    }

    #endregion

    #region ID's

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
        Cover = 27,
        IronWill = 28,
        SpiritsWithin = 29,
        HallowedGround = 30,
        GoringBlade = 3538,
        DivineVeil = 3540,
        PassageOfArms = 7385,
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
        HolySheltron = 25746,
        Clemency = 3541;

    public static class Buffs
    {
        public const ushort
            IronWill = 79,
            HallowedGround = 82,
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
            PassageOfArms = 1175,
            Sheltron = 1856,
            Intervention = 2020,
            Guardian = 3829,
            Sentinel = 74,
            Bulwark = 77;
    }

    public static class Debuffs
    {
        public const ushort
            BladeOfValor = 2721,
            GoringBlade = 725;
    }

    #endregion
}
