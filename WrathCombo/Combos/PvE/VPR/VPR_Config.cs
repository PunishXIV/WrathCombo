using Dalamud.Interface.Colors;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using WrathCombo.Resources.Localization.JobConfigs;
using static WrathCombo.Window.Functions.UserConfig;
using static WrathCombo.Window.Text;
namespace WrathCombo.Combos.PvE;

internal partial class VPR
{
    internal static class Config
    {
        internal static void Draw(Preset preset)
        {
            switch (preset)
            {
                #region ST

                case Preset.VPR_ST_Opener:
                    DrawBossOnlyChoice(VPR_Balance_Content);

                    DrawAdditionalBoolChoice(VPR_Opener_ExcludeUF,
                        FormatAndCache(Generics.Exclude0, UncoiledFury.ActionName()),
                        "");
                    break;

                case Preset.VPR_ST_SerpentsIre:
                    DrawSliderInt(0, 50, VPR_ST_SerpentsIreHPOption,
                        Generics.StopEnemyHpPercent);

                    ImGui.Indent();
                    ImGui.TextColored(ImGuiColors.DalamudYellow,
                        Generics.EnemyTypeCheck);

                    DrawHorizontalRadioButton(VPR_ST_SerpentsIreBossOption,
                        Generics.NonBosses,
                        Generics.HPCheckNonBosses, 0);

                    DrawHorizontalRadioButton(VPR_ST_SerpentsIreBossOption,
                        Generics.AllEnemies,
                        Generics.HPCheckAllEnemies, 1);
                    ImGui.Unindent();
                    break;

                case Preset.VPR_ST_Reawaken:
                    DrawSliderInt(0, 100, VPR_ST_ReawakenBossOption,
                        Generics.BossOnlyHpPercent);

                    DrawSliderInt(0, 100, VPR_ST_ReawakenBossAddsOption,
                        Generics.BossEncounterNonBossHpPercent);

                    DrawSliderInt(0, 100, VPR_ST_ReawakenTrashOption,
                        Generics.NonBossHpPercent);

                    DrawSliderInt(0, 5, VPR_ST_ReAwakenAlwaysUse,
                        FormatAndCache(VPR_Config.HPPThresholdWheneverAvailableBossesOnly, Reawaken.ActionName()));
                    break;

                case Preset.VPR_ST_UncoiledFury:
                    DrawSliderInt(0, 3, VPR_ST_UncoiledFuryHoldCharges,
                        FormatAndCache(Generics.HowManyChargesToKeepReady, UncoiledFury.ActionName()));

                    DrawSliderInt(0, 5, VPR_ST_UncoiledFuryAlwaysUse,
                        FormatAndCache(Generics.HPPercentThresholdUseAllCharges0, UncoiledFury.ActionName()));
                    break;

                case Preset.VPR_ST_Vicewinder:
                    DrawAdditionalBoolChoice(VPR_TrueNorthVicewinder,
                        FormatAndCache(Generics._0Option, Role.TrueNorth.ActionName()),
                        FormatAndCache(VPR_Config.Add0WhenAvailableRespectManualCharge, Role.TrueNorth.ActionName()));
                    break;

                case Preset.VPR_ST_VicewinderCombo:
                    DrawAdditionalBoolChoice(VPR_VicewinderBuffPrio,
                        Generics.PrioBuffUpkeep,
                        FormatAndCache(VPR_Config.Forces0Or1ForBuffs, HuntersCoil.ActionName(), SwiftskinsCoil.ActionName()));
                    break;

                case Preset.VPR_TrueNorthDynamic:
                    DrawSliderInt(0, 1, VPR_ManualTN,
                        Generics.ChargePool);

                    DrawAdditionalBoolChoice(VPR_ST_TrueNorthDynamicHoldCharge,
                        FormatAndCache(Generics.Hold0For1, Role.TrueNorth.ActionName(), Vicewinder.ActionName()),
                        FormatAndCache(VPR_Config.WillHoldLastChargeOf0For1, Role.TrueNorth.ActionName(), Vicewinder.ActionName()));
                    break;

                case Preset.VPR_ST_ComboHeals:
                    DrawSliderInt(0, 100, VPR_ST_SecondWindHPThreshold,
                        FormatAndCache(Generics.HPPercentageThreshold, Role.SecondWind.ActionName()));

                    DrawSliderInt(0, 100, VPR_ST_BloodbathHPThreshold,
                        FormatAndCache(Generics.HPPercentageThreshold, Role.Bloodbath.ActionName()));
                    break;

                #endregion

                #region AoE

                case Preset.VPR_AoE_SerpentsIre:
                    DrawSliderInt(0, 100, VPR_AoE_SerpentsIreHPThreshold,
                        Generics.StopEnemyHpPercent);
                    break;

                case Preset.VPR_AoE_UncoiledFury:
                    DrawSliderInt(0, 3, VPR_AoE_UncoiledFuryHoldCharges,
                        FormatAndCache(Generics.HowManyChargesToKeepReady, UncoiledFury.ActionName()));

                    DrawSliderInt(0, 5, VPR_AoE_UncoiledFuryAlwaysUse,
                        FormatAndCache(Generics.HPPercentThresholdUseAllCharges0, UncoiledFury.ActionName()));
                    break;

                case Preset.VPR_AoE_Reawaken:
                    DrawHorizontalRadioButton(VPR_AoE_ReawakenRangecheck,
                        Generics.InRange,
                        string.Format(VPR_Config.AddRangeCheckFor0, Reawaken.ActionName()), 0);

                    DrawHorizontalRadioButton(VPR_AoE_ReawakenRangecheck,
                        Generics.DisableRangeCheck,
                        string.Format(VPR_Config.DisableRangeCheckFor0, Reawaken.ActionName()), 1);

                    DrawSliderInt(0, 100, VPR_AoE_ReawakenHPThreshold,
                        FormatAndCache(Generics.StopEnemyHpPercent));
                    break;

                case Preset.VPR_AoE_Vicepit:
                    DrawHorizontalRadioButton(VPR_AoE_VicepitRangeCheck,
                        Generics.InRange,
                        string.Format(VPR_Config.AddRangeCheckFor0, Vicepit.ActionName()), 0);

                    DrawHorizontalRadioButton(VPR_AoE_VicepitRangeCheck,
                        Generics.DisableRangeCheck,
                        string.Format(VPR_Config.DisableRangeCheckFor0, Vicepit.ActionName()), 1);
                    break;

                case Preset.VPR_AoE_VicepitCombo:
                    DrawHorizontalRadioButton(VPR_AoE_VicepitComboRangeCheck,
                        Generics.InRange,
                        string.Format(VPR_Config.AddRangeCheckFor0And1, HuntersDen.ActionName(), SwiftskinsDen.ActionName()), 0);

                    DrawHorizontalRadioButton(VPR_AoE_VicepitComboRangeCheck,
                        Generics.DisableRangeCheck,
                        string.Format(VPR_Config.DisableRangeCheckFor0And1, HuntersDen.ActionName(), SwiftskinsDen.ActionName()), 1);
                    break;

                case Preset.VPR_AoE_ComboHeals:
                    DrawSliderInt(0, 100, VPR_AoE_SecondWindHPThreshold,
                        FormatAndCache(Generics.HPPercentageThreshold, Role.SecondWind.ActionName()));

                    DrawSliderInt(0, 100, VPR_AoE_BloodbathHPThreshold,
                        FormatAndCache(Generics.HPPercentageThreshold, Role.Bloodbath.ActionName()));
                    break;

                #endregion

                #region Misc

                case Preset.VPR_ReawakenLegacy:
                    DrawRadioButton(VPR_ReawakenLegacyButton,
                        FormatAndCache(Generics.Replaces0, Reawaken.ActionName()),
                        string.Format(VPR_Config.Replace0WithFullCombo, Reawaken.ActionName()), 0);

                    DrawRadioButton(VPR_ReawakenLegacyButton,
                        FormatAndCache(Generics.Replaces0, ReavingFangs.ActionName()),
                        string.Format(VPR_Config.Replace0WithFullCombo, ReavingFangs.ActionName()), 1);
                    break;

                case Preset.VPR_Retarget_Slither:
                    DrawAdditionalBoolChoice(VPR_Slither_FieldMouseover,
                        Generics.FieldMouseover,
                        Generics.AddFieldMouseoverTargetting);
                    break;

                #endregion
            }
        }

