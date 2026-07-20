using Dalamud.Game.ClientState.JobGauge.Types;
using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Collections.Generic;
using WrathCombo.Combos.PvE.ALL;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.Combos.PvE.MCH.Config;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
namespace WrathCombo.Combos.PvE;

internal partial class MCH
{
    #region Queen

    private static bool ShouldUseQueenST()
    {
        if (Battery is 100)
            return true;

        if (Battery > 80 &&
            (HasStatusEffect(Buffs.ExcavatorReady) ||
             ActionReady(Chainsaw) ||
             ActionReady(OriginalHook(AirAnchor))))
            return true;

        return Battery > 90 && ComboAction == OriginalHook(SlugShot);
    }

    private static bool CanQueen(
        bool onAoE = false,
        int batteryThreshold = 100,
        int hpThreshold = 0,
        bool batteryOnly = false,
        int wildfireBossOnlyOption = 1,
        int turretUsage = 100)
    {
        if (onAoE)
        {
            if (!ActionReady(OriginalHook(RookAutoturret)))
                return false;

            return batteryOnly
                ? Battery is 100
                : Battery >= batteryThreshold &&
                  GetTargetHPPercent() > hpThreshold;
        }

        if (!HasStatusEffect(Buffs.Wildfire) &&
            ActionReady(OriginalHook(RookAutoturret)) &&
            !IsRobotActive &&
            GetTargetHPPercent() > hpThreshold)
        {
            if (LevelChecked(Wildfire))
            {
                if (wildfireBossOnlyOption == 0 || TargetIsBoss())
                {
                    if (ShouldUseQueenST())
                        return true;
                }

                if (wildfireBossOnlyOption == 1 && !TargetIsBoss() && Battery >= turretUsage)
                    return true;
            }

            if (!LevelChecked(Wildfire) && Battery >= turretUsage)
                return true;
        }

        return false;
    }

    #endregion

    #region Hypercharge

    private static bool CanHypercharge(
        bool onAoE,
        bool useAirAnchor = true,
        float toolHoldThreshold = 8f,
        int hpThreshold = 25,
        bool skipExcavatorHold = false,
        bool skipHyperchargeHold = false,
        float wildfireHyperchargeCutoff = 9f,
        int wildfireBossOnlyOption = 1) =>
        onAoE
            ? CanHyperchargeAoE(useAirAnchor, toolHoldThreshold, hpThreshold)
            : CanHyperchargeST(hpThreshold, skipExcavatorHold, skipHyperchargeHold, wildfireHyperchargeCutoff,
                wildfireBossOnlyOption);

    private static bool IsHyperchargeReady() =>
        (ActionReady(Hypercharge) || HasStatusEffect(Buffs.Hypercharged)) && !IsOverheated;

    private static bool AreHyperchargeToolsReady(
        float toolCutoff,
        bool skipHyperchargeHold,
        bool skipExcavatorHold) =>
        IsDrillCD(toolCutoff) && IsAirAnchorCD(toolCutoff) &&
        (IsChainSawCD(toolCutoff) || skipHyperchargeHold) &&
        (!HasStatusEffect(Buffs.ExcavatorReady) || skipExcavatorHold);

    private static bool ShouldUseHyperchargeST(int wildfireBossOnlyOption) =>
        ActionReady(Wildfire) ||
        JustUsed(FullMetalField, GCD / 2) ||
        wildfireBossOnlyOption == 1 && !TargetIsBoss() ||
        GetCooldownRemainingTime(Wildfire) > GCD * 15 ||
        Heat is 100 && GetCooldownRemainingTime(Wildfire) > 10 ||
        !LevelChecked(Wildfire);

    private static bool CanHyperchargeST(
        int hpThreshold = 25,
        bool skipExcavatorHold = false,
        bool skipHyperchargeHold = false,
        float wildfireHyperchargeCutoff = 9f,
        int wildfireBossOnlyOption = 1)
    {
        if (GetTargetHPPercent() <= hpThreshold)
            return false;

        return IsHyperchargeReady() &&
               (!IsComboExpiring(6) || skipHyperchargeHold) &&
               AreHyperchargeToolsReady(wildfireHyperchargeCutoff, skipHyperchargeHold, skipExcavatorHold) &&
               !HasStatusEffect(Buffs.FullMetalMachinist) &&
               ShouldUseHyperchargeST(wildfireBossOnlyOption);
    }

