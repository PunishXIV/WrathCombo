using Dalamud.Game.ClientState.Objects.Types;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.Window.Functions.UserConfig;
using static WrathCombo.Combos.PvP.BLMPvP.Config;

namespace WrathCombo.Combos.PvP;

internal static class BLMPvP
{
    #region IDs
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
            BLMPVP_BurstButtonOption = new("BLMPVP_BurstButtonOption"),
            BLMPvP_Xenoglossy_TargetHP = new("BLMPvP_Xenoglossy_TargetHP", 50),
            BLMPvP_PhantomDartThreshold = new("BLMPvP_PhantomDartThreshold", 50),
            BLMPvP_Manipulation_Feature_PurifyMPThreshold = new("BLMPvP_Manipulation_Feature_PurifyMPThreshold", 5000);

        public static UserBool
            BLMPvP_Burst_SubOption = new("BLMPvP_Burst_SubOption"),
            BLMPvP_ElementalWeave_SubOption = new("BLMPvP_ElementalWeave_SubOption"),
            BLMPvP_Lethargy_SubOption = new("BLMPvP_Lethargy_SubOption"),
            BLMPvP_Xenoglossy_SubOption = new("BLMPvP_Xenoglossy_SubOption");

        public static UserFloat
            BLMPvP_Movement_Threshold = new("BLMPvP_Movement_Threshold", 0.1f);

        internal static void Draw(Preset preset)
        {
            switch (preset)
            {
                // Movement Threshold
                case Preset.BLMPvP_BurstMode:
                    DrawHorizontalRadioButton(BLMPVP_BurstButtonOption, "One Button Mode", "Combines Fire & Blizzard onto one button", 0);
                    DrawHorizontalRadioButton(BLMPVP_BurstButtonOption, "Dual Button Mode", "Puts the combo onto separate Fire & Blizzard buttons, which will only use that element.", 1);

                    if (BLMPVP_BurstButtonOption == 0)
                    {
                        ImGui.Indent();
                        DrawRoundedSliderFloat(0.1f, 3, BLMPvP_Movement_Threshold, "Movement Threshold", 137);
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
                case Preset.BLMPvP_Burst:
                    DrawAdditionalBoolChoice(BLMPvP_Burst_SubOption, "Defensive Burst",
                        "Also uses Burst when under 50% HP.\n- Will not use outside combat.");
                    break;

                // Elemental Weave
                case Preset.BLMPvP_ElementalWeave:
                    DrawSliderInt(10, 100, BLMPvP_ElementalWeave_PlayerHP, "Player HP%", 180);
                    ImGui.Spacing();
                    DrawAdditionalBoolChoice(BLMPvP_ElementalWeave_SubOption, "Defensive Elemental Weave",
                        "When under, uses Wreath of Ice instead.\n- Will not use outside combat.");
                    break;

                // Lethargy
                case Preset.BLMPvP_Lethargy:
                    DrawSliderInt(10, 100, BLMPvP_Lethargy_TargetHP, "Target HP%", 180);
                    ImGui.Spacing();
                    DrawAdditionalBoolChoice(BLMPvP_Lethargy_SubOption, "Defensive Lethargy",
                        "Also uses Lethargy when under 50% HP.\n- Uses only when targeted by enemy.");
                    break;

                // Xenoglossy
                case Preset.BLMPvP_Xenoglossy:
                    DrawSliderInt(10, 100, BLMPvP_Xenoglossy_TargetHP, "Target HP%", 180);
                    ImGui.Spacing();
                    DrawAdditionalBoolChoice(BLMPvP_Xenoglossy_SubOption, "Defensive Xenoglossy",
                        "Also uses Xenoglossy when under 50% HP.");
                    break;

                // Phantom Dart
                case Preset.BLMPvP_PhantomDart:
                    DrawSliderInt(1, 100, BLMPvP_PhantomDartThreshold, "Target HP% to use Phantom Dart at or below");
                    break;
                
                case Preset.BLMPvP_Manipulation_Feature:
                    DrawSliderInt(2500, 10000, BLMPvP_Manipulation_Feature_PurifyMPThreshold, "Do not use Purify below set MP.");
                    break;
                    
            }
        }
    }
    #endregion

