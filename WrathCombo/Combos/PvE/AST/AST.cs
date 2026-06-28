using Dalamud.Game.ClientState.Objects.Types;
using ECommons;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using System.Linq;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using WrathCombo.Extensions;
using WrathCombo.Native;
using static WrathCombo.Combos.PvE.AST.Config;
using EZ = ECommons.Throttlers.EzThrottler;
using TS = System.TimeSpan;
namespace WrathCombo.Combos.PvE;

internal partial class AST : Healer
{
    #region Simple Dps Combos
    internal class AST_ST_Simple_DPS : CustomCombo
    {
        protected internal override Preset Preset => Preset.AST_ST_Simple_DPS;
        protected override uint Invoke(uint actionID)
        {
            #region Variables
            bool actionFound = MaleficList.Contains(actionID);
            var replacedActions = MaleficList.ToArray();
            #endregion

            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.SingleTargetDPS, MaleficList.ToArray()))
                return actionID;

            if (CustomActionHelper.CustomActionEnabled(CustomActionType.SingleTargetDPS))
                replacedActions = [All.SingleTargetDPS];

            #region Out of combat
            // Out-of-combat Card Draw
            if (!InCombat())
            {
                if (ActionReady(OriginalHook(AstralDraw)) && HasNoDPSCard)
                    return OriginalHook(AstralDraw);
            }
            #endregion

            #region Special Content
            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;
            #endregion

            #region OGCDs

            if (ActionReady(Lightspeed) && InCombat() && IsMoving() &&
                !HasStatusEffect(Buffs.Lightspeed))
                return Lightspeed;

            if (CanWeave() && InCombat())
            {
                //Lucid Dreaming
                if (Role.CanLucidDream(6500))
                    return Role.LucidDreaming;

                //Play Card
                if (HasDPSCard && LevelChecked(Play1))
                    return OriginalHook(Play1).Retarget(replacedActions, CardResolver);

                //Minor Arcana / Lord of Crowns
                if (HasLord && HasBattleTarget() && LevelChecked(MinorArcana))
                    return OriginalHook(MinorArcana);

                //Card Draw
                if (ActionReady(OriginalHook(AstralDraw)) && HasNoDPSCard)
                    return OriginalHook(AstralDraw);

                //Divination
                if (ActionReady(Divination) && HasBattleTarget() &&
                    !HasDivination && !HasStatusEffect(Buffs.Divining) &&
                    (GetTargetHPPercent() >= 10 || InBossEncounter()) &&
                    StandStill)
                    return Divination;

                //Earthly Star
                if (!HasStatusEffect(Buffs.EarthlyDominance) &&
                    ActionReady(EarthlyStar) && StandStill &&
                    (GetTargetHPPercent() >= 10 || InBossEncounter()) &&
                    IsOffCooldown(EarthlyStar))
                    return EarthlyStar.Retarget(replacedActions, SimpleTarget.Self);

                //Oracle
                if (HasStatusEffect(Buffs.Divining))
                    return Oracle;
            }
            #endregion

            #region GCDS
            var dotAction = OriginalHook(Combust);
            CombustList.TryGetValue(dotAction, out var dotDebuffID);
            var target = IsMoving() && !HasStatusEffect(Buffs.Lightspeed)
                ? SimpleTarget.DottableEnemy(dotAction, dotDebuffID, 0, 30, 99)
                : SimpleTarget.DottableEnemy(dotAction, dotDebuffID, 0, 3, 99);
            
            if (target is not null && ActionReady(dotAction) && CanApplyStatus(target, dotDebuffID) && !JustUsedOn(dotAction, target) && PartyInCombat())
                return dotAction.Retarget(replacedActions, target);

