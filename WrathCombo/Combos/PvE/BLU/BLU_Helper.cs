using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using static WrathCombo.Combos.PvE.BLU.Config;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Combos.PvE;

internal partial class BLU
{
    private static bool _surpanakhaReady;

    private static IGameObject? Target =>
        SimpleTarget.HardTarget.IfHostile() ??
        SimpleTarget.LastHostileHardTarget;

    private static bool WantDoT(uint spell, ushort debuff)
    {
        if (!ActionReady(spell) || JustUsed(spell))
            return false;

        if (Target is null || !CanApplyStatus(Target, debuff))
            return false;

        return !HasStatusEffect(debuff, Target) ||
               GetStatusEffectRemainingTime(debuff, Target) <= BLU_DoTTime;
    }

    private static bool UseDoT(ref uint actionID, bool tank)
    {
        if (GetTargetHPPercent() <= BLU_DoTHP)
            return false;

        bool soT = tank
            ? IsEnabled(Preset.BLU_ST_Tank_SongOfTorment)
            : IsEnabled(Preset.BLU_ST_DPS_SongOfTorment);

        if (soT && WantDoT(SongOfTorment, Debuffs.SongOfTorment))
        {
            if (ActionReady(Bristle) && !HasStatusEffect(Buffs.Bristle) && !JustUsed(Bristle))
            {
                actionID = Bristle;
                return true;
            }

            actionID = SongOfTorment;
            return true;
        }

        if (!tank &&
            IsEnabled(Preset.BLU_ST_DPS_Breath) &&
            WantDoT(BreathOfMagic, Debuffs.BreathOfMagic))
        {
            actionID = BreathOfMagic;
            return true;
        }

        if (!tank &&
            IsEnabled(Preset.BLU_ST_DPS_Flame) &&
            WantDoT(MortalFlame, Debuffs.MortalFlame))
        {
            actionID = MortalFlame;
            return true;
        }

        return false;
    }

    private static bool UsePrimalCDs(ref uint actionID, uint retargetFrom, Preset option)
    {
        if (HasStatusEffect(Buffs.PhantomFlurry))
        {
            actionID = OriginalHook(PhantomFlurry);
            return true;
        }

        if (GetStatusEffect(Buffs.WingedReprobation)?.Param > 1 &&
            ActionReady(WingedReprobation))
        {
            actionID = OriginalHook(WingedReprobation);
            return true;
        }

        if (!IsEnabled(option))
            return false;

        if (ActionReady(FeatherRain))
        {
            actionID = FeatherRain.Retarget(retargetFrom, Target);
            return true;
        }

        if (ActionReady(Eruption))
        {
            actionID = Eruption;
            return true;
        }

        if (ActionReady(ShockStrike))
        {
            actionID = ShockStrike;
            return true;
        }

        if (ActionReady(RoseOfDestruction))
        {
            actionID = RoseOfDestruction;
            return true;
        }

        if (ActionReady(GlassDance))
        {
            actionID = GlassDance;
            return true;
        }

        if (ActionReady(JKick))
        {
            actionID = JKick;
            return true;
        }

        if (ActionReady(Nightbloom))
        {
            actionID = Nightbloom;
            return true;
        }

        if (ActionReady(MatraMagic) && HasStatusEffect(Buffs.DPSMimicry))
        {
            actionID = MatraMagic;
            return true;
        }

        if (GetRemainingCharges(Surpanakha) == 4)
            _surpanakhaReady = true;
        if (_surpanakhaReady && ActionReady(Surpanakha))
        {
            actionID = Surpanakha;
            return true;
        }
        if (GetRemainingCharges(Surpanakha) == 0)
            _surpanakhaReady = false;

        if (ActionReady(WingedReprobation))
        {
            actionID = OriginalHook(WingedReprobation);
            return true;
        }

        if (ActionReady(SeaShanty))
        {
            actionID = SeaShanty;
            return true;
        }

        if (ActionReady(PhantomFlurry))
        {
            actionID = PhantomFlurry;
            return true;
        }

        return false;
    }

