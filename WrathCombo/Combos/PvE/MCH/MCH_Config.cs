using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility.Raii;
using ECommons.ImGuiMethods;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using WrathCombo.Resources.Localization.JobConfigs;
using WrathCombo.Resources.Localization.Presets;
using static WrathCombo.Window.Text;
using static WrathCombo.Window.Functions.UserConfig;
namespace WrathCombo.Combos.PvE;

internal partial class MCH
{
    internal static class Config
    {
        internal static void Draw(Preset preset)
        {
            switch (preset)
            {
                #region ST

                case Preset.MCH_ST_Adv_Opener:
                    DrawBossOnlyChoice(MCH_Balance_Content);

                    using (ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.DalamudYellow))
                    {
                        ImGui.TextUnformatted("Select whether the opener requires a battle target:");
                    }

                    DrawHorizontalRadioButton(MCH_HaveTarget,
                        Generics.HaveBattleTarget,
                        Generics.RequireTarget, 0,
                        descriptionColor: ImGuiColors.DalamudYellow);

                    DrawHorizontalRadioButton(MCH_HaveTarget,
                        Generics.NoTarget,
                        Generics.NoRequireTarget, 1,
                        descriptionColor: ImGuiColors.DalamudYellow);

                    DrawOpenerPotionChoice(MCH_Opener_Potion);
                    ImGuiEx.TextUnderlined("Select Opener");
                    ImGui.Spacing();
                    DrawRadioButton(MCH_SelectedOpener,
                        Generics.StandardOpener,
                        Generics.UsesStandardOpener, 0, descriptionAsTooltip: true);

                    DrawRadioButton(MCH_SelectedOpener,
                        MCH_Config.Early0Opener,
                        FormatAndCache(MCH_Config.UseEarly0Opener, Wildfire.ActionName()), 1, descriptionAsTooltip: true);
                    break;

                case Preset.MCH_ST_Adv_WildFire:
                    DrawHorizontalRadioButton(MCH_ST_WildfireBossOnlyOption,
                        Generics.AllEnemies,
                        FormatAndCache(Generics.Use0RegardlessOfTarget, Wildfire.ActionName()), 0);

                    DrawHorizontalRadioButton(MCH_ST_WildfireBossOnlyOption,
                        Generics.OnlyBoss,
                        FormatAndCache(Generics.OnlyUseWhenTargetIsBoss, Wildfire.ActionName()), 1);

                    if (MCH_ST_WildfireBossOnlyOption == 0)
                    {
                        DrawSliderInt(0, 50, MCH_ST_WildfireHPOption,
                            Generics.StopEnemyHpPercent);

                        ImGui.Indent();
                        ImGui.TextColored(ImGuiColors.DalamudYellow,
                            Generics.EnemyTypeCheck);

                        DrawHorizontalRadioButton(MCH_ST_WildfireHPBossOption,
                            Generics.NonBosses,
                            Generics.HPCheckNonBosses, 0);

                        DrawHorizontalRadioButton(MCH_ST_WildfireHPBossOption,
                            Generics.AllEnemies,
                            Generics.HPCheckAllEnemies, 1);
                        ImGui.Unindent();
                    }
                    break;

                case Preset.MCH_ST_Adv_Stabilizer:
                    DrawHorizontalRadioButton(MCH_ST_BarrelStabilizerBossOnlyOption,
                        Generics.AllEnemies,
                        FormatAndCache(Generics.Use0RegardlessOfTarget, BarrelStabilizer.ActionName()), 0);

                    DrawHorizontalRadioButton(MCH_ST_BarrelStabilizerBossOnlyOption,
                        Generics.OnlyBoss,
                        FormatAndCache(Generics.OnlyUseWhenTargetIsBoss, BarrelStabilizer.ActionName()), 1);

                    if (MCH_ST_BarrelStabilizerBossOnlyOption == 0)
                    {
                        DrawSliderInt(0, 50, MCH_ST_BarrelStabilizerHPOption,
                            Generics.StopEnemyHpPercent);

                        ImGui.Indent();
                        ImGui.TextColored(ImGuiColors.DalamudYellow,
                            Generics.EnemyTypeCheck);

                        DrawHorizontalRadioButton(MCH_ST_BarrelStabilizerHPBossOption,
                            Generics.NonBosses,
                            Generics.HPCheckNonBosses, 0);

                        DrawHorizontalRadioButton(MCH_ST_BarrelStabilizerHPBossOption,
                            Generics.AllEnemies,
                            Generics.HPCheckAllEnemies, 1);
                        ImGui.Unindent();
                    }
                    break;

                case Preset.MCH_ST_Adv_Hypercharge:
                    DrawSliderInt(0, 50, MCH_ST_HyperchargeHPOption,
                        Generics.StopEnemyHpPercent);

                    ImGui.Indent();
                    ImGui.TextColored(ImGuiColors.DalamudYellow,
                        Generics.EnemyTypeCheck);

                    DrawHorizontalRadioButton(MCH_ST_HyperchargeHPBossOption,
                        Generics.NonBosses,
                        Generics.HPCheckNonBosses, 0);

                    DrawHorizontalRadioButton(MCH_ST_HyperchargeHPBossOption,
                        Generics.AllEnemies,
                        Generics.HPCheckAllEnemies, 1);
                    ImGui.Unindent();
                    break;

                case Preset.MCH_ST_Adv_TurretQueen:
                    DrawSliderInt(50, 100, MCH_ST_TurretUsage,
                        FormatAndCache(MCH_Config.UseQueenOutsideOfBoss, AutomatonQueen.ActionName()));

                    DrawSliderInt(0, 50, MCH_ST_QueenHPOption,
                        Generics.StopEnemyHpPercent);

                    ImGui.Indent();

                    ImGui.TextColored(ImGuiColors.DalamudYellow,
                        Generics.EnemyTypeCheck);

                    DrawHorizontalRadioButton(MCH_ST_QueenHPBossOption,
                        Generics.NonBosses,
                        Generics.HPCheckNonBosses, 0);

                    DrawHorizontalRadioButton(MCH_ST_QueenHPBossOption,
                        Generics.AllEnemies,
                        Generics.HPCheckAllEnemies, 1);

                    ImGui.Unindent();
                    break;

                case Preset.MCH_ST_Adv_GaussRicochet:
                    DrawHorizontalRadioButton(MCH_ST_GaussOnlyOrBoth,
                        FormatAndCache(Generics.Use0And1, GaussRound.ActionName(), Ricochet.ActionName()),
                        FormatAndCache(Generics.Use0And1, GaussRound.ActionName(), Ricochet.ActionName()), 0);

                    DrawHorizontalRadioButton(MCH_ST_GaussOnlyOrBoth,
                        FormatAndCache(Generics.OnlyUse0, GaussRound.ActionName()),
                        FormatAndCache(MCH_Config.NotRecommended), 1);

                    if (MCH_ST_GaussOnlyOrBoth == 0)
                    {
                        DrawSliderInt(0, 2, MCH_ST_GaussRicoManualUse,
                            Generics.ChargePool);
                    }
                    break;

                case Preset.MCH_ST_Adv_Reassemble:
                    DrawHorizontalRadioButton(MCH_ST_Adv_ReassembleChoice,
                        MCH_Config.SaveForEvenWindows,
                        FormatAndCache(MCH_Config.Save0ForEvenWindows, Reassemble.ActionName()), 0);

                    DrawHorizontalRadioButton(MCH_ST_Adv_ReassembleChoice,
                        MCH_Config.UseEveryMinute,
                        FormatAndCache(MCH_Config.Use0EveryMinute, Reassemble.ActionName()), 1);

                    DrawSliderInt(0, 50, MCH_ST_ReassembleHPOption,
                        Generics.StopEnemyHpPercent);

                    ImGui.Indent();

                    ImGui.TextColored(ImGuiColors.DalamudYellow,
                        Generics.EnemyTypeCheck);

                    DrawHorizontalRadioButton(MCH_ST_ReassembleHPBossOption,
                        Generics.NonBosses,
                        Generics.HPCheckNonBosses, 0);

                    DrawHorizontalRadioButton(MCH_ST_ReassembleHPBossOption,
                        Generics.AllEnemies,
                        Generics.HPCheckAllEnemies, 1);

                    ImGui.Unindent();

                    DrawSliderInt(0, 1, MCH_ST_ReassemblePool,
                        Generics.ChargePool);

                    break;

                case Preset.MCH_ST_Adv_Tools:
                    DrawSliderInt(0, 50, MCH_ST_ToolsHPOption,
                        Generics.StopEnemyHpPercent);

                    ImGui.Indent();

                    ImGui.TextColored(ImGuiColors.DalamudYellow,
                        Generics.EnemyTypeCheck);

                    DrawHorizontalRadioButton(MCH_ST_ToolsHPBossOption,
                        Generics.NonBosses,
                        Generics.HPCheckNonBosses, 0);

                    DrawHorizontalRadioButton(MCH_ST_ToolsHPBossOption,
                        Generics.AllEnemies,
                        Generics.HPCheckAllEnemies, 1);

                    DrawSliderFloat(0, 9, MCH_ST_WildfireHyperchargeCutoffThreshold, CustomComboPresets.MCH_ST_Adv_WildfireHyperchargeCutoffThreshold);

                    ImGui.Unindent();
                    break;

                case Preset.MCH_ST_Adv_QueenOverdrive:
                    DrawSliderInt(0, 100, MCH_ST_QueenOverDriveHPThreshold,
                        Generics.StopFriendlyHpPercent100);
                    break;

                case Preset.MCH_ST_Adv_SecondWind:
                    DrawSliderInt(0, 100, MCH_ST_SecondWindHPThreshold,
                        FormatAndCache(Generics.HPPercentageThreshold, Role.SecondWind.ActionName()));
                    break;

                #endregion

                #region AoE

                case Preset.MCH_AoE_Adv_Reassemble:
                    DrawSliderInt(0, 100, MCH_AoE_ReassembleHPThreshold,
                        FormatAndCache(Generics.StopUsing0WhenBelowTargetHPPercentage, Reassemble.ActionName()));

                    DrawSliderInt(0, 2, MCH_AoE_ReassemblePool,
                        Generics.ChargePool);
                    break;

                case Preset.MCH_AoE_Adv_QueenOverdrive:
                    DrawSliderInt(0, 100, MCH_AoE_QueenOverDriveHPThreshold,
                        Generics.StopFriendlyHpPercent100);
                    break;

                case Preset.MCH_AoE_Adv_SecondWind:
                    DrawSliderInt(0, 100, MCH_AoE_SecondWindHPThreshold,
                        FormatAndCache(Generics.HPPercentageThreshold, Role.SecondWind.ActionName()));
                    break;

                case Preset.MCH_AoE_Adv_Queen:
                    DrawSliderInt(0, 100, MCH_AoE_QueenHpThreshold,
                        FormatAndCache(Generics.StopUsing0WhenBelowTargetHPPercentage, RookAutoturret.ActionName()));

                    DrawSliderInt(50, 100, MCH_AoE_TurretBatteryUsage,
                        MCH_Config.BatteryThreshold, sliderIncrement: 5);
                    break;

                case Preset.MCH_AoE_Adv_FlameThrower:
                    DrawHorizontalRadioButton(MCH_AoE_FlamethrowerMovement,
                        Generics.StationaryOnly,
                        FormatAndCache(Generics.UseActionOnlyWhileStationary, Flamethrower.ActionName()), 0);

                    DrawHorizontalRadioButton(MCH_AoE_FlamethrowerMovement,
                        Generics.AnyMovement,
                        FormatAndCache(Generics.Uses0RegardlessOfAnyMovementConditions, Flamethrower.ActionName()), 1);

                    ImGui.Spacing();
                    if (MCH_AoE_FlamethrowerMovement == 0)
                    {
                        ImGui.SetCursorPosX(48);
                        DrawSliderFloat(0, 3, MCH_AoE_FlamethrowerTimeStill,
                            Generics.StationaryDelayCheck, decimals: 1);
                    }

                    DrawSliderInt(0, 50, MCH_AoE_FlamethrowerHPOption,
                        Generics.StopEnemyHpPercent);

                    break;

                case Preset.MCH_AoE_Adv_Hypercharge:
                    DrawSliderInt(0, 100, MCH_AoE_HyperchargeHPThreshold,
                        FormatAndCache(Generics.StopUsing0WhenBelowTargetHPPercentage, Hypercharge.ActionName()));

                    DrawSliderFloat(5, 9, MCH_AoE_HyperchargeToolHold,
                        CustomComboPresets.MCH_AoE_Adv_HyperchargeToolHold, decimals: 1);
                    break;

                case Preset.MCH_AoE_Adv_Tools:
                    DrawSliderInt(0, 100, MCH_AoE_ToolsHPThreshold,
                        MCH_Config.StopUsingToolsBelowHpPercentage);

                    DrawAdditionalBoolChoice(MCH_AoE_AirAnchor,
                        FormatAndCache(Generics.Add0Or1, HotShot.ActionName(), AirAnchor.ActionName()),
                        FormatAndCache(MCH_Config.AlsoUse0Or1OnCooldown, HotShot.ActionName(), AirAnchor.ActionName()));
                    break;

                case Preset.MCH_AoE_Adv_Stabilizer:
                    DrawSliderInt(0, 100, MCH_AoE_BarrelStabilizerHPThreshold,
                        FormatAndCache(Generics.StopUsing0WhenBelowTargetHPPercentage, BarrelStabilizer.ActionName()));
                    break;

                #endregion

                #region Misc

                case Preset.MCH_GaussRoundRicochet:
                    DrawHorizontalRadioButton(MCH_GaussRico,
                        FormatAndCache(Generics.Change0Or1, GaussRound.ActionName(), DoubleCheck.ActionName()),
                        FormatAndCache(MCH_Config.ChangesTo0Or1, Ricochet.ActionName(), CheckMate.ActionName()), 0);

                    DrawHorizontalRadioButton(MCH_GaussRico,
                        FormatAndCache(Generics.Change0Or1, Ricochet.ActionName(), CheckMate.ActionName()),
                        FormatAndCache(MCH_Config.ChangesTo0Or1, GaussRound.ActionName(), DoubleCheck.ActionName()), 1);
                    break;

                case Preset.MCH_ST_Dismantle:
                    DrawSliderInt(0, 5, MCH_DismantledDuration,
                        FormatAndCache(Generics.TimeRemainingOn0, Debuffs.Dismantled.StatusName()));
                    break;

                #endregion
            }
        }

