using Dalamud.Game.ClientState.Objects.Types;
using System;
using System.Linq;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using WrathCombo.Data;
using WrathCombo.Extensions;
using static WrathCombo.Combos.PvE.PLD.Config;
using BossAvoidance = WrathCombo.Combos.PvE.All.Enums.BossAvoidance;

namespace WrathCombo.Combos.PvE;

internal partial class PLD : Tank
{
    #region Simple Modes

    internal class PLD_ST_SimpleMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.PLD_ST_SimpleMode;
        internal static int RoyalAuthorityCount => ActionWatching.CombatActions.Count(x => x == OriginalHook(RageOfHalone));

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not FastBlade)
                return actionID;

            if (IsEnabled(Preset.PLD_BlockForWings) && (HasStatusEffect(Buffs.PassageOfArms) || JustUsed(PassageOfArms)))
                return All.SavageBlade;

            // Interrupt
            if (Role.CanInterject())
                return Role.Interject;

            // Stun
            if (Role.CanLowBlow())
                return Role.LowBlow;

            if (ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            #region Mitigations

            if (PLD_ST_MitOptions == 0)
            {
                // Mitigation
                if (IsPlayerTargeted() &&
                    !JustMitted && InCombat())
                {
                    // Hallowed Ground
                    if (ActionReady(HallowedGround) &&
                        PlayerHealthPercentageHp() < 30)
                        return HallowedGround;

                    // Sheltron
                    if (LevelChecked(Sheltron) &&
                        Gauge.OathGauge >= 50 &&
                        PlayerHealthPercentageHp() < 95 &&
                        !HasStatusEffect(Buffs.Sheltron) &&
                        !HasStatusEffect(Buffs.HolySheltron))
                        return OriginalHook(Sheltron);

                    // Reprisal
                    if (Role.CanReprisal() && RaidWideCasting(5f))
                        return Role.Reprisal;

                    // Divine Veil
                    if (ActionReady(DivineVeil) && RaidWideCasting(5f) &&
                        NumberOfAlliesInRange(DivineVeil) >= GetPartyMembers().Count * .75 &&
                        !HasStatusEffect(Role.Debuffs.Reprisal, CurrentTarget, true))
                        return OriginalHook(DivineVeil);

                    // Sentinel / Guardian
                    if (ActionReady(OriginalHook(Sentinel)) &&
                        PlayerHealthPercentageHp() < 50)
                        return OriginalHook(Sentinel);

                    // Bulwark
                    if (ActionReady(Bulwark) &&
                        PlayerHealthPercentageHp() < 60)
                        return Bulwark;
                }
            }

            #endregion

            if (HasBattleTarget())
            {
                // Weavables
                if (CanWeave())
                {
                    // Requiescat
                    if (ActionReady(Requiescat) && CooldownFightOrFlight > 50 &&
                        InActionRange(OriginalHook(Requiescat))) //Moved out of melee range check bc Imperator is 25y but req is melee range. 
                        return OriginalHook(Requiescat);

                    if (InMeleeRange())
                    {
                        // Fight or Flight
                        if (CanFightOrFlight)
                        {
                            if (!LevelChecked(Requiescat))
                            {
                                if (!LevelChecked(RageOfHalone))
                                {
                                    // Level 2-25
                                    if (ComboAction is FastBlade)
                                        return OriginalHook(FightOrFlight);
                                }

                                // Level 26-67
                                else if (ComboAction is RiotBlade)
                                    return OriginalHook(FightOrFlight);
                            }

                            // Level 68+
                            else if (CooldownRequiescat < 0.5f && HasRequiescatMPSimple &&
                                     !HasWeaved() && (ComboAction is RoyalAuthority || LevelChecked(BladeOfFaith) && RoyalAuthorityCount > 0))
                                return OriginalHook(FightOrFlight);
                        }

                        // Circle of Scorn / Spirits Within
                        switch (CooldownFightOrFlight)
                        {
                            case > 15 when ActionReady(CircleOfScorn):
                                return CircleOfScorn;

                            case > 15 when ActionReady(SpiritsWithin):
                                return OriginalHook(SpiritsWithin);
                        }
                    }

                    // Blade of Honor
                    if (LevelChecked(BladeOfHonor) &&
                        OriginalHook(Requiescat) == BladeOfHonor)
                        return OriginalHook(Requiescat);
                }

                // Requiescat Phase
                if (HasDivineMagicMP)
                {
                    // Confiteor & Blades
                    if (HasStatusEffect(Buffs.ConfiteorReady) || LevelChecked(BladeOfFaith) && OriginalHook(Confiteor) != Confiteor)
                        return OriginalHook(Confiteor);

                    // Pre-Blades
                    if (HasRequiescat)
                        return HolySpirit;
                }

                // Goring Blade
                if (HasStatusEffect(Buffs.GoringBladeReady) && InMeleeRange())
                    return GoringBlade;

                switch (HasDivineMight)
                {
                    // Holy Spirit Prioritization
                    case true when HasDivineMagicMP:
                    {
                        // Delay Sepulchre / Prefer Sepulchre
                        if (InAtonementFinisher && (CooldownFightOrFlight < 3 || DurationFightOrFlight > 3))
                            return HolySpirit;

                        // Fit in Burst
                        if (!InAtonementFinisher && HasFightOrFlight && DurationFightOrFlight < 3)
                            return HolySpirit;
                        break;
                    }
                }

                // Atonement: During Burst / Before Expiring / Spend Starter / Before Refreshing
                if (InAtonementPhase && InMeleeRange() &&
                    (InBurstWindow || IsAtonementExpiring || InAtonementStarter || ComboAction is RiotBlade))
                    return OriginalHook(Atonement);

                // Holy Spirit: During Burst / Before Expiring / Outside Melee / Before Refreshing
                if (HasDivineMight && HasDivineMagicMP &&
                    (InBurstWindow || IsDivineMightExpiring || !InMeleeRange() || ComboAction is RiotBlade))
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
        protected internal override Preset Preset => Preset.PLD_AoE_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not TotalEclipse)
                return actionID;

            if (IsEnabled(Preset.PLD_BlockForWings) && (HasStatusEffect(Buffs.PassageOfArms) || JustUsed(PassageOfArms)))
                return All.SavageBlade;

            // Interrupt
            if (Role.CanInterject())
                return Role.Interject;

            // Stun
            if (Role.CanLowBlow())
                return Role.LowBlow;

            if (ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            if (PLD_AoE_MitOptions == 0)
            {
                // Mitigation
                if (IsEnabled(Preset.PLD_AoE_AdvancedMode_Mitigation) &&
                    IsPlayerTargeted() && !JustMitted && InCombat())
                {
                    // Hallowed Ground
                    if (ActionReady(HallowedGround) &&
                        PlayerHealthPercentageHp() < PLD_AoE_HallowedGround_Health)
                        return HallowedGround;

                    // Sheltron
                    if (LevelChecked(Sheltron) &&
                        Gauge.OathGauge >= 50 &&
                        PlayerHealthPercentageHp() < 95 &&
                        !HasStatusEffect(Buffs.Sheltron) &&
                        !HasStatusEffect(Buffs.HolySheltron))
                        return OriginalHook(Sheltron);

                    // Reprisal
                    if (Role.CanReprisal(80, 3, false))
                        return Role.Reprisal;

                    // Divine Veil
                    if (ActionReady(DivineVeil) &&
                        NumberOfAlliesInRange(DivineVeil) >= GetPartyMembers().Count * .75 &&
                        PlayerHealthPercentageHp() < 75)
                        return DivineVeil;

                    // Rampart
                    if (Role.CanRampart(50))
                        return Role.Rampart;

                    // Arm's Length
                    if (Role.CanArmsLength(3))
                        return Role.ArmsLength;

                    // Bulwark
                    if (ActionReady(Bulwark) &&
                        PlayerHealthPercentageHp() < 60)
                        return Bulwark;

                    // Sentinel / Guardian
                    if (ActionReady(OriginalHook(Sentinel)) &&
                        PlayerHealthPercentageHp() < 50)
                        return OriginalHook(Sentinel);
                }
            }

            if (HasBattleTarget())
            {
                // Weavables
                if (CanWeave())
                {
                    // Requiescat
                    if (ActionReady(Requiescat) && CooldownFightOrFlight > 50 && InActionRange(OriginalHook(Requiescat)))
                        return OriginalHook(Requiescat);

                    if (InMeleeRange())
                    {
                        // Fight or Flight
                        if (CanFightOrFlight &&
                            (!LevelChecked(Requiescat) ||
                             CooldownRequiescat < 0.5f && HasRequiescatMPSimple && !HasWeaved()))
                            return OriginalHook(FightOrFlight);

                        switch (CooldownFightOrFlight)
                        {
                            case > 15 when ActionReady(CircleOfScorn):
                                return CircleOfScorn;

                            case > 15 when ActionReady(SpiritsWithin):
                                return OriginalHook(SpiritsWithin);
                        }
                    }

                    // Blade of Honor
                    if (LevelChecked(BladeOfHonor) &&
                        OriginalHook(Requiescat) == BladeOfHonor)
                        return OriginalHook(Requiescat);
                }

                // Confiteor & Blades
                if (HasDivineMagicMP &&
                    (HasStatusEffect(Buffs.ConfiteorReady) ||
                     LevelChecked(BladeOfFaith) && OriginalHook(Confiteor) != Confiteor))
                    return OriginalHook(Confiteor);
            }

            // Holy Circle
            if (LevelChecked(HolyCircle) &&
                HasDivineMagicMP && (HasDivineMight || HasRequiescat))
                return HolyCircle;

            // Basic Combo
            if (ComboTimer > 0 && ComboAction is TotalEclipse && LevelChecked(Prominence))
                return Prominence;

            return actionID;
        }
    }

    #endregion

    #region Advanced Modes

    internal class PLD_ST_AdvancedMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.PLD_ST_AdvancedMode;
        internal static int RoyalAuthorityCount => ActionWatching.CombatActions.Count(x => x == OriginalHook(RageOfHalone));

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not FastBlade)
                return actionID;

            if (IsEnabled(Preset.PLD_BlockForWings) && (HasStatusEffect(Buffs.PassageOfArms) || JustUsed(PassageOfArms)))
                return All.SavageBlade;

            //Opener
            if (IsEnabled(Preset.PLD_ST_AdvancedMode_BalanceOpener) &&
                Opener().FullOpener(ref actionID))
                return actionID;

            // Interrupt
            if (IsEnabled(Preset.PLD_ST_Interrupt)
                && Role.CanInterject())
                return Role.Interject;

            // Stun
            if (CanStunToInterruptEnemy())
                if (IsEnabled(Preset.PLD_ST_ShieldBash) &&
                    ActionReady(ShieldBash) &&
                    !JustUsedOn(ShieldBash, CurrentTarget, 10))
                    return ShieldBash;

                else if (IsEnabled(Preset.PLD_ST_LowBlow) && Role.CanLowBlow())
                    return Role.LowBlow;

            if (ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            if (HasBattleTarget())
            {
                // Weavables
                if (CanWeave())
                {
                    // Requiescat
                    if (IsEnabled(Preset.PLD_ST_AdvancedMode_Requiescat) &&
                        ActionReady(Requiescat) && CooldownFightOrFlight > 50 && InActionRange(OriginalHook(Requiescat)))
                        return OriginalHook(Requiescat);
                    if (InMeleeRange())
                    {
                        // Fight or Flight
                        if (IsEnabled(Preset.PLD_ST_AdvancedMode_FoF) &&
                            CanFightOrFlight && GetTargetHPPercent() > HPThresholdFoF)
                        {
                            if (!LevelChecked(Requiescat))
                            {
                                if (!LevelChecked(RageOfHalone))
                                {
                                    // Level 2-25
                                    if (ComboAction is FastBlade)
                                        return OriginalHook(FightOrFlight);
                                }

                                // Level 26-67
                                else if (ComboAction is RiotBlade)
                                    return OriginalHook(FightOrFlight);
                            }

                            // Level 68+
                            else if (CooldownRequiescat < 0.5f && HasRequiescatMPAdv && !HasWeaved() &&
                                     (ComboAction is RoyalAuthority || LevelChecked(BladeOfFaith) && RoyalAuthorityCount > 0))
                                return OriginalHook(FightOrFlight);
                        }

                        switch (CooldownFightOrFlight)
                        {
                            // Circle of Scorn / Spirits Within
                            case > 15 when IsEnabled(Preset.PLD_ST_AdvancedMode_CircleOfScorn) &&
                                           ActionReady(CircleOfScorn):
                                return CircleOfScorn;

                            case > 15 when IsEnabled(Preset.PLD_ST_AdvancedMode_SpiritsWithin) &&
                                           ActionReady(SpiritsWithin):
                                return OriginalHook(SpiritsWithin);
                        }
                    }

                    // Intervene
                    if (IsEnabled(Preset.PLD_ST_AdvancedMode_Intervene) &&
                        ActionReady(Intervene) && CooldownFightOrFlight > 40 &&
                        GetRemainingCharges(Intervene) > PLD_ST_Intervene_Charges && !JustUsed(Intervene) &&
                        (PLD_ST_Intervene_Movement == 1 ||
                         PLD_ST_Intervene_Movement == 0 && !IsMoving() && TimeStoodStill > TimeSpan.FromSeconds(PLD_ST_InterveneTimeStill)) &&
                        GetTargetDistance() <= PLD_ST_Intervene_Distance)
                        return Intervene;

                    // Blade of Honor
                    if (IsEnabled(Preset.PLD_ST_AdvancedMode_BladeOfHonor) &&
                        LevelChecked(BladeOfHonor) && OriginalHook(Requiescat) == BladeOfHonor)
                        return OriginalHook(Requiescat);

                    // Mitigation
                    if (IsEnabled(Preset.PLD_ST_AdvancedMode_Mitigation) &&
                        InMitigationContent && IsPlayerTargeted() &&
                        !JustMitted && InCombat())
                    {
                        // Hallowed Ground
                        if (IsEnabled(Preset.PLD_ST_AdvancedMode_HallowedGround) &&
                            ActionReady(HallowedGround) &&
                            PlayerHealthPercentageHp() < PLD_ST_HallowedGround_Health &&
                            (PLD_ST_MitHallowedGroundBoss == (int)BossAvoidance.On && InBossEncounter() ||
                             PLD_ST_MitHallowedGroundBoss == (int)BossAvoidance.Off))
                            return HallowedGround;

                        // Sheltron
                        if (IsEnabled(Preset.PLD_ST_AdvancedMode_Sheltron) &&
                            LevelChecked(Sheltron) &&
                            Gauge.OathGauge >= PLD_ST_SheltronOption &&
                            PlayerHealthPercentageHp() < PLD_ST_Sheltron_Health &&
                            !HasStatusEffect(Buffs.Sheltron) && !HasStatusEffect(Buffs.HolySheltron) &&
                            (PLD_ST_MitSheltronBoss == (int)BossAvoidance.On && InBossEncounter() ||
                             PLD_ST_MitSheltronBoss == (int)BossAvoidance.Off))
                            return OriginalHook(Sheltron);

                        // Reprisal
                        if (IsEnabled(Preset.PLD_ST_AdvancedMode_Reprisal) &&
                            Role.CanReprisal() && RaidWideCasting(5f))
                            return Role.Reprisal;

                        // Divine Veil
                        if (IsEnabled(Preset.PLD_ST_AdvancedMode_DivineVeil) &&
                            ActionReady(DivineVeil) && RaidWideCasting(5f) &&
                            NumberOfAlliesInRange(DivineVeil) >= GetPartyMembers().Count * .75 &&
                            (IsNotEnabled(Preset.PLD_ST_AdvancedMode_DivineVeilAvoid) ||
                             !HasStatusEffect(Role.Debuffs.Reprisal, CurrentTarget, true)))
                            return OriginalHook(DivineVeil);

                        // Sentinel / Guardian
                        if (IsEnabled(Preset.PLD_ST_AdvancedMode_Sentinel) &&
                            ActionReady(OriginalHook(Sentinel)) &&
                            PlayerHealthPercentageHp() < PLD_ST_Sentinel_Health)
                            return OriginalHook(Sentinel);

                        // Bulwark
                        if (IsEnabled(Preset.PLD_ST_AdvancedMode_Bulwark) &&
                            ActionReady(Bulwark) &&
                            PlayerHealthPercentageHp() < PLD_ST_Bulwark_Health)
                            return Bulwark;
                    }
                }

                // Requiescat Phase
                switch (HasDivineMagicMP)
                {
                    // Confiteor & Blades
                    case true when IsEnabled(Preset.PLD_ST_AdvancedMode_Confiteor) && HasStatusEffect(Buffs.ConfiteorReady) ||
                                   IsEnabled(Preset.PLD_ST_AdvancedMode_Blades) && LevelChecked(BladeOfFaith) && OriginalHook(Confiteor) != Confiteor:
                        return OriginalHook(Confiteor);

                    // Pre-Blades
                    case true when (IsEnabled(Preset.PLD_ST_AdvancedMode_Confiteor) || IsEnabled(Preset.PLD_ST_AdvancedMode_Blades)) && HasRequiescat:
                        return HolySpirit;
                }

                // Goring Blade
                if (IsEnabled(Preset.PLD_ST_AdvancedMode_GoringBlade) &&
                    HasStatusEffect(Buffs.GoringBladeReady) && InMeleeRange())
                    return GoringBlade;

                // Holy Spirit Prioritization
                if (IsEnabled(Preset.PLD_ST_AdvancedMode_HolySpirit) &&
                    HasDivineMight && HasDivineMagicMP && IsAboveMPReserveST)
                {
                    switch (InAtonementFinisher)
                    {
                        // Delay Sepulchre / Prefer Sepulchre
                        case true when (CooldownFightOrFlight < 3 || DurationFightOrFlight > 3):

                        // Fit in Burst
                        case false when HasFightOrFlight && DurationFightOrFlight < 3:
                            return HolySpirit;
                    }
                }

                // Atonement: During Burst / Before Expiring / Spend Starter / Before Refreshing
                if (IsEnabled(Preset.PLD_ST_AdvancedMode_Atonement) &&
                    InAtonementPhase && InMeleeRange() &&
                    (InBurstWindow || IsAtonementExpiring || InAtonementStarter || ComboAction is RiotBlade))
                    return OriginalHook(Atonement);

                // Holy Spirit: During Burst / Before Expiring / Outside Melee / Before Refreshing
                if (IsEnabled(Preset.PLD_ST_AdvancedMode_HolySpirit) &&
                    HasDivineMight && HasDivineMagicMP && IsAboveMPReserveST &&
                    (InBurstWindow || IsDivineMightExpiring || !InMeleeRange() || ComboAction is RiotBlade))
                    return HolySpirit;

                // Out of Range
                if (IsEnabled(Preset.PLD_ST_AdvancedMode_ShieldLob) &&
                    !InMeleeRange())
                {
                    // Holy Spirit (Not Moving)
                    if (LevelChecked(HolySpirit) &&
                        HasDivineMagicMP && IsAboveMPReserveST &&
                        TimeMoving.Ticks == 0 &&
                        PLD_ST_ShieldLob_SubOption == 1)
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
        protected internal override Preset Preset => Preset.PLD_AoE_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not TotalEclipse)
                return actionID;

            if (IsEnabled(Preset.PLD_BlockForWings) && (HasStatusEffect(Buffs.PassageOfArms) || JustUsed(PassageOfArms, 0.5f)))
                return All.SavageBlade;

            // Interrupt
            if (IsEnabled(Preset.PLD_AoE_Interrupt)
                && Role.CanInterject())
                return Role.Interject;

            // Stun
            if (CanStunToInterruptEnemy())
                if (IsEnabled(Preset.PLD_AoE_ShieldBash) &&
                    ActionReady(ShieldBash) && !JustUsedOn(ShieldBash, CurrentTarget, 10))
                    return ShieldBash;

                else if (IsEnabled(Preset.PLD_AoE_LowBlow) &&
                         Role.CanLowBlow())
                    return Role.LowBlow;

            if (ContentSpecificActions.TryGet(out uint contentAction))
                return contentAction;

            if (HasBattleTarget())
            {
                // Weavables
                if (CanWeave())
                {
                    // Requiescat
                    if (IsEnabled(Preset.PLD_AoE_AdvancedMode_Requiescat) &&
                        ActionReady(Requiescat) && CooldownFightOrFlight > 50 && InActionRange(OriginalHook(Requiescat)))
                        return OriginalHook(Requiescat);

                    if (InMeleeRange())
                    {
                        // Fight or Flight
                        if (IsEnabled(Preset.PLD_AoE_AdvancedMode_FoF) &&
                            CanFightOrFlight && GetTargetHPPercent() >= PLD_AoE_FoF_Trigger &&
                            (!LevelChecked(Requiescat) || CooldownRequiescat < 0.5f && HasRequiescatMPAdvAoE && !HasWeaved()))
                            return OriginalHook(FightOrFlight);

                        // Circle of Scorn / Spirits Within
                        switch (CooldownFightOrFlight)
                        {
                            case > 15 when IsEnabled(Preset.PLD_AoE_AdvancedMode_CircleOfScorn) && ActionReady(CircleOfScorn):
                                return CircleOfScorn;

                            case > 15 when IsEnabled(Preset.PLD_AoE_AdvancedMode_SpiritsWithin) && ActionReady(SpiritsWithin):
                                return OriginalHook(SpiritsWithin);
                        }
                    }

                    // Intervene
                    if (IsEnabled(Preset.PLD_AoE_AdvancedMode_Intervene) &&
                        ActionReady(Intervene) && CooldownFightOrFlight > 40 &&
                        GetRemainingCharges(Intervene) > PLD_AoE_Intervene_Charges && !JustUsed(Intervene) &&
                        (PLD_AoE_Intervene_Movement == 1 ||
                         PLD_AoE_Intervene_Movement == 0 && !IsMoving() && TimeStoodStill > TimeSpan.FromSeconds(PLD_AoE_InterveneTimeStill)) &&
                        GetTargetDistance() <= PLD_AoE_Intervene_Distance)
                        return Intervene;

                    // Blade of Honor
                    if (IsEnabled(Preset.PLD_AoE_AdvancedMode_BladeOfHonor) &&
                        LevelChecked(BladeOfHonor) && OriginalHook(Requiescat) == BladeOfHonor)
                        return OriginalHook(Requiescat);

                    // Mitigation
                    if (IsEnabled(Preset.PLD_AoE_AdvancedMode_Mitigation) &&
                        IsPlayerTargeted() && !JustMitted && InCombat())
                    {
                        // Hallowed Ground
                        if (IsEnabled(Preset.PLD_AoE_AdvancedMode_HallowedGround) &&
                            ActionReady(HallowedGround) &&
                            PlayerHealthPercentageHp() < PLD_AoE_HallowedGround_Health)
                            return HallowedGround;

                        // Sheltron
                        if (IsEnabled(Preset.PLD_AoE_AdvancedMode_Sheltron) &&
                            LevelChecked(Sheltron) &&
                            Gauge.OathGauge >= PLD_AoE_SheltronOption &&
                            PlayerHealthPercentageHp() < PLD_AoE_Sheltron_Health &&
                            !HasStatusEffect(Buffs.Sheltron) &&
                            !HasStatusEffect(Buffs.HolySheltron))
                            return OriginalHook(Sheltron);

                        // Reprisal
                        if (IsEnabled(Preset.PLD_AoE_AdvancedMode_Reprisal) &&
                            Role.CanReprisal(PLD_AoE_Reprisal_Health, PLD_AoE_Reprisal_Count, false))
                            return Role.Reprisal;

                        // Divine Veil
                        if (IsEnabled(Preset.PLD_AoE_AdvancedMode_DivineVeil) &&
                            ActionReady(DivineVeil) &&
                            NumberOfAlliesInRange(DivineVeil) >= GetPartyMembers().Count * .75 &&
                            PlayerHealthPercentageHp() < PLD_AoE_DivineVeil_Health)
                            return DivineVeil;

                        // Rampart
                        if (IsEnabled(Preset.PLD_AoE_AdvancedMode_Rampart) &&
                            Role.CanRampart(PLD_AoE_Rampart_Health))
                            return Role.Rampart;

                        // Arm's Length
                        if (IsEnabled(Preset.PLD_AoE_AdvancedMode_ArmsLength) &&
                            Role.CanArmsLength(PLD_AoE_ArmsLength_Count))
                            return Role.ArmsLength;

                        // Bulwark
                        if (IsEnabled(Preset.PLD_AoE_AdvancedMode_Bulwark) &&
                            ActionReady(Bulwark) &&
                            PlayerHealthPercentageHp() < PLD_AoE_Bulwark_Health)
                            return Bulwark;

                        // Sentinel / Guardian
                        if (IsEnabled(Preset.PLD_AoE_AdvancedMode_Sentinel) &&
                            ActionReady(OriginalHook(Sentinel)) &&
                            PlayerHealthPercentageHp() < PLD_AoE_Sentinel_Health)
                            return OriginalHook(Sentinel);
                    }
                }

                // Confiteor & Blades
                if (HasDivineMagicMP && (IsEnabled(Preset.PLD_AoE_AdvancedMode_Confiteor) && HasStatusEffect(Buffs.ConfiteorReady) ||
                                         IsEnabled(Preset.PLD_AoE_AdvancedMode_Blades) && LevelChecked(BladeOfFaith) && OriginalHook(Confiteor) != Confiteor))
                    return OriginalHook(Confiteor);
            }

            // Holy Circle
            if (LevelChecked(HolyCircle) && HasDivineMagicMP &&
                (IsEnabled(Preset.PLD_AoE_AdvancedMode_HolyCircle) && IsAboveMPReserveAoE && HasDivineMight ||
                 (IsEnabled(Preset.PLD_AoE_AdvancedMode_Confiteor) || IsEnabled(Preset.PLD_AoE_AdvancedMode_Blades)) && HasRequiescat))
                return HolyCircle;

            // Basic Combo
            if (ComboTimer > 0 && ComboAction is TotalEclipse && LevelChecked(Prominence))
                return Prominence;

            return actionID;
        }
    }

    #endregion

    #region Standalone Features

    internal class PLD_ST_BasicCombo : CustomCombo
    {
        protected internal override Preset Preset => Preset.PLD_ST_BasicCombo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (RageOfHalone or RoyalAuthority))
                return actionID;

            if (ComboTimer > 0)
            {
                if (ComboAction is FastBlade && LevelChecked(RiotBlade))
                    return RiotBlade;

                if (ComboAction is RiotBlade && LevelChecked(RageOfHalone))
                    return OriginalHook(RageOfHalone);
            }

            return FastBlade;
        }
    }

    internal class PLD_Requiescat_Confiteor : CustomCombo
    {
        protected internal override Preset Preset => Preset.PLD_Requiescat_Options;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Requiescat or Imperator))
                return actionID;

            bool canFightOrFlight = OriginalHook(FightOrFlight) is FightOrFlight && ActionReady(FightOrFlight);

            // Fight or Flight
            if (PLD_Requiescat_SubOption == 2 && (!LevelChecked(Requiescat) || canFightOrFlight && ActionReady(Requiescat)))
                return OriginalHook(FightOrFlight);

            // Confiteor & Blades
            if (HasStatusEffect(Buffs.ConfiteorReady) || LevelChecked(BladeOfFaith) && OriginalHook(Confiteor) != Confiteor)
                return OriginalHook(Confiteor);

            // Pre-Blades
            return HasStatusEffect(Buffs.Requiescat)
                // AoE
                ? LevelChecked(HolyCircle) && NumberOfEnemiesInRange(HolyCircle) > 2
                    ? HolyCircle
                    : HolySpirit
                : actionID;
        }
    }

    internal class PLD_CircleOfScorn : CustomCombo
    {
        protected internal override Preset Preset => Preset.PLD_SpiritsWithin;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (SpiritsWithin or Expiacion))
                return actionID;

            if (IsOffCooldown(OriginalHook(SpiritsWithin)))
                return OriginalHook(SpiritsWithin);

            if (ActionReady(CircleOfScorn) &&
                (PLD_SpiritsWithin_SubOption == 1 ||
                 PLD_SpiritsWithin_SubOption == 2 && JustUsed(OriginalHook(SpiritsWithin), 5f)))
                return CircleOfScorn;

            return actionID;
        }
    }

    internal class PLD_ShieldLob_HolySpirit : CustomCombo
    {
        protected internal override Preset Preset => Preset.PLD_ShieldLob_Feature;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not ShieldLob)
                return actionID;

            if (LevelChecked(HolySpirit) && GetResourceCost(HolySpirit) <= LocalPlayer.CurrentMp && (TimeMoving.Ticks == 0 || HasStatusEffect(Buffs.DivineMight)))
                return HolySpirit;

            return actionID;
        }
    }

    internal class PLD_RetargetClemency : CustomCombo
    {
        protected internal override Preset Preset => Preset.PLD_RetargetClemency;

        protected override uint Invoke(uint actionID)
        {
            if (actionID != Clemency)
                return actionID;

            int healthThreshold = PLD_RetargetClemency_Health;

            IGameObject? target =
                //Mouseover retarget option
                (IsEnabled(Preset.PLD_RetargetClemency_MO)
                    ? SimpleTarget.UIMouseOverTarget.IfNotThePlayer().IfInParty()
                    : null) ??

                //Hard target
                SimpleTarget.HardTarget.IfFriendly() ??

                //Lowest HP option
                (IsEnabled(Preset.PLD_RetargetClemency_LowHP)
                 && PlayerHealthPercentageHp() > healthThreshold
                    ? SimpleTarget.LowestHPAlly.IfNotThePlayer().IfAlive()
                    : null);

            return target != null
                ? actionID.Retarget(target)
                : actionID;
        }
    }

    internal class PLD_RetargetSheltron : CustomCombo
    {
        protected internal override Preset Preset => Preset.PLD_RetargetSheltron;

        protected override uint Invoke(uint action)
        {
            if (action is not (Sheltron or HolySheltron))
                return action;

            IGameObject? target =
                //Mouseover retarget option
                (IsEnabled(Preset.PLD_RetargetSheltron_MO)
                    ? SimpleTarget.UIMouseOverTarget.IfNotThePlayer().IfInParty()
                    : null) ??

                //Hard target retarget
                SimpleTarget.HardTarget.IfNotThePlayer().IfInParty() ??

                //Targets target retarget option
                (IsEnabled(Preset.PLD_RetargetSheltron_TT)
                 && !PlayerHasAggro
                    ? SimpleTarget.TargetsTarget.IfNotThePlayer().IfInParty()
                    : null);

            // Intervention if trying to Buff an ally
            if (ActionReady(Intervention) &&
                target != null &&
                CanApplyStatus(target, Buffs.Intervention))
                return Intervention.Retarget([Sheltron, HolySheltron], target);

            return action;
        }
    }

    #region One-Button Mitigation

    internal class PLD_Mit_OneButton : CustomCombo
    {
        protected internal override Preset Preset => Preset.PLD_Mit_OneButton;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Bulwark)
                return actionID;

            if (IsEnabled(Preset.PLD_Mit_HallowedGround_Max) &&
                ActionReady(HallowedGround) &&
                PlayerHealthPercentageHp() <= PLD_Mit_HallowedGround_Max_Health &&
                ContentCheck.IsInConfiguredContent(
                    PLD_Mit_HallowedGround_Max_Difficulty,
                    PLD_Mit_HallowedGround_Max_DifficultyListSet
                ))
                return HallowedGround;

            foreach(int priority in PLD_Mit_Priorities.OrderBy(x => x))
            {
                int index = PLD_Mit_Priorities.IndexOf(priority);
                if (CheckMitigationConfigMeetsRequirements(index, out uint action))
                    return action;
            }

            return actionID;
        }
    }

    internal class PLD_Mit_OneButton_Party : CustomCombo
    {
        protected internal override Preset Preset => Preset.PLD_Mit_Party;

        protected override uint Invoke(uint action)
        {
            if (action is not DivineVeil)
                return action;

            if (Role.CanReprisal())
                return Role.Reprisal;

            if (ActionReady(DivineVeil))
                return DivineVeil;

            if (ActionReady(PassageOfArms) &&
                IsEnabled(Preset.PLD_Mit_Party_Wings) &&
                !HasStatusEffect(Buffs.PassageOfArms, anyOwner: true))
                return PassageOfArms;

            return action;
        }
    }

    #endregion

    internal class PLD_RetargetShieldBash : CustomCombo
    {
        protected internal override Preset Preset => Preset.PLD_RetargetShieldBash;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not ShieldBash)
                return actionID;

            IGameObject? tar = SimpleTarget.StunnableEnemy(PLD_RetargetStunLockout ? PLD_RetargetShieldBash_Strength : 3);

            if (tar is not null)
                return ShieldBash.Retarget(actionID, tar);

            if (PLD_RetargetStunLockout)
                return All.SavageBlade;

            return actionID;
        }
    }

    internal class PLD_RetargetCover : CustomCombo
    {
        protected internal override Preset Preset => Preset.PLD_RetargetCover;

        protected override uint Invoke(uint actionID)
        {
            if (actionID != Cover)
                return actionID;

            int healthThreshold = PLD_RetargetCover_Health;

            IGameObject? target =
                //Mouseover retarget option
                (IsEnabled(Preset.PLD_RetargetCover_MO)
                    ? SimpleTarget.UIMouseOverTarget.IfNotThePlayer().IfInParty()
                    : null) ??

                //Hard target
                SimpleTarget.HardTarget.IfNotThePlayer().IfInParty() ??

                //Lowest HP option
                (IsEnabled(Preset.PLD_RetargetCover_LowHP)
                 && SimpleTarget.LowestHPPAlly.HPP < healthThreshold
                    ? SimpleTarget.LowestHPPAlly.IfNotThePlayer().IfInParty()
                    : null);

            return target != null
                ? actionID.Retarget(target)
                : actionID;
        }
    }

    #endregion
}