    private static bool UseFlyingSardine(ref uint actionID, uint retargetFrom, Preset option)
    {
        if (!IsEnabled(option) || !ActionReady(FlyingSardine))
            return false;

        var interruptTarget = CurrentTarget;
        if (interruptTarget is null ||
            !CanInterruptEnemy(null, interruptTarget) ||
            !InActionRange(FlyingSardine, interruptTarget))
        {
            interruptTarget = Svc.Objects
                .OfType<IBattleChara>()
                .Where(x => x.IsHostile() && x.IsTargetable &&
                            InActionRange(FlyingSardine, x) &&
                            CanInterruptEnemy(null, x))
                .OrderByDescending(x => CurrentTarget?.GameObjectId == x.GameObjectId)
                .FirstOrDefault();
        }

        if (interruptTarget is null)
            return false;

        actionID = FlyingSardine.Retarget(retargetFrom, interruptTarget);
        return true;
    }

    private static bool UseProvoke(ref uint actionID, bool onAoE, Preset frog, Preset tongue)
    {
        if (!InCombat())
            return false;

        var provokeTarget = CurrentTarget;
        bool targetNotOnUs = provokeTarget is IBattleChara enemy &&
                             enemy.TargetObjectId != LocalPlayer?.GameObjectId;

        if (!targetNotOnUs && PlayerHasAggro)
            return false;

        if (onAoE &&
            IsEnabled(frog) &&
            ActionReady(FrogLegs) &&
            !JustUsed(FrogLegs))
        {
            actionID = FrogLegs;
            return true;
        }

        if (IsEnabled(tongue) &&
            ActionReady(StickyTongue) &&
            provokeTarget is not null &&
            InActionRange(StickyTongue, provokeTarget) &&
            !JustUsed(StickyTongue))
        {
            actionID = StickyTongue;
            return true;
        }

        if (!onAoE &&
            IsEnabled(frog) &&
            ActionReady(FrogLegs) &&
            !JustUsed(FrogLegs))
        {
            actionID = FrogLegs;
            return true;
        }

        return false;
    }

    private static bool UseSharpenedKnife() =>
        ActionReady(SharpenedKnife) &&
        InActionRange(SharpenedKnife);

    private static bool UseSoloInstinct(ref uint actionID, Preset option)
    {
        if (!IsEnabled(option) ||
            !HasCondition(ConditionFlag.BoundByDuty) ||
            GetPartyMembers().Count != 0 ||
            !ActionReady(BasicInstinct) ||
            HasStatusEffect(Buffs.BasicInstinct))
            return false;

        if (ActionReady(MightyGuard) &&
            !HasStatusEffect(Buffs.MightyGuard) &&
            !JustUsed(MightyGuard))
        {
            actionID = MightyGuard;
            return true;
        }

        actionID = BasicInstinct;
        return true;
    }

    private static bool UseTankMit(ref uint actionID, Preset gate, Preset dragon)
    {
        if (!HasIncomingTankBusterEffect() && !GroupDamageIncoming())
            return false;

        if (IsEnabled(gate) &&
            ActionReady(ChelonianGate) &&
            !HasStatusEffect(Buffs.ChelonianGate) &&
            !JustUsed(ChelonianGate))
        {
            actionID = ChelonianGate;
            return true;
        }

        if (IsEnabled(dragon) &&
            ActionReady(DragonForce) &&
            !JustUsed(DragonForce))
        {
            actionID = DragonForce;
            return true;
        }

        return false;
    }