    private static bool UsedBioBlaster(float time = 9f) =>
        !LevelChecked(BioBlaster) ||
        IsBioBlasterCD(time) ||
        HasStatusEffect(Debuffs.Bioblaster, CurrentTarget, true);

    private static bool UsedDrill(float time = 9f) =>
        !LevelChecked(Drill) || IsDrillCD(time);

    private static bool CanHyperchargeAoE(bool useAirAnchor = true, float toolHoldThreshold = 8f, int hpThreshold = 25)
    {
        if (GetTargetHPPercent() <= hpThreshold)
            return false;

        if (!IsHyperchargeReady())
            return false;

        if (LevelChecked(BioBlaster))
        {
            if (!UsedBioBlaster(toolHoldThreshold))
                return false;
        }
        else if (!UsedDrill(toolHoldThreshold))
            return false;

        if (!IsChainSawCD(toolHoldThreshold) || HasStatusEffect(Buffs.ExcavatorReady))
            return false;

        return !useAirAnchor || IsAirAnchorCD(toolHoldThreshold);
    }

    private static bool IsWildfireAboutToBeUsed(int wildfireHpThreshold, int wildfireBossOnlyOption) =>
        (wildfireBossOnlyOption == 0 && GetTargetHPPercent() > wildfireHpThreshold || TargetIsBoss()) &&
        CanApplyStatus(CurrentTarget, Debuffs.Wildfire) &&
        ActionReady(Wildfire);

    #endregion

    #region Misc

    private static bool CanUseFullMetalField =>
        HasStatusEffect(Buffs.FullMetalMachinist) &&
        !IsOverheated &&
        (ActionReady(Wildfire) ||
         GetCooldownRemainingTime(Wildfire) > 90 ||
         GetCooldownRemainingTime(Wildfire) <= GCD ||
         GetStatusEffectRemainingTime(Buffs.FullMetalMachinist) <= 6);

    private static bool JustUsedOverheatGCD(float window, bool onAoE) =>
        onAoE
            ? JustUsed(OriginalHook(AutoCrossbow), window) ||
              JustUsed(OriginalHook(Heatblast), window)
            : JustUsed(OriginalHook(Heatblast), window);

    private static uint OverheatGCD(bool onAoE, bool gaussRicoEnabled = true, bool alwaysAutoCrossbow = false)
    {
        if (!onAoE)
            return OriginalHook(Heatblast);

        if (alwaysAutoCrossbow ||
            !LevelChecked(CheckMate) && ActionReady(AutoCrossbow) ||
            LevelChecked(CheckMate) && LevelChecked(BlazingShot) &&
            NumberOfEnemiesInRange(AutoCrossbow, CurrentTarget) >= 5 ||
            !gaussRicoEnabled && ActionReady(AutoCrossbow))
            return AutoCrossbow;

        return OriginalHook(Heatblast);
    }

    private static bool CanBarrelStabilizer(
        bool onAoE = false,
        int hpThreshold = 0,
        int bossOnlyOption = 1,
        bool requireBoss = false) =>
        ActionReady(BarrelStabilizer) && !HasStatusEffect(Buffs.FullMetalMachinist) &&
        (onAoE
            ? GetTargetHPPercent() > hpThreshold
            : (requireBoss
                  ? TargetIsBoss()
                  : bossOnlyOption == 0 &&
                  GetTargetHPPercent() > hpThreshold || TargetIsBoss()) &&
              GetCooldownRemainingTime(Wildfire) <= 20);

