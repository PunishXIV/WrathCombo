using Dalamud.Game.ClientState.Objects.Types;
using WrathCombo.Combos.PvE;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.Window.Functions.UserConfig;
using static WrathCombo.Combos.PvP.RDMPvP.Config;


namespace WrathCombo.Combos.PvP;

internal static class RDMPvP
{
    #region IDs
    internal class Role : PvPCaster;
    public const uint
        EnchantedRiposte = 41488,
        Resolution = 41492,
        CorpsACorps = 29699,
        Displacement = 29700,
        EnchantedZwerchhau = 41489,
        EnchantedRedoublement = 41490,
        SouthernCross = 41498,
        Embolden = 41494,
        Forte = 41496,
        Scorch = 41491,
        GrandImpact = 41487,
        Jolt3 = 41486,
        ViceOfThorns = 41493,
        Prefulgence = 41495;

    public static class Buffs
    {
        public const ushort
            Dualcast = 1393,
            EnchantedRiposte = 3234,
            EnchantedRedoublement = 3236,
            EnchantedZwerchhau = 3235,
            VermilionRadiance = 3233,
            Displacement = 3243,
            Embolden = 2282,
            Forte = 4320,
            PrefulgenceReady = 4322,
            ThornedFlourish = 4321;
    }
    public static class Debuffs
    {
        public const ushort
            Monomachy = 3242;
    }
    #endregion

    #region Config
    public static class Config
    {
        internal static UserInt
            RDMPvP_Corps_Range = new("RDMPvP_Corps_Range", 5),
            RDMPvP_Corps_Charges = new("RDMPvP_Corps_Charges", 1),
            RDMPvP_Displacement_Charges = new("RDMPvP_Displacement_Charges", 1),
            RDMPvP_Forte_PlayerHP = new("RDMPvP_Forte_PlayerHP", 50),
            RDMPvP_Resolution_TargetHP = new("RDMPvP_Resolution_TargetHP", 50),
            RDMPvP_PhantomDartThreshold = new("RDMPvP_PhantomDartThreshold", 50),
            RDMPvP_Dash_Feature_PurifyMPThreshold = new("RDMPvP_Dash_Feature_PurifyMPThreshold", 5000);

        public static UserBool
            RDMPvP_Forte_SubOption = new("RDMPvP_Forte_SubOption"),
            RDMPvP_Embolden_SubOption = new("RDMPvP_Embolden_SubOption"),
            RDMPvP_Displacement_SubOption = new("RDMPvP_Displacement_SubOption");

        internal static void Draw(Preset preset)
        {
            switch (preset)
            {
                // Resolution
                case Preset.RDMPvP_Resolution:
                    DrawSliderInt(10, 100, RDMPvP_Resolution_TargetHP, "Target HP%");
                    break;

                // Embolden / Prefulgence
                case Preset.RDMPvP_Embolden:
                    DrawAdditionalBoolChoice(RDMPvP_Embolden_SubOption, "Prefulgence Option",
                        "Uses Prefulgence when available.");
                    break;

                // Corps-a-Corps
                case Preset.RDMPvP_Corps:
                    DrawSliderInt(0, 1, RDMPvP_Corps_Charges, "Charges to Keep");
                    DrawSliderInt(5, 10, RDMPvP_Corps_Range, "Maximum Range");
                    break;

                // Displacement
                case Preset.RDMPvP_Displacement:
                    DrawSliderInt(0, 1, RDMPvP_Displacement_Charges, "Charges to Keep");
                    DrawAdditionalBoolChoice(RDMPvP_Displacement_SubOption, "No Movement Option", "Uses Displacement only when not moving.");
                    break;

                // Forte / Vice of Thorns
                case Preset.RDMPvP_Forte:
                    DrawSliderInt(10, 100, RDMPvP_Forte_PlayerHP, "Player HP%");
                    DrawAdditionalBoolChoice(RDMPvP_Forte_SubOption, "Vice of Thorns Option", "Uses Vice of Thorns when available.");
                    break;

                // Phantom Dart
                case Preset.RDMPvP_PhantomDart:
                    DrawSliderInt(1, 100, RDMPvP_PhantomDartThreshold, "Target HP% to use Phantom Dart at or below");
                    break;
                
                case Preset.RDMPvP_Dash_Feature:
                    DrawSliderInt(2500, 10000, RDMPvP_Dash_Feature_PurifyMPThreshold, "Do not use Purify below set MP.");
                    break;
            }
        }
    }
    #endregion