    private static uint DoDPS(uint actionID, uint retargetFrom, bool onAoE)
    {
        var instinct = onAoE ? Preset.BLU_AoE_DPS_BasicInstinct : Preset.BLU_ST_DPS_BasicInstinct;
        var sardine = onAoE ? Preset.BLU_AoE_DPS_FlyingSardine : Preset.BLU_ST_DPS_FlyingSardine;
        var primals = onAoE ? Preset.BLU_AoE_DPS_Primals : Preset.BLU_ST_DPS_Primals;

        if (LocalPlayer!.Status(Buffs.PhantomFlurry).RemainingTimeOrZero() > 0)
            return All.Cease;

        if (HasStatusEffect(Buffs.WaningNocturne))
            return actionID;

        if (UseSoloInstinct(ref actionID, instinct))
            return actionID;

        if (!onAoE &&
            IsEnabled(Preset.BLU_ST_DPS_Opener) &&
            Opener().FullOpener(ref actionID))
        {
            if (actionID is FeatherRain)
                actionID = FeatherRain.Retarget(retargetFrom, Target);
            return actionID;
        }

        if (UseFlyingSardine(ref actionID, retargetFrom, sardine))
            return actionID;

        if (UsePrimalCDs(ref actionID, retargetFrom, primals))
            return actionID;

        if (!onAoE && UseDoT(ref actionID, false))
            return actionID;

        if (!onAoE &&
            IsEnabled(Preset.BLU_ST_DPS_TripleTrident) &&
            ActionReady(TripleTrident) &&
            InActionRange(TripleTrident))
            return TripleTrident;

        if (onAoE)
        {
            if (IsSpellActive(Electrogenesis))
                return Electrogenesis;
            if (IsEnabled(Preset.BLU_AoE_DPS_HydroPull) && IsSpellActive(HydroPull))
                return HydroPull;
        }
        else if (IsEnabled(Preset.BLU_ST_DPS_SharpenedKnife) && UseSharpenedKnife())
            return SharpenedKnife;

        return IsSpellActive(SonicBoom) ? SonicBoom : actionID;
    }

    private static uint DoTank(uint actionID, uint retargetFrom, bool onAoE)
    {
        var mighty = onAoE ? Preset.BLU_AoE_Tank_MightyGuard : Preset.BLU_ST_Tank_MightyGuard;
        var instinct = onAoE ? Preset.BLU_AoE_Tank_BasicInstinct : Preset.BLU_ST_Tank_BasicInstinct;
        var gate = onAoE ? Preset.BLU_AoE_Tank_ChelonianGate : Preset.BLU_ST_Tank_ChelonianGate;
        var dragon = onAoE ? Preset.BLU_AoE_Tank_DragonForce : Preset.BLU_ST_Tank_DragonForce;
        var sardine = onAoE ? Preset.BLU_AoE_Tank_FlyingSardine : Preset.BLU_ST_Tank_FlyingSardine;
        var frog = onAoE ? Preset.BLU_AoE_Tank_FrogLegs : Preset.BLU_ST_Tank_FrogLegs;
        var tongue = onAoE ? Preset.BLU_AoE_Tank_StickyTongue : Preset.BLU_ST_Tank_StickyTongue;
        var primals = onAoE ? Preset.BLU_AoE_Tank_Primals : Preset.BLU_ST_Tank_Primals;
        var devour = onAoE ? Preset.BLU_AoE_Tank_Devour : Preset.BLU_ST_Tank_Devour;
        var lucid = onAoE ? Preset.BLU_AoE_Tank_Lucid : Preset.BLU_ST_Tank_Lucid;
        var badBreath = onAoE ? Preset.BLU_AoE_Tank_BadBreath : Preset.BLU_ST_Tank_BadBreath;

        if (HasStatusEffect(Buffs.WaningNocturne))
            return actionID;

        if (IsEnabled(mighty) &&
            ActionReady(MightyGuard) &&
            !HasStatusEffect(Buffs.MightyGuard) &&
            !JustUsed(MightyGuard))
            return MightyGuard;

        if (UseSoloInstinct(ref actionID, instinct))
            return actionID;

        if (HasStatusEffect(Buffs.AuspiciousTrance) ||
            OriginalHook(ChelonianGate) == DivineCataract)
            return DivineCataract;

        if (HasStatusEffect(Buffs.ChelonianGate))
            return All.Cease;

        if (UseTankMit(ref actionID, gate, dragon))
            return actionID;

        if (UseFlyingSardine(ref actionID, retargetFrom, sardine))
            return actionID;

        if (UseProvoke(ref actionID, onAoE, frog, tongue))
            return actionID;

        if (UsePrimalCDs(ref actionID, retargetFrom, primals))
            return actionID;

        if (CanWeave() &&
            IsEnabled(lucid) &&
            Role.CanLucidDream(9000))
            return Role.LucidDreaming;

        if (IsEnabled(devour) &&
            ActionReady(Devour) &&
            InActionRange(Devour) &&
            HasTankMimicry &&
            !JustUsed(Devour))
            return Devour;

        if (!onAoE &&
            IsEnabled(Preset.BLU_ST_Tank_Offguard) &&
            ActionReady(Offguard) &&
            Target is not null &&
            !HasStatusEffect(Debuffs.Offguard, Target, true))
            return Offguard;

        if (IsEnabled(badBreath) &&
            ActionReady(BadBreath) &&
            HasTankMimicry &&
            Target is not null &&
            !HasStatusEffect(Debuffs.Malodorous, Target, true))
            return BadBreath;

        if (!onAoE && UseDoT(ref actionID, true))
            return actionID;

        if (onAoE)
        {
            if (IsSpellActive(RightRound))
                return RightRound;
            if (IsSpellActive(Electrogenesis))
                return Electrogenesis;
            if (IsEnabled(Preset.BLU_AoE_Tank_HydroPull) && IsSpellActive(HydroPull))
                return HydroPull;
            return IsSpellActive(SonicBoom) ? SonicBoom : actionID;
        }

        if (IsSpellActive(SonicBoom) && !InMeleeRange())
            return SonicBoom;

        return IsSpellActive(GoblinPunch) ? GoblinPunch : actionID;
    }

