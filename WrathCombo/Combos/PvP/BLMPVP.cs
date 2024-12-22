using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;

namespace WrathCombo.Combos.PvP
{
    internal static class BLMPvP
    {
        public const byte JobID = 25;

        public const uint
            Fire = 29649,
            Blizzard = 29653,
            Burst = 29657,
            Paradox = 29663,
            AetherialManipulation = 29660,
            Fire3 = 30896,
            Fire4 = 29650,
            Flare = 29651,
            Blizzard3 = 30897,
            Blizzard4 = 29654,
            Freeze = 29655,
            Lethargy = 41510,
            HighFireII = 41473,
            HighBlizzardII = 41474,
            ElementalWeave = 41475,
            FlareStar = 41480,
            FrostStar = 41481,
            SoulResonance = 29662,
            Xenoglossy = 29658;

        public static class Buffs
        {
            public const ushort
                AstralFire1 = 3212,
                AstralFire2 = 3213,
                AstralFire3 = 3381,
                UmbralIce1 = 3214,
                UmbralIce2 = 3215,
                UmbralIce3 = 3382,
                Burst = 3221,
                SoulResonance = 3222,
                ElementalStar = 4317,
                WreathOfFire = 4315,
                WreathOfIce = 4316,
                Paradox = 3223;
        }

        public static class Debuffs
        {
            public const ushort
                Burns = 3218,
                DeepFreeze = 3219,
                Lethargy = 4333;
        }

        public static class Config
        {
            internal static UserInt
                BLMPvP_ElementalWeave_PlayerHP = new("BLMPvP_ElementalWeave_PlayerHP", 50),
                BLMPvP_Lethargy_TargetHP = new("BLMPvP_Lethargy_TargetHP", 50),
                BLMPvP_Xenoglossy_TargetHP = new("BLMPvP_Xenoglossy_TargetHP", 50);

            public static UserBool
                BLMPvP_Burst_SubOption = new("BLMPvP_Burst_SubOption"),
                BLMPvP_ElementalWeave_SubOption = new ("BLMPvP_ElementalWeave_SubOption"),
                BLMPvP_Lethargy_SubOption = new("BLMPvP_Lethargy_SubOption"),
                BLMPvP_Xenoglossy_SubOption = new ("BLMPvP_Xenoglossy_SubOption");
        }

        internal class BLMPvP_BurstMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLMPvP_BurstMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                #region Variables
                float targetDistance = GetTargetDistance();
                float targetCurrentPercentHp = GetTargetHPPercent();
                float playerCurrentPercentHp = PlayerHealthPercentageHp();
                uint chargesXenoglossy = HasCharges(Xenoglossy) ? GetCooldown(Xenoglossy).RemainingCharges : 0;
                bool isMoving = IsMoving;
                bool inCombat = InCombat();
                bool hasTarget = HasTarget();
                bool isTargetNPC = HasBattleTarget();
                bool hasParadox = HasEffect(Buffs.Paradox);
                bool hasIceAoE = HasEffect(Buffs.UmbralIce3);
                bool hasResonance = HasEffect(Buffs.SoulResonance);
                bool hasWreathOfFire = HasEffect(Buffs.WreathOfFire);
                bool hasFlareStar = OriginalHook(SoulResonance) is FlareStar;
                bool hasFrostStar = OriginalHook(SoulResonance) is FrostStar;
                bool targetHasGuard = TargetHasEffectAny(PvPCommon.Buffs.Guard);
                bool targetHasHeavy = TargetHasEffectAny(PvPCommon.Debuffs.Heavy);
                bool isPlayerTargeted = CurrentTarget?.TargetObjectId == LocalPlayer.GameObjectId;
                bool isParadoxPrimed = HasEffect(Buffs.UmbralIce1) || HasEffect(Buffs.AstralFire1);
                bool isResonanceExpiring = HasEffect(Buffs.SoulResonance) && GetBuffRemainingTime(Buffs.SoulResonance) <= 10;
                bool hasUmbralIce = HasEffect(Buffs.UmbralIce1) || HasEffect(Buffs.UmbralIce2) || HasEffect(Buffs.UmbralIce3);
                bool isElementalStarExpiring = HasEffect(Buffs.ElementalStar) && GetBuffRemainingTime(Buffs.ElementalStar) <= 10;
                bool hasAstralFire = HasEffect(Buffs.AstralFire1) || HasEffect(Buffs.AstralFire2) || HasEffect(Buffs.AstralFire3);
                bool targetHasImmunity = TargetHasEffectAny(PLDPvP.Buffs.HallowedGround) || TargetHasEffectAny(DRKPvP.Buffs.UndeadRedemption);
                #endregion

