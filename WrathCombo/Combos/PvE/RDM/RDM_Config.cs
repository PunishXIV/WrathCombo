using Dalamud.Interface.Colors;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using WrathCombo.Resources.Localization.JobConfigs;
using static WrathCombo.Window.Functions.SliderIncrements;
using static WrathCombo.Window.Functions.UserConfig;
using static WrathCombo.Window.Text;
namespace WrathCombo.Combos.PvE;
internal partial class RDM
{
    internal static class Config
    {
        #region Options
        public static UserBool RDM_ST_ThunderAero_Pull = new("RDM_ST_ThunderAero_Pull", true),
            RDM_VerAero_Dynamic = new("RDM_VerAero_Dynamic", true),
            RDM_VerThunder_Dynamic = new("RDM_VerThunder_Dynamic", true),
            RDM_VerAero2_Dynamic = new("RDM_VerAero2_Dynamic", true),
            RDM_VerThunder2_Dynamic = new("RDM_VerThunder2_Dynamic", true);


        public static UserInt
            RDM_ST_Lucid_Threshold = new("RDM_LucidDreaming_Threshold", 6500),
            RDM_AoE_Lucid_Threshold = new("RDM_AoE_Lucid_Threshold", 6500),
            RDM_BalanceOpener_Content = new("RDM_BalanceOpener_Content", 1),
            RDM_ST_Acceleration_Charges = new("RDM_ST_Acceleration_Charges", 0),
            RDM_AoE_Acceleration_Charges = new("RDM_AoE_Acceleration_Charges", 0),
            RDM_ST_Corpsacorps_Distance = new("RDM_ST_Corpsacorps_Distance", 25),
            RDM_ST_Corpsacorps_Time = new("RDM_ST_Corpsacorps_Time", 0),
            RDM_ST_GapCloseCorpsacorps_Time = new("RDM_ST_GapCloseCorpsacorps_Time", 0),
            RDM_ST_VerCureThreshold = new("RDM_ST_VerCureThreshold", 40),
            RDM_ST_MeleeCombo_IncludeReprise_Distance = new("RDM_ST_MeleeCombo_IncludeReprise_Distance", 5),
            RDM_ST_Embolden_Threshold = new("RDM_ST_Embolden_Threshold", 20),
            RDM_ST_Embolden_SubOption = new("RDM_ST_Embolden_SubOption"),
            RDM_ST_Manafication_Threshold = new("RDM_ST_Manafication_Threshold", 20),
            RDM_ST_Manafication_SubOption = new("RDM_ST_Manafication_SubOption"),
            RDM_AoE_Corpsacorps_Distance = new("RDM_AoE_Corpsacorps_Distance", 25),
            RDM_AoE_Corpsacorps_Time = new("RDM_AoE_Corpsacorps_Time", 0),
            RDM_AoE_GapCloseCorpsacorps_Time = new("RDM_AoE_GapCloseCorpsacorps_Time", 0),
            RDM_AoE_VerCureThreshold = new("RDM_AoE_VerCureThreshold", 40),
            RDM_AoE_Embolden_Threshold = new("RDM_AoE_Embolden_Threshold", 20),
            RDM_AoE_Embolden_SubOption = new("RDM_AoE_Embolden_SubOption"),
            RDM_AoE_Manafication_Threshold = new("RDM_AoE_Manafication_Threshold", 20),
            RDM_AoE_Manafication_SubOption = new("RDM_AoE_Manafication_SubOption"),
            RDM_Opener_Selection = new("RDM_Opener_Selection", 0),
            RDM_Riposte_Weaves_Options_EngagementCharges = new("RDM_Riposte_Weaves_Options_EngagementCharges", 0),
            RDM_Riposte_Weaves_Options_CorpsCharges = new("RDM_Riposte_Weaves_Options_CorpsCharges", 0),
            RDM_Riposte_Weaves_Options_Corpsacorps_Distance = new("RDM_Riposte_Weaves_Options_Corpsacorps_Distance", 25),
            RDM_Moulinet_Weaves_Options_EngagementCharges = new("RDM_Moulinet_Weaves_Options_EngagementCharges", 0),
            RDM_Moulinet_Weaves_Options_CorpsCharges = new("RDM_Moulinet_Weaves_Options_CorpsCharges", 0),
            RDM_Moulinet_Weaves_Options_Corpsacorps_Distance = new("RDM_Moulinet_Weaves_Options_Corpsacorps_Distance", 25),
            RDM_OGCDs_Options_CorpsCharges = new("RDM_OGCDs_Options_CorpsCharges", 0),
            RDM_OGCDs_Options_EngagementCharges = new("RDM_OGCDs_Options_EngagementCharges", 0),
            RDM_OGCDs_Options_Corpsacorps_Distance = new("RDM_OGCDs_Options_Corpsacorps_Distance", 25),
            RDM_RetargetVercure_Health = new("RDM_RetargetVercure_Health", 50),
            RDM_MagickProtectionDuration = new("RDM_MagickProtectionDuration"),
            RDM_AddleDuration = new("RDM_AddleDuration");