        #region Variables

        public static UserInt

            //ST
            MCH_Balance_Content = new("MCH_Balance_Content", 1),
            MCH_SelectedOpener = new("MCH_SelectedOpener"),
            MCH_HaveTarget = new("MCH_HaveTarget"),
            MCH_ST_QueenOverDriveHPThreshold = new("MCH_ST_QueenOverDriveHPThreshold", 1),
            MCH_ST_BarrelStabilizerBossOnlyOption = new("MCH_ST_BarrelStabilizerBossOnlyOption", 1),
            MCH_ST_BarrelStabilizerHPOption = new("MCH_ST_BarrelStabilizerHPOption", 10),
            MCH_ST_BarrelStabilizerHPBossOption = new("MCH_ST_BarrelStabilizerHPBossOption"),
            MCH_ST_WildfireBossOnlyOption = new("MCH_ST_WildfireBossOnlyOption", 1),
            MCH_ST_WildfireHPOption = new("MCH_ST_WildfireHPOption", 25),
            MCH_ST_WildfireHPBossOption = new("MCH_ST_WildfireHPBossOption"),
            MCH_ST_HyperchargeHPBossOption = new("MCH_ST_HyperchargeHPBossOption"),
            MCH_ST_HyperchargeHPOption = new("MCH_ST_HyperchargeHPOption", 25),
            MCH_ST_ReassembleHPBossOption = new("MCH_ST_ReassembleHPBossOption"),
            MCH_ST_Adv_ReassembleChoice = new("MCH_ST_Adv_ReassembleChoice"),
            MCH_ST_ReassembleHPOption = new("MCH_ST_ReassembleHPOption", 25),
            MCH_ST_ToolsHPBossOption = new("MCH_ST_ToolsHPBossOption"),
            MCH_ST_ToolsHPOption = new("MCH_ST_ToolsHPOption", 25),
            MCH_ST_QueenHPOption = new("MCH_ST_QueenHPOption", 25),
            MCH_ST_QueenHPBossOption = new("MCH_ST_QueenHPBossOption"),
            MCH_ST_TurretUsage = new("MCH_ST_TurretUsage", 100),
            MCH_ST_ReassemblePool = new("MCH_ST_ReassemblePool"),
            MCH_ST_GaussRicoManualUse = new("MCH_ST_GaussRicoManualUse"),
            MCH_ST_GaussOnlyOrBoth = new("MCH_ST_GaussOnlyOrBoth"),
            MCH_ST_SecondWindHPThreshold = new("MCH_ST_SecondWindHPThreshold", 40),

