using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;

namespace WrathCombo.Combos.PvP
{
    internal static class VPRPvP
    {
        public const byte JobID = 41;

        internal const uint
            SteelFangs = 39157,
            HuntersSting = 39159,
            BarbarousSlice = 39161,
            PiercingFangs = 39158,
            SwiftskinsSting = 39160,
            RavenousBite = 39163,
            SanguineFeast = 39167,
            Bloodcoil = 39166,
            UncoiledFury = 39168,
            SerpentsTail = 39183,
            FirstGeneration = 39169,
            SecondGeneration = 39170,
            ThirdGeneration = 39171,
            FourthGeneration = 39172,
            Ouroboros = 39173,
            DeathRattle = 39174,
            TwinfangBite = 39175,
            TwinbloodBite = 39176,
            UncoiledTwinfang = 39177,
            UncoiledTwinblood = 39178,
            FirstLegacy = 39179,
            SecondLegacy = 39180,
            ThirdLegacy = 39181,
            FourthLegacy = 39182,
            Slither = 39184,
            SnakeScales = 39185,
            Backlash = 39186,
            RattlingCoil = 39189;

        internal class Buffs
        {
            internal const ushort
                Slither = 4095,
                HardenedScales = 4096,
                SnakesBane = 4098;
        }

        internal class Config
        {
            internal static UserInt
                VPRPvP_Bloodcoil_TargetHP = new("VPRPvP_Bloodcoil_TargetHP", 80),
                VPRPvP_Bloodcoil_PlayerHP = new("VPRPvP_Bloodcoil_PlayerHP", 80),
                VPRPvP_UncoiledFury_TargetHP = new("VPRPvP_UncoiledFury_TargetHP", 60),
                VPRPvP_Slither_Charges = new("VPRPvP_Slither_Charges", 1),
                VPRPvP_Slither_Range = new("VPRPvP_Slither_Range", 6);

            internal static UserBoolArray
                VPRPvP_RattlingCoil_SubOptions = new("VPRPvP_RattlingCoil_SubOptions");

            public static UserBool
                VPRPvP_Backlash_SubOption = new("VPRPvP_Backlash_SubOption");
        }

        internal class VPRPvP_Burst : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.VPRPvP_Burst;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                #region Variables
                float targetDistance = GetTargetDistance();
                float targetCurrentPercentHp = GetTargetHPPercent();
                float playerCurrentPercentHp = PlayerHealthPercentageHp();
                uint chargesSlither = HasCharges(Slither) ? GetCooldown(Slither).RemainingCharges : 0;
                uint chargesUncoiledFury = HasCharges(UncoiledFury) ? GetCooldown(UncoiledFury).RemainingCharges : 0;
                bool[] optionsRattlingCoil = Config.VPRPvP_RattlingCoil_SubOptions;
                bool hasTarget = HasTarget();
                bool inMeleeRange = targetDistance <= 5;
                bool hasSlither = HasEffect(Buffs.Slither);
                bool hasBind = HasEffectAny(PvPCommon.Debuffs.Bind);
                bool targetHasImmunity = PvPCommon.IsImmuneToDamage();
                bool hasBacklash = OriginalHook(SnakeScales) is Backlash;
                bool hasOuroboros = OriginalHook(Bloodcoil) is Ouroboros;
                bool hasSnakesBane = hasBacklash && HasEffect(Buffs.SnakesBane);
                bool hasSanguineFeast = OriginalHook(Bloodcoil) is SanguineFeast;
                bool isMeleeDependant = !hasTarget || (hasTarget && inMeleeRange);
                bool isSnakeScalesDown = IsOnCooldown(SnakeScales) && !hasBacklash;
                bool isUncoiledFuryEnabled = IsEnabled(CustomComboPreset.VPRPvP_UncoiledFury);
                bool hasRangedWeave = OriginalHook(SerpentsTail) is UncoiledTwinfang or UncoiledTwinblood;
                bool hasCommonWeave = OriginalHook(SerpentsTail) is DeathRattle or TwinfangBite or TwinbloodBite;
                bool hasLegacyWeave = OriginalHook(SerpentsTail) is FirstLegacy or SecondLegacy or ThirdLegacy or FourthLegacy;
                bool isBloodcoilPrimed = IsOffCooldown(Bloodcoil) && hasTarget && !targetHasImmunity && !hasOuroboros && !hasSanguineFeast;
                bool inGenerationsCombo = OriginalHook(actionID) is FirstGeneration or SecondGeneration or ThirdGeneration or FourthGeneration;
                bool isUncoiledFuryPrimed = chargesUncoiledFury > 0 && hasTarget && !targetHasImmunity && targetCurrentPercentHp < Config.VPRPvP_UncoiledFury_TargetHP;
                bool isUncoiledFuryDependant = !isUncoiledFuryEnabled || !(isUncoiledFuryEnabled && isUncoiledFuryPrimed);
                bool isSlitherPrimed = hasTarget && !inMeleeRange && isUncoiledFuryDependant && !hasSlither && !hasBind;
                #endregion

                if (actionID is SteelFangs or HuntersSting or BarbarousSlice or PiercingFangs or SwiftskinsSting or RavenousBite)
                {
                    // Backlash
                    if (IsEnabled(CustomComboPreset.VPRPvP_Backlash) && ((!Config.VPRPvP_Backlash_SubOption && hasBacklash) ||
                        (Config.VPRPvP_Backlash_SubOption && hasSnakesBane)))
                        return OriginalHook(SnakeScales);

                    // Rattling Coil
                    if (IsEnabled(CustomComboPreset.VPRPvP_RattlingCoil) && IsOffCooldown(RattlingCoil) &&
                        ((optionsRattlingCoil[0] && chargesUncoiledFury == 0) || (optionsRattlingCoil[1] && isSnakeScalesDown)))
                        return OriginalHook(RattlingCoil);

                    // Slither
                    if (IsEnabled(CustomComboPreset.VPRPvP_Slither) && chargesSlither > Config.VPRPvP_Slither_Charges &&
                        isSlitherPrimed && targetDistance <= Config.VPRPvP_Slither_Range)
                        return OriginalHook(Slither);

                    // Serpent's Tail
                    if (hasRangedWeave || ((isMeleeDependant || isUncoiledFuryDependant) && (hasLegacyWeave || (hasCommonWeave && !inGenerationsCombo))))
                        return OriginalHook(SerpentsTail);

                    if (isMeleeDependant || isUncoiledFuryDependant)
                    {
                        // Reawakened
                        if (inGenerationsCombo)
                            return OriginalHook(actionID);

                        // Ouroboros / Sanguine Feast / Bloodcoil
                        if (hasOuroboros || hasSanguineFeast || (IsEnabled(CustomComboPreset.VPRPvP_Bloodcoil) && isBloodcoilPrimed &&
                            (targetCurrentPercentHp < Config.VPRPvP_Bloodcoil_TargetHP || playerCurrentPercentHp < Config.VPRPvP_Bloodcoil_PlayerHP)))
                            return OriginalHook(Bloodcoil);
                    }

                    // Uncoiled Fury
                    if (isUncoiledFuryEnabled && isUncoiledFuryPrimed)
                        return OriginalHook(UncoiledFury);
                }

                return actionID;
            }
        }

        internal class VPRPvP_SnakeScales_Feature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.VPRPvP_SnakeScales_Feature;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is SnakeScales)
                {
                    if (IsOffCooldown(RattlingCoil) && IsOnCooldown(SnakeScales) && OriginalHook(SnakeScales) is not Backlash)
                        return OriginalHook(RattlingCoil);
                }

                return actionID;
            }
        }
    }
}