        internal static UserBoolArray
            RDM_OGCDs_Options = new("RDM_OGCDs_Options"),
            RDM_Riposte_Weaves_Options = new("RDM_Riposte_Weaves_Options"),
            RDM_Moulinet_Weaves_Options = new("RDM_Moulinet_Weaves_Options"),
            RDM_VerAero_Options = new("RDM_VerAero_Options"),
            RDM_VerThunder_Options = new("RDM_VerThunder_Options"),
            RDM_VerAero2_Options = new("RDM_VerAero2_Options"),
            RDM_VerThunder2_Options = new("RDM_VerThunder2_Options");

        #endregion

        internal static void Draw(Preset preset)
        {
            switch (preset)
            {
                #region Single Target
                case Preset.RDM_Balance_Opener:
                    DrawBossOnlyChoice(RDM_BalanceOpener_Content);
                    ImGui.NewLine();
                    DrawRadioButton(RDM_Opener_Selection, Generics.StandardOpener,
                        RDM_Config.RDMOpenerWarning, 0, descriptionAsTooltip: true);
                    DrawRadioButton(RDM_Opener_Selection, RDM_Config.RDMGapCloserOpener,
                        RDM_Config.RDMOpenerWarning, 1, descriptionAsTooltip: true);
                    break;

                case Preset.RDM_ST_ThunderAero:
                    DrawAdditionalBoolChoice(RDM_ST_ThunderAero_Pull, FormatAndCache(RDM_Config.PullWith0_1, Verthunder.ActionName(), Veraero.ActionName()), FormatAndCache(RDM_Config.StartsWith0_1, Verthunder.ActionName(), Veraero.ActionName()));
                    break;

                case Preset.RDM_ST_VerCure:
                    DrawSliderInt(1, 100, RDM_ST_VerCureThreshold, Generics.PlayerHPLessOrEqual, 200);
                    break;

                case Preset.RDM_ST_Embolden:
                    DrawSliderInt(0, 100, RDM_ST_Embolden_Threshold,
                        FormatAndCache(Generics.StopUsing0WhenBelowTargetHPPercentage, Embolden.ActionName()));
                    ImGui.Indent();
                    ImGui.TextColored(ImGuiColors.DalamudYellow, Generics.EnemyTypeCheck);
                    DrawHorizontalRadioButton(RDM_ST_Embolden_SubOption,
                        Generics.NonBossEncountersOnly, Generics.HPCheckNonBossEncountersOnly, 0);
                    DrawHorizontalRadioButton(RDM_ST_Embolden_SubOption,
                        Generics.AllContent, Generics.HPCheckAllContent, 1);
                    ImGui.Unindent();
                    break;

                case Preset.RDM_ST_Manafication:
                    DrawSliderInt(0, 100, RDM_ST_Manafication_Threshold,
                        FormatAndCache(Generics.StopUsing0WhenBelowTargetHPPercentage, Manafication.ActionName()));
                    ImGui.Indent();
                    ImGui.TextColored(ImGuiColors.DalamudYellow, Generics.EnemyTypeCheck);
                    DrawHorizontalRadioButton(RDM_ST_Manafication_SubOption,
                        Generics.NonBossEncountersOnly, Generics.HPCheckNonBossEncountersOnly, 0);
                    DrawHorizontalRadioButton(RDM_ST_Manafication_SubOption,
                        Generics.AllContent, Generics.HPCheckAllContent, 1);
                    ImGui.Unindent();
                    break;

                case Preset.RDM_ST_MeleeCombo:
                    if (CustomComboFunctions.IsNotEnabled(Preset.RDM_ST_MeleeCombo_IncludeRiposte))
                    {
                        ImGui.Indent();
                        ImGui.TextColored(ImGuiColors.DalamudRed, FormatAndCache(RDM_Config.AutoRotationWarning1, Riposte.ActionName()).ToUpper());
                        ImGui.TextColored(ImGuiColors.DalamudRed, RDM_Config.AutoRotationWarning2);
                        ImGui.Unindent();
                    }
                    break;

                case Preset.RDM_ST_Corpsacorps:
                    DrawSliderInt(0, 25, RDM_ST_Corpsacorps_Distance,
                        Generics.UseWhenDistanceFromTargetIsLessThanOrEqualTo);

                    DrawSliderInt(0, 5, RDM_ST_Corpsacorps_Time,
                        Generics.StationaryDelayCheck);
                    break;

                case Preset.RDM_ST_MeleeCombo_GapCloser:
                    DrawSliderInt(0, 5, RDM_ST_GapCloseCorpsacorps_Time,
                        Generics.StationaryDelayCheck);
                    break;

                case Preset.RDM_ST_MeleeCombo_IncludeReprise:
                    DrawSliderInt(4, 25, RDM_ST_MeleeCombo_IncludeReprise_Distance,
                        Generics.UseWhenDistanceFromTargetIsGreaterThanOrEqualTo);
                    break;

                #endregion

                #region AOE
                case Preset.RDM_AoE_Corpsacorps:
                    DrawSliderInt(0, 25, RDM_AoE_Corpsacorps_Distance,
                        Generics.UseWhenDistanceFromTargetIsLessThanOrEqualTo);
                    DrawSliderInt(0, 5, RDM_AoE_Corpsacorps_Time,
                         Generics.StationaryDelayCheck);
                    break;

                case Preset.RDM_AoE_MeleeCombo_GapCloser:
                    DrawSliderInt(0, 5, RDM_AoE_GapCloseCorpsacorps_Time,
                         Generics.StationaryDelayCheck);
                    break;

                case Preset.RDM_AoE_Embolden:
                    DrawSliderInt(0, 100, RDM_AoE_Embolden_Threshold,
                        FormatAndCache(Generics.StopUsing0WhenBelowTargetHPPercentage, Embolden.ActionName()));
                    ImGui.Indent();
                    ImGui.TextColored(ImGuiColors.DalamudYellow, Generics.EnemyTypeCheck);
                    DrawHorizontalRadioButton(RDM_AoE_Embolden_SubOption,
                        Generics.NonBossEncountersOnly, Generics.HPCheckNonBossEncountersOnly, 0);
                    DrawHorizontalRadioButton(RDM_AoE_Embolden_SubOption,
                        Generics.AllContent, Generics.HPCheckAllContent, 1);
                    ImGui.Unindent();
                    break;

                case Preset.RDM_AoE_Manafication:
                    DrawSliderInt(0, 100, RDM_AoE_Manafication_Threshold,
                       FormatAndCache(Generics.StopUsing0WhenBelowTargetHPPercentage, Manafication.ActionName()));
                    ImGui.Indent();
                    ImGui.TextColored(ImGuiColors.DalamudYellow, Generics.EnemyTypeCheck);
                    DrawHorizontalRadioButton(RDM_AoE_Manafication_SubOption,
                        Generics.NonBossEncountersOnly, Generics.HPCheckNonBossEncountersOnly, 0);
                    DrawHorizontalRadioButton(RDM_AoE_Manafication_SubOption,
                        Generics.AllContent, Generics.HPCheckAllContent, 1);
                    ImGui.Unindent();
                    break;

                case Preset.RDM_AoE_VerCure:
                    DrawSliderInt(1, 100, RDM_AoE_VerCureThreshold, Generics.PlayerHPLessOrEqual, 200);
                    break;

                case Preset.RDM_ST_Lucid:
                    DrawSliderInt(0, 10000, RDM_ST_Lucid_Threshold, Generics.LucidMP, sliderIncrement: Hundreds);
                    break;

                case Preset.RDM_AoE_Lucid:
                    DrawSliderInt(0, 10000, RDM_AoE_Lucid_Threshold, Generics.LucidMP, sliderIncrement: Hundreds);
                    break;

                case Preset.RDM_ST_Acceleration:
                    DrawSliderInt(0, 1, RDM_ST_Acceleration_Charges, Generics.HowManyChargesToKeepReady);
                    break;

                case Preset.RDM_AoE_Acceleration:
                    DrawSliderInt(0, 1, RDM_AoE_Acceleration_Charges, Generics.HowManyChargesToKeepReady);
                    break;
                #endregion

                #region Standalones
                case Preset.RDM_RetargetVercure_LowHP:
                    DrawSliderInt(1, 100, RDM_RetargetVercure_Health, FormatAndCache(Generics.HPPercentageThreshold, Vercure.ActionName()), 200);
                    break;
                
                case Preset.RDM_Riposte_Weaves:
                    DrawHorizontalMultiChoice(RDM_Riposte_Weaves_Options, Fleche.ActionName(), "", 6, 0);
                    DrawHorizontalMultiChoice(RDM_Riposte_Weaves_Options, ContreSixte.ActionName(), "", 6, 1);
                    DrawHorizontalMultiChoice(RDM_Riposte_Weaves_Options, ViceOfThorns.ActionName(), "", 6, 2);
                    DrawHorizontalMultiChoice(RDM_Riposte_Weaves_Options, Prefulgence.ActionName(), "", 6, 3);
                    DrawHorizontalMultiChoice(RDM_Riposte_Weaves_Options, Engagement.ActionName(), "", 6, 4);
                    DrawHorizontalMultiChoice(RDM_Riposte_Weaves_Options, Corpsacorps.ActionName(), "", 6, 5);

                    if (RDM_Riposte_Weaves_Options[4])
                    {
                        DrawSliderInt(0, 1, RDM_Riposte_Weaves_Options_EngagementCharges, Generics.ChargePool);
                    }

                    if (RDM_Riposte_Weaves_Options[5])
                    {
                        DrawSliderInt(0, 1, RDM_Riposte_Weaves_Options_CorpsCharges, Generics.ChargePool);
                        DrawSliderInt(0, 25, RDM_Riposte_Weaves_Options_Corpsacorps_Distance, Generics.UseWhenDistanceFromTargetIsLessThanOrEqualTo);
                    }
                    break;

                case Preset.RDM_Moulinet_Weaves:
                    DrawHorizontalMultiChoice(RDM_Moulinet_Weaves_Options, Fleche.ActionName(), "", 6, 0);
                    DrawHorizontalMultiChoice(RDM_Moulinet_Weaves_Options, ContreSixte.ActionName(), "", 6, 1);
                    DrawHorizontalMultiChoice(RDM_Moulinet_Weaves_Options, ViceOfThorns.ActionName(), "", 6, 2);
                    DrawHorizontalMultiChoice(RDM_Moulinet_Weaves_Options, Prefulgence.ActionName(), "", 6, 3);
                    DrawHorizontalMultiChoice(RDM_Moulinet_Weaves_Options, Engagement.ActionName(), "", 6, 4);
                    DrawHorizontalMultiChoice(RDM_Moulinet_Weaves_Options, Corpsacorps.ActionName(), "", 6, 5);

                    if (RDM_Moulinet_Weaves_Options[4])
                    {
                        DrawSliderInt(0, 1, RDM_Moulinet_Weaves_Options_EngagementCharges, Generics.ChargePool);
                    }

                    if (RDM_Moulinet_Weaves_Options[5])
                    {
                        DrawSliderInt(0, 1, RDM_Moulinet_Weaves_Options_CorpsCharges, Generics.ChargePool);
                        DrawSliderInt(0, 25, RDM_Moulinet_Weaves_Options_Corpsacorps_Distance, Generics.UseWhenDistanceFromTargetIsLessThanOrEqualTo);
                    }
                    break;

                case Preset.RDM_MagickBarrierAddle:
                    DrawSliderInt(0, 5, RDM_AddleDuration,
                        FormatAndCache(Generics.TimeRemainingOnOthers, Role.Addle.ActionName()));
                    break;

                case Preset.RDM_MagickProtection:
                    DrawSliderInt(0, 5, RDM_MagickProtectionDuration,
                        FormatAndCache(Generics.TimeRemainingOnOthers, MagickBarrier.ActionName()));
                    break;

                case Preset.RDM_OGCDs:
                    DrawHorizontalMultiChoice(RDM_OGCDs_Options, ContreSixte.ActionName(), "", 5, 0);
                    DrawHorizontalMultiChoice(RDM_OGCDs_Options, ViceOfThorns.ActionName(), "", 5, 1);
                    DrawHorizontalMultiChoice(RDM_OGCDs_Options, Prefulgence.ActionName(), "", 5, 2);
                    DrawHorizontalMultiChoice(RDM_OGCDs_Options, Engagement.ActionName(), "", 5, 3);
                    DrawHorizontalMultiChoice(RDM_OGCDs_Options, Corpsacorps.ActionName(), "", 5, 4);

                    if (RDM_OGCDs_Options[3])
                    {
                        DrawSliderInt(0, 1, RDM_OGCDs_Options_EngagementCharges, Generics.ChargePool);
                    }

                    if (RDM_OGCDs_Options[4])
                    {
                        DrawSliderInt(0, 1, RDM_OGCDs_Options_CorpsCharges, Generics.ChargePool);
                        DrawSliderInt(0, 25, RDM_OGCDs_Options_Corpsacorps_Distance, Generics.UseWhenDistanceFromTargetIsLessThanOrEqualTo);
                    }
                    break;

                case Preset.RDM_VerAero:
                    DrawHorizontalMultiChoice(RDM_VerAero_Options, Verholy.ActionName(), FormatAndCache(Generics.Add0, Verholy.ActionName()), 4, 0);
                    DrawHorizontalMultiChoice(RDM_VerAero_Options, Verstone.ActionName(), FormatAndCache(Generics.Add0, Verstone.ActionName()), 4, 1);
                    DrawHorizontalMultiChoice(RDM_VerAero_Options, $"{Scorch.ActionName()}/{Resolution.ActionName()}", FormatAndCache(RDM_Config.Add0_1Finishers, Scorch.ActionName(), Resolution.ActionName()), 4, 2);
                    DrawHorizontalMultiChoice(RDM_VerAero_Options, Jolt.ActionName(), FormatAndCache(Generics.Add0, Jolt.ActionName()), 4, 3);
                    if (RDM_VerAero_Options[0])
                        DrawAdditionalBoolChoice(RDM_VerAero_Dynamic, FormatAndCache(RDM_Config.DynamicallySwitchTo0, Verflare.ActionName()), FormatAndCache(RDM_Config.SwitchToBlack, Verflare.ActionName()));
                    break;

                case Preset.RDM_VerThunder:
                    DrawHorizontalMultiChoice(RDM_VerThunder_Options, Verflare.ActionName(), FormatAndCache(Generics.Add0, Verflare.ActionName()), 4, 0);
                    DrawHorizontalMultiChoice(RDM_VerThunder_Options, Verfire.ActionName(), FormatAndCache(Generics.Add0, Verfire.ActionName()), 4, 1);
                    DrawHorizontalMultiChoice(RDM_VerThunder_Options, $"{Scorch.ActionName()}/{Resolution.ActionName()}", FormatAndCache(RDM_Config.Add0_1Finishers, Scorch.ActionName(), Resolution.ActionName()), 4, 2);
                    DrawHorizontalMultiChoice(RDM_VerThunder_Options, Jolt.ActionName(), FormatAndCache(Generics.Add0, Jolt.ActionName()), 4, 3);
                    if (RDM_VerThunder_Options[0])
                        DrawAdditionalBoolChoice(RDM_VerThunder_Dynamic, FormatAndCache(RDM_Config.DynamicallySwitchTo0, Verholy.ActionName()), FormatAndCache(RDM_Config.SwitchToWhite, Verholy.ActionName()));
                    break;

                case Preset.RDM_VerAero2:
                    DrawHorizontalMultiChoice(RDM_VerAero2_Options, Verholy.ActionName(), FormatAndCache(Generics.Add0, Verholy.ActionName()), 3, 0);
                    DrawHorizontalMultiChoice(RDM_VerAero2_Options, $"{Scorch.ActionName()}/{Resolution.ActionName()}", FormatAndCache(RDM_Config.Add0_1Finishers, Scorch.ActionName(), Resolution.ActionName()), 3, 1);
                    DrawHorizontalMultiChoice(RDM_VerAero2_Options, Impact.ActionName(), FormatAndCache(Generics.Add0, Impact.ActionName()), 3, 2);
                    if (RDM_VerAero2_Options[0])
                        DrawAdditionalBoolChoice(RDM_VerAero2_Dynamic, FormatAndCache(RDM_Config.DynamicallySwitchTo0, Verflare.ActionName()), FormatAndCache(RDM_Config.SwitchToBlack, Verflare.ActionName()));
                    break;

                case Preset.RDM_VerThunder2:
                    DrawHorizontalMultiChoice(RDM_VerThunder2_Options, Verflare.ActionName(), FormatAndCache(Generics.Add0, Verflare.ActionName()), 3, 0);
                    DrawHorizontalMultiChoice(RDM_VerThunder2_Options, $"{Scorch.ActionName()}/{Resolution.ActionName()}", FormatAndCache(RDM_Config.Add0_1Finishers, Scorch.ActionName(), Resolution.ActionName()), 3, 1);
                    DrawHorizontalMultiChoice(RDM_VerThunder2_Options, Impact.ActionName(), FormatAndCache(Generics.Add0, Impact.ActionName()), 3, 2);
                    if (RDM_VerThunder2_Options[0])
                        DrawAdditionalBoolChoice(RDM_VerThunder2_Dynamic, FormatAndCache(RDM_Config.DynamicallySwitchTo0, Verholy.ActionName()), FormatAndCache(RDM_Config.SwitchToWhite, Verholy.ActionName()));
                    break;
                    #endregion
            }
        }
    }
}
