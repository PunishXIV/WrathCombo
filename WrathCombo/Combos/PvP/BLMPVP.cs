using Dalamud.Game.ClientState.Objects.Types;
using ImGuiNET;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Window.Functions;

namespace WrathCombo.Combos.PvP
{
    internal static class BLMPvP
    {
        #region IDs

        public const byte JobID = 25;
        
        internal class Role : PvPCaster;

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
            HighFire2 = 41473,
            HighBlizzard2 = 41474,
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
        #endregion

        #region Config
        public static class Config
        {
            public static UserInt
                BLMPvP_ElementalWeave_PlayerHP = new("BLMPvP_ElementalWeave_PlayerHP", 50),
                BLMPvP_Lethargy_TargetHP = new("BLMPvP_Lethargy_TargetHP", 50),
                BLMPvP_BurstMode_WreathOfIce = new("BLMPvP_BurstMode_WreathOfIce", 0),
                BLMPvP_BurstMode_WreathOfFireExecute = new("BLMPvP_BurstMode_WreathOfFireExecute", 0),
                BLMPVP_BurstButtonOption = new("BLMPVP_BurstButtonOption"),
                BLMPvP_Xenoglossy_TargetHP = new("BLMPvP_Xenoglossy_TargetHP", 50),
                BLMPvP_PhantomDartThreshold = new("BLMPvP_PhantomDartThreshold", 50);

            public static UserBool
                BLMPvP_Burst_SubOption = new("BLMPvP_Burst_SubOption"),
                BLMPvP_ElementalWeave_SubOption = new("BLMPvP_ElementalWeave_SubOption"),
                BLMPvP_Lethargy_SubOption = new("BLMPvP_Lethargy_SubOption"),
                BLMPvP_Xenoglossy_SubOption = new("BLMPvP_Xenoglossy_SubOption");

            public static UserFloat
                BLMPvP_Movement_Threshold = new("BLMPvP_Movement_Threshold", 0.1f);

            internal static void Draw(CustomComboPreset preset)
            {
                switch (preset)
                {
                    // Movement Threshold
                    case CustomComboPreset.BLMPvP_BurstMode:
                        UserConfig.DrawHorizontalRadioButton(BLMPvP.Config.BLMPVP_BurstButtonOption, "One Button Mode", "Combines Fire & Blizzard onto one button", 0);
                        UserConfig.DrawHorizontalRadioButton(BLMPvP.Config.BLMPVP_BurstButtonOption, "Dual Button Mode", "Puts the combo onto separate Fire & Blizzard buttons, which will only use that element.", 1);

                        if (BLMPvP.Config.BLMPVP_BurstButtonOption == 0)
                        {
                            ImGui.Indent();
                            UserConfig.DrawRoundedSliderFloat(0.1f, 3, BLMPvP.Config.BLMPvP_Movement_Threshold, "Movement Threshold", 137);
                            ImGui.Unindent();
                            if (ImGui.IsItemHovered())
                            {
                                ImGui.BeginTooltip();
                                ImGui.TextUnformatted("When under the effect of Astral Fire, must be\nmoving this long before using Blizzard spells.");
                                ImGui.EndTooltip();
                            }
                        }
                        break;

                    // Burst
                    case CustomComboPreset.BLMPvP_Burst:
                        UserConfig.DrawAdditionalBoolChoice(BLMPvP.Config.BLMPvP_Burst_SubOption, "Defensive Burst",
                            "Also uses Burst when under 50%% HP.\n- Will not use outside combat.");

                        break;

                    // Elemental Weave
                    case CustomComboPreset.BLMPvP_ElementalWeave:
                        UserConfig.DrawSliderInt(10, 100, BLMPvP.Config.BLMPvP_ElementalWeave_PlayerHP, "Player HP%", 180);
                        ImGui.Spacing();
                        UserConfig.DrawAdditionalBoolChoice(BLMPvP.Config.BLMPvP_ElementalWeave_SubOption, "Defensive Elemental Weave",
                            "When under, uses Wreath of Ice instead.\n- Will not use outside combat.");

                        break;

                    // Lethargy
                    case CustomComboPreset.BLMPvP_Lethargy:
                        UserConfig.DrawSliderInt(10, 100, BLMPvP.Config.BLMPvP_Lethargy_TargetHP, "Target HP%", 180);
                        ImGui.Spacing();
                        UserConfig.DrawAdditionalBoolChoice(BLMPvP.Config.BLMPvP_Lethargy_SubOption, "Defensive Lethargy",
                            "Also uses Lethargy when under 50%% HP.\n- Uses only when targeted by enemy.");

                        break;

                    // Xenoglossy
                    case CustomComboPreset.BLMPvP_Xenoglossy:
                        UserConfig.DrawSliderInt(10, 100, BLMPvP.Config.BLMPvP_Xenoglossy_TargetHP, "Target HP%", 180);
                        ImGui.Spacing();
                        UserConfig.DrawAdditionalBoolChoice(BLMPvP.Config.BLMPvP_Xenoglossy_SubOption, "Defensive Xenoglossy",
                            "Also uses Xenoglossy when under 50%% HP.");

                        break;

                    // Phantom Dart
                    case CustomComboPreset.BLMPvP_PhantomDart:
                        UserConfig.DrawSliderInt(1, 100, BLMPvP.Config.BLMPvP_PhantomDartThreshold,
                            "Target HP% to use Phantom Dart at or below");

                        break;
                }
            }
        }
        #endregion