                if (actionID is Fire or Fire3 or Fire4 or HighFireII or Flare)
                {
                    if (inCombat)
                    {
                        // Burst (Defensive)
                        if (IsEnabled(CustomComboPreset.BLMPvP_Burst) && Config.BLMPvP_Burst_SubOption && IsOffCooldown(Burst) && playerCurrentPercentHp < 50)
                            return OriginalHook(Burst);

                        // Elemental Weave (Defensive)
                        if (IsEnabled(CustomComboPreset.BLMPvP_ElementalWeave) && Config.BLMPvP_ElementalWeave_SubOption &&
                            IsOffCooldown(ElementalWeave) && hasUmbralIce && playerCurrentPercentHp < Config.BLMPvP_ElementalWeave_PlayerHP)
                            return OriginalHook(ElementalWeave);
                    }

                    if (hasTarget && !targetHasImmunity)
                    {
                        // Elemental Weave (Offensive)
                        if (IsEnabled(CustomComboPreset.BLMPvP_ElementalWeave) && IsOffCooldown(ElementalWeave) && hasAstralFire &&
                            targetDistance <= 25 && playerCurrentPercentHp >= Config.BLMPvP_ElementalWeave_PlayerHP)
                            return OriginalHook(ElementalWeave);

                        if (!targetHasGuard)
                        {
                            // Lethargy
                            if (IsEnabled(CustomComboPreset.BLMPvP_Lethargy) && IsOffCooldown(Lethargy) && !isTargetNPC)
                            {
                                // Offensive
                                if (targetCurrentPercentHp < Config.BLMPvP_Lethargy_TargetHP && !targetHasHeavy)
                                    return OriginalHook(Lethargy);

                                // Defensive
                                if (Config.BLMPvP_Lethargy_SubOption && playerCurrentPercentHp < 50 && isPlayerTargeted)
                                    return OriginalHook(Lethargy);
                            }

                            // Burst (Offensive)
                            if (IsEnabled(CustomComboPreset.BLMPvP_Burst) && IsOffCooldown(Burst) && targetDistance <= 4)
                                return OriginalHook(Burst);

                            // Flare Star / Frost Star
                            if (IsEnabled(CustomComboPreset.BLMPvP_ElementalStar) && ((hasFlareStar && !isMoving) || (hasFrostStar && isElementalStarExpiring)))
                                return OriginalHook(SoulResonance);

                            // Xenoglossy
                            if (IsEnabled(CustomComboPreset.BLMPvP_Xenoglossy) && chargesXenoglossy > 0)
                            {
                                // Defensive
                                if (Config.BLMPvP_Xenoglossy_SubOption && playerCurrentPercentHp < 50)
                                    return OriginalHook(Xenoglossy);

                                // Offensive
                                if (!isResonanceExpiring && (isTargetNPC ? chargesXenoglossy > 1 && hasWreathOfFire : targetCurrentPercentHp < Config.BLMPvP_Xenoglossy_TargetHP))
                                    return OriginalHook(Xenoglossy);
                            }
                        }
                    }

                    // Paradox
                    if (hasParadox && ((isParadoxPrimed && !hasResonance) || (hasAstralFire && isMoving)))
                        return OriginalHook(Paradox);

                    // Fire Mode
                    if (!isMoving)
                    {
                        // High Blizzard II
                        if (hasIceAoE && !hasResonance)
                            return OriginalHook(Blizzard);

                        return OriginalHook(actionID);
                    }

                    // Ice Mode
                    else return OriginalHook(Blizzard);
                }

                return actionID;
            }
        }

        internal class BLMPvP_Manipulation_Feature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLMPvP_Manipulation_Feature;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                bool hasCrowdControl = HasEffectAny(PvPCommon.Debuffs.Stun) || HasEffectAny(PvPCommon.Debuffs.DeepFreeze) ||
                                       HasEffectAny(PvPCommon.Debuffs.Bind) || HasEffectAny(PvPCommon.Debuffs.Silence) || HasEffectAny(PvPCommon.Debuffs.MiracleOfNature);

                if (actionID is AetherialManipulation)
                {
                    if (IsOffCooldown(AetherialManipulation) && IsOffCooldown(PvPCommon.Purify) && hasCrowdControl)
                        return OriginalHook(PvPCommon.Purify);
                }

                return actionID;
            }
        }
    }
}