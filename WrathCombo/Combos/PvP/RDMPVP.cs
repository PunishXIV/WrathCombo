using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;

namespace WrathCombo.Combos.PvP
{
    internal static class RDMPvP
    {
        public const byte JobID = 35;

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
        public static class Config
        {
            internal static UserInt
                RDMPvP_Corps_Range = new("RDMPvP_Corps_Range", 5),
                RDMPvP_Corps_Charges = new("RDMPvP_Corps_Charges", 1),
                RDMPvP_Displacement_Charges = new("RDMPvP_Displacement_Charges", 1),
                RDMPvP_Forte_PlayerHP = new("RDMPvP_Forte_PlayerHP", 50),
                RDMPvP_Resolution_TargetHP = new("RDMPvP_Resolution_TargetHP", 50);

            public static UserBool
                RDMPvP_Forte_SubOption = new("RDMPvP_Forte_SubOption"),
                RDMPvP_Embolden_SubOption = new("RDMPvP_Embolden_SubOption"),
                RDMPvP_Displacement_SubOption = new("RDMPvP_Displacement_SubOption");
        }

        internal class RDMPvP_BurstMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDMPvP_BurstMode;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (actionID is Jolt3)
                {
                    #region Variables
                    float targetDistance = GetTargetDistance();
                    float targetCurrentPercentHp = GetTargetHPPercent();
                    float playerCurrentPercentHp = PlayerHealthPercentageHp();
                    uint chargesCorps = HasCharges(CorpsACorps) ? GetCooldown(CorpsACorps).RemainingCharges : 0;
                    uint chargesDisplacement = HasCharges(Displacement) ? GetCooldown(Displacement).RemainingCharges : 0;
                    bool isMoving = IsMoving();
                    bool inCombat = InCombat();
                    bool hasTarget = HasTarget();
                    bool isTargetNPC = HasBattleTarget();
                    bool inMeleeRange = targetDistance <= 5;
                    bool hasBind = HasEffectAny(PvPCommon.Debuffs.Bind);
                    bool isCorpsAvailable = chargesCorps > 0 && !hasBind;
                    bool hasScorch = OriginalHook(EnchantedRiposte) is Scorch;
                    bool hasViceOfThorns = OriginalHook(Forte) is ViceOfThorns;
                    bool hasPrefulgence = OriginalHook(Embolden) is Prefulgence;
                    bool hasGrandImpact = OriginalHook(actionID) is GrandImpact;
                    bool targetHasGuard = TargetHasEffectAny(PvPCommon.Buffs.Guard);
                    bool hasForte = IsOffCooldown(Forte) && OriginalHook(Forte) is Forte;
                    bool hasEmbolden = IsOffCooldown(Embolden) && OriginalHook(Embolden) is Embolden;
                    bool isEmboldenDelayDependant = !JustUsed(Embolden, 5f) || IsOnCooldown(EnchantedRiposte);
                    bool hasMeleeCombo = OriginalHook(EnchantedRiposte) is EnchantedZwerchhau or EnchantedRedoublement;
                    bool isEnabledViceOfThorns = IsEnabled(CustomComboPreset.RDMPvP_Forte) && Config.RDMPvP_Forte_SubOption;
                    bool isEnabledPrefulgence = IsEnabled(CustomComboPreset.RDMPvP_Embolden) && Config.RDMPvP_Embolden_SubOption;
                    bool hasEnchantedRiposte = IsOffCooldown(EnchantedRiposte) && OriginalHook(EnchantedRiposte) is EnchantedRiposte;
                    bool isViceOfThornsExpiring = HasEffect(Buffs.ThornedFlourish) && GetBuffRemainingTime(Buffs.ThornedFlourish) <= 3;
                    bool isPrefulgenceExpiring = HasEffect(Buffs.PrefulgenceReady) && GetBuffRemainingTime(Buffs.PrefulgenceReady) <= 3;
                    bool isMovementDependant = !Config.RDMPvP_Displacement_SubOption || (Config.RDMPvP_Displacement_SubOption && !isMoving);
                    bool targetHasImmunity = TargetHasEffectAny(PLDPvP.Buffs.HallowedGround) || TargetHasEffectAny(DRKPvP.Buffs.UndeadRedemption);
                    bool isDisplacementPrimed = !hasBind && !JustUsed(Displacement, 8f) && !HasEffect(Buffs.Displacement) && hasScorch && inMeleeRange;
                    bool isCorpsPrimed = !hasBind && !JustUsed(CorpsACorps, 8f) && chargesCorps > Config.RDMPvP_Corps_Charges && targetDistance <= Config.RDMPvP_Corps_Range;
                    #endregion