    private static bool CanWildfireWeave(
        int hpThreshold = 0,
        int bossOnlyOption = 1,
        bool requireBoss = false,
        float? hyperchargeWindow = null) =>
        CanApplyStatus(CurrentTarget, Debuffs.Wildfire) &&
        ActionReady(Wildfire) &&
        JustUsed(Hypercharge, hyperchargeWindow ?? GCD + 0.9f) &&
        !HasStatusEffect(Buffs.Wildfire) &&
        (requireBoss
            ? TargetIsBoss()
            : bossOnlyOption == 0 &&
            GetTargetHPPercent() > hpThreshold || TargetIsBoss());

    #endregion

    #region Reassembled

    private static uint CurrentReassembleCharges = uint.MaxValue;
    private static bool UseBothCharges;

    private static bool TwoChargesUnlocked => GetMaxCharges(Reassemble) >= 2;

    private static bool IsWildfireActive => HasStatusEffect(Buffs.Wildfire);

    private static void UpdateReassembleChargeTracking()
    {
        uint charges = GetRemainingCharges(Reassemble);
        if (charges == CurrentReassembleCharges)
            return;

        switch (TwoChargesUnlocked)
        {
            case true:
                switch (charges)
                {
                    case 2 when CurrentReassembleCharges != 2:
                        UseBothCharges = true;
                        break;

                    case 0:

                    case 1 when CurrentReassembleCharges == 0:
                        UseBothCharges = false;
                        break;
                }
                break;

            default:
                UseBothCharges = false;
                break;
        }

        CurrentReassembleCharges = charges;
    }

    private static bool ShouldReassemble() =>
        !TwoChargesUnlocked || UseBothCharges;

    private static int ReadyTools()
    {
        int numberOfReadyTools = 0;

        if (ActionReady(Drill))
            numberOfReadyTools += (int)GetRemainingCharges(Drill);

        if (ToolReadyForReassembleWeave(Chainsaw))
        {
            numberOfReadyTools++;
            if (LevelChecked(Excavator))
                numberOfReadyTools++;
        }

        else if (HasStatusEffect(Buffs.ExcavatorReady))
        {
            numberOfReadyTools++;
        }

        if (ToolReadyForReassembleWeave(AirAnchor))
            numberOfReadyTools++;

        if (!LevelChecked(Drill) && ComboTimer > 0 && ComboAction is SlugShot &&
            LevelChecked(CleanShot))
            numberOfReadyTools++;

        return numberOfReadyTools;
    }

    private static bool CanReassembleAoE(int chargePool = 0, int hpThreshold = 25, bool forPrepull = false)
    {
        uint remainingCharges = GetRemainingCharges(Reassemble);

        if (!ActionReady(Reassemble) || HasStatusEffect(Buffs.Reassembled) ||
            !HasBattleTarget() || GetTargetHPPercent() <= hpThreshold ||
            !InReassembleRange() || JustUsed(Reassemble, 2f))
            return false;

        if (remainingCharges == 0 || remainingCharges <= chargePool)
            return false;

        if (ToolReadyForReassemble(Excavator, forPrepull) && HasStatusEffect(Buffs.ExcavatorReady))
            return true;

        if (ToolReadyForReassemble(Chainsaw, forPrepull) && !HasStatusEffect(Buffs.ExcavatorReady))
            return true;

        if (ToolReadyForReassemble(AirAnchor, forPrepull) &&
            (!LevelChecked(Chainsaw) || GetCooldownRemainingTime(Chainsaw) > GCD * 2))
            return true;

        if (CanUseDrill(true) && ToolReadyForReassemble(Drill, forPrepull))
            return true;

        if (LevelChecked(Scattergun) && ActionReady(Scattergun))
            return true;

        return ActionReady(OriginalHook(SpreadShot));
    }

    private static bool InReassembleRange() =>
        LevelChecked(Drill) && InActionRange(Drill) ||
        LevelChecked(AirAnchor) && InActionRange(AirAnchor) ||
        LevelChecked(Chainsaw) && InActionRange(Chainsaw) ||
        LevelChecked(Scattergun) && InActionRange(OriginalHook(SpreadShot)) ||
        !LevelChecked(Drill) && InActionRange(OriginalHook(SpreadShot));