            return OriginalHook(Malefic);
            #endregion
        }
    }

    internal class AST_AOE_Simple_DPS : CustomCombo
    {
        protected internal override Preset Preset => Preset.AST_AOE_Simple_DPS;
        protected override uint Invoke(uint actionID)
        {
            var actions = GravityList.ToArray();
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.AoEDPS, actions))
                return actionID;

            if (CustomActionHelper.CustomActionEnabled(CustomActionType.AoEDPS))
                actions = [All.AoEDPS];

            #region Special Content

            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;

            #endregion

            #region OGCDs
            if (ActionReady(Lightspeed) && IsMoving() &&
                !HasStatusEffect(Buffs.Lightspeed))
                return Lightspeed;

            if (InCombat() && CanWeave())
            {
                //Lucid Dreaming
                if (Role.CanLucidDream(6500))
                    return Role.LucidDreaming;

                //Play Card
                if (HasDPSCard && LevelChecked(Play1))
                    return OriginalHook(Play1).Retarget(actions, CardResolver);

                //Minor Arcana / Lord of Crowns
                if (HasLord && HasBattleTarget() && LevelChecked(MinorArcana))
                    return OriginalHook(MinorArcana);

                //Card Draw
                if (ActionReady(OriginalHook(AstralDraw)) && HasNoDPSCard)
                    return OriginalHook(AstralDraw);

                //Divination
                if (HasBattleTarget() && ActionReady(Divination) && StandStill &&
                    !HasStatusEffect(Buffs.Divining) && !HasDivination &&
                    (GetTargetHPPercent() >= 10 || InBossEncounter()))
                    return Divination;

                //Earthly Star
                if (LevelChecked(EarthlyStar) && IsOffCooldown(EarthlyStar) &&
                    !HasStatusEffect(Buffs.EarthlyDominance) && StandStill &&
                    (GetTargetHPPercent() >= 10 || InBossEncounter()))
                    return EarthlyStar.Retarget(actions, SimpleTarget.Self);

                //Oracle
                if (HasStatusEffect(Buffs.Divining))
                    return Oracle;
            }
            #endregion

            #region GCDs

            if (ActionReady(Macrocosmos) && StandStill && !InBossEncounter() &&
                !HasStatusEffect(Buffs.Macrocosmos))
                return Macrocosmos;

            var dotAction = OriginalHook(Combust);
            CombustList.TryGetValue(dotAction, out var dotDebuffID);
            var target =
                SimpleTarget.DottableEnemy(dotAction, dotDebuffID, 30, 3, 4);

            if (ActionReady(dotAction) && target != null)
                return OriginalHook(Combust).Retarget(actions, target);

            return OriginalHook(Gravity);
            #endregion
        }
    }
    #endregion

    #region Advanced DPS Combos
    internal class AST_ST_DPS : CustomCombo
    {
        protected internal override Preset Preset => Preset.AST_ST_DPS;
        protected override uint Invoke(uint actionID)
        {
            #region Button Selection

            var replacedActions = (int)AST_ST_DPS_AltMode switch
            {
                1 => CombustList.Keys.ToArray(),
                2 => [Malefic2],
                _ => MaleficList.ToArray(),
            };

            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.SingleTargetDPS, replacedActions.ToArray()))
                return actionID;

            if (CustomActionHelper.CustomActionEnabled(CustomActionType.SingleTargetDPS))
                replacedActions = [All.SingleTargetDPS];

            #endregion

            #region Variables
            bool cardPooling = IsEnabled(Preset.AST_DPS_CardPool);
            bool lordPooling = IsEnabled(Preset.AST_DPS_LordPool);
            int divHPThreshold = AST_ST_DPS_DivinationSubOption == 1 || !InBossEncounter() ? AST_ST_DPS_DivinationOption : 0;
            #endregion

            #region Out of Combat
            if (!InCombat())
            {
                if (IsEnabled(Preset.AST_DPS_AutoDraw) &&
                    ActionReady(OriginalHook(AstralDraw)) &&
                    (HasNoCards || HasNoDPSCard && AST_ST_DPS_OverwriteHealCards))
                    return OriginalHook(AstralDraw);
            }
            #endregion

            #region Opener
            if (IsEnabled(Preset.AST_ST_DPS_Opener) &&
                Opener().FullOpener(ref actionID))
            {
                if (actionID is EarthlyStar && IsEnabled(Preset.AST_ST_DPS_EarthlyStar))
                    return actionID.Retarget(replacedActions,
                        SimpleTarget.AnyEnemy ?? SimpleTarget.Stack.Allies);
                if (actionID is (Balance or Spear) && IsEnabled(Preset.AST_Cards_QuickTargetCards))
                    return actionID.Retarget(replacedActions, CardResolver);
                return actionID;
            }
            #endregion

            #region Special Content
            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;
            #endregion

            #region Healing Helper

            if (RaidwideCollectiveUnconscious())
                return CollectiveUnconscious;
            if (RaidwideNeutralSect())
                return OriginalHook(NeutralSect);
            if (RaidwideAspectedHelios())
                return OriginalHook(AspectedHelios);

            #endregion

            #region OGCDs
            if (IsEnabled(Preset.AST_DPS_LightSpeed) && ActionReady(Lightspeed) &&
                InCombat() && IsMoving() && !HasStatusEffect(Buffs.Lightspeed) &&
                GetTargetHPPercent() > AST_ST_DPS_LightSpeedOption && //Hp Check
                (IsNotEnabled(Preset.AST_DPS_LightSpeedHold) || GetRemainingCharges(Lightspeed) >= 2)) //Hold for 2 charges
                return Lightspeed;

            if (InCombat() && CanWeave())
            {
                //Lucid Dreaming
                if (IsEnabled(Preset.AST_DPS_Lucid) && Role.CanLucidDream(AST_ST_DPS_LucidDreaming))
                    return Role.LucidDreaming;

                //Play Card
                if (IsEnabled(Preset.AST_DPS_AutoPlay) && HasDPSCard && LevelChecked(Play1) &&
                    (HasDivination || !cardPooling || !LevelChecked(Divination)))
                    return IsEnabled(Preset.AST_Cards_QuickTargetCards)
                        ? OriginalHook(Play1).Retarget(replacedActions, CardResolver)
                        : OriginalHook(Play1);

                //Minor Arcana / Lord of Crowns
                if (IsEnabled(Preset.AST_DPS_LazyLord) && HasLord &&
                    HasBattleTarget() && LevelChecked(MinorArcana) &&
                    (HasDivination || !lordPooling || !LevelChecked(Divination)))
                    return OriginalHook(MinorArcana);

                //Card Draw
                if (IsEnabled(Preset.AST_DPS_AutoDraw) && ActionReady(OriginalHook(AstralDraw)) &&
                    (HasNoCards || HasNoDPSCard && AST_ST_DPS_OverwriteHealCards))
                    return OriginalHook(AstralDraw);

                //Lightspeed Burst
                if (IsEnabled(Preset.AST_DPS_LightspeedBurst) && IsEnabled(Preset.AST_DPS_Divination) && ActionReady(Lightspeed) &&
                    !HasStatusEffect(Buffs.Lightspeed) && DivinationCD < 5 && WaitGCDs)
                    return Lightspeed;

                //Divination
                if (IsEnabled(Preset.AST_DPS_Divination) && ActionReady(Divination) &&
                    !HasDivination && HasBattleTarget() &&
                    !HasStatusEffect(Buffs.Divining) &&
                    GetTargetHPPercent() > divHPThreshold &&
                    (WaitGCDs || StandStill))
                    return Divination;

                //Earthly Star
                if (IsEnabled(Preset.AST_ST_DPS_EarthlyStar) && IsOffCooldown(EarthlyStar) &&
                    LevelChecked(EarthlyStar) && !HasStatusEffect(Buffs.EarthlyDominance) &&
                    (WaitGCDs || StandStill))
                    return AST_ST_DPS_EarthlyStarSubOption == 1
                        ? EarthlyStar.Retarget(replacedActions, SimpleTarget.Self)
                        : EarthlyStar.Retarget(replacedActions, SimpleTarget.HardTarget.IfHostile() ?? SimpleTarget.Stack.Allies);

                //Stellar Detonation
                if (IsEnabled(Preset.AST_ST_DPS_StellarDetonation) &&
                    HasStatusEffect(Buffs.GiantDominance, anyOwner: false) &&
                    HasBattleTarget() &&
                    GetTargetHPPercent() <= AST_ST_DPS_StellarDetonation_Threshold &&
                    (AST_ST_DPS_StellarDetonation_SubOption == 1 || !InBossEncounter()))
                    return StellarDetonation;

                //Oracle
                if (IsEnabled(Preset.AST_DPS_Oracle) &&
                    HasStatusEffect(Buffs.Divining))
                    return Oracle;
            }
            #endregion

            #region GCDs

            #region Movement Options

            if (IsMoving())
            {
                var dotAction = OriginalHook(Combust);
                CombustList.TryGetValue(dotAction, out var dotDebuffID);
                var target = SimpleTarget.DottableEnemy(
                    dotAction, dotDebuffID, 0, 30, 99);
                if (IsEnabled(Preset.AST_ST_DPS_Move_DoT) &&
                    !HasStatusEffect(Buffs.Lightspeed) &&
                    target is not null)
                    return dotAction.Retarget(replacedActions, target);
            }
            #endregion

            if (IsEnabled(Preset.AST_ST_DPS_CombustUptime) && PartyInCombat())
            {
                var dotAction = OriginalHook(Combust);
                CombustList.TryGetValue(dotAction, out var dotDebuffID);
                var target = SimpleTarget.DottableEnemy(dotAction, dotDebuffID, ComputeHpThreshold, AST_ST_DPS_CombustUptime_Threshold, 2);

                //Single Target Dotting, needed because dottableenemy will not maintain single dot on main target of more than one target exists. 
                if (NeedsDoT())
                    return OriginalHook(Combust);

                //2 target Dotting System to maintain dots on 2 enemies. Works with the same sliders and one target
                if (target is not null && ActionReady(dotAction) && CanApplyStatus(target, dotDebuffID) && !JustUsedOn(dotAction, target) && AST_ST_DPS_CombustUptime_TwoTarget)
                    return dotAction.Retarget(replacedActions, target);
            }
            #endregion

            return OriginalHook(Malefic);
        }
    }
    internal class AST_AOE_DPS : CustomCombo
    {
        protected internal override Preset Preset => Preset.AST_AOE_DPS;
        protected override uint Invoke(uint actionID)
        {
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.AoEDPS, GravityList.ToArray()))
                return actionID;

            #region Variables
            bool cardPooling = IsEnabled(Preset.AST_AOE_CardPool);
            bool lordPooling = IsEnabled(Preset.AST_AOE_LordPool);
            int divHPThreshold = AST_ST_DPS_DivinationSubOption == 1 || !InBossEncounter() ? AST_ST_DPS_DivinationOption : 0;
            #endregion

            #region Special Content
            if (ContentSpecificActions.TryGet(out var contentAction))
                return contentAction;
            #endregion

            #region Healing Helper

            if (RaidwideCollectiveUnconscious())
                return CollectiveUnconscious;
            if (RaidwideNeutralSect())
                return OriginalHook(NeutralSect);
            if (RaidwideAspectedHelios())
                return OriginalHook(AspectedHelios);

            #endregion

            #region OGCDs
            if (IsEnabled(Preset.AST_AOE_LightSpeed) && ActionReady(Lightspeed) &&
                IsMoving() && InCombat() && !HasStatusEffect(Buffs.Lightspeed) &&
                GetTargetHPPercent() > AST_AOE_LightSpeedOption &&
                (IsNotEnabled(Preset.AST_AOE_LightSpeedHold) || GetRemainingCharges(Lightspeed) >= 2))
                return Lightspeed;

            if (InCombat() && CanWeave())
            {
                //Lucid Dreaming
                if (IsEnabled(Preset.AST_AOE_Lucid) && Role.CanLucidDream(AST_AOE_LucidDreaming))
                    return Role.LucidDreaming;

                //Play Card
                if (IsEnabled(Preset.AST_AOE_AutoPlay) && HasDPSCard && LevelChecked(Play1) &&
                    (HasDivination || !cardPooling || !LevelChecked(Divination)))
                    return IsEnabled(Preset.AST_Cards_QuickTargetCards)
                        ? OriginalHook(Play1).Retarget(GravityList.ToArray(), CardResolver)
                        : OriginalHook(Play1);

                //Minor Arcana / Lord of Crowns
                if (IsEnabled(Preset.AST_AOE_LazyLord) && HasLord &&
                    HasBattleTarget() && LevelChecked(MinorArcana) &&
                    (HasDivination || !lordPooling || !LevelChecked(Divination)))
                    return OriginalHook(MinorArcana);

                //Card Draw
                if (IsEnabled(Preset.AST_AOE_AutoDraw) && ActionReady(OriginalHook(AstralDraw)) &&
                    (HasNoCards || HasNoDPSCard && AST_AOE_DPS_OverwriteHealCards))
                    return OriginalHook(AstralDraw);

                //Lightspeed Burst
                if (IsEnabled(Preset.AST_AOE_LightspeedBurst) && IsEnabled(Preset.AST_AOE_Divination) && ActionReady(Lightspeed) &&
                    !HasStatusEffect(Buffs.Lightspeed) &&
                    DivinationCD < 5 && WaitGCDs)
                    return Lightspeed;

                //Divination
                if (IsEnabled(Preset.AST_AOE_Divination) && ActionReady(Divination) &&
                    !HasDivination && HasBattleTarget() &&
                    !HasStatusEffect(Buffs.Divining) &&
                    GetTargetHPPercent() > divHPThreshold &&
                    (WaitGCDs || StandStill))
                    return Divination;

                //Earthly Star
                if (IsEnabled(Preset.AST_AOE_DPS_EarthlyStar) &&
                    LevelChecked(EarthlyStar) && IsOffCooldown(EarthlyStar) &&
                    !HasStatusEffect(Buffs.EarthlyDominance) &&
                    (WaitGCDs || StandStill))
                    return AST_AOE_DPS_EarthlyStarSubOption == 1
                        ? EarthlyStar.Retarget(GravityList.ToArray(), SimpleTarget.Self)
                        : EarthlyStar.Retarget(GravityList.ToArray(), SimpleTarget.HardTarget.IfHostile() ?? SimpleTarget.Stack.Allies);

                //Stellar Detonation
                if (IsEnabled(Preset.AST_AOE_DPS_StellarDetonation) &&
                    HasStatusEffect(Buffs.GiantDominance, anyOwner: false) && HasBattleTarget() &&
                    GetTargetHPPercent() <= AST_AOE_DPS_StellarDetonation_Threshold &&
                    (AST_AOE_DPS_StellarDetonation_SubOption == 1 || !InBossEncounter()))
                    return StellarDetonation;

                //Oracle
                if (IsEnabled(Preset.AST_AOE_Oracle) &&
                    HasStatusEffect(Buffs.Divining))
                    return Oracle;
            }
            #endregion

            #region GCDS

            //MacroCosmos
            if (IsEnabled(Preset.AST_AOE_DPS_MacroCosmos) && ActionReady(Macrocosmos) &&
                InCombat() && StandStill && !HasStatusEffect(Buffs.Macrocosmos) &&
                (AST_AOE_DPS_MacroCosmos_SubOption == 1 || !InBossEncounter()))
                return Macrocosmos;

            var dotAction = OriginalHook(Combust);
            CombustList.TryGetValue(dotAction, out var dotDebuffID);
            var target = SimpleTarget.DottableEnemy(dotAction, dotDebuffID,
                AST_AOE_DPS_DoT_HPThreshold,
                AST_AOE_DPS_DoT_Reapply,
                AST_AOE_DPS_DoT_MaxTargets);

            if (IsEnabled(Preset.AST_AOE_DPS_DoT) &&
                ActionReady(dotAction) && target != null)
                return OriginalHook(Combust).Retarget([Gravity, Gravity2], target);

            return OriginalHook(Gravity);
            #endregion
        }
    }
    #endregion

    #region Simple Healing Combos
    internal class AST_Simple_ST_Heals : CustomCombo
    {
        protected internal override Preset Preset => Preset.AST_Simple_ST_Heals;
        protected override uint Invoke(uint actionID)
        {
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.SingleTargetHeals, Benefic))
                return actionID;

            if (ActionReady(OriginalHook(AstralDraw)) && HasNoDPSCard)
                return OriginalHook(AstralDraw);

            IGameObject? healTarget = SimpleTarget.Stack.OneButtonHealLogic;
            bool cleansableTarget =
                HealRetargeting.RetargetSettingOn && SimpleTarget.Stack.AllyToEsuna is not null ||
                HasCleansableDebuff(healTarget);

            if (ActionReady(Role.Esuna) &&
                GetTargetHPPercent(healTarget) >= 40 &&
                cleansableTarget)
                return Role.Esuna.RetargetIfEnabled(actionID);

            if (CanWeave() && Role.CanLucidDream(6500))
                return Role.LucidDreaming;

            if (ActionReady(EssentialDignity) && GetTargetHPPercent(healTarget) <= 30)
                return EssentialDignity.RetargetIfEnabled(actionID);

            if (ActionReady(Exaltation) && (healTarget.IsInParty() && healTarget.Role is CombatRole.Tank || !IsInParty()))
                return Exaltation.RetargetIfEnabled(actionID);

            if (!InBossEncounter())
            {
                if (ActionReady(OriginalHook(CelestialOpposition)))
                    return OriginalHook(CelestialOpposition);

                if (ActionReady(OriginalHook(NeutralSect)))
                    return OriginalHook(NeutralSect);

                if (HasLady && LevelChecked(MinorArcana))
                    return OriginalHook(LadyOfCrown);

                if (ActionReady(OriginalHook(CollectiveUnconscious)))
                    return OriginalHook(CollectiveUnconscious);
            }

            if (ActionReady(AspectedBenefic) &&
                (!HasStatusEffect(Buffs.AspectedBenefic, healTarget) ||
                 !HasStatusEffect(Buffs.NeutralSectShield, healTarget) && HasStatusEffect(Buffs.NeutralSect)))
                return OriginalHook(AspectedBenefic).RetargetIfEnabled(actionID);

            if ((HasArrow || HasBole) &&
                (healTarget.IsInParty() && healTarget.Role is CombatRole.Tank || !IsInParty()))
                return OriginalHook(Play2).RetargetIfEnabled(actionID);

            if (HasEwer || HasSpire)
                return OriginalHook(Play3).RetargetIfEnabled(actionID);

            if (ActionReady(CelestialIntersection) && !HasStatusEffect(Buffs.Intersection) && GetRemainingCharges(EssentialDignity) <= GetRemainingCharges(CelestialIntersection))
                return CelestialIntersection.RetargetIfEnabled(actionID);

            if (ActionReady(EssentialDignity))
                return EssentialDignity.RetargetIfEnabled(actionID);

            return !LevelChecked(Benefic2)
                ? Benefic.RetargetIfEnabled(actionID)
                : Benefic2.RetargetIfEnabled(actionID);
        }
    }

    internal class AST_Simple_AoE_Heals : CustomCombo
    {
        protected internal override Preset Preset => Preset.AST_Simple_AoE_Heals;

        protected override uint Invoke(uint actionID)
        {
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.AoEHeals, Helios))
                return actionID;

            if (ActionReady(OriginalHook(AstralDraw)) && HasNoDPSCard)
                return OriginalHook(AstralDraw);

            if (OriginalHook(Macrocosmos) == MicroCosmos && GetPartyAvgHPPercent() < 50)
                return MicroCosmos;

            if (HasStatusEffect(Buffs.GiantDominance))
                return StellarDetonation;

            if (HasStatusEffect(Buffs.HoroscopeHelios))
                return HoroscopeHeal;

            if (ActionReady(OriginalHook(CelestialOpposition)))
                return OriginalHook(CelestialOpposition);

            if (HasLady && LevelChecked(MinorArcana))
                return OriginalHook(LadyOfCrown);

            if (ActionReady(OriginalHook(CollectiveUnconscious)))
                return OriginalHook(CollectiveUnconscious);

            if (ActionReady(OriginalHook(NeutralSect)))
                return OriginalHook(NeutralSect);

            if (LevelChecked(Macrocosmos) && IsOffCooldown(Macrocosmos))
                return Macrocosmos;

            if (ActionReady(OriginalHook(AspectedHelios)) &&
                GetPartyBuffPercent(Buffs.AspectedHelios) <= 50 &&
                GetPartyBuffPercent(Buffs.HeliosConjunction) <= 50)
                return LevelChecked(Horoscope) && IsOffCooldown(Horoscope)
                    ? Horoscope
                    : OriginalHook(AspectedHelios);

            return LevelChecked(Horoscope) && IsOffCooldown(Horoscope)
                ? Horoscope
                : Helios;
        }
    }
    #endregion

    #region Advanced Healing Combos
    internal class AST_ST_Heals : CustomCombo
    {
        protected internal override Preset Preset => Preset.AST_ST_Heals;
        protected override uint Invoke(uint actionID)
        {
            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.SingleTargetHeals, Benefic))
                return actionID;

            #region Healing Helper

            if (RaidwideCollectiveUnconscious())
                return CollectiveUnconscious;
            if (RaidwideNeutralSect())
                return OriginalHook(NeutralSect);
            if (RaidwideAspectedHelios())
                return OriginalHook(AspectedHelios);

            #endregion

            IGameObject? healTarget = SimpleTarget.Stack.OneButtonHealLogic;

            bool cleansableTarget =
                HealRetargeting.RetargetSettingOn && SimpleTarget.Stack.AllyToEsuna is not null ||
                HasCleansableDebuff(healTarget);

            if (IsEnabled(Preset.AST_ST_Heals_Esuna) &&
                ActionReady(Role.Esuna) &&
                GetTargetHPPercent(healTarget, AST_ST_SimpleHeals_IncludeShields) >= AST_ST_SimpleHeals_Esuna &&
                cleansableTarget)
                return Role.Esuna.RetargetIfEnabled(actionID);

            //Priority List
            for (int i = 0; i < AST_ST_SimpleHeals_Priority.Count; i++)
            {
                int index = AST_ST_SimpleHeals_Priority.IndexOf(i + 1);
                int config = GetMatchingConfigST(index, healTarget, out uint spell, out bool enabled);

                if (enabled)
                {
                    if (GetTargetHPPercent(healTarget, AST_ST_SimpleHeals_IncludeShields) <= config &&
                        ActionReady(spell))
                        return spell.RetargetIfEnabled(actionID);
                }
            }
            return !LevelChecked(Benefic2) ?
                Benefic.RetargetIfEnabled(actionID) :
                Benefic2.RetargetIfEnabled(actionID);
        }
    }

    internal class AST_AoE_Heals : CustomCombo
    {
        protected internal override Preset Preset => Preset.AST_AoE_Heals;

        protected override uint Invoke(uint actionID)
        {
            bool nonAspectedMode = AST_AoE_SimpleHeals_AltMode > 0; //(0 or 1 radio values)

            if (!CustomActionHelper.OneButtonRotationChecker(actionID, CustomActionType.AoEHeals, nonAspectedMode ? Helios : AspectedHelios, HeliosConjuction))
                return actionID;

            //Level check to return helios immediately below 40
            if (!LevelChecked(AspectedHelios))
                return Helios;

            #region Healing Helper

            if (RaidwideCollectiveUnconscious())
                return CollectiveUnconscious;
            if (RaidwideNeutralSect())
                return OriginalHook(NeutralSect);
            if (RaidwideAspectedHelios())
                return OriginalHook(AspectedHelios);

            #endregion

            //Horoscope check to trigger the ability to do the larger Horoscope Heal
            if (HasStatusEffect(Buffs.Horoscope))
                return HasStatusEffect(Buffs.HeliosConjunction) || HasStatusEffect(Buffs.AspectedHelios)
                    ? Helios
                    : OriginalHook(AspectedHelios);

            //Check for Suntouched to finish the combo after Neutral sect regardless of priorities
            if (IsEnabled(Preset.AST_AoE_Heals_NeutralSect) && HasStatusEffect(Buffs.Suntouched) && CanWeave())
                return SunSign;

            //Priority List
            float averagePartyHP = GetPartyAvgHPPercent();
            for (int i = 0; i < AST_AoE_SimpleHeals_Priority.Count; i++)
            {
                int index = AST_AoE_SimpleHeals_Priority.IndexOf(i + 1);
                int config = GetMatchingConfigAoE(index, out uint spell, out bool enabled);

                if (enabled && averagePartyHP <= config && ActionReady(spell))
                    return spell;
            }

            return
                nonAspectedMode ? Helios : OriginalHook(AspectedHelios);
        }
    }
    #endregion

    #region Standalone Features
    internal class AST_RetargetManualCards : CustomCombo
    {
        protected internal override Preset Preset => Preset.AST_Cards_QuickTargetCards;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Play1 ||
                !AST_QuickTarget_Manuals)
                return actionID;

            OriginalHook(Play1).Retarget(Play1, CardResolver);

            return actionID;
        }
    }
    internal class AST_Benefic : CustomCombo
    {
        protected internal override Preset Preset => Preset.AST_Benefic;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Benefic2)
                return actionID;

            var healStack = SimpleTarget.Stack.AllyToHeal;

            if (!LevelChecked(Benefic2))
                return IsEnabled(Preset.AST_Retargets_Benefic) ? Benefic.Retarget(healStack) : Benefic;

            return IsEnabled(Preset.AST_Retargets_Benefic) ? Benefic2.Retarget(healStack) : Benefic2;
        }
    }
    internal class AST_Lightspeed : CustomCombo
    {
        protected internal override Preset Preset => Preset.AST_Lightspeed_Protection;
        protected override uint Invoke(uint actionID) =>
            actionID is Lightspeed && HasStatusEffect(Buffs.Lightspeed)
                ? All.SavageBlade
                : actionID;
    }
    internal class AST_Raise_Alternative : CustomCombo
    {
        protected internal override Preset Preset => Preset.AST_Raise_Alternative;
        protected override uint Invoke(uint actionID) =>
            actionID == Role.Swiftcast && IsOnCooldown(Role.Swiftcast)
                ? IsEnabled(Preset.AST_Raise_Alternative_Retarget)
                    ? Ascend.Retarget(Role.Swiftcast,
                        SimpleTarget.Stack.AllyToRaise)
                    : Ascend
                : actionID;
    }
    internal class AST_Mit_ST : CustomCombo
    {
        protected internal override Preset Preset => Preset.AST_Mit_ST;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Exaltation)
                return actionID;

            var healStack = SimpleTarget.Stack.AllyToHeal;

            if (ActionReady(Exaltation))
                return IsEnabled(Preset.AST_Retargets_Exaltation)
                    ? Exaltation.Retarget(healStack)
                    : Exaltation;

            if (AST_Mit_ST_Options[0] &&
                ActionReady(CelestialIntersection) &&
                !HasStatusEffect(Buffs.Intersection, target: healStack))
                return IsEnabled(Preset.AST_Retargets_CelestialIntersection)
                    ? CelestialIntersection.Retarget(Exaltation, healStack)
                    : CelestialIntersection;

            if (AST_Mit_ST_Options[1] &&
                ActionReady(EssentialDignity) &&
                GetTargetHPPercent(healStack) < AST_Mit_ST_EssentialDignityThreshold)
                return IsEnabled(Preset.AST_Retargets_EssentialDignity)
                    ? EssentialDignity.Retarget(Exaltation, healStack)
                    : EssentialDignity;

            return actionID;
        }
    }
    internal class AST_Mit_AoE : CustomCombo
    {
        protected internal override Preset Preset => Preset.AST_Mit_AoE;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not CollectiveUnconscious)
                return actionID;

            if (ActionReady(CollectiveUnconscious))
                return CollectiveUnconscious;

            if (ActionReady(OriginalHook(NeutralSect)))
                return OriginalHook(NeutralSect);

            if (HasStatusEffect(Buffs.NeutralSect) && !HasStatusEffect(Buffs.NeutralSectShield))
                return OriginalHook(AspectedHelios);

            return actionID;
        }
    }
    internal class AST_Retargets : CustomCombo
    {
        protected internal override Preset Preset => Preset.AST_Retargets;
        protected override uint Invoke(uint actionID)
        {
            var healStack = SimpleTarget.Stack.AllyToHeal;

            if (!EZ.Throttle("ASTRetargetingFeature", TS.FromSeconds(.1)))
                return actionID;

            if (IsEnabled(Preset.AST_Retargets_Benefic))
            {
                Benefic.Retarget(healStack);
                Benefic2.Retarget(healStack);
            }

            if (IsEnabled(Preset.AST_Retargets_AspectedBenefic))
                AspectedBenefic.Retarget(healStack);

            if (IsEnabled(Preset.AST_Retargets_EssentialDignity))
                EssentialDignity.Retarget(healStack);

            if (IsEnabled(Preset.AST_Retargets_Exaltation))
                Exaltation.Retarget(healStack);

            if (IsEnabled(Preset.AST_Retargets_Synastry))
                Synastry.Retarget(healStack);

            if (IsEnabled(Preset.AST_Retargets_CelestialIntersection))
                CelestialIntersection.Retarget(healStack);

            if (IsEnabled(Preset.AST_Retargets_HealCards))
            {
                OriginalHook(Play2).Retarget(Play2, healStack);
                OriginalHook(Play3).Retarget(Play3, healStack);
            }

            if (IsEnabled(Preset.AST_Retargets_EarthlyStar))
            {
                var starTarget =
                    (AST_EarthlyStarOptions[0]
                        ? SimpleTarget.HardTarget.IfHostile()
                        : null) ??
                    (AST_EarthlyStarOptions[1]
                        ? SimpleTarget.HardTarget.IfFriendly()
                        : null) ??
                    SimpleTarget.Self;
                EarthlyStar.Retarget(starTarget);
            }

            return actionID;
        }
    }
    #endregion
}
