using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.Window.Functions.UserConfig;
using static WrathCombo.Combos.PvP.PCTPvP.Config;

namespace WrathCombo.Combos.PvP;

internal static class PCTPvP
{
    #region IDs
    internal class Role : PvPCaster;
    internal const uint
        FireInRed = 39191,
        AeroInGreen = 39192,
        WaterInBlue = 39193,
        HolyInWhite = 39198,
        CreatureMotif = 39204,
        LivingMuse = 39209,
        Smudge = 39210,
        TemperaCoat = 39211,
        SubtractivePalette = 39213,
        StarPrism = 39216,
        MogOfTheAges = 39782;

    internal class Buffs
    {
        internal const ushort
            PomMotif = 4105,
            WingMotif = 4106,
            ClawMotif = 4107,
            MawMotif = 4108,
            TemperaCoat = 4114,
            Starstruck = 4118,
            MooglePortrait = 4103,
            MadeenPortrait = 4104,
            SubtractivePalette = 4102;
    }
    #endregion

    #region Config
    public static class Config
    {
        public static UserInt
            PCTPvP_BurstHP = new("PCTPvP_BurstHP", 100),
            PCTPvP_TemperaHP = new("PCTPvP_TemperaHP", 50),
            PCTPvP_PhantomDartThreshold = new("PCTPvP_PhantomDartThreshold", 50);

        internal static void Draw(Preset preset)
        {
            switch (preset)
            {
                // Phantom Dart
                case Preset.PCTPvP_PhantomDart: DrawSliderInt(1, 100, PCTPvP_PhantomDartThreshold,
                        "Target HP% to use Phantom Dart at or below");
                    break;

                case Preset.PCTPvP_BurstControl: DrawSliderInt(1, 100, PCTPvP_BurstHP, "Target HP%", 200);
                    break;

                case Preset.PCTPvP_TemperaCoat: DrawSliderInt(1, 100, PCTPvP_TemperaHP, "Player HP%", 200);
                    break;
            }
        }            
    }
    #endregion

    internal class PCTPvP_Burst : CustomCombo
    {
        protected internal override Preset Preset => Preset.PCTPvP_Burst;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (FireInRed or AeroInGreen or WaterInBlue)) 
                return actionID;
            
            #region Variables
            bool isMoving = IsMoving();
            bool hasTarget = HasTarget();
            bool hasStarPrism = HasStatusEffect(Buffs.Starstruck);
            bool hasSubtractivePalette = HasStatusEffect(Buffs.SubtractivePalette);
            bool hasPortrait = HasStatusEffect(Buffs.MooglePortrait) || HasStatusEffect(Buffs.MadeenPortrait);
            bool isStarPrismExpiring = HasStatusEffect(Buffs.Starstruck) && GetStatusEffectRemainingTime(Buffs.Starstruck) <= 3;
            bool isTemperaCoatExpiring = HasStatusEffect(Buffs.TemperaCoat) && GetStatusEffectRemainingTime(Buffs.TemperaCoat) <= 3;
            bool hasMotifDrawn = HasStatusEffect(Buffs.PomMotif) || HasStatusEffect(Buffs.WingMotif) || HasStatusEffect(Buffs.ClawMotif) || HasStatusEffect(Buffs.MawMotif);
            bool isBurstControlled = IsNotEnabled(Preset.PCTPvP_BurstControl) || (IsEnabled(Preset.PCTPvP_BurstControl) && GetTargetHPPercent() < PCTPvP_BurstHP);
            #endregion
            
            // Tempera Coat / Tempera Grassa
            if (IsEnabled(Preset.PCTPvP_TemperaCoat))
            {
                if ((IsOffCooldown(TemperaCoat) &&
                     InCombat() && PlayerHealthPercentageHp() < PCTPvP_TemperaHP) || isTemperaCoatExpiring)
                    return OriginalHook(TemperaCoat);
            }
            if (hasTarget && !PvPCommon.TargetImmuneToDamage())
            {
                // Star Prism
                if (IsEnabled(Preset.PCTPvP_StarPrism))
                {
                    if (hasStarPrism && (isBurstControlled || isStarPrismExpiring))
                        return StarPrism;
                }
                if (IsEnabled(Preset.PCTPvP_PhantomDart) && Role.CanPhantomDart() && CanWeave() && GetTargetHPPercent() <= (PCTPvP_PhantomDartThreshold))
                    return Role.PhantomDart;

                // Moogle / Madeen Portrait
                if (IsEnabled(Preset.PCTPvP_MogOfTheAges) && hasPortrait && isBurstControlled)
                    return OriginalHook(MogOfTheAges);

                // Living Muse
                if (IsEnabled(Preset.PCTPvP_LivingMuse) && hasMotifDrawn && HasCharges(OriginalHook(LivingMuse)) && isBurstControlled)
                    return OriginalHook(LivingMuse);

                // Holy in White / Comet in Black
                if (IsEnabled(Preset.PCTPvP_HolyInWhite) && HasCharges(OriginalHook(HolyInWhite)) && isBurstControlled)
                    return OriginalHook(HolyInWhite);
            }
            // Creature Motif
            if (IsEnabled(Preset.PCTPvP_CreatureMotif) && !hasMotifDrawn && !isMoving)
                return OriginalHook(CreatureMotif);

            // Subtractive Palette
            if (IsEnabled(Preset.PCTPvP_SubtractivePalette))
            {
                if (IsOffCooldown(OriginalHook(SubtractivePalette)) &&
                    hasTarget && ((isMoving && hasSubtractivePalette) || (!isMoving && !hasSubtractivePalette)))
                    return OriginalHook(SubtractivePalette);
            }
            return actionID;
        }
    }
}