    private static bool CanReassemble(bool onAoE, int reassembleChoice = 1, int chargePool = 0, int hpThreshold = 25, bool forPrepull = false) =>
        onAoE ? CanReassembleAoE(chargePool, hpThreshold, forPrepull) : CanReassembleST(reassembleChoice, chargePool, hpThreshold, forPrepull);

    private static bool CanReassembleST(int reassembleChoice = 1, int chargePool = 0, int hpThreshold = 25, bool forPrepull = false)
    {
        UpdateReassembleChargeTracking();

        uint remainingCharges = GetRemainingCharges(Reassemble);

        if (!ActionReady(Reassemble) || HasStatusEffect(Buffs.Reassembled) || IsWildfireActive || !HasBattleTarget() ||
            GetTargetHPPercent() <= hpThreshold ||
            !InReassembleRange() || JustUsed(Reassemble, 2f))
            return false;

        if (remainingCharges == 0 || remainingCharges <= chargePool)
            return false;

        if (reassembleChoice == 0 && !ShouldReassemble())
            return false;

        switch (reassembleChoice)
        {
            case 0:
            {
                int numberOfReadyTools = ReadyTools();
                return numberOfReadyTools >= remainingCharges;
            }

            case 1 when ToolReadyForReassemble(Excavator, forPrepull) && HasStatusEffect(Buffs.ExcavatorReady):
            case 1 when ToolReadyForReassemble(Chainsaw, forPrepull) && !HasStatusEffect(Buffs.ExcavatorReady):
            case 1 when ToolReadyForReassemble(AirAnchor, forPrepull) && (!LevelChecked(Chainsaw) || GetCooldownRemainingTime(Chainsaw) > GCD * 2):
            case 1 when ToolReadyForReassemble(Drill, forPrepull) && (!LevelChecked(AirAnchor) || GetCooldownRemainingTime(AirAnchor) > GCD * 2):
            case 1 when !LevelChecked(Drill) && ComboTimer > 0 && ComboAction is SlugShot && LevelChecked(CleanShot):
            case 1 when !LevelChecked(CleanShot) && ToolReadyForReassemble(HotShot, forPrepull):
                return true;

            default:
                return false;
        }
    }

    #endregion

    #region Gauss and Rico

    private static bool IsOvercapping(uint action) =>
        ActionReady(action) &&
        (!LevelChecked(Traits.ChargedActionMastery) && GetRemainingCharges(action) is 1 ||
         LevelChecked(Traits.ChargedActionMastery) && GetRemainingCharges(action) is 2) &&
        GetCooldownChargeRemainingTime(action) < 25;

    private static bool OvercapGaussRound =>
        IsOvercapping(OriginalHook(GaussRound)) ||
        ActionReady(OriginalHook(GaussRound)) &&
        !LevelChecked(Hypercharge) &&
        GetRemainingCharges(OriginalHook(GaussRound)) is 2;

    private static bool OvercapRicochet =>
        IsOvercapping(OriginalHook(Ricochet));

    private static bool CanGaussRound =>
        ActionReady(OriginalHook(GaussRound)) &&
        GetRemainingCharges(OriginalHook(GaussRound)) >= GetRemainingCharges(OriginalHook(Ricochet));

    private static bool CanRicochet =>
        ActionReady(OriginalHook(Ricochet)) &&
        GetRemainingCharges(OriginalHook(Ricochet)) > GetRemainingCharges(OriginalHook(GaussRound));

    private static bool OvercapGaussRicochetProtection(out uint action, bool allowRicochet = true)
    {
        action = 0;

        if (OvercapGaussRound)
        {
            action = OriginalHook(GaussRound);
            return true;
        }

        if (allowRicochet && OvercapRicochet)
        {
            action = OriginalHook(Ricochet);
            return true;
        }

        return false;
    }

