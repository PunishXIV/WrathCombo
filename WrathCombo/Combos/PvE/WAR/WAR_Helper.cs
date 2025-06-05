#region Dependencies
using Dalamud.Game.ClientState.JobGauge.Types;
using System.Collections.Generic;
using WrathCombo.Combos.PvE.Content;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using PartyRequirement = WrathCombo.Combos.PvE.All.Enums.PartyRequirement;
#endregion

namespace WrathCombo.Combos.PvE;

internal partial class WAR : Tank
{
    #region Variables
    internal static WARGauge Gauge = GetJobGauge<WARGauge>(); //WAR gauge
    internal static int BeastGauge => Gauge.BeastGauge;
    internal static int MaxDashCharges => TraitLevelChecked(Traits.EnhancedOnslaught) ? 3 : LevelChecked(Onslaught) ? 2 : 0;
    internal static bool CanInfuriate(int gauge = 50, int charges = 0) => InCombat() && ActionReady(Infuriate) && !HasNC && GetRemainingCharges(Infuriate) > charges && BeastGauge <= gauge;
    internal static bool CanOnslaught(int charges = 0, float distance = 20, bool movement = true) => ActionReady(Onslaught) && GetRemainingCharges(Onslaught) > charges && GetTargetDistance() <= distance && movement;
    internal static bool CanPRend(float distance = 20, bool movement = true) => LevelChecked(PrimalRend) && HasStatusEffect(Buffs.PrimalRendReady) && GetTargetDistance() <= distance && movement;
    internal static bool CanFC(int gauge = 50) => LevelChecked(OriginalHook(InnerBeast)) && (HasIR.Stacks || (BeastGauge >= 50 && ((HasNC && BeastGauge >= gauge && LevelChecked(InnerChaos)) || IR.Cooldown is < 1f or > 40f)) || BeastGauge >= gauge);
    internal static (float Cooldown, float Status, int Stacks) IR => (GetCooldownRemainingTime(OriginalHook(Berserk)), GetStatusEffectRemainingTime(Buffs.InnerReleaseBuff), GetStatusEffectStacks(Buffs.InnerReleaseStacks));
    internal static (float Status, int Stacks) BF => (GetStatusEffectRemainingTime(Buffs.BurgeoningFury), GetStatusEffectStacks(Buffs.BurgeoningFury));
    internal static (bool Status, bool Stacks) HasIR => (IR.Status > 0, IR.Stacks > 0 || HasStatusEffect(Buffs.InnerReleaseStacks));
    internal static (bool Status, bool Stacks) HasBF => (BF.Status > 0 || HasStatusEffect(Buffs.BurgeoningFury), (BF.Stacks > 0 || HasStatusEffect(Buffs.BurgeoningFury)));
    internal static bool HasST => !LevelChecked(StormsEye) || HasStatusEffect(Buffs.SurgingTempest);
    internal static bool HasNC => HasStatusEffect(Buffs.NascentChaos);
    internal static bool HasWrath => HasStatusEffect(Buffs.Wrathful);
    internal static bool Minimal => InCombat() && HasBattleTarget();
    internal static bool MitUsed => JustUsed(OriginalHook(RawIntuition), 4f) || JustUsed(OriginalHook(Vengeance), 5f) || JustUsed(ThrillOfBattle, 5f) || JustUsed(Role.Rampart, 5f) || JustUsed(Holmgang, 9f);
    #endregion

