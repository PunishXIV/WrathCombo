using Dalamud.Interface.Colors;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using WrathCombo.Window.Functions;
using static WrathCombo.Window.Functions.UserConfig;
namespace WrathCombo.Combos.PvE;

internal partial class PCT
{
    internal static class Config
    {
        #region Options
        public static UserInt
            CombinedAetherhueChoices = new("CombinedAetherhueChoices", 0),
            PCT_ST_AdvancedMode_BurnBoss = new("PCT_ST_AdvancedMode_BurnBoss"),
            PCT_AoE_AdvancedMode_BurnBoss = new("PCT_AoE_AdvancedMode_BurnBoss"),
            PCT_ST_AdvancedMode_LucidOption = new("PCT_ST_AdvancedMode_LucidOption", 6500),
            PCT_ST_AdvancedMode_HolyinWhiteOption = new("PCT_ST_AdvancedMode_HolyinWhiteOption", 2),
            PCT_AoE_AdvancedMode_HolyinWhiteOption = new("PCT_AoE_AdvancedMode_HolyinWhiteOption", 2),
            PCT_AoE_AdvancedMode_LucidOption = new("PCT_AoE_AdvancedMode_LucidOption", 6500),
            PCT_AoE_AdvancedMode_ScenicMuse_Threshold = new("PCT_AoE_AdvancedMode_ScenicMuse_Threshold", 20),
            PCT_ST_AdvancedMode_ScenicMuse_Threshold = new("PCT_ST_AdvancedMode_ScenicMuse_Threshold", 20),
            PCT_AoE_AdvancedMode_ScenicMuse_SubOption = new("PCT_AoE_AdvancedMode_ScenicMuse_SubOption"),
            PCT_ST_AdvancedMode_ScenicMuse_SubOption = new("PCT_ST_AdvancedMode_ScenicMuse_SubOption"),
            PCT_ST_CreatureStop = new("PCT_ST_CreatureStop", 10),
            PCT_AoE_CreatureStop = new("PCT_AoE_CreatureStop", 10),
            PCT_ST_WeaponStop = new("PCT_ST_WeaponStop", 10),
            PCT_AoE_WeaponStop = new("PCT_AoE_WeaponStop", 10),
            PCT_ST_LandscapeStop = new("PCT_ST_LandscapeStop", 10),
            PCT_AoE_LandscapeStop = new("PCT_AoE_LandscapeStop", 10),
            PCT_Opener_Choice = new("PCT_Opener_Choice", 0),
            PCT_Balance_Content = new("PCT_Balance_Content", 1);

        public static UserBool
            PCT_ST_AdvancedMode_ScenicMuse_MovementOption = new("PCT_ST_AdvancedMode_ScenicMuse_MovementOption"),
            PCT_AoE_AdvancedMode_ScenicMuse_MovementOption = new("PCT_AoE_AdvancedMode_ScenicMuse_MovementOption"),
            CombinedMotifsMog = new("CombinedMotifsMog"),
            CombinedMotifsMadeen = new("CombinedMotifsMadeen"),
            CombinedMotifsWeapon = new("CombinedMotifsWeapon"),
            CombinedMotifsLandscape = new("CombinedMotifsLandscape");
        
        public static UserFloat
            PCT_ST_AdvancedMode_HammerStampCombo_Timing = new("PCT_ST_AdvancedMode_HammerStampCombo_Timing", 30),
            PCT_AoE_AdvancedMode_HammerStampCombo_Timing = new("PCT_AoE_AdvancedMode_HammerStampCombo_Timing", 30);
        
        #endregion
    
