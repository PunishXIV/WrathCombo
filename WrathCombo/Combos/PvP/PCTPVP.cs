using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;

namespace WrathCombo.Combos.PvP
{
    internal static class PCTPvP
    {
        public const byte JobID = 42;

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

        internal class Config
        {
            internal static UserInt
                PCTPvP_BurstHP = new("PCTPvP_BurstHP", 100),
                PCTPvP_TemperaHP = new("PCTPvP_TemperaHP", 50);
        }

        internal class PCTPvP_Burst : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PCTPvP_Burst;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
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