        #region Variables

        public static UserInt

            //ST
            VPR_Balance_Content = new("VPR_Balance_Content", 1),
            VPR_ST_UncoiledFuryHoldCharges = new("VPR_ST_UncoiledFuryHoldCharges", 1),
            VPR_ST_UncoiledFuryAlwaysUse = new("VPR_ST_UncoiledFuryAlwaysUse", 5),
            VPR_ST_ReawakenBossOption = new("VPR_ST_ReawakenBossOption"),
            VPR_ST_ReawakenBossAddsOption = new("VPR_ST_ReawakenBossAddsOption", 10),
            VPR_ST_ReawakenTrashOption = new("VPR_ST_ReawakenTrashOption", 25),
            VPR_ST_ReAwakenAlwaysUse = new("VPR_ST_ReAwakenAlwaysUse", 5),
            VPR_ST_SerpentsIreHPOption = new("VPR_ST_SerpentsIreHPOption", 25),
            VPR_ST_SerpentsIreBossOption = new("VPR_ST_SerpentsIreBossOption"),
            VPR_ManualTN = new("VPR_ManualTN"),
            VPR_ST_SecondWindHPThreshold = new("VPR_ST_SecondWindHPThreshold", 40),
            VPR_ST_BloodbathHPThreshold = new("VPR_ST_BloodbathHPThreshold", 30),

