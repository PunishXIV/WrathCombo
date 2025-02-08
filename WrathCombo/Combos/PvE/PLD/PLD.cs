using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;
using System.Linq;
using WrathCombo.Combos.PvE.Content;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Data;

namespace WrathCombo.Combos.PvE;

internal partial class PLD
{
    private static PLDGauge Gauge => CustomComboFunctions.GetJobGauge<PLDGauge>();

    internal class PLD_ST_SimpleMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PLD_ST_SimpleMode;
        internal static int RoyalAuthorityCount => ActionWatching.CombatActions.Count(x => x == OriginalHook(RageOfHalone));

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not FastBlade)
                return actionID;

            #region Variables
            float durationFightOrFlight = GetBuffRemainingTime(Buffs.FightOrFlight);
            float cooldownFightOrFlight = GetCooldownRemainingTime(FightOrFlight);
            float cooldownRequiescat = GetCooldownRemainingTime(Requiescat);
            uint playerMP = LocalPlayer.CurrentMp;
            bool canWeave = CanWeave();
            bool canEarlyWeave = CanWeave(1.5f);
            bool hasRequiescat = HasEffect(Buffs.Requiescat);
            bool hasDivineMight = HasEffect(Buffs.DivineMight);
            bool hasFightOrFlight = HasEffect(Buffs.FightOrFlight);
            bool hasDivineMagicMP = playerMP >= GetResourceCost(HolySpirit);
            bool hasRequiescatMP = playerMP >= GetResourceCost(HolySpirit) * 3.6;
            bool inBurstWindow = JustUsed(FightOrFlight, 30f);
            bool inAtonementStarter = HasEffect(Buffs.AtonementReady);
            bool inAtonementFinisher = HasEffect(Buffs.SepulchreReady);
            bool afterOpener = LevelChecked(BladeOfFaith) && RoyalAuthorityCount > 0;
            bool inAtonementPhase = HasEffect(Buffs.AtonementReady) || HasEffect(Buffs.SupplicationReady) || HasEffect(Buffs.SepulchreReady);
            bool isDivineMightExpiring = GetBuffRemainingTime(Buffs.DivineMight) < 6;
            bool isAtonementExpiring = (HasEffect(Buffs.AtonementReady) && GetBuffRemainingTime(Buffs.AtonementReady) < 6) ||
                                       (HasEffect(Buffs.SupplicationReady) && GetBuffRemainingTime(Buffs.SupplicationReady) < 6) ||
                                       (HasEffect(Buffs.SepulchreReady) && GetBuffRemainingTime(Buffs.SepulchreReady) < 6);
            bool justMitted = JustUsed(OriginalHook(Sheltron), 3f) ||
                             JustUsed(OriginalHook(Sentinel), 4f) ||
                             JustUsed(DivineVeil, 4f) ||
                             JustUsed(All.Rampart, 4f) ||
                             JustUsed(HallowedGround, 9f);
            #endregion

            // Interrupt
            if (ActionReady(All.Interject)
                && CanInterruptEnemy())
                return All.Interject;

            // Variant Cure
            if (IsEnabled(CustomComboPreset.PLD_Variant_Cure) && IsEnabled(Variant.VariantCure) &&
                PlayerHealthPercentageHp() <= Config.PLD_VariantCure)
                return Variant.VariantCure;

            #region Mitigations

            if (Config.PLD_ST_MitsOptions != 1)
            {
                if (InCombat() && //Player is in combat
                    !justMitted) //Player has not used a mitigation ability in the last 4-9 seconds
                {
                    //HallowedGround
                    if (ActionReady(HallowedGround) && //HallowedGround is ready
                        PlayerHealthPercentageHp() < 30) //Player's health is below 30%
                        return HallowedGround;

                    if (IsPlayerTargeted())
                    {
                        //Sentinel / Damnation
                        if (ActionReady(OriginalHook(Sentinel)) && //Sentinel is ready
                            PlayerHealthPercentageHp() < 60) //Player's health is below 60%
                            return OriginalHook(Sentinel);

                        //Rampart
                        if (ActionReady(All.Rampart) && //Rampart is ready
                            PlayerHealthPercentageHp() < 80) //Player's health is below 80%
                            return All.Rampart;

                        //Reprisal
                        if (ActionReady(All.Reprisal) && //Reprisal is ready
                            InActionRange(All.Reprisal) && //Target is within range of Reprisal
                            PlayerHealthPercentageHp() < 90) //Player's health is below 80%
                            return All.Reprisal;
                    }

                    //Bulwark
                    if (ActionReady(Bulwark) && //Bulwark is ready
                        PlayerHealthPercentageHp() < 70) //Player's health is below 80%
                        return Bulwark;

                    //Sheltron
                    if (ActionReady(OriginalHook(Sheltron)) && //Sheltron
                        PlayerHealthPercentageHp() < 90) //Player's health is below 95%
                        return OriginalHook(Sheltron);
                }
            }
            #endregion

            if (HasBattleTarget())
            {
                // Variant DoT Check
                Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);

                // Weavables
                if (canWeave)
                {
                    if (InMeleeRange())
                    {
                        // Requiescat
                        if (ActionReady(Requiescat) && cooldownFightOrFlight > 50)
                            return OriginalHook(Requiescat);

                        // Fight or Flight
                        if (ActionReady(FightOrFlight))
                        {
                            if (!LevelChecked(Requiescat))
                            {
                                if (!LevelChecked(RageOfHalone))
                                {
                                    // Level 2-25
                                    if (ComboAction is FastBlade)
                                        return FightOrFlight;
                                }

                                // Level 26-67
                                else if (ComboAction is RiotBlade)
                                    return FightOrFlight;
                            }

                            // Level 68+
                            else if (cooldownRequiescat < 0.5f && hasRequiescatMP && canEarlyWeave && (ComboAction is RoyalAuthority || afterOpener))
                                return FightOrFlight;
                        }

                        // Variant Ultimatum
                        if (IsEnabled(CustomComboPreset.PLD_Variant_Ultimatum) && IsEnabled(Variant.VariantUltimatum) &&
                            IsOffCooldown(Variant.VariantUltimatum))
                            return Variant.VariantUltimatum;

                        // Circle of Scorn / Spirits Within
                        if (cooldownFightOrFlight > 15)
                        {
                            if (ActionReady(CircleOfScorn))
                                return CircleOfScorn;

                            if (ActionReady(SpiritsWithin))
                                return OriginalHook(SpiritsWithin);
                        }
                    }

                    // Variant Spirit Dart
                    if (IsEnabled(CustomComboPreset.PLD_Variant_SpiritDart) && IsEnabled(Variant.VariantSpiritDart) &&
                        (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3))
                        return Variant.VariantSpiritDart;

                    // Blade of Honor
                    if (LevelChecked(BladeOfHonor) && OriginalHook(Requiescat) == BladeOfHonor)
                        return OriginalHook(Requiescat);
                }

                // Requiescat Phase
                if (hasDivineMagicMP)
                {
                    // Confiteor & Blades
                    if (HasEffect(Buffs.ConfiteorReady) || (LevelChecked(BladeOfFaith) && OriginalHook(Confiteor) != Confiteor))
                        return OriginalHook(Confiteor);

                    // Pre-Blades
                    if (hasRequiescat)
                        return HolySpirit;
                }

                // Goring Blade
                if (HasEffect(Buffs.GoringBladeReady) && InMeleeRange())
                    return GoringBlade;

                // Holy Spirit Prioritization
                if (hasDivineMight && hasDivineMagicMP)
                {
                    // Delay Sepulchre / Prefer Sepulchre
                    if (inAtonementFinisher && (cooldownFightOrFlight < 3 || durationFightOrFlight > 3))
                        return HolySpirit;

                    // Fit in Burst
                    if (!inAtonementFinisher && hasFightOrFlight && durationFightOrFlight < 3)
                        return HolySpirit;
                }

                // Atonement: During Burst / Before Expiring / Spend Starter / Before Refreshing
                if (inAtonementPhase && InMeleeRange() && (inBurstWindow || isAtonementExpiring || inAtonementStarter || ComboAction is RiotBlade))
                    return OriginalHook(Atonement);

                // Holy Spirit: During Burst / Before Expiring / Outside Melee / Before Refreshing
                if (hasDivineMight && hasDivineMagicMP && (inBurstWindow || isDivineMightExpiring || !InMeleeRange() || ComboAction is RiotBlade))
                    return HolySpirit;

                // Out of Range
                if (LevelChecked(ShieldLob) && !InMeleeRange())
                    return ShieldLob;
            }

            // Basic Combo
            if (ComboTimer > 0)
            {
                if (ComboAction is FastBlade && LevelChecked(RiotBlade))
                    return RiotBlade;

                if (ComboAction is RiotBlade && LevelChecked(RageOfHalone))
                    return OriginalHook(RageOfHalone);
            }

            return actionID;
        }
    }

    internal class PLD_AoE_SimpleMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PLD_AoE_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not TotalEclipse)
                return actionID;

            #region Variables
            float cooldownFightOrFlight = GetCooldownRemainingTime(FightOrFlight);
            float cooldownRequiescat = GetCooldownRemainingTime(Requiescat);
            uint playerMP = LocalPlayer.CurrentMp;
            bool canWeave = CanWeave();
            bool canEarlyWeave = CanWeave(1.5f);
            bool hasRequiescat = HasEffect(Buffs.Requiescat);
            bool hasDivineMight = HasEffect(Buffs.DivineMight);
            bool hasDivineMagicMP = playerMP >= GetResourceCost(HolySpirit);
            bool hasRequiescatMP = playerMP >= GetResourceCost(HolySpirit) * 3.6;
            bool justMitted = JustUsed(OriginalHook(Sheltron), 3f) ||
                             JustUsed(OriginalHook(Sentinel), 4f) ||
                             JustUsed(DivineVeil, 4f) ||
                             JustUsed(All.Rampart, 4f) ||
                             JustUsed(HallowedGround, 9f);
            #endregion

            // Interrupt
            if (ActionReady(All.Interject)
                && CanInterruptEnemy())
                return All.Interject;

            // Stun
            if (TargetIsCasting())
                if (ActionReady(ShieldBash))
                    return ShieldBash;
                else if (ActionReady(All.LowBlow))
                    return All.LowBlow;

            // Variant Cure
            if (IsEnabled(CustomComboPreset.PLD_Variant_Cure) && IsEnabled(Variant.VariantCure) &&
                PlayerHealthPercentageHp() <= Config.PLD_VariantCure)
                return Variant.VariantCure;

            if (Config.PLD_AoE_MitsOptions != 1)
            {
                if (InCombat() && //Player is in combat
                    !justMitted) //Player has not used a mitigation ability in the last 4-9 seconds
                {
                    //Hallowed Ground
                    if (ActionReady(HallowedGround) && //Hallowed Ground is ready
                        PlayerHealthPercentageHp() < 30) //Player's health is below 30%
                        return HallowedGround;

                    if (IsPlayerTargeted())
                    {
                        //Sentinel / Guardian
                        if (ActionReady(OriginalHook(Sentinel)) && //Sentinel is ready
                            PlayerHealthPercentageHp() < 60) //Player's health is below 60%
                            return OriginalHook(Sentinel);

                        //Rampart
                        if (ActionReady(All.Rampart) && //Rampart is ready
                            PlayerHealthPercentageHp() < 80) //Player's health is below 80%
                            return All.Rampart;

                        //Reprisal
                        if (ActionReady(All.Reprisal) && //Reprisal is ready
                            InActionRange(All.Reprisal) && //Target is within range of Reprisal
                            PlayerHealthPercentageHp() < 90) //Player's health is below 80%
                            return All.Reprisal;
                    }

                    //Bulwark
                    if (ActionReady(Bulwark) && //Bulwark is ready
                        PlayerHealthPercentageHp() < 70) //Player's health is below 80%
                        return Bulwark;

                    //Sheltron
                    if (ActionReady(OriginalHook(Sheltron)) && //Sheltron
                        PlayerHealthPercentageHp() < 90) //Player's health is below 95%
                        return OriginalHook(Sheltron);
                }
            }

            if (HasBattleTarget())
            {
                // Variant DoT Check
                Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);

                // Weavables
                if (canWeave)
                {
                    if (InMeleeRange())
                    {
                        // Requiescat
                        if (ActionReady(Requiescat) && cooldownFightOrFlight > 50)
                            return OriginalHook(Requiescat);

                        // Fight or Flight
                        if (ActionReady(FightOrFlight) && ((cooldownRequiescat < 0.5f && hasRequiescatMP && canEarlyWeave) || !LevelChecked(Requiescat)))
                            return FightOrFlight;

                        // Variant Ultimatum
                        if (IsEnabled(CustomComboPreset.PLD_Variant_Ultimatum) && IsEnabled(Variant.VariantUltimatum) &&
                            IsOffCooldown(Variant.VariantUltimatum))
                            return Variant.VariantUltimatum;

                        // Circle of Scorn / Spirits Within
                        if (cooldownFightOrFlight > 15)
                        {
                            if (ActionReady(CircleOfScorn))
                                return CircleOfScorn;

                            if (ActionReady(SpiritsWithin))
                                return OriginalHook(SpiritsWithin);
                        }
                    }

                    // Variant Spirit Dart
                    if (IsEnabled(CustomComboPreset.PLD_Variant_SpiritDart) && IsEnabled(Variant.VariantSpiritDart) &&
                        (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3))
                        return Variant.VariantSpiritDart;

                    // Blade of Honor
                    if (LevelChecked(BladeOfHonor) && OriginalHook(Requiescat) == BladeOfHonor)
                        return OriginalHook(Requiescat);
                }

                // Confiteor & Blades
                if (hasDivineMagicMP && (HasEffect(Buffs.ConfiteorReady) || (LevelChecked(BladeOfFaith) && OriginalHook(Confiteor) != Confiteor)))
                    return OriginalHook(Confiteor);
            }

            // Holy Circle
            if (LevelChecked(HolyCircle) && hasDivineMagicMP && (hasDivineMight || hasRequiescat))
                return HolyCircle;

            // Basic Combo
            if (ComboTimer > 0 && ComboAction is TotalEclipse && LevelChecked(Prominence))
                return Prominence;

            return actionID;
        }
    }

    internal class PLD_ST_AdvancedMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PLD_ST_AdvancedMode;
        internal static int RoyalAuthorityCount => ActionWatching.CombatActions.Count(x => x == OriginalHook(RageOfHalone));

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not FastBlade)
                return actionID;

            #region Variables
            float durationFightOrFlight = GetBuffRemainingTime(Buffs.FightOrFlight);
            float cooldownFightOrFlight = GetCooldownRemainingTime(FightOrFlight);
            float cooldownRequiescat = GetCooldownRemainingTime(Requiescat);
            uint playerMP = LocalPlayer.CurrentMp;
            bool canWeave = CanWeave();
            bool canEarlyWeave = CanWeave(1.5f);
            bool hasRequiescat = HasEffect(Buffs.Requiescat);
            bool hasDivineMight = HasEffect(Buffs.DivineMight);
            bool hasFightOrFlight = HasEffect(Buffs.FightOrFlight);
            bool hasDivineMagicMP = playerMP >= GetResourceCost(HolySpirit);
            bool hasJustUsedMitigation = JustUsed(OriginalHook(Sheltron), 3f) || JustUsed(OriginalHook(Sentinel), 5f) ||
                                         JustUsed(All.Rampart, 5f) || JustUsed(HallowedGround, 9f);
            bool hasRequiescatMP = (IsNotEnabled(CustomComboPreset.PLD_ST_AdvancedMode_MP_Reserve) && playerMP >= GetResourceCost(HolySpirit) * 3.6) ||
                                   (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_MP_Reserve) && playerMP >= (GetResourceCost(HolySpirit) * 3.6) + Config.PLD_ST_MP_Reserve);
            bool inBurstWindow = JustUsed(FightOrFlight, 30f);
            bool inAtonementStarter = HasEffect(Buffs.AtonementReady);
            bool inAtonementFinisher = HasEffect(Buffs.SepulchreReady);
            bool afterOpener = LevelChecked(BladeOfFaith) && RoyalAuthorityCount > 0;
            bool isDivineMightExpiring = GetBuffRemainingTime(Buffs.DivineMight) < 6;
            bool isAboveMPReserve = IsNotEnabled(CustomComboPreset.PLD_ST_AdvancedMode_MP_Reserve) ||
                                    (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_MP_Reserve) && playerMP >= GetResourceCost(HolySpirit) + Config.PLD_ST_MP_Reserve);
            bool inAtonementPhase = HasEffect(Buffs.AtonementReady) || HasEffect(Buffs.SupplicationReady) || HasEffect(Buffs.SepulchreReady);
            bool isAtonementExpiring = (HasEffect(Buffs.AtonementReady) && GetBuffRemainingTime(Buffs.AtonementReady) < 6) ||
                                       (HasEffect(Buffs.SupplicationReady) && GetBuffRemainingTime(Buffs.SupplicationReady) < 6) ||
                                       (HasEffect(Buffs.SepulchreReady) && GetBuffRemainingTime(Buffs.SepulchreReady) < 6);
            #endregion

            // Interrupt
            if (IsEnabled(CustomComboPreset.PLD_ST_Interrupt)
                && ActionReady(All.Interject)
                && CanInterruptEnemy())
                return All.Interject;

            // Variant Cure
            if (IsEnabled(CustomComboPreset.PLD_Variant_Cure) && IsEnabled(Variant.VariantCure) &&
                PlayerHealthPercentageHp() <= Config.PLD_VariantCure)
                return Variant.VariantCure;

            if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_BalanceOpener) &&
                Opener().FullOpener(ref actionID))
                return actionID;

            if (HasBattleTarget())
            {
                // Variant DoT Check
                Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);

                // Weavables
                if (canWeave)
                {
                    if (InMeleeRange())
                    {
                        // Requiescat
                        if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Requiescat) && ActionReady(Requiescat) && cooldownFightOrFlight > 50)
                            return OriginalHook(Requiescat);

                        // Fight or Flight
                        if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_FoF) && ActionReady(FightOrFlight) && GetTargetHPPercent() >= Config.PLD_ST_FoF_Trigger)
                        {
                            if (!LevelChecked(Requiescat))
                            {
                                if (!LevelChecked(RageOfHalone))
                                {
                                    // Level 2-25
                                    if (ComboAction is FastBlade)
                                        return FightOrFlight;
                                }

                                // Level 26-67
                                else if (ComboAction is RiotBlade)
                                    return FightOrFlight;
                            }

                            // Level 68+
                            else if (cooldownRequiescat < 0.5f && hasRequiescatMP && canEarlyWeave && (ComboAction is RoyalAuthority || afterOpener))
                                return FightOrFlight;
                        }

                        // Variant Ultimatum
                        if (IsEnabled(CustomComboPreset.PLD_Variant_Ultimatum) && IsEnabled(Variant.VariantUltimatum) &&
                            IsOffCooldown(Variant.VariantUltimatum))
                            return Variant.VariantUltimatum;

                        // Circle of Scorn / Spirits Within
                        if (cooldownFightOrFlight > 15)
                        {
                            if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_CircleOfScorn) && ActionReady(CircleOfScorn))
                                return CircleOfScorn;

                            if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_SpiritsWithin) && ActionReady(SpiritsWithin))
                                return OriginalHook(SpiritsWithin);
                        }
                    }

                    // Variant Spirit Dart
                    if (IsEnabled(CustomComboPreset.PLD_Variant_SpiritDart) && IsEnabled(Variant.VariantSpiritDart) &&
                        (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3))
                        return Variant.VariantSpiritDart;

                    // Intervene
                    if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Intervene) && LevelChecked(Intervene) && TimeMoving.Ticks == 0 &&
                        cooldownFightOrFlight > 40 && GetRemainingCharges(Intervene) > Config.PLD_Intervene_HoldCharges && !WasLastAction(Intervene) &&
                        ((Config.PLD_Intervene_MeleeOnly == 1 && InMeleeRange()) || (GetTargetDistance() == 0 && Config.PLD_Intervene_MeleeOnly == 2)))
                        return Intervene;

                    // Blade of Honor
                    if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_BladeOfHonor) && LevelChecked(BladeOfHonor) && OriginalHook(Requiescat) == BladeOfHonor)
                        return OriginalHook(Requiescat);

                    // Mitigation
                    if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Mitigation) && IsPlayerTargeted() && !hasJustUsedMitigation && InCombat())
                    {
                        // Hallowed Ground
                        if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_HallowedGround) && ActionReady(HallowedGround) &&
                            PlayerHealthPercentageHp() < Config.PLD_ST_HallowedGround_Health && (Config.PLD_ST_HallowedGround_SubOption == 1 ||
                                (TargetIsBoss() && Config.PLD_ST_HallowedGround_SubOption == 2)))
                            return HallowedGround;

                        // Sentinel / Guardian
                        if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Sentinel) && ActionReady(OriginalHook(Sentinel)) &&
                            PlayerHealthPercentageHp() < Config.PLD_ST_Sentinel_Health && (Config.PLD_ST_Sentinel_SubOption == 1 ||
                                (TargetIsBoss() && Config.PLD_ST_Sentinel_SubOption == 2)))
                            return OriginalHook(Sentinel);

                        // Rampart
                        if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Rampart) && ActionReady(All.Rampart) &&
                            PlayerHealthPercentageHp() < Config.PLD_ST_Rampart_Health && (Config.PLD_ST_Rampart_SubOption == 1 ||
                                (TargetIsBoss() && Config.PLD_ST_Rampart_SubOption == 2)))
                            return All.Rampart;

                        // Sheltron
                        if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Sheltron) && LevelChecked(Sheltron) &&
                            Gauge.OathGauge >= Config.PLD_ST_SheltronOption && PlayerHealthPercentageHp() < 95 &&
                            !HasEffect(Buffs.Sheltron) && !HasEffect(Buffs.HolySheltron))
                            return OriginalHook(Sheltron);
                    }
                }

                // Requiescat Phase
                if (hasDivineMagicMP)
                {
                    // Confiteor & Blades
                    if ((IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Confiteor) && HasEffect(Buffs.ConfiteorReady)) ||
                        (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Blades) && LevelChecked(BladeOfFaith) && OriginalHook(Confiteor) != Confiteor))
                        return OriginalHook(Confiteor);

                    // Pre-Blades
                    if ((IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Confiteor) || IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Blades)) && hasRequiescat)
                        return HolySpirit;
                }

                // Goring Blade
                if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_GoringBlade) && HasEffect(Buffs.GoringBladeReady) && InMeleeRange())
                    return GoringBlade;

                // Holy Spirit Prioritization
                if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_HolySpirit) && hasDivineMight && hasDivineMagicMP && isAboveMPReserve)
                {
                    // Delay Sepulchre / Prefer Sepulchre
                    if (inAtonementFinisher && (cooldownFightOrFlight < 3 || durationFightOrFlight > 3))
                        return HolySpirit;

                    // Fit in Burst
                    if (!inAtonementFinisher && hasFightOrFlight && durationFightOrFlight < 3)
                        return HolySpirit;
                }

                // Atonement: During Burst / Before Expiring / Spend Starter / Before Refreshing
                if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_Atonement) && inAtonementPhase && InMeleeRange() &&
                    (inBurstWindow || isAtonementExpiring || inAtonementStarter || ComboAction is RiotBlade))
                    return OriginalHook(Atonement);

                // Holy Spirit: During Burst / Before Expiring / Outside Melee / Before Refreshing
                if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_HolySpirit) && hasDivineMight && hasDivineMagicMP && isAboveMPReserve &&
                    (inBurstWindow || isDivineMightExpiring || !InMeleeRange() || ComboAction is RiotBlade))
                    return HolySpirit;

                // Out of Range
                if (IsEnabled(CustomComboPreset.PLD_ST_AdvancedMode_ShieldLob) && !InMeleeRange())
                {
                    // Holy Spirit (Not Moving)
                    if (LevelChecked(HolySpirit) && hasDivineMagicMP && isAboveMPReserve && TimeMoving.Ticks == 0 && Config.PLD_ShieldLob_SubOption == 2)
                        return HolySpirit;

                    // Shield Lob
                    if (LevelChecked(ShieldLob))
                        return ShieldLob;
                }
            }

            // Basic Combo
            if (ComboTimer > 0)
            {
                if (ComboAction is FastBlade && LevelChecked(RiotBlade))
                    return RiotBlade;

                if (ComboAction is RiotBlade && LevelChecked(RageOfHalone))
                    return OriginalHook(RageOfHalone);
            }

            return actionID;
        }
    }

    internal class PLD_AoE_AdvancedMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PLD_AoE_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not TotalEclipse)
                return actionID;

            #region Variables
            float cooldownFightOrFlight = GetCooldownRemainingTime(FightOrFlight);
            float cooldownRequiescat = GetCooldownRemainingTime(Requiescat);
            uint playerMP = LocalPlayer.CurrentMp;
            bool canWeave = CanWeave();
            bool canEarlyWeave = CanWeave(1.5f);
            bool hasRequiescat = HasEffect(Buffs.Requiescat);
            bool hasDivineMight = HasEffect(Buffs.DivineMight);
            bool hasDivineMagicMP = playerMP >= GetResourceCost(HolySpirit);
            bool hasJustUsedMitigation = JustUsed(OriginalHook(Sheltron), 3f) || JustUsed(OriginalHook(Sentinel), 5f) ||
                                         JustUsed(All.Rampart, 5f) || JustUsed(HallowedGround, 9f);
            bool hasRequiescatMP = (IsNotEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_MP_Reserve) && playerMP >= GetResourceCost(HolySpirit) * 3.6) ||
                                   (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_MP_Reserve) && playerMP >= (GetResourceCost(HolySpirit) * 3.6) + Config.PLD_AoE_MP_Reserve);
            bool isAboveMPReserve = IsNotEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_MP_Reserve) ||
                                    (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_MP_Reserve) && playerMP >= GetResourceCost(HolySpirit) + Config.PLD_AoE_MP_Reserve);
            #endregion

            // Interrupt
            if (IsEnabled(CustomComboPreset.PLD_AoE_Interrupt)
                && ActionReady(All.Interject)
                && CanInterruptEnemy())
                return All.Interject;

            // Stun
            if (IsEnabled(CustomComboPreset.PLD_AoE_Stun) && TargetIsCasting())
                if (ActionReady(ShieldBash))
                    return ShieldBash;
                else if (ActionReady(All.LowBlow))
                    return All.LowBlow;

            // Variant Cure
            if (IsEnabled(CustomComboPreset.PLD_Variant_Cure) && IsEnabled(Variant.VariantCure) &&
                PlayerHealthPercentageHp() <= Config.PLD_VariantCure)
                return Variant.VariantCure;

            if (HasBattleTarget())
            {
                // Variant DoT Check
                Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);

                // Weavables
                if (canWeave)
                {
                    if (InMeleeRange())
                    {
                        // Requiescat
                        if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_Requiescat) && ActionReady(Requiescat) && cooldownFightOrFlight > 50)
                            return OriginalHook(Requiescat);

                        // Fight or Flight
                        if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_FoF) && ActionReady(FightOrFlight) && GetTargetHPPercent() >= Config.PLD_AoE_FoF_Trigger &&
                            ((cooldownRequiescat < 0.5f && hasRequiescatMP && canEarlyWeave) || !LevelChecked(Requiescat)))
                            return FightOrFlight;

                        // Variant Ultimatum
                        if (IsEnabled(CustomComboPreset.PLD_Variant_Ultimatum) && IsEnabled(Variant.VariantUltimatum) &&
                            IsOffCooldown(Variant.VariantUltimatum))
                            return Variant.VariantUltimatum;

                        // Circle of Scorn / Spirits Within
                        if (cooldownFightOrFlight > 15)
                        {
                            if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_CircleOfScorn) && ActionReady(CircleOfScorn))
                                return CircleOfScorn;

                            if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_SpiritsWithin) && ActionReady(SpiritsWithin))
                                return OriginalHook(SpiritsWithin);
                        }
                    }

                    // Variant Spirit Dart
                    if (IsEnabled(CustomComboPreset.PLD_Variant_SpiritDart) && IsEnabled(Variant.VariantSpiritDart) &&
                        (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3))
                        return Variant.VariantSpiritDart;

                    // Intervene
                    if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_Intervene) && LevelChecked(Intervene) && TimeMoving.Ticks == 0 &&
                        cooldownFightOrFlight > 40 && GetRemainingCharges(Intervene) > Config.PLD_AoE_Intervene_HoldCharges && !WasLastAction(Intervene) &&
                        ((Config.PLD_AoE_Intervene_MeleeOnly == 1 && InMeleeRange()) || (GetTargetDistance() == 0 && Config.PLD_AoE_Intervene_MeleeOnly == 2)))
                        return Intervene;

                    // Blade of Honor
                    if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_BladeOfHonor) && LevelChecked(BladeOfHonor) && OriginalHook(Requiescat) == BladeOfHonor)
                        return OriginalHook(Requiescat);

                    // Mitigation
                    if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_Mitigation) && IsPlayerTargeted() && !hasJustUsedMitigation && InCombat())
                    {
                        // Hallowed Ground
                        if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_HallowedGround) && ActionReady(HallowedGround) &&
                            PlayerHealthPercentageHp() < Config.PLD_AoE_HallowedGround_Health && (Config.PLD_AoE_HallowedGround_SubOption == 1 ||
                                (TargetIsBoss() && Config.PLD_AoE_HallowedGround_SubOption == 2)))
                            return HallowedGround;

                        // Sentinel / Guardian
                        if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_Sentinel) && ActionReady(OriginalHook(Sentinel)) &&
                            PlayerHealthPercentageHp() < Config.PLD_AoE_Sentinel_Health && (Config.PLD_AoE_Sentinel_SubOption == 1 ||
                                (TargetIsBoss() && Config.PLD_AoE_Sentinel_SubOption == 2)))
                            return OriginalHook(Sentinel);

                        // Rampart
                        if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_Rampart) && ActionReady(All.Rampart) &&
                            PlayerHealthPercentageHp() < Config.PLD_AoE_Rampart_Health && (Config.PLD_AoE_Rampart_SubOption == 1 ||
                                (TargetIsBoss() && Config.PLD_AoE_Rampart_SubOption == 2)))
                            return All.Rampart;

                        // Sheltron
                        if (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_Sheltron) && LevelChecked(Sheltron) &&
                            Gauge.OathGauge >= Config.PLD_AoE_SheltronOption && PlayerHealthPercentageHp() < 95 &&
                            !HasEffect(Buffs.Sheltron) && !HasEffect(Buffs.HolySheltron))
                            return OriginalHook(Sheltron);
                    }
                }

                // Confiteor & Blades
                if (hasDivineMagicMP && ((IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_Confiteor) && HasEffect(Buffs.ConfiteorReady)) ||
                                         (IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_Blades) && LevelChecked(BladeOfFaith) && OriginalHook(Confiteor) != Confiteor)))
                    return OriginalHook(Confiteor);
            }

            // Holy Circle
            if (LevelChecked(HolyCircle) && hasDivineMagicMP && ((IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_HolyCircle) && isAboveMPReserve && hasDivineMight) ||
                    ((IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_Confiteor) || IsEnabled(CustomComboPreset.PLD_AoE_AdvancedMode_Blades)) && hasRequiescat)))
                return HolyCircle;

            // Basic Combo
            if (ComboTimer > 0 && ComboAction is TotalEclipse && LevelChecked(Prominence))
                return Prominence;

            return actionID;
        }
    }

    internal class PLD_Requiescat_Confiteor : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PLD_Requiescat_Options;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Requiescat or Imperator))
                return actionID;

            // Fight or Flight
            if (Config.PLD_Requiescat_SubOption == 2 && ((ActionReady(FightOrFlight) && ActionReady(Requiescat)) || !LevelChecked(Requiescat)))
                return FightOrFlight;

            // Confiteor & Blades
            if (HasEffect(Buffs.ConfiteorReady) || (LevelChecked(BladeOfFaith) && OriginalHook(Confiteor) != Confiteor))
                return OriginalHook(Confiteor);

            // Pre-Blades
            if (HasEffect(Buffs.Requiescat))
            {
                // AoE
                if (LevelChecked(HolyCircle) && NumberOfEnemiesInRange(HolyCircle, null) > 2)
                    return HolyCircle;

                else
                    return HolySpirit;
            }

            return actionID;
        }
    }

    internal class PLD_CircleOfScorn : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PLD_SpiritsWithin;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (SpiritsWithin or Expiacion))
                return actionID;

            if (IsOffCooldown(OriginalHook(SpiritsWithin)))
                return OriginalHook(SpiritsWithin);

            if (ActionReady(CircleOfScorn) && (Config.PLD_SpiritsWithin_SubOption == 1 || (Config.PLD_SpiritsWithin_SubOption == 2 && JustUsed(OriginalHook(SpiritsWithin), 5f))))
                return CircleOfScorn;

            return actionID;
        }
    }

    internal class PLD_ShieldLob_HolySpirit : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PLD_ShieldLob_Feature;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not ShieldLob)
                return actionID;

            if (LevelChecked(HolySpirit) && GetResourceCost(HolySpirit) <= LocalPlayer.CurrentMp && (TimeMoving.Ticks == 0 || HasEffect(Buffs.DivineMight)))
                return HolySpirit;

            return actionID;
        }
    }

    #region One-Button Mitigation
    internal class PLD_Mit_OneButton : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PLD_Mit_OneButton;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Bulwark)
                return actionID;

            if (IsEnabled(CustomComboPreset.PLD_Mit_HallowedGround_Max) &&
                ActionReady(HallowedGround) &&
                PlayerHealthPercentageHp() <= Config.PLD_Mit_HallowedGround_Max_Health &&
                ContentCheck.IsInConfiguredContent(
                    Config.PLD_Mit_HallowedGround_Max_Difficulty,
                    Config.PLD_Mit_HallowedGround_Max_DifficultyListSet
                ))
                return HallowedGround;

            foreach (int priority in Config.PLD_Mit_Priorities.Items.OrderBy(x => x))
            {
                int index = Config.PLD_Mit_Priorities.IndexOf(priority);
                if (CheckMitigationConfigMeetsRequirements(index, out uint action))
                    return action;
            }

            return actionID;
        }
    }
    #endregion

}