    private static uint DoHeal(uint actionID, bool onAoE)
    {
        var lucid = onAoE ? Preset.BLU_AoE_Heal_Lucid : Preset.BLU_ST_Heal_Lucid;
        var snack = onAoE ? Preset.BLU_AoE_Heal_AngelsSnack : Preset.BLU_ST_Heal_AngelsSnack;

        if (IsEnabled(lucid) && Role.CanLucidDream(9000))
            return Role.LucidDreaming;

        if (onAoE)
        {
            if (IsEnabled(snack) && ActionReady(AngelsSnack))
                return AngelsSnack;
            if (IsEnabled(Preset.BLU_AoE_Heal_Stotram))
            {
                uint stotram = OriginalHook(Stotram);
                if (ActionReady(stotram))
                    return stotram;
            }
            if (IsEnabled(Preset.BLU_AoE_Heal_Gobskin) && ActionReady(Gobskin))
                return Gobskin;
            if (ActionReady(WhiteWind) && PlayerHealthPercentageHp() > 50)
                return WhiteWind;
            return actionID;
        }

        var healTarget = SimpleTarget.Stack.AllyToHeal;
        if (IsEnabled(Preset.BLU_ST_Heal_Exuviation) &&
            ActionReady(Exuviation) &&
            HasCleansableDebuff(healTarget))
            return Exuviation.RetargetIfEnabled(actionID);

        if (IsEnabled(snack) && ActionReady(AngelsSnack))
            return AngelsSnack;
        if (ActionReady(PomCure))
            return PomCure.RetargetIfEnabled(actionID);

        return actionID;
    }

    internal static bool HasTankMimicry =>
        HasStatusEffect(Buffs.TankMimicry);

    internal static bool HasHealerMimicry =>
        HasStatusEffect(Buffs.HealerMimicry);

    internal static bool HasDPSMimicry =>
        HasStatusEffect(Buffs.DPSMimicry);

    #region Openers