    private static bool GaussRicochetWeaves(out uint action, bool onAoE, bool duringHypercharge,
        bool enabled = true, int gaussOnlyOrBoth = 0, int chargePool = 0)
    {
        action = 0;

        if (!enabled)
            return false;

        if (duringHypercharge)
        {
            if (!JustUsedOverheatGCD(1f, onAoE) || HasWeaved())
                return false;
        }
        else if (!onAoE && !JustUsedTool(2f))
            return false;

        const float spacing = 2f;

        if (gaussOnlyOrBoth == 1)
        {
            if (HasCharges(GaussRound) && !LevelChecked(DoubleCheck))
            {
                action = GaussRound;
                return true;
            }

            return false;
        }

        if (GetRemainingCharges(OriginalHook(GaussRound)) > chargePool &&
            (CanGaussRound || !LevelChecked(Ricochet)) &&
            (duringHypercharge || !JustUsed(OriginalHook(GaussRound), spacing) || !LevelChecked(Ricochet)))
        {
            action = OriginalHook(GaussRound);
            return true;
        }

        if (GetRemainingCharges(OriginalHook(Ricochet)) > chargePool &&
            CanRicochet && (duringHypercharge || !JustUsed(OriginalHook(Ricochet), spacing)))
        {
            action = OriginalHook(Ricochet);
            return true;
        }

        return false;
    }

    #endregion

    #region HP Threshold

    private static int BossHpThreshold(int hpBossOption, int hpOption, bool isBoss) =>
        hpBossOption == 1 || !isBoss ? hpOption : 0;

    private static int ReassembleHPThreshold =>
        BossHpThreshold(MCH_ST_ReassembleHPBossOption, MCH_ST_ReassembleHPOption, TargetIsBoss());

    private static int HyperchargeHPThreshold =>
        BossHpThreshold(MCH_ST_HyperchargeHPBossOption, MCH_ST_HyperchargeHPOption, TargetIsBoss());

    private static int QueenHPThreshold =>
        BossHpThreshold(MCH_ST_QueenHPBossOption, MCH_ST_QueenHPOption, InBossEncounter());

    private static int ToolsHPThreshold =>
        BossHpThreshold(MCH_ST_ToolsHPBossOption, MCH_ST_ToolsHPOption, TargetIsBoss());

    private static int BarrelStabilizerHPThreshold =>
        BossHpThreshold(MCH_ST_BarrelStabilizerHPBossOption, MCH_ST_BarrelStabilizerHPOption, TargetIsBoss());

    private static int WildfireHPThreshold =>
        BossHpThreshold(MCH_ST_WildfireHPBossOption, MCH_ST_WildfireHPOption, TargetIsBoss());

    #endregion

    #region Tools

    private static bool IsBelowMaxCharges(uint actionId) =>
        GetMaxCharges(actionId) > 1 && GetRemainingCharges(actionId) < GetMaxCharges(actionId);

    private static float GetToolCDRemaining(uint actionId) =>
        IsBelowMaxCharges(actionId)
            ? GetCooldownChargeRemainingTime(actionId)
            : GetCooldownRemainingTime(actionId);

    private static bool ToolReadyForReassembleWeave(uint actionId) =>
        LevelChecked(actionId) &&
        (HasCharges(actionId) || GetToolCDRemaining(actionId) <= GCD * 0.5f);

    private static bool ToolReadyForReassemble(uint actionId, bool forPrepull) =>
        forPrepull ? ActionReady(actionId) : ToolReadyForReassembleWeave(actionId);

    private static bool CanUseDrill(bool onAoE) =>
        !onAoE || !LevelChecked(BioBlaster);

    private static bool IsChargedToolCD(uint actionId, float time = 9f)
    {
        if (!LevelChecked(actionId))
            return true;

        if (HasCharges(actionId) && !IsBelowMaxCharges(actionId))
            return false;

        return GetToolCDRemaining(actionId) >= time;
    }

    private static bool IsDrillCD(float time = 9f) => IsChargedToolCD(Drill, time);

    private static bool IsBioBlasterCD(float time = 9f) => IsChargedToolCD(BioBlaster, time);

    private static bool IsAirAnchorCD(float time = 9f) =>
        !LevelChecked(OriginalHook(HotShot)) ||
        GetCooldownRemainingTime(OriginalHook(HotShot)) >= time;

