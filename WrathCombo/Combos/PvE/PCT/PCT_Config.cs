using WrathCombo.Combos.PvP;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using WrathCombo.Window.Functions;
using static WrathCombo.Window.Functions.UserConfig;

namespace WrathCombo.Combos.PvE;

internal partial class PCT
{
    internal static class Config
    {
        public static UserInt
            CombinedAetherhueChoices = new("CombinedAetherhueChoices"),
            PCT_ST_AdvancedMode_LucidOption = new("PCT_ST_AdvancedMode_LucidOption", 6500),
            PCT_AoE_AdvancedMode_HolyinWhiteOption = new("PCT_AoE_AdvancedMode_HolyinWhiteOption", 0),
            PCT_AoE_AdvancedMode_LucidOption = new("PCT_AoE_AdvancedMode_LucidOption", 6500),
            PCT_VariantCure = new("PCT_VariantCure"),
            PCT_ST_CreatureStop = new("PCT_ST_CreatureStop", 0),
            PCT_AoE_CreatureStop = new("PCT_AoE_CreatureStop", 5),
            PCT_ST_WeaponStop = new("PCT_ST_WeaponStop", 0),
            PCT_AoE_WeaponStop = new("PCT_AoE_WeaponStop", 5),
            PCT_ST_LandscapeStop = new("PCT_ST_LandscapeStop", 0),
            PCT_AoE_LandscapeStop = new("PCT_AoE_LandscapeStop", 5),
            PCT_Opener_Choice = new("PCT_Opener_Choice", 0),
            PCT_Balance_Content = new("PCT_Balance_Content", 1),
            PCT_ST_StarryMuse_SubOption = new("PCT_ST_StarryMuse_SubOption", 1);

        public static UserBool
            CombinedMotifsMog = new("CombinedMotifsMog"),
            CombinedMotifsMadeen = new("CombinedMotifsMadeen"),
            CombinedMotifsWeapon = new("CombinedMotifsWeapon"),
            CombinedMotifsLandscape = new("CombinedMotifsLandscape");

        internal static void Draw(CustomComboPreset preset)
        {
            switch (preset)
            {
                case CustomComboPreset.PCT_ST_Advanced_Openers:
                    DrawHorizontalRadioButton(PCT_Opener_Choice, $"2nd GCD {StarryMuse.ActionName()}", "", 0);
                    DrawHorizontalRadioButton(PCT_Opener_Choice, $"3rd GCD {StarryMuse.ActionName()}", "", 1);

                    DrawBossOnlyChoice(PCT_Balance_Content);
                    break;

                case CustomComboPreset.CombinedAetherhues:
                    DrawRadioButton(CombinedAetherhueChoices, "Both Single Target & AoE",
                        $"Replaces both {FireInRed.ActionName()} & {FireIIinRed.ActionName()}", 0);

                    DrawRadioButton(CombinedAetherhueChoices, "Single Target Only",
                        $"Replace only {FireInRed.ActionName()}", 1);

                    DrawRadioButton(CombinedAetherhueChoices, "AoE Only",
                        $"Replace only {FireIIinRed.ActionName()}", 2);

                    break;

                case CustomComboPreset.CombinedMotifs:
                    DrawAdditionalBoolChoice(CombinedMotifsMog, $"{MogoftheAges.ActionName()} Feature",
                        $"Add {MogoftheAges.ActionName()} when fully drawn and off cooldown.");

                    DrawAdditionalBoolChoice(CombinedMotifsMadeen,
                        $"{RetributionoftheMadeen.ActionName()} Feature",
                        $"Add {RetributionoftheMadeen.ActionName()} when fully drawn and off cooldown.");

                    DrawAdditionalBoolChoice(CombinedMotifsWeapon, $"{HammerStamp.ActionName()} Feature",
                        $"Add {HammerStamp.ActionName()} when under the effect of {Buffs.HammerTime.StatusName()}.");

                    DrawAdditionalBoolChoice(CombinedMotifsLandscape, $"{StarPrism.ActionName()} Feature",
                        $"Add {StarPrism.ActionName()} when under the effect of {Buffs.Starstruck.StatusName()}.");
                    break;

                case CustomComboPreset.PCT_ST_AdvancedMode_LucidDreaming:
                    DrawSliderInt(0, 10000, PCT_ST_AdvancedMode_LucidOption,
                        "Add Lucid Dreaming when below this MP", sliderIncrement: SliderIncrements.Hundreds);

                    break;

                case CustomComboPreset.PCT_AoE_AdvancedMode_HolyinWhite:
                    DrawSliderInt(0, 5, PCT_AoE_AdvancedMode_HolyinWhiteOption,
                        "How many charges to keep ready? (0 = Use all)");

                    break;

                case CustomComboPreset.PCT_AoE_AdvancedMode_LucidDreaming:
                    DrawSliderInt(0, 10000, PCT_AoE_AdvancedMode_LucidOption,
                        "Add Lucid Dreaming when below this MP", sliderIncrement: SliderIncrements.Hundreds);

                    break;

                case CustomComboPreset.PCT_ST_AdvancedMode_ScenicMuse:
                    DrawHorizontalRadioButton(PCT_ST_StarryMuse_SubOption,
                        "All content",
                        $"Uses {StarryMuse.ActionName()} logic regardless of content.", 0);

                    DrawHorizontalRadioButton(PCT_ST_StarryMuse_SubOption,
                        "Boss encounters Only",
                        $"Only uses {StarryMuse.ActionName()} logic when in Boss encounters.", 1);

                    break;

                case CustomComboPreset.PCT_ST_AdvancedMode_LandscapeMotif:
                    DrawSliderInt(0, 10, PCT_ST_LandscapeStop, "Health % to stop Drawing Motif");

                    break;

                case CustomComboPreset.PCT_ST_AdvancedMode_CreatureMotif:
                    DrawSliderInt(0, 10, PCT_ST_CreatureStop, "Health % to stop Drawing Motif");

                    break;

                case CustomComboPreset.PCT_ST_AdvancedMode_WeaponMotif:
                    DrawSliderInt(0, 10, PCT_ST_WeaponStop, "Health % to stop Drawing Motif");

                    break;

                case CustomComboPreset.PCT_AoE_AdvancedMode_LandscapeMotif:
                    DrawSliderInt(0, 10, PCT_AoE_LandscapeStop, "Health % to stop Drawing Motif");

                    break;

                case CustomComboPreset.PCT_AoE_AdvancedMode_CreatureMotif:
                    DrawSliderInt(0, 10, PCT_AoE_CreatureStop, "Health % to stop Drawing Motif");

                    break;

                case CustomComboPreset.PCT_AoE_AdvancedMode_WeaponMotif:
                    DrawSliderInt(0, 10, PCT_AoE_WeaponStop, "Health % to stop Drawing Motif");

                    break;

                case CustomComboPreset.PCT_Variant_Cure:
                    DrawSliderInt(1, 100, PCT_VariantCure, "HP% to be at or under", 200);

                    break;

                // PvP
                case CustomComboPreset.PCTPvP_BurstControl:
                    DrawSliderInt(1, 100, PCTPvP.Config.PCTPvP_BurstHP, "Target HP%", 200);

                    break;

                case CustomComboPreset.PCTPvP_TemperaCoat:
                    DrawSliderInt(1, 100, PCTPvP.Config.PCTPvP_TemperaHP, "Player HP%", 200);

                    break;
            }
        }
    }
}