    internal static WrathOpener Opener()
    {
        if (MoonFluteDoTOpener.LevelChecked && BLU_SelectedOpener == 1)
            return MoonFluteDoTOpener;

        if (MoonFluteOpener.LevelChecked)
            return MoonFluteOpener;

        return WrathOpener.Dummy;
    }

    internal static BLUMoonFluteOpener MoonFluteOpener = new();
    internal static BLUMoonFluteDoTOpener MoonFluteDoTOpener = new();

    internal abstract class BLUOpenerBase : WrathOpener
    {
        public override int MinOpenerLevel => 1;

        public override int MaxOpenerLevel => 80;

        public override Preset Preset => Preset.BLU_ST_DPS_Opener;

        internal override UserData ContentCheckConfig => BLU_Balance_Content;

        internal override bool IncludePot => false;

        public override bool AllowReopener { get; set; } = true;

        public override List<(int[] Steps, Func<float> HoldDelay)> PrepullDelays { get; set; } =
        [
            ([1], () => CountdownRemaining - 5),
            ([2], () => CountdownRemaining - 3),
            ([3], () => CountdownRemaining)
        ];

        public override bool HasCooldowns() =>
            ActionReady(MoonFlute) &&
            (!IsSpellActive(JKick) || ActionReady(JKick)) &&
            (!IsSpellActive(Nightbloom) || ActionReady(Nightbloom)) &&
            (!IsSpellActive(PhantomFlurry) || ActionReady(PhantomFlurry)) &&
            (!IsSpellActive(Surpanakha) || GetRemainingCharges(Surpanakha) == 4);
    }

    internal class BLUMoonFluteOpener : BLUOpenerBase
    {
        public override List<Func<uint>> OpenerActions { get; set; } =
        [
            () => Whistle, //1
            () => Tingle, //2
            () => RoseOfDestruction, //3
            () => MoonFlute, //4 
            () => JKick, //5
            () => TripleTrident, //6
            () => Nightbloom, //7
            () => WingedReprobation, //8
            () => FeatherRain.Retarget(SonicBoom, CurrentTarget), //9
            () => SeaShanty, //10
            () => WingedReprobation, //11
            () => ShockStrike, //12
            () => BeingMortal, //13
            () => Bristle, //14
            () => Role.Swiftcast, //15
            () => Surpanakha, //16
            () => Surpanakha, //17
            () => Surpanakha, //18
            () => Surpanakha, //19
            () => MatraMagic, //20
            () => PhantomFlurry //21
        ];

        public override List<(int[] Steps, Func<bool> Condition)> SkipSteps { get; set; } =
        [
            ([1], () => !IsSpellActive(Whistle) || HasStatusEffect(Buffs.Whistle)),
            ([2], () => !IsSpellActive(Tingle) || HasStatusEffect(Buffs.Tingle, Target, true)),
            ([3], () => !IsSpellActive(RoseOfDestruction)),
            ([5], () => !IsSpellActive(JKick)),
            ([6], () => !IsSpellActive(TripleTrident) || !ActionReady(TripleTrident)),
            ([7], () => !IsSpellActive(Nightbloom)),
            ([8], () => !IsSpellActive(WingedReprobation)),
            ([9], () => !IsSpellActive(FeatherRain)),
            ([10], () => !IsSpellActive(SeaShanty)),
            ([11], () => !IsSpellActive(WingedReprobation)),
            ([12], () => !IsSpellActive(ShockStrike)),
            ([13], () => !IsSpellActive(BeingMortal)),
            ([14], () => !IsSpellActive(Bristle) || HasStatusEffect(Buffs.Bristle)),
            ([15], () => !ActionReady(Role.Swiftcast)),
            ([16, 17, 18, 19], () => !IsSpellActive(Surpanakha)),
            ([20], () => !IsSpellActive(MatraMagic) || !HasStatusEffect(Buffs.DPSMimicry)),
            ([21], () => !IsSpellActive(PhantomFlurry))
        ];

        public override List<int> AllowUpgradeSteps { get; set; } = [8, 11];
    }