    private static bool IsChainSawCD(float time = 9f) =>
        !LevelChecked(Chainsaw) ||
        GetCooldownRemainingTime(Chainsaw) >= time;

    private static bool JustUsedTool(float window) =>
        JustUsed(OriginalHook(AirAnchor), window) ||
        JustUsed(Chainsaw, window) ||
        JustUsed(Drill, window) ||
        JustUsed(Excavator, window);

    private static bool CanUseTools(ref uint actionID, bool onAoE, bool useAirAnchor = true, bool holdExcavatorForWildfire = false)
    {
        if (ActionReady(Chainsaw) && !HasStatusEffect(Buffs.ExcavatorReady))
        {
            actionID = Chainsaw;
            return true;
        }

        if (ActionReady(Excavator) && HasStatusEffect(Buffs.ExcavatorReady) &&
            (onAoE || !holdExcavatorForWildfire ||
             GetStatusEffectRemainingTime(Buffs.ExcavatorReady) <= GCD * 3))
        {
            actionID = Excavator;
            return true;
        }

        if ((!onAoE || useAirAnchor) && ActionReady(AirAnchor))
        {
            actionID = AirAnchor;
            return true;
        }

        if (onAoE &&
            ActionReady(BioBlaster) &&
            !HasStatusEffect(Debuffs.Bioblaster, CurrentTarget) &&
            CanApplyStatus(CurrentTarget, Debuffs.Bioblaster))
        {
            actionID = BioBlaster;
            return true;
        }

        if (CanUseDrill(onAoE) && ActionReady(Drill))
        {
            actionID = Drill;
            return true;
        }

        if (onAoE &&
            HasStatusEffect(Buffs.Reassembled) &&
            ActionReady(OriginalHook(SpreadShot)))
        {
            actionID = OriginalHook(SpreadShot);
            return true;
        }

        if (!onAoE && !LevelChecked(AirAnchor) && ActionReady(HotShot) &&
            (!LevelChecked(CleanShot) || !HasStatusEffect(Buffs.Reassembled)))
        {
            actionID = HotShot;
            return true;
        }

        return false;
    }

    #endregion

    #region Combos

    private static float GCD => GetCooldown(OriginalHook(SplitShot)).CooldownTotal;

    private static unsafe bool IsComboExpiring(float times)
    {
        float gcd = GCD * times;

        return ActionManager.Instance()->Combo.Timer != 0 && ActionManager.Instance()->Combo.Timer < gcd;
    }

    private static uint ContinueBasicCombo(
        bool onAoE = false,
        bool allowReassembleOnClean = false,
        int reassembleChoice = 1,
        int chargePool = 0,
        int hpThreshold = 25)
    {
        if (onAoE)
            return OriginalHook(SpreadShot);

        if (ComboTimer > 0)
        {
            if (ComboAction is SplitShot && ActionReady(OriginalHook(SlugShot)))
                return OriginalHook(SlugShot);

            if (ComboAction is SlugShot && ActionReady(OriginalHook(CleanShot)))
            {
                if (allowReassembleOnClean && CanReassemble(false, reassembleChoice, chargePool, hpThreshold))
                    return Reassemble;

                return OriginalHook(CleanShot);
            }
        }

        return OriginalHook(SplitShot);
    }

    private static uint DoBasicCombo(
        bool onAoE = false,
        bool allowReassembleOnClean = false,
        int reassembleChoice = 1,
        int chargePool = 0,
        int hpThreshold = 25) =>
        ContinueBasicCombo(onAoE, allowReassembleOnClean, reassembleChoice, chargePool, hpThreshold);

    #endregion

    #region Openers

    internal static WrathOpener Opener()
    {
        if (Lvl100StandardOpener.LevelChecked &&
            MCH_SelectedOpener == 0)
            return Lvl100StandardOpener;

        if (Lvl100EarlyWFOpener.LevelChecked &&
            MCH_SelectedOpener == 1)
            return Lvl100EarlyWFOpener;

        if (Lvl90EarlyTools.LevelChecked)
            return Lvl90EarlyTools;

        return WrathOpener.Dummy;
    }