            //AoE
            VPR_AoE_SerpentsIreHPThreshold = new("VPR_AoE_SerpentsIreHPThreshold", 25),
            VPR_AoE_UncoiledFuryAlwaysUse = new("VPR_AoE_UncoiledFuryAlwaysUse", 5),
            VPR_AoE_UncoiledFuryHoldCharges = new("VPR_AoE_UncoiledFuryHoldCharges"),
            VPR_AoE_VicepitRangeCheck = new("VPR_AoE_VicepitRangeCheck"),
            VPR_AoE_VicepitComboRangeCheck = new("VPR_AoE_VicepitComboRangeCheck"),
            VPR_AoE_ReawakenHPThreshold = new("VPR_AoE_ReawakenHPThreshold", 25),
            VPR_AoE_ReawakenRangecheck = new("VPR_AoE_ReawakenRangecheck"),
            VPR_AoE_SecondWindHPThreshold = new("VPR_AoE_SecondWindHPThreshold", 40),
            VPR_AoE_BloodbathHPThreshold = new("VPR_AoE_BloodbathHPThreshold", 30),

            //Misc
            VPR_ReawakenLegacyButton = new("VPR_ReawakenLegacyButton");

        public static UserBool
            VPR_Opener_ExcludeUF = new("VPR_Opener_ExcludeUF"),
            VPR_TrueNorthVicewinder = new("VPR_TrueNorthVicewinder"),
            VPR_Slither_FieldMouseover = new("VPR_Slither_FieldMouseover"),
            VPR_ST_TrueNorthDynamicHoldCharge = new("VPR_ST_TrueNorthDynamicHoldCharge"),
            VPR_VicewinderBuffPrio = new("VPR_VicewinderBuffPrio");

        #endregion
    }
}