    internal class BLMPvP_BurstMode : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLMPvP_BurstMode;

        protected override uint Invoke(uint actionID)
        {
            bool actionIsFire = actionID is Fire or Fire3 or Fire4 or HighFire2 or Flare;
            bool actionIsIce = actionID is Blizzard or Blizzard3 or Blizzard4 or HighBlizzard2 or Freeze;

            if (!actionIsFire && (!actionIsIce || BLMPVP_BurstButtonOption != 1)) 
                return actionID;

            #region Variables
            float targetDistance = GetTargetDistance();
            float targetCurrentPercentHp = GetTargetHPPercent();
            float playerCurrentPercentHp = PlayerHealthPercentageHp();
            uint chargesXenoglossy = HasCharges(Xenoglossy) ? GetCooldown(Xenoglossy).RemainingCharges : 0;
            bool isMoving = IsMoving();
            bool inCombat = InCombat();
            bool hasTarget = HasTarget();
            bool isTargetNPC = CurrentTarget is IBattleNpc && CurrentTarget.DataId != 8016;
            bool hasParadox = HasStatusEffect(Buffs.Paradox);
            bool hasResonance = HasStatusEffect(Buffs.SoulResonance);
            bool hasWreathOfFire = HasStatusEffect(Buffs.WreathOfFire);
            bool hasFlareStar = OriginalHook(SoulResonance) is FlareStar;
            bool hasFrostStar = OriginalHook(SoulResonance) is FrostStar;
            bool targetHasGuard = HasStatusEffect(PvPCommon.Buffs.Guard, CurrentTarget, true);
            bool targetHasHeavy = HasStatusEffect(PvPCommon.Debuffs.Heavy, CurrentTarget, true);
            bool isPlayerTargeted = CurrentTarget?.TargetObjectId == LocalPlayer.GameObjectId;
            bool isParadoxPrimed = HasStatusEffect(Buffs.UmbralIce1) || HasStatusEffect(Buffs.AstralFire1);
            bool isMovingAdjusted = TimeMoving.TotalMilliseconds / 1000f >= BLMPvP_Movement_Threshold;
            bool isResonanceExpiring = HasStatusEffect(Buffs.SoulResonance) && GetStatusEffectRemainingTime(Buffs.SoulResonance) <= 10;
            bool hasUmbralIce = HasStatusEffect(Buffs.UmbralIce1) || HasStatusEffect(Buffs.UmbralIce2) || HasStatusEffect(Buffs.UmbralIce3);
            bool isElementalStarDelayed = HasStatusEffect(Buffs.ElementalStar) && GetStatusEffectRemainingTime(Buffs.ElementalStar) <= 20;
            bool hasAstralFire = HasStatusEffect(Buffs.AstralFire1) || HasStatusEffect(Buffs.AstralFire2) || HasStatusEffect(Buffs.AstralFire3);
            bool targetHasImmunity = HasStatusEffect(PLDPvP.Buffs.HallowedGround, CurrentTarget, true) || HasStatusEffect(DRKPvP.Buffs.UndeadRedemption, CurrentTarget, true);
            #endregion

            if (inCombat)
            {
                // Burst (Defensive)
                if (IsEnabled(Preset.BLMPvP_Burst) && BLMPvP_Burst_SubOption && IsOffCooldown(Burst) && playerCurrentPercentHp < 50)
                    return OriginalHook(Burst);

                // Elemental Weave (Defensive)
                if (IsEnabled(Preset.BLMPvP_ElementalWeave) && BLMPvP_ElementalWeave_SubOption &&
                    IsOffCooldown(ElementalWeave) && hasUmbralIce && playerCurrentPercentHp < BLMPvP_ElementalWeave_PlayerHP)
                    return OriginalHook(ElementalWeave);
            }

            if (hasTarget && !targetHasImmunity)
            {
                // Elemental Weave (Offensive)
                if (IsEnabled(Preset.BLMPvP_ElementalWeave) && IsOffCooldown(ElementalWeave) && hasAstralFire &&
                    targetDistance <= 25 && playerCurrentPercentHp >= BLMPvP_ElementalWeave_PlayerHP)
                    return OriginalHook(ElementalWeave);

                if (!targetHasGuard)
                {
                    // Lethargy
                    if (IsEnabled(Preset.BLMPvP_Lethargy) && IsOffCooldown(Lethargy) && !isTargetNPC)
                    {
                        // Offensive
                        if (targetCurrentPercentHp < BLMPvP_Lethargy_TargetHP && !targetHasHeavy)
                            return OriginalHook(Lethargy);

                        // Defensive
                        if (BLMPvP_Lethargy_SubOption && playerCurrentPercentHp < 50 && isPlayerTargeted)
                            return OriginalHook(Lethargy);
                    }

                    if (IsEnabled(Preset.BLMPvP_PhantomDart) && Role.CanPhantomDart() && CanWeave() && GetTargetHPPercent() <= BLMPvP_PhantomDartThreshold)
                        return Role.PhantomDart;

                    // Burst (Offensive)
                    if (IsEnabled(Preset.BLMPvP_Burst) && IsOffCooldown(Burst) && targetDistance <= 4)
                        return OriginalHook(Burst);

                    // Flare Star / Frost Star
                    if (IsEnabled(Preset.BLMPvP_ElementalStar) && ((hasFlareStar && !isMoving) || (hasFrostStar && isElementalStarDelayed)))
                        return OriginalHook(SoulResonance);

                    // Xenoglossy
                    if (IsEnabled(Preset.BLMPvP_Xenoglossy) && chargesXenoglossy > 0)
                    {
                        // Defensive
                        if (BLMPvP_Xenoglossy_SubOption && playerCurrentPercentHp < 50)
                            return OriginalHook(Xenoglossy);

                        // Offensive
                        if (!isResonanceExpiring && (isTargetNPC ? chargesXenoglossy > 1 && hasWreathOfFire : targetCurrentPercentHp < BLMPvP_Xenoglossy_TargetHP))
                            return OriginalHook(Xenoglossy);
                    }
                }
            }
            // Paradox
            if (hasParadox && ((isParadoxPrimed && !hasResonance) || (hasAstralFire && isMoving)))
                return OriginalHook(Paradox);

            // Basic Spells
            return isMovingAdjusted && BLMPVP_BurstButtonOption == 0
                ? OriginalHook(Blizzard)
                : OriginalHook(actionID);
        }
    }
    internal class BLMPvP_Manipulation_Feature : CustomCombo
    {
        protected internal override Preset Preset => Preset.BLMPvP_Manipulation_Feature;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not AetherialManipulation) 
                return actionID;
            
            bool hasCrowdControl = HasStatusEffect(PvPCommon.Debuffs.Stun, anyOwner: true) || HasStatusEffect(PvPCommon.Debuffs.DeepFreeze, anyOwner: true) ||
                                   HasStatusEffect(PvPCommon.Debuffs.Bind, anyOwner: true) || HasStatusEffect(PvPCommon.Debuffs.Silence, anyOwner: true) || HasStatusEffect(PvPCommon.Debuffs.MiracleOfNature, anyOwner: true);

            if (IsOffCooldown(AetherialManipulation) && IsOffCooldown(PvPCommon.Purify) && hasCrowdControl && LocalPlayer.CurrentMp >= BLMPvP_Manipulation_Feature_PurifyMPThreshold)
                return OriginalHook(PvPCommon.Purify);

            return actionID;
        }
    }
}