    internal static MCHLvl90EarlyToolsOpener Lvl90EarlyTools = new();
    internal static MCHLvl100EarlyWFOpener Lvl100EarlyWFOpener = new();
    internal static MCHLvl100StandardOpener Lvl100StandardOpener = new();

    internal class MCHLvl100StandardOpener : WrathOpener
    {
        public override int MinOpenerLevel => 100;

        public override int MaxOpenerLevel => 100;

        public override List<uint> OpenerActions { get; set; } =
        [
            Reassemble,
            Items.UseItem(Items.GetStrongestPotionRow(Items.PotionType.Dex)),
            AirAnchor,
            CheckMate,
            DoubleCheck,
            Drill,
            BarrelStabilizer,
            Chainsaw,
            Excavator,
            AutomatonQueen,
            Reassemble,
            Drill,
            CheckMate,
            Wildfire,
            FullMetalField,
            Hypercharge,
            DoubleCheck,
            BlazingShot,
            CheckMate,
            BlazingShot,
            DoubleCheck,
            BlazingShot,
            CheckMate,
            BlazingShot,
            DoubleCheck,
            BlazingShot,
            CheckMate,
            Drill,
            DoubleCheck,
            CheckMate,
            HeatedSplitShot,
            DoubleCheck,
            HeatedSlugShot,
            HeatedCleanShot
        ];

        public override Preset Preset => Preset.MCH_ST_Adv_Opener;

        internal override UserData ContentCheckConfig => MCH_Balance_Content;
        internal override bool IncludePot => MCH_Opener_Potion;

        public override List<(int[] Steps, Func<int> HoldDelay)> PrepullDelays { get; set; } =
        [
            ([2], () => 3)
        ];

        public override bool HasCooldowns() =>
            GetRemainingCharges(Reassemble) is 2 &&
            GetRemainingCharges(OriginalHook(GaussRound)) is 3 &&
            GetRemainingCharges(OriginalHook(Ricochet)) is 3 &&
            IsOffCooldown(Chainsaw) &&
            IsOffCooldown(Wildfire) &&
            IsOffCooldown(BarrelStabilizer) &&
            IsOffCooldown(Excavator) &&
            IsOffCooldown(FullMetalField);
    }

    internal class MCHLvl100EarlyWFOpener : WrathOpener
    {
        public override int MinOpenerLevel => 100;

        public override int MaxOpenerLevel => 100;

        public override List<uint> OpenerActions { get; set; } =
        [
            Reassemble,
            Items.UseItem(Items.GetStrongestPotionRow(Items.PotionType.Dex)),
            AirAnchor,
            CheckMate,
            DoubleCheck,
            Drill,
            BarrelStabilizer,
            Reassemble,
            Chainsaw,
            DoubleCheck,
            Wildfire,
            Excavator,
            Hypercharge,
            AutomatonQueen,
            BlazingShot,
            CheckMate,
            BlazingShot,
            DoubleCheck,
            BlazingShot,
            CheckMate,
            BlazingShot,
            DoubleCheck,
            BlazingShot,
            CheckMate,
            Drill,
            DoubleCheck,
            CheckMate,
            FullMetalField,
            DoubleCheck,
            CheckMate,
            Drill,
            HeatedSplitShot,
            HeatedSlugShot,
            HeatedCleanShot
        ];

        public override Preset Preset => Preset.MCH_ST_Adv_Opener;

        internal override UserData ContentCheckConfig => MCH_Balance_Content;
        internal override bool IncludePot => MCH_Opener_Potion;

        public override List<(int[] Steps, Func<int> HoldDelay)> PrepullDelays { get; set; } =
        [
            ([2], () => 3)
        ];

        public override bool HasCooldowns() =>
            GetRemainingCharges(Reassemble) is 2 &&
            GetRemainingCharges(OriginalHook(GaussRound)) is 3 &&
            GetRemainingCharges(OriginalHook(Ricochet)) is 3 &&
            IsOffCooldown(Chainsaw) &&
            IsOffCooldown(Wildfire) &&
            IsOffCooldown(BarrelStabilizer) &&
            IsOffCooldown(Excavator) &&
            IsOffCooldown(FullMetalField);
    }