            //AoE
            MCH_AoE_ReassemblePool = new("MCH_AoE_ReassemblePool"),
            MCH_AoE_TurretBatteryUsage = new("MCH_AoE_TurretBatteryUsage", 100),
            MCH_AoE_FlamethrowerMovement = new("MCH_AoE_FlamethrowerMovement"),
            MCH_AoE_FlamethrowerHPOption = new("MCH_AoE_FlamethrowerHPOption", 25),
            MCH_AoE_HyperchargeHPThreshold = new("MCH_AoE_HyperchargeHPThreshold", 25),
            MCH_AoE_ReassembleHPThreshold = new("MCH_AoE_ReassembleHPThreshold", 25),
            MCH_AoE_ToolsHPThreshold = new("MCH_AoE_ToolsHPThreshold", 25),
            MCH_AoE_QueenHpThreshold = new("MCH_AoE_QueenHpThreshold", 25),
            MCH_AoE_BarrelStabilizerHPThreshold = new("MCH_AoE_BarrelStabilizerHPThreshold", 25),
            MCH_AoE_QueenOverDriveHPThreshold = new("MCH_AoE_QueenOverDriveHPThreshold", 25),
            MCH_AoE_SecondWindHPThreshold = new("MCH_AoE_SecondWindHPThreshold", 40),

            //Misc
            MCH_GaussRico = new("MCHGaussRico"),
            MCH_DismantledDuration = new("MCH_DismantledDuration");

        public static UserFloat
            MCH_AoE_FlamethrowerTimeStill = new("MCH_AoE_FlamethrowerTimeStill", 2.5f),
            MCH_AoE_HyperchargeToolHold = new("MCH_AoE_HyperchargeToolHold", 8f),
            MCH_ST_WildfireHyperchargeCutoffThreshold = new("MCH_ST_WildfireHyperchargeCutoffThreshold", 9f);

        public static UserBool
            MCH_Opener_Potion = new("MCH_Opener_Potion"),
            MCH_AoE_AirAnchor = new("MCH_AoE_AirAnchor");

        #endregion
    }
}