    internal class BLUMoonFluteDoTOpener : BLUOpenerBase
    {
        public override List<Func<uint>> OpenerActions { get; set; } =
        [
            () => Whistle, // 1
            () => Tingle, // 2
            () => RoseOfDestruction, // 3
            () => MoonFlute, // 4
            () => JKick, // 5
            () => TripleTrident, // 6
            () => Nightbloom, // 7
            () => Bristle, // 8
            () => FeatherRain.Retarget(SonicBoom, CurrentTarget), // 9
            () => SeaShanty, // 10
            () => BreathOfMagic, // 11
            () => ShockStrike, // 12
            () => Bristle, // 13
            () => Role.Swiftcast, // 14
            () => Surpanakha, // 15
            () => Surpanakha, // 16
            () => Surpanakha, // 17
            () => Surpanakha, // 18
            () => MatraMagic, // 19
            () => BeingMortal, // 20
            () => PhantomFlurry // 21
        ];

        public override List<(int[] Steps, uint NewAction, Func<bool> Condition)> SubstitutionSteps { get; set; } =
        [
            ([11], MortalFlame, () => !IsSpellActive(BreathOfMagic) || HasStatusEffect(Debuffs.BreathOfMagic, Target, true))
        ];

        public override List<(int[] Steps, Func<bool> Condition)> SkipSteps { get; set; } =
        [
            ([1], () => !IsSpellActive(Whistle) || HasStatusEffect(Buffs.Whistle)),
            ([2], () => !IsSpellActive(Tingle) || HasStatusEffect(Buffs.Tingle, Target, true)),
            ([3], () => !IsSpellActive(RoseOfDestruction)),
            ([5], () => !IsSpellActive(JKick)),
            ([6], () => !IsSpellActive(TripleTrident) || !ActionReady(TripleTrident)),
            ([7], () => !IsSpellActive(Nightbloom)),
            ([8], () => !IsSpellActive(Bristle) || HasStatusEffect(Buffs.Bristle)),
            ([9], () => !IsSpellActive(FeatherRain)),
            ([10], () => !IsSpellActive(SeaShanty)),
            ([11], () => !IsSpellActive(BreathOfMagic) && !IsSpellActive(MortalFlame)),
            ([12], () => !IsSpellActive(ShockStrike)),
            ([13], () => !IsSpellActive(Bristle) || HasStatusEffect(Buffs.Bristle)),
            ([14], () => !ActionReady(Role.Swiftcast)),
            ([15, 16, 17, 18], () => !IsSpellActive(Surpanakha)),
            ([19], () => !IsSpellActive(MatraMagic) || !HasStatusEffect(Buffs.DPSMimicry)),
            ([20], () => !IsSpellActive(BeingMortal)),
            ([21], () => !IsSpellActive(PhantomFlurry))
        ];
    }

    #endregion

    #region ID's