        internal static void Draw(Preset preset)
        {
            switch (preset)
            {
                #region Single Target
                case Preset.PCT_ST_AdvancedMode:
                    DrawSliderInt(0, 10, PCT_ST_AdvancedMode_BurnBoss, "Stop pooling charges and burn bosses below this HP % (0% = Don't Burn).");
                    break;
                
                case Preset.PCT_ST_Advanced_Openers:
                    DrawBossOnlyChoice(PCT_Balance_Content);
                    ImGui.NewLine();
                    DrawRadioButton(PCT_Opener_Choice, $"2nd GCD {StarryMuse.ActionName()}", 
                        "Opener Failure Timeout (in Settings Tab) Must be set to 5+ seconds for opener to function due to long initial spell cast.", 0, descriptionAsTooltip: true);
                    DrawRadioButton(PCT_Opener_Choice, $"3rd GCD {StarryMuse.ActionName()}", 
                        "Opener Failure Timeout (in Settings Tab) Must be set to 5+ seconds for opener to function due to long initial spell cast.", 1, descriptionAsTooltip: true);
                    break;

                case Preset.PCT_ST_AdvancedMode_LucidDreaming:
                    DrawSliderInt(0, 10000, PCT_ST_AdvancedMode_LucidOption,
                        "Add Lucid Dreaming when below this MP", sliderIncrement: SliderIncrements.Hundreds);
                    break;
                
                case Preset.PCT_ST_AdvancedMode_ScenicMuse:
                    DrawAdditionalBoolChoice(PCT_ST_AdvancedMode_ScenicMuse_MovementOption, "Dont Use if Moving", "Will only use if not moving.");
                
                    DrawSliderInt(0, 100, PCT_ST_AdvancedMode_ScenicMuse_Threshold,
                        "Stop using Scenic Muse on targets below this HP % (0% = always use, 100% = never use).");
                    ImGui.Indent();
                    ImGui.TextColored(ImGuiColors.DalamudYellow, "Select what kind of enemies the HP check should be applied to:");
                    DrawHorizontalRadioButton(PCT_ST_AdvancedMode_ScenicMuse_SubOption,
                        "Non-boss Encounters Only", $"Applies HP check to Non-Boss Encounters only", 0);
                    DrawHorizontalRadioButton(PCT_ST_AdvancedMode_ScenicMuse_SubOption,
                        "All Content", $"Applies HP Check to All Content", 1);
                    ImGui.Unindent();
                    break;
                
                case Preset.PCT_AoE_AdvancedMode_ScenicMuse:
                    DrawAdditionalBoolChoice(PCT_AoE_AdvancedMode_ScenicMuse_MovementOption, "Dont Use if Moving", "Will only use if not moving.");
                    
                    DrawSliderInt(0, 100, PCT_AoE_AdvancedMode_ScenicMuse_Threshold,
                        "Stop using Scenic Muse on targets below this HP % (0% = always use, 100% = never use).");
                    ImGui.Indent();
                    ImGui.TextColored(ImGuiColors.DalamudYellow, "Select what kind of enemies the HP check should be applied to:");
                    DrawHorizontalRadioButton(PCT_AoE_AdvancedMode_ScenicMuse_SubOption,
                        "Non-boss Encounters Only", $"Applies HP check to Non-Boss Encounters only", 0);
                    DrawHorizontalRadioButton(PCT_AoE_AdvancedMode_ScenicMuse_SubOption,
                        "All Content", $"Applies HP Check to All Content", 1);
                    ImGui.Unindent();
                    break;
                
                case Preset.PCT_ST_AdvancedMode_HammerStampCombo:
                    DrawSliderFloat(15, 30, PCT_ST_AdvancedMode_HammerStampCombo_Timing, "Time Remaining on Hammer Time (seconds) to use combo. 30 = Use Immediately.", decimals: 0);
                    break;

                case Preset.PCT_ST_AdvancedMode_LandscapeMotif:
                    DrawSliderInt(0, 10, PCT_ST_LandscapeStop, "Health % to stop Drawing Motif");
                    break;

                case Preset.PCT_ST_AdvancedMode_CreatureMotif:
                    DrawSliderInt(0, 10, PCT_ST_CreatureStop, "Health % to stop Drawing Motif");
                    break;

                case Preset.PCT_ST_AdvancedMode_WeaponMotif:
                    DrawSliderInt(0, 10, PCT_ST_WeaponStop, "Health % to stop Drawing Motif");
                    break;
                
                case Preset.PCT_ST_AdvancedMode_HolyinWhite:
                    DrawSliderInt(0, 5, PCT_ST_AdvancedMode_HolyinWhiteOption,
                        "How many charges to keep ready? (0 = Use all)");
                    break;

                #endregion

                #region AoE
                
                case Preset.PCT_AoE_AdvancedMode:
                    DrawSliderInt(0, 10, PCT_AoE_AdvancedMode_BurnBoss, "Stop pooling charges and burn bosses below this HP % (0% = Don't Burn).");
                    break;
                
                case Preset.PCT_AoE_AdvancedMode_HolyinWhite:
                    DrawSliderInt(0, 5, PCT_AoE_AdvancedMode_HolyinWhiteOption,
                        "How many charges to keep ready? (0 = Use all)");
                    break;

                case Preset.PCT_AoE_AdvancedMode_LucidDreaming:
                    DrawSliderInt(0, 10000, PCT_AoE_AdvancedMode_LucidOption,
                        "Add Lucid Dreaming when below this MP", sliderIncrement: SliderIncrements.Hundreds);
                    break;
                
                case Preset.PCT_AoE_AdvancedMode_HammerStampCombo:
                    DrawSliderFloat(15, 30, PCT_AoE_AdvancedMode_HammerStampCombo_Timing, "Time Remaining on Hammer Time (seconds) to use combo. 30 = Use Immediately.", decimals: 0);
                    break;

                case Preset.PCT_AoE_AdvancedMode_LandscapeMotif:
                    DrawSliderInt(0, 10, PCT_AoE_LandscapeStop, "Health % to stop Drawing Motif");
                    break;

                case Preset.PCT_AoE_AdvancedMode_CreatureMotif:
                    DrawSliderInt(0, 10, PCT_AoE_CreatureStop, "Health % to stop Drawing Motif");
                    break;

                case Preset.PCT_AoE_AdvancedMode_WeaponMotif:
                    DrawSliderInt(0, 10, PCT_AoE_WeaponStop, "Health % to stop Drawing Motif");
                    break;

                #endregion

                #region Standalone
                case Preset.CombinedAetherhues:
                    DrawRadioButton(CombinedAetherhueChoices, "Both Single Target & AoE",
                        $"Replaces both {FireInRed.ActionName()} & {FireIIinRed.ActionName()}", 0);
                    DrawRadioButton(CombinedAetherhueChoices, "Single Target Only",
                        $"Replace only {FireInRed.ActionName()}", 1);
                    DrawRadioButton(CombinedAetherhueChoices, "AoE Only",
                        $"Replace only {FireIIinRed.ActionName()}", 2);
                    break;

                case Preset.CombinedMotifs:
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

                #endregion
            }
        }
    }
    
}