        internal class BLMPvP_BurstMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLMPvP_BurstMode;

            protected override uint Invoke(uint actionID)
            {
                bool actionIsFire = actionID is Fire or Fire3 or Fire4 or HighFire2 or Flare;
                bool actionIsIce = actionID is Blizzard or Blizzard3 or Blizzard4 or HighBlizzard2 or Freeze;

                if (actionIsFire || (actionIsIce && Config.BLMPVP_BurstButtonOption == 1))
                {
                    #region Variables
                    float targetDistance = GetTargetDistance();
                    float targetCurrentPercentHp = GetTargetHPPercent();
                    float playerCurrentPercentHp = PlayerHealthPercentageHp();
                    uint chargesXenoglossy = HasCharges(Xenoglossy) ? GetCooldown(Xenoglossy).RemainingCharges : 0;
                    bool isMoving = IsMoving();
                    bool inCombat = InCombat();
                    bool hasTarget = HasTarget();
                    bool isTargetNPC = CurrentTarget is IBattleNpc && CurrentTarget.DataId != 8016;
                    bool hasParadox = HasEffect(Buffs.Paradox);
                    bool hasResonance = HasEffect(Buffs.SoulResonance);
                    bool hasWreathOfFire = HasEffect(Buffs.WreathOfFire);
                    bool hasFlareStar = OriginalHook(SoulResonance) is FlareStar;
                    bool hasFrostStar = OriginalHook(SoulResonance) is FrostStar;
                    bool targetHasGuard = TargetHasEffectAny(PvPCommon.Buffs.Guard);
                    bool targetHasHeavy = TargetHasEffectAny(PvPCommon.Debuffs.Heavy);
                    bool isPlayerTargeted = CurrentTarget?.TargetObjectId == LocalPlayer.GameObjectId;
                    bool isParadoxPrimed = HasEffect(Buffs.UmbralIce1) || HasEffect(Buffs.AstralFire1);
                    bool isMovingAdjusted = TimeMoving.TotalMilliseconds / 1000f >= Config.BLMPvP_Movement_Threshold;
                    bool isResonanceExpiring = HasEffect(Buffs.SoulResonance) && GetBuffRemainingTime(Buffs.SoulResonance) <= 10;
                    bool hasUmbralIce = HasEffect(Buffs.UmbralIce1) || HasEffect(Buffs.UmbralIce2) || HasEffect(Buffs.UmbralIce3);
                    bool isElementalStarDelayed = HasEffect(Buffs.ElementalStar) && GetBuffRemainingTime(Buffs.ElementalStar) <= 20;
                    bool hasAstralFire = HasEffect(Buffs.AstralFire1) || HasEffect(Buffs.AstralFire2) || HasEffect(Buffs.AstralFire3);
                    bool targetHasImmunity = TargetHasEffectAny(PLDPvP.Buffs.HallowedGround) || TargetHasEffectAny(DRKPvP.Buffs.UndeadRedemption);
                    #endregion

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

                            if (IsEnabled(CustomComboPreset.BLMPvP_PhantomDart) && Role.CanPhantomDart() && CanWeave() && GetTargetHPPercent() <= GetOptionValue(Config.BLMPvP_PhantomDartThreshold))
                                return Role.PhantomDart;

                            // Burst (Offensive)
                            if (IsEnabled(CustomComboPreset.BLMPvP_Burst) && IsOffCooldown(Burst) && targetDistance <= 4)
                                return OriginalHook(Burst);

                            // Flare Star / Frost Star
                            if (IsEnabled(CustomComboPreset.BLMPvP_ElementalStar) && ((hasFlareStar && !isMoving) || (hasFrostStar && isElementalStarDelayed)))
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


                    // Basic Spells
                    return isMovingAdjusted && Config.BLMPVP_BurstButtonOption == 0
                        ? OriginalHook(Blizzard)
                        : OriginalHook(actionID);


                }

                return actionID;
            }
        }

        internal class BLMPvP_Manipulation_Feature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLMPvP_Manipulation_Feature;
            protected override uint Invoke(uint actionID)
            {
                if (actionID is AetherialManipulation)
                {
                    bool hasCrowdControl = HasEffectAny(PvPCommon.Debuffs.Stun) || HasEffectAny(PvPCommon.Debuffs.DeepFreeze) ||
                                           HasEffectAny(PvPCommon.Debuffs.Bind) || HasEffectAny(PvPCommon.Debuffs.Silence) || HasEffectAny(PvPCommon.Debuffs.MiracleOfNature);

                    if (IsOffCooldown(AetherialManipulation) && IsOffCooldown(PvPCommon.Purify) && hasCrowdControl)
                        return OriginalHook(PvPCommon.Purify);
                }

                return actionID;
            }
        }
    }
}