    public const uint
        WaterCannon = 11385,
        FlameThrower = 11402,
        AquaBreath = 11390,
        FlyingFrenzy = 11389,
        DrillCannons = 11398,
        HighVoltage = 11387,
        Loom = 11401,
        FinalSting = 11407,
        SongOfTorment = 11386,
        Glower = 11404,
        Plaincracker = 11391,
        Bristle = 11393,
        WhiteWind = 11406,
        Level5Petrify = 11414,
        SharpenedKnife = 11400,
        IceSpikes = 11418,
        BloodDrain = 11395,
        AcornBomb = 11392,
        BombToss = 11396,
        Offguard = 11411,
        SelfDestruct = 11408,
        Transfusion = 11409,
        Faze = 11403,
        FlyingSardine = 11423,
        Snort = 11383,
        FourTonzeWeight = 11384,
        TheLook = 11399,
        BadBreath = 11388,
        Diamondback = 11424,
        MightyGuard = 11417,
        StickyTongue = 11412,
        ToadOil = 11410,
        RamsVoice = 11419,
        DragonsVoice = 11420,
        Missile = 11405,
        ThousandNeedles = 11397,
        InkJet = 11422,
        FireAngon = 11425,
        MoonFlute = 11415,
        TailScrew = 11413,
        MindBlast = 11394,
        Doom = 11416,
        PeculiarLight = 11421,
        FeatherRain = 11426,
        Eruption = 11427,
        MountainBuster = 11428,
        ShockStrike = 11429,
        GlassDance = 11430,
        VeilOfTheWhorl = 11431,
        AlpineDraft = 18295,
        ProteanWave = 18296,
        Northerlies = 18297,
        Electrogenesis = 18298,
        Kaltstrahl = 18299,
        AbyssalTransfixion = 18300,
        Chirp = 18301,
        EerieSoundwave = 18302,
        PomCure = 18303,
        Gobskin = 18304,
        MagicHammer = 18305,
        Avail = 18306,
        FrogLegs = 18307,
        SonicBoom = 18308,
        Whistle = 18309,
        WhiteKnightsTour = 18310,
        BlackKnightsTour = 18311,
        Level5Death = 18312,
        Launcher = 18313,
        PerpetualRay = 18314,
        Cactguard = 18315,
        RevengeBlast = 18316,
        AngelWhisper = 18317,
        Exuviation = 18318,
        Reflux = 18319,
        Devour = 18320,
        CondensedLibra = 18321,
        AethericMimicry = 18322,
        Surpanakha = 18323,
        Quasar = 18324,
        JKick = 18325,
        TripleTrident = 23264,
        Tingle = 23265,
        Tatamigaeshi = 23266,
        ColdFog = 23267,
        Stotram = 23269,
        StotramHeal = 23416,
        SaintlyBeam = 23270,
        FeculentFlood = 23271,
        AngelsSnack = 23272,
        ChelonianGate = 23273,
        DivineCataract = 23274,
        RoseOfDestruction = 23275,
        BasicInstinct = 23276,
        Ultravibration = 23277,
        Blaze = 23278,
        MustardBomb = 23279,
        DragonForce = 23280,
        AetherialSpark = 23281,
        HydroPull = 23282,
        MaledictionOfWater = 23283,
        ChocoMeteor = 23284,
        MatraMagic = 23285,
        PeripheralSynthesis = 23286,
        BothEnds = 23287,
        PhantomFlurry = 23288,
        Nightbloom = 23290,
        GoblinPunch = 34563,
        RightRound = 34564,
        Schiltron = 34565,
        Rehydration = 34566,
        BreathOfMagic = 34567,
        WildRage = 34568,
        PeatPelt = 34569,
        DeepClean = 34570,
        RubyDynamics = 34571,
        DivinationRune = 34572,
        DimensionalShift = 34573,
        ConvictionMarcato = 34574,
        ForceField = 34575,
        WingedReprobation = 34576,
        LaserEye = 34577,
        CandyCane = 34578,
        MortalFlame = 34579,
        SeaShanty = 34580,
        Apokalypsis = 34581,
        BeingMortal = 34582;

    #endregion

    public static class Buffs
    {
        public const ushort
            MoonFlute = 1718,
            Bristle = 1716,
            WaningNocturne = 1727,
            PhantomFlurry = 2502,
            Tingle = 2492,
            Whistle = 2118,
            MightyGuard = 1719,
            TankMimicry = 2124,
            DPSMimicry = 2125,
            HealerMimicry = 2126,
            BasicInstinct = 2498,
            ChelonianGate = 2496,
            AuspiciousTrance = 2497,
            WingedReprobation = 3640;
    }

    public static class Debuffs
    {
        public const ushort
            Slow = 9,
            Bind = 13,
            Stun = 142,
            SongOfTorment = 1714,
            DeepFreeze = 1731,
            Offguard = 1717,
            Malodorous = 1715,
            Conked = 2115,
            Lightheaded = 2501,
            MortalFlame = 3643,
            BreathOfMagic = 3712,
            Begrimed = 3636;
    }
}