    internal class RDMPvP_BurstMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.RDMPvP_BurstMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Jolt3) return actionID;

            #region Variables
            float targetDistance = GetTargetDistance();
            float targetCurrentPercentHp = GetTargetHPPercent();
            float playerCurrentPercentHp = PlayerHealthPercentageHp();
            uint chargesCorps = HasCharges(CorpsACorps) ? GetCooldown(CorpsACorps).RemainingCharges : 0;
            uint chargesDisplacement = HasCharges(Displacement) ? GetCooldown(Displacement).RemainingCharges : 0;
            bool isMoving = IsMoving();
            bool inCombat = InCombat();
            bool hasTarget = HasTarget();
            bool isTargetNPC = CurrentTarget is IBattleNpc && CurrentTarget.DataId != 8016;
            bool inMeleeRange = targetDistance <= 5;
            bool hasBind = HasStatusEffect(PvPCommon.Debuffs.Bind, anyOwner: true);
            bool isCorpsAvailable = chargesCorps > 0 && !hasBind;
            bool hasScorch = OriginalHook(EnchantedRiposte) is Scorch;
            bool hasViceOfThorns = OriginalHook(Forte) is ViceOfThorns;
            bool hasPrefulgence = OriginalHook(Embolden) is Prefulgence;
            bool hasGrandImpact = OriginalHook(actionID) is GrandImpact;
            bool targetHasGuard = HasStatusEffect(PvPCommon.Buffs.Guard, CurrentTarget, true);
            bool hasForte = IsOffCooldown(Forte) && OriginalHook(Forte) is Forte;
            bool hasEmbolden = IsOffCooldown(Embolden) && OriginalHook(Embolden) is Embolden;
            bool isEmboldenDelayDependant = !JustUsed(Embolden, 5f) || IsOnCooldown(EnchantedRiposte);
            bool hasMeleeCombo = OriginalHook(EnchantedRiposte) is EnchantedZwerchhau or EnchantedRedoublement;
            bool isEnabledViceOfThorns = IsEnabled(Preset.RDMPvP_Forte) && RDMPvP_Forte_SubOption;
            bool isEnabledPrefulgence = IsEnabled(Preset.RDMPvP_Embolden) && RDMPvP_Embolden_SubOption;
            bool hasEnchantedRiposte = IsOffCooldown(EnchantedRiposte) && OriginalHook(EnchantedRiposte) is EnchantedRiposte;
            bool isViceOfThornsExpiring = HasStatusEffect(Buffs.ThornedFlourish) && GetStatusEffectRemainingTime(Buffs.ThornedFlourish) <= 3;
            bool isPrefulgenceExpiring = HasStatusEffect(Buffs.PrefulgenceReady) && GetStatusEffectRemainingTime(Buffs.PrefulgenceReady) <= 3;
            bool isMovementDependant = !RDMPvP_Displacement_SubOption || (RDMPvP_Displacement_SubOption && !isMoving);
            bool targetHasImmunity = HasStatusEffect(PLDPvP.Buffs.HallowedGround, CurrentTarget, true) || HasStatusEffect(DRKPvP.Buffs.UndeadRedemption, CurrentTarget, true);
            bool isDisplacementPrimed = !hasBind && !JustUsed(Displacement, 8f) && !HasStatusEffect(Buffs.Displacement) && hasScorch && inMeleeRange;
            bool isCorpsPrimed = !hasBind && !JustUsed(CorpsACorps, 8f) && chargesCorps > RDMPvP_Corps_Charges && targetDistance <= RDMPvP_Corps_Range;
            #endregion

            // Forte
            if (IsEnabled(Preset.RDMPvP_Forte) && hasForte && inCombat &&
                playerCurrentPercentHp < RDMPvP_Forte_PlayerHP)
                return OriginalHook(Forte);

            if (hasTarget && !targetHasImmunity)
            {
                if (!targetHasGuard)
                {
                    if (IsEnabled(Preset.RDMPvP_PhantomDart) && Role.CanPhantomDart() && CanWeave() && GetTargetHPPercent() <= (RDMPvP_PhantomDartThreshold))
                        return Role.PhantomDart;

                    // Vice of Thorns
                    if (isEnabledViceOfThorns && hasViceOfThorns && (!isTargetNPC || isViceOfThornsExpiring))
                        return OriginalHook(Forte);

                    // Displacement
                    if (IsEnabled(Preset.RDMPvP_Displacement) && isDisplacementPrimed &&
                        isMovementDependant && chargesDisplacement > RDMPvP_Displacement_Charges)
                        return OriginalHook(Displacement);
                }

                if (hasEnchantedRiposte)
                {
                    // Embolden
                    if (IsEnabled(Preset.RDMPvP_Embolden) && hasEmbolden)
                    {
                        // Combo Setting
                        if (IsEnabled(Preset.RDMPvP_Corps) && (isCorpsPrimed || (!isCorpsPrimed && inMeleeRange)))
                            return OriginalHook(Embolden);

                        // Solo Setting
                        if (IsNotEnabled(Preset.RDMPvP_Corps) && (inMeleeRange || (inCombat && isCorpsAvailable)))
                            return OriginalHook(Embolden);
                    }

                    // Corps-a-Corps
                    if (IsEnabled(Preset.RDMPvP_Corps) && isCorpsPrimed)
                        return OriginalHook(CorpsACorps);
                }

                // Riposte Combo
                if (IsEnabled(Preset.RDMPvP_Riposte) && inMeleeRange && (hasEnchantedRiposte || hasMeleeCombo))
                    return OriginalHook(EnchantedRiposte);

                // Prefulgence
                if (isEnabledPrefulgence && hasPrefulgence && isEmboldenDelayDependant)
                {
                    // Conditional
                    if (isPrefulgenceExpiring || playerCurrentPercentHp < 50)
                        return OriginalHook(Embolden);

                    // Offensive
                    if (!targetHasGuard && !hasScorch)
                        return OriginalHook(Embolden);
                }

                if (!targetHasGuard)
                {
                    // Scorch
                    if (IsEnabled(Preset.RDMPvP_Riposte) && hasScorch)
                        return OriginalHook(EnchantedRiposte);

                    // Resolution
                    if (IsEnabled(Preset.RDMPvP_Resolution) && IsOffCooldown(Resolution) &&
                        !isTargetNPC && targetCurrentPercentHp < RDMPvP_Resolution_TargetHP)
                        return OriginalHook(Resolution);
                }
            }

            // Grand Impact / Jolt III
            return hasGrandImpact || !isMoving ? OriginalHook(actionID) : All.SavageBlade;

        }
    }
    internal class RDMPvP_Dash_Feature : CustomCombo
    {
        protected internal override Preset Preset => Preset.RDMPvP_Dash_Feature;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (CorpsACorps or Displacement)) 
                return actionID;
            
            bool hasCrowdControl = HasStatusEffect(PvPCommon.Debuffs.Stun, anyOwner: true) || HasStatusEffect(PvPCommon.Debuffs.DeepFreeze, anyOwner: true) ||
                                   HasStatusEffect(PvPCommon.Debuffs.Bind, anyOwner: true) || HasStatusEffect(PvPCommon.Debuffs.Silence, anyOwner: true) ||
                                   HasStatusEffect(PvPCommon.Debuffs.MiracleOfNature, anyOwner: true);
            
            if (HasCharges(CorpsACorps) && IsOffCooldown(PvPCommon.Purify) && hasCrowdControl && LocalPlayer.CurrentMp >= RDMPvP_Dash_Feature_PurifyMPThreshold)
                return OriginalHook(PvPCommon.Purify);
           
            return actionID;
        }
    }
}