    #region Openers
    //TODO: add some stuff similar to GNB
    internal static WAROpenerMaxLevel1 Opener1 = new();
    internal static WrathOpener Opener()
    {
        if (Opener1.LevelChecked)
            return Opener1;
        
        return WrathOpener.Dummy;
    }

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
            FellCleave,
            Onslaught,
            FellCleave,
            Onslaught,
            FellCleave,
            PrimalWrath,
            Infuriate,
            PrimalRend,
            PrimalRuination,
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
        internal override UserData ContentCheckConfig => Config.WAR_BalanceOpener_Content;
        public override bool HasCooldowns() => IsOffCooldown(InnerRelease) && IsOffCooldown(Upheaval) && GetRemainingCharges(Infuriate) >= 2 && GetRemainingCharges(Onslaught) >= 3;
    }

    #endregion

    #region Helpers
    internal static uint GetVariantAction()
    {
        if (Variant.CanCure(CustomComboPreset.WAR_Variant_Cure, Config.WAR_VariantCure))
            return Variant.Cure;
        if (Variant.CanSpiritDart(CustomComboPreset.WAR_Variant_SpiritDart) && CanWeave())
            return Variant.SpiritDart;
        if (Variant.CanUltimatum(CustomComboPreset.WAR_Variant_Ultimatum) && CanWeave())
            return Variant.Ultimatum;

        return 0; //No conditions met
    }
    internal static uint GetBozjaAction()
    {
        if (!Bozja.IsInBozja)
            return 0;

        bool CanUse(uint action) => HasActionEquipped(action) && IsOffCooldown(action);
        bool IsEnabledAndUsable(CustomComboPreset preset, uint action) => IsEnabled(preset) && CanUse(action);

        //Out-of-Combat
        if (!InCombat() && IsEnabledAndUsable(CustomComboPreset.WAR_Bozja_LostStealth, Bozja.LostStealth))
            return Bozja.LostStealth;
        //OGCDs
        if (CanWeave())
        {
            foreach (var (preset, action) in new[]
            {
            (CustomComboPreset.WAR_Bozja_LostFocus, Bozja.LostFocus),
            (CustomComboPreset.WAR_Bozja_LostFontOfPower, Bozja.LostFontOfPower),
            (CustomComboPreset.WAR_Bozja_LostSlash, Bozja.LostSlash),
            (CustomComboPreset.WAR_Bozja_LostFairTrade, Bozja.LostFairTrade),
            (CustomComboPreset.WAR_Bozja_LostAssassination, Bozja.LostAssassination),
        })
                if (IsEnabledAndUsable(preset, action))
                    return action;

            foreach (var (preset, action, powerPreset) in new[]
            {
            (CustomComboPreset.WAR_Bozja_BannerOfNobleEnds, Bozja.BannerOfNobleEnds, CustomComboPreset.WAR_Bozja_PowerEnds),
            (CustomComboPreset.WAR_Bozja_BannerOfHonoredSacrifice, Bozja.BannerOfHonoredSacrifice, CustomComboPreset.WAR_Bozja_PowerSacrifice)
        })
                if (IsEnabledAndUsable(preset, action) && (!IsEnabled(powerPreset) || JustUsed(Bozja.LostFontOfPower, 5f)))
                    return action;

            if (IsEnabledAndUsable(CustomComboPreset.WAR_Bozja_BannerOfHonedAcuity, Bozja.BannerOfHonedAcuity) &&
                !HasStatusEffect(Bozja.Buffs.BannerOfTranscendentFinesse))
                return Bozja.BannerOfHonedAcuity;
        }
        //GCDs
        foreach (var (preset, action, condition) in new[]
        {
        (CustomComboPreset.WAR_Bozja_LostDeath, Bozja.LostDeath, true),
        (CustomComboPreset.WAR_Bozja_LostCure, Bozja.LostCure, PlayerHealthPercentageHp() <= Config.WAR_Bozja_LostCure_Health),
        (CustomComboPreset.WAR_Bozja_LostArise, Bozja.LostArise, GetTargetHPPercent() == 0 && !HasStatusEffect(RoleActions.Magic.Buffs.Raise)),
        (CustomComboPreset.WAR_Bozja_LostReraise, Bozja.LostReraise, PlayerHealthPercentageHp() <= Config.WAR_Bozja_LostReraise_Health),
        (CustomComboPreset.WAR_Bozja_LostProtect, Bozja.LostProtect, !HasStatusEffect(Bozja.Buffs.LostProtect)),
        (CustomComboPreset.WAR_Bozja_LostShell, Bozja.LostShell, !HasStatusEffect(Bozja.Buffs.LostShell)),
        (CustomComboPreset.WAR_Bozja_LostBravery, Bozja.LostBravery, !HasStatusEffect(Bozja.Buffs.LostBravery)),
        (CustomComboPreset.WAR_Bozja_LostBubble, Bozja.LostBubble, !HasStatusEffect(Bozja.Buffs.LostBubble)),
        (CustomComboPreset.WAR_Bozja_LostParalyze3, Bozja.LostParalyze3, !JustUsed(Bozja.LostParalyze3, 60f))
        })
            if (IsEnabledAndUsable(preset, action) && condition)
                return action;
        if (IsEnabled(CustomComboPreset.WAR_Bozja_LostSpellforge) &&
            CanUse(Bozja.LostSpellforge) &&
            (!HasStatusEffect(Bozja.Buffs.LostSpellforge) || !HasStatusEffect(Bozja.Buffs.LostSteelsting)))
            return Bozja.LostSpellforge;
        if (IsEnabled(CustomComboPreset.WAR_Bozja_LostSteelsting) &&
            CanUse(Bozja.LostSteelsting) &&
            (!HasStatusEffect(Bozja.Buffs.LostSpellforge) || !HasStatusEffect(Bozja.Buffs.LostSteelsting)))
            return Bozja.LostSteelsting;

        return 0; //No conditions met
    }
    internal static uint OtherAction
    {
        get
        {
            if (GetVariantAction() is uint va && va != 0)
                return va;
            if (Bozja.IsInBozja && GetBozjaAction() is uint ba && ba != 0)
                return ba;
            return 0;
        }
    }
    internal static bool ShouldUseOther => OtherAction != 0;
    #endregion

    #region Rotation
    internal static bool ShouldUseInnerRelease(int targetHP = 0) => ActionReady(OriginalHook(Berserk)) && !HasWrath && CanWeave() && (HasST || !LevelChecked(StormsEye)) && Minimal && GetTargetHPPercent() >= targetHP;
    internal static bool ShouldUseInfuriate(int gauge = 40, int charges = 0) => CanInfuriate() && CanWeave() && !HasNC && !HasIR.Stacks && BeastGauge <= gauge && GetRemainingCharges(Infuriate) > charges && Minimal;
    internal static bool ShouldUseUpheaval => ActionReady(Upheaval) && CanWeave() && HasST && InMeleeRange() && Minimal;
    internal static bool ShouldUsePrimalWrath => LevelChecked(PrimalWrath) && CanWeave() && HasWrath && HasST && GetTargetDistance() <= 4.99f && Minimal;
    internal static bool ShouldUseOnslaught(int charges = 0, float distance = 20, bool movement = true) => CanOnslaught(charges, distance, movement) && CanWeave() && HasST && (IR.Cooldown > 40 || GetRemainingCharges(Onslaught) == MaxDashCharges);
    internal static bool ShouldUsePrimalRuination => LevelChecked(PrimalRuination) && HasStatusEffect(Buffs.PrimalRuinationReady) && HasST;
    internal static bool ShouldUsePrimalRend(float distance = 20, bool movement = true) => CanPRend(distance, movement) && !JustUsed(InnerRelease) && HasST;
    internal static bool ShouldUseFellCleave(int gauge = 90) => CanFC(gauge) && HasST && InMeleeRange() && Minimal;
    internal static bool ShouldUseTomahawk => LevelChecked(Tomahawk) && !InMeleeRange() && HasBattleTarget();
    internal static uint STCombo 
        => ComboTimer > 0 ? LevelChecked(Maim) && ComboAction == HeavySwing ? Maim
        : LevelChecked(StormsPath) && ComboAction == Maim
        ? (LevelChecked(StormsEye) && ((IsEnabled(CustomComboPreset.WAR_ST_Simple) && GetStatusEffectRemainingTime(Buffs.SurgingTempest) <= 29) || (IsEnabled(CustomComboPreset.WAR_ST_Advanced) && IsEnabled(CustomComboPreset.WAR_ST_StormsEye) && GetStatusEffectRemainingTime(Buffs.SurgingTempest) <= Config.WAR_SurgingRefreshRange))
        ? StormsEye : StormsPath) : HeavySwing : HeavySwing;
    internal static uint AOECombo => (ComboTimer > 0 && LevelChecked(MythrilTempest) && ComboAction == Overpower) ? MythrilTempest : Overpower;
    #endregion

    #region Mitigation Priorities
    /// <summary>
    /// The list of Mitigations to use in the One-Button Mitigation combo.<br />
    /// The order of the list needs to match the order in
    /// <see cref="CustomComboPreset" />.
    /// </summary>
    /// <value>
    /// <c>Action</c> is the action to use.<br />
    /// <c>Preset</c> is the preset to check if the action is enabled.<br />
    /// <c>Logic</c> is the logic for whether to use the action.
    /// </value>
    /// <remarks>
    /// Each logic check is already combined with checking if the preset is
    /// enabled and if the action is <see cref="ActionReady(uint)">ready</see>
    /// and <see cref="LevelChecked(uint)">level-checked</see>.<br />
    /// Do not add any of these checks to <c>Logic</c>.
    /// </remarks>
    private static (uint Action, CustomComboPreset Preset, System.Func<bool> Logic)[]
        PrioritizedMitigation =>
        [
            //Bloodwhetting
            (OriginalHook(RawIntuition), CustomComboPreset.WAR_Mit_Bloodwhetting,
            () => !HasStatusEffect(Buffs.RawIntuition) && !HasStatusEffect(Buffs.BloodwhettingDefenseLong) && PlayerHealthPercentageHp() <= Config.WAR_Mit_Bloodwhetting_Health),
            //Equilibrium
            (Equilibrium, CustomComboPreset.WAR_Mit_Equilibrium,
            () => PlayerHealthPercentageHp() <= Config.WAR_Mit_Equilibrium_Health),
            // Reprisal
            (Role.Reprisal, CustomComboPreset.WAR_Mit_Reprisal,
            () => Role.CanReprisal(checkTargetForDebuff: false)),
            //Thrill of Battle
            (ThrillOfBattle, CustomComboPreset.WAR_Mit_ThrillOfBattle,
            () => PlayerHealthPercentageHp() <= Config.WAR_Mit_ThrillOfBattle_Health),
            //Rampart
            (Role.Rampart, CustomComboPreset.WAR_Mit_Rampart,
            () => Role.CanRampart(Config.WAR_Mit_Rampart_Health)),
            //Shake it Off
            (ShakeItOff, CustomComboPreset.WAR_Mit_ShakeItOff,
            () => !HasStatusEffect(Buffs.ShakeItOff) && (Config.WAR_Mit_ShakeItOff_PartyRequirement == (int)PartyRequirement.No || IsInParty())),
            //Arm's Length
            (Role.ArmsLength, CustomComboPreset.WAR_Mit_ArmsLength,
            () => Role.CanArmsLength(Config.WAR_Mit_ArmsLength_EnemyCount, Config.WAR_Mit_ArmsLength_Boss)),
            //Vengeance
            (OriginalHook(Vengeance), CustomComboPreset.WAR_Mit_Vengeance,
            () => PlayerHealthPercentageHp() <= Config.WAR_Mit_Vengeance_Health)
        ];

    /// <summary>
    /// Given the index of a mitigation in <see cref="PrioritizedMitigation" />,
    /// checks if the mitigation is ready and meets the provided requirements.
    /// </summary>
    /// <param name="index">
    /// The index of the mitigation in <see cref="PrioritizedMitigation" />,
    /// which is the order of the mitigation in <see cref="CustomComboPreset" />.
    /// </param>
    /// <param name="action">
    /// The variable to set to the action to, if the mitigation is set to be used.
    /// </param>
    /// <returns>
    /// Whether the mitigation is ready, enabled, and passes the provided logic check.
    /// </returns>
    private static bool CheckMitigationConfigMeetsRequirements(int index, out uint action)
    {
        action = PrioritizedMitigation[index].Action;
        return ActionReady(action) && LevelChecked(action) &&
            PrioritizedMitigation[index].Logic() &&
            IsEnabled(PrioritizedMitigation[index].Preset);
    }
    #endregion

    #region IDs

    public const byte ClassID = 3; //Marauder (MRD)
    public const byte JobID = 21; //Warrior (WAR)

    #region Actions
    public const uint

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
    #endregion

    #region Traits
    public static class Traits
    {
        public const ushort
        None = 0,
        TankMastery = 318, // L1
        TheBeastWithin = 249, // L35, gauge generation
        InnerBeastMastery = 265, // L54, IB->FC upgrade
        SteelCycloneMastery = 266, // L60, steel cyclone -> decimate upgrade
        EnhancedInfuriate = 157, // L66, gauge spenders reduce cd by 5
        BerserkMastery = 218, // L70, berserk -> IR upgrade
        NascentChaos = 267, // L72, decimate -> chaotic cyclone after infuriate
        MasteringTheBeast = 268, // L74, mythril tempest gives gauge
        EnhancedShakeItOff = 417, // L76, adds heal
        EnhancedThrillOfBattle = 269, // L78, adds incoming heal buff
        RawIntuitionMastery = 418, // L82, raw intuition -> bloodwhetting
        EnhancedNascentFlash = 419, // L82, duration increase
        EnhancedEquilibrium = 420, // L84, adds hot
        MeleeMastery1 = 505, // L84, potency increase
        EnhancedOnslaught = 421, // L88, 3rd onslaught charge
        VengeanceMastery = 567, // L92, vengeance -> damnation
        EnhancedRampart = 639, // L94, adds incoming heal buff
        MeleeMastery2 = 654, // L94, potency increase
        EnhancedInnerRelease = 568, // L96, primal wrath mechanic
        EnhancedReprisal = 640, // L98, extend duration to 15s
        EnhancedPrimalRend = 569; // L100, primal ruination mechanic
    }
    #endregion

    #region Buffs
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
        ShakeItOffHot = 2108, //applied by Shake It Off to self/target
        RawIntuition = 735, //applied by Raw Intuition to self
        NascentFlashSelf = 1857, //applied by Nascent Flash to self, heal on hit
        NascentFlashTarget = 1858, //applied by Nascent Flash to target, -10% damage taken + heal on hit
        BloodwhettingDefenseLong = 2678, //applied by Bloodwhetting to self, -10% damage taken + heal on hit for 8 sec
        BloodwhettingDefenseShort = 2679, //applied by Bloodwhetting, Nascent Flash to self/target, -10% damage taken for 4 sec
        BloodwhettingShield = 2680, //applied by Bloodwhetting, Nascent Flash to self/target, damage shield
        Defiance = 91, //applied by Defiance to self, tank stance
        ShieldWall = 194, //applied by Shield Wall to self/target
        Stronghold = 195, //applied by Stronghold to self/target
        LandWaker = 863; //applied by Land Waker to self/target
        #endregion
    }
    #endregion

    #region Debuffs
    public static class Debuffs
    {
        public const ushort
            Placeholder = 1;
    }
    #endregion

    #endregion
}