                    // Forte
                    if (IsEnabled(CustomComboPreset.RDMPvP_Forte) && hasForte && inCombat &&
                        playerCurrentPercentHp < Config.RDMPvP_Forte_PlayerHP)
                        return OriginalHook(Forte);

                    if (hasTarget && !targetHasImmunity)
                    {
                        if (!targetHasGuard)
                        {
                            // Vice of Thorns
                            if (isEnabledViceOfThorns && hasViceOfThorns && (!isTargetNPC || isViceOfThornsExpiring))
                                return OriginalHook(Forte);

                            // Displacement
                            if (IsEnabled(CustomComboPreset.RDMPvP_Displacement) && isDisplacementPrimed &&
                                isMovementDependant && chargesDisplacement > Config.RDMPvP_Displacement_Charges)
                                return OriginalHook(Displacement);
                        }

                        if (hasEnchantedRiposte)
                        {
                            // Embolden
                            if (IsEnabled(CustomComboPreset.RDMPvP_Embolden) && hasEmbolden)
                            {
                                // Combo Setting
                                if (IsEnabled(CustomComboPreset.RDMPvP_Corps) && (isCorpsPrimed || (!isCorpsPrimed && inMeleeRange)))
                                    return OriginalHook(Embolden);

                                // Solo Setting
                                if (IsNotEnabled(CustomComboPreset.RDMPvP_Corps) && (inMeleeRange || (inCombat && isCorpsAvailable)))
                                    return OriginalHook(Embolden);
                            }

                            // Corps-a-Corps
                            if (IsEnabled(CustomComboPreset.RDMPvP_Corps) && isCorpsPrimed)
                                return OriginalHook(CorpsACorps);
                        }

                        // Riposte Combo
                        if (IsEnabled(CustomComboPreset.RDMPvP_Riposte) && inMeleeRange && (hasEnchantedRiposte || hasMeleeCombo))
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
                            if (IsEnabled(CustomComboPreset.RDMPvP_Riposte) && hasScorch)
                                return OriginalHook(EnchantedRiposte);

                            // Resolution
                            if (IsEnabled(CustomComboPreset.RDMPvP_Resolution) && IsOffCooldown(Resolution) &&
                                !isTargetNPC && targetCurrentPercentHp < Config.RDMPvP_Resolution_TargetHP)
                                return OriginalHook(Resolution);
                        }
                    }

                    // Grand Impact / Jolt III
                    return hasGrandImpact || !isMoving ? OriginalHook(actionID) : OriginalHook(11);
                }

                return actionID;
            }
        }

        internal class RDMPvP_Dash_Feature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RDMPvP_Dash_Feature;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is CorpsACorps)
                {
                    bool hasCrowdControl = HasEffectAny(PvPCommon.Debuffs.Stun) || HasEffectAny(PvPCommon.Debuffs.DeepFreeze) ||
                                           HasEffectAny(PvPCommon.Debuffs.Bind) || HasEffectAny(PvPCommon.Debuffs.Silence) ||
                                           HasEffectAny(PvPCommon.Debuffs.MiracleOfNature);

                    if (HasCharges(CorpsACorps) && IsOffCooldown(PvPCommon.Purify) && hasCrowdControl)
                        return OriginalHook(PvPCommon.Purify);
                }

                if (actionID is Displacement)
                {
                    bool hasCrowdControl = HasEffectAny(PvPCommon.Debuffs.Stun) || HasEffectAny(PvPCommon.Debuffs.DeepFreeze) ||
                                           HasEffectAny(PvPCommon.Debuffs.Bind) || HasEffectAny(PvPCommon.Debuffs.Silence) ||
                                           HasEffectAny(PvPCommon.Debuffs.MiracleOfNature);

                    if (HasCharges(Displacement) && IsOffCooldown(PvPCommon.Purify) && hasCrowdControl)
                        return OriginalHook(PvPCommon.Purify);
                }

                return actionID;
            }
        }
    }
}