    internal class MCHLvl90EarlyToolsOpener : WrathOpener
    {
        public override int MinOpenerLevel => 90;

        public override int MaxOpenerLevel => 90;

        public override List<uint> OpenerActions { get; set; } =
        [
            Reassemble,
            AirAnchor,
            GaussRound,
            Ricochet,
            Drill,
            BarrelStabilizer,
            Chainsaw,
            GaussRound,
            Ricochet,
            HeatedSplitShot,
            GaussRound,
            Ricochet,
            HeatedSlugShot,
            Wildfire,
            HeatedCleanShot,
            AutomatonQueen,
            Hypercharge,
            BlazingShot,
            Ricochet,
            BlazingShot,
            GaussRound,
            BlazingShot,
            Ricochet,
            BlazingShot,
            GaussRound,
            BlazingShot,
            Reassemble,
            Drill
        ];

        public override Preset Preset => Preset.MCH_ST_Adv_Opener;

        internal override UserData ContentCheckConfig => MCH_Balance_Content;
        internal override bool IncludePot => MCH_Opener_Potion;

        public override List<(int[] Steps, Func<int> HoldDelay)> PrepullDelays { get; set; } =
        [
            ([2], () => 4)
        ];

        public override List<int> DelayedWeaveSteps { get; set; } =
        [
            14
        ];

        public override bool HasCooldowns() =>
            GetRemainingCharges(Reassemble) is 2 &&
            GetRemainingCharges(OriginalHook(GaussRound)) is 3 &&
            GetRemainingCharges(OriginalHook(Ricochet)) is 3 &&
            IsOffCooldown(Chainsaw) &&
            IsOffCooldown(Wildfire) &&
            IsOffCooldown(BarrelStabilizer);
    }

    #endregion

    #region Gauge

    private static MCHGauge Gauge => GetJobGauge<MCHGauge>();

    private static bool IsOverheated => Gauge.IsOverheated;

    private static bool IsRobotActive => Gauge.IsRobotActive;

    private static byte Heat => Gauge.Heat;

    private static byte Battery => Gauge.Battery;

    #endregion

    #region ID's

    public const uint
        CleanShot = 2873,
        HeatedCleanShot = 7413,
        SplitShot = 2866,
        HeatedSplitShot = 7411,
        SlugShot = 2868,
        HeatedSlugShot = 7412,
        GaussRound = 2874,
        Ricochet = 2890,
        Reassemble = 2876,
        Drill = 16498,
        HotShot = 2872,
        AirAnchor = 16500,
        Hypercharge = 17209,
        Heatblast = 7410,
        SpreadShot = 2870,
        Scattergun = 25786,
        AutoCrossbow = 16497,
        RookAutoturret = 2864,
        RookOverdrive = 7415,
        AutomatonQueen = 16501,
        QueenOverdrive = 16502,
        Tactician = 16889,
        Chainsaw = 25788,
        BioBlaster = 16499,
        BarrelStabilizer = 7414,
        Wildfire = 2878,
        Dismantle = 2887,
        Flamethrower = 7418,
        BlazingShot = 36978,
        DoubleCheck = 36979,
        CheckMate = 36980,
        Excavator = 36981,
        FullMetalField = 36982;

    public static class Buffs
    {
        public const ushort
            Reassembled = 851,
            Tactician = 1951,
            Wildfire = 1946,
            Overheated = 2688,
            Flamethrower = 1205,
            Hypercharged = 3864,
            ExcavatorReady = 3865,
            FullMetalMachinist = 3866;
    }

    public static class Debuffs
    {
        public const ushort
            Dismantled = 860,
            Wildfire = 861,
            Bioblaster = 1866;
    }

    public static class Traits
    {
        public const ushort
            EnhancedMultiWeapon = 605,
            ChargedActionMastery = 292;
    }

    #endregion
}
