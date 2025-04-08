using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Window.Functions;

namespace WrathCombo.Combos.PvP
{
    internal static class PCTPvP
    {

        #region IDs

        public const byte JobID = 42;

        internal class Role : PvPCaster;

        internal const uint
            FireInRed = 39191,
            AeroInGreen = 39192,
            WaterInBlue = 39193,
            HolyInWhite = 39198,
            CreatureMotif = 39204,
            LivingMuse = 39209,
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

            internal static void Draw(CustomComboPreset preset)
            {
                switch (preset)
                {
                    // Phantom Dart
                    case CustomComboPreset.PCTPvP_PhantomDart:
                        UserConfig.DrawSliderInt(1, 100, PCTPvP.Config.PCTPvP_PhantomDartThreshold,
                            "Target HP% to use Phantom Dart at or below");

                        break;

                    case CustomComboPreset.PCTPvP_BurstControl:
                        UserConfig.DrawSliderInt(1, 100, PCTPvP.Config.PCTPvP_BurstHP, "Target HP%", 200);

                        break;

                    case CustomComboPreset.PCTPvP_TemperaCoat:
                        UserConfig.DrawSliderInt(1, 100, PCTPvP.Config.PCTPvP_TemperaHP, "Player HP%", 200);

                        break;
                }
            }            
        }
        #endregion

        internal class PCTPvP_Burst : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PCTPvP_Burst;
            protected override uint Invoke(uint actionID)
            {
                #region Variables
                bool isMoving = IsMoving();
                bool hasTarget = HasTarget();
                bool hasStarPrism = HasEffect(Buffs.Starstruck);
                bool hasSubtractivePalette = HasEffect(Buffs.SubtractivePalette);
                bool hasPortrait = HasEffect(Buffs.MooglePortrait) || HasEffect(Buffs.MadeenPortrait);
                bool isStarPrismExpiring = HasEffect(Buffs.Starstruck) && GetBuffRemainingTime(Buffs.Starstruck) <= 3;
                bool isTemperaCoatExpiring = HasEffect(Buffs.TemperaCoat) && GetBuffRemainingTime(Buffs.TemperaCoat) <= 3;
                bool hasMotifDrawn = HasEffect(Buffs.PomMotif) || HasEffect(Buffs.WingMotif) || HasEffect(Buffs.ClawMotif) || HasEffect(Buffs.MawMotif);
                bool isBurstControlled = IsNotEnabled(CustomComboPreset.PCTPvP_BurstControl) || (IsEnabled(CustomComboPreset.PCTPvP_BurstControl) && GetTargetHPPercent() < Config.PCTPvP_BurstHP);
                #endregion

                if (actionID is FireInRed or AeroInGreen or WaterInBlue)
                {
                    // Tempera Coat / Tempera Grassa
                    if (IsEnabled(CustomComboPreset.PCTPvP_TemperaCoat))
                    {
                        if ((IsOffCooldown(TemperaCoat) &&
                        InCombat() && PlayerHealthPercentageHp() < Config.PCTPvP_TemperaHP) || isTemperaCoatExpiring)
                            return OriginalHook(TemperaCoat);
                    }

                    if (hasTarget && !PvPCommon.TargetImmuneToDamage())
                    {
                        // Star Prism
                        if (IsEnabled(CustomComboPreset.PCTPvP_StarPrism))
                        {
                            if (hasStarPrism && (isBurstControlled || isStarPrismExpiring))
                                return StarPrism;
                        }

                        if (IsEnabled(CustomComboPreset.PCTPvP_PhantomDart) && Role.CanPhantomDart() && CanWeave() && GetTargetHPPercent() <= GetOptionValue(Config.PCTPvP_PhantomDartThreshold))
                            return Role.PhantomDart;

                        // Moogle / Madeen Portrait
                        if (IsEnabled(CustomComboPreset.PCTPvP_MogOfTheAges) && hasPortrait && isBurstControlled)
                            return OriginalHook(MogOfTheAges);

                        // Living Muse
                        if (IsEnabled(CustomComboPreset.PCTPvP_LivingMuse) && hasMotifDrawn && HasCharges(OriginalHook(LivingMuse)) && isBurstControlled)
                            return OriginalHook(LivingMuse);

                        // Holy in White / Comet in Black
                        if (IsEnabled(CustomComboPreset.PCTPvP_HolyInWhite) && HasCharges(OriginalHook(HolyInWhite)) && isBurstControlled)
                            return OriginalHook(HolyInWhite);
                    }

                    // Creature Motif
                    if (IsEnabled(CustomComboPreset.PCTPvP_CreatureMotif) && !hasMotifDrawn && !isMoving)
                        return OriginalHook(CreatureMotif);

                    // Subtractive Palette
                    if (IsEnabled(CustomComboPreset.PCTPvP_SubtractivePalette))
                    {
                        if (IsOffCooldown(OriginalHook(SubtractivePalette)) &&
                            hasTarget && ((isMoving && hasSubtractivePalette) || (!isMoving && !hasSubtractivePalette)))
                            return OriginalHook(SubtractivePalette);
                    }
                }

                return actionID;
            }
        }
    }
}
