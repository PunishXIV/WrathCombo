using Dalamud.Interface.Colors;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using WrathCombo.Resources.Localization.JobConfigs;
using static WrathCombo.Window.Text;
using static WrathCombo.Window.Functions.UserConfig;
namespace WrathCombo.Combos.PvE;

internal partial class DRG
{
    internal static class Config
    {
        internal static void Draw(Preset preset)
        {
            switch (preset)
            {
                case Preset.DRG_ST_Opener:
                    DrawHorizontalRadioButton(DRG_SelectedOpener,
                        Generics.StandardOpener, Generics.UsesStandardOpener, 0);

                    DrawHorizontalRadioButton(DRG_SelectedOpener,
                        FormatAndCache(Generics.Action_Opener, PiercingTalon.ActionName()),
                        FormatAndCache(Generics.Use_0_Opener, PiercingTalon.ActionName()), 1);
               
                    DrawBossOnlyChoice(DRG_BalanceContent);
                    break;

                case Preset.DRG_ST_BattleLitany:
                    DrawSliderInt(0, 50, DRG_ST_BattleLitanyHPOption,
                        Generics.StopEnemyHpPercent);

                    ImGui.Indent();
                    ImGui.TextColored(ImGuiColors.DalamudYellow,
                        Generics.EnemyTypeCheck);

                    DrawHorizontalRadioButton(DRG_ST_BattleLitanyBossOption,
                        Generics.NonBosses,
                        Generics.HPCheckNonBosses, 0);

                    DrawHorizontalRadioButton(DRG_ST_BattleLitanyBossOption,
                        Generics.AllEnemies,
                        Generics.HPCheckAllEnemies, 1);
                    ImGui.Unindent();
                    break;

                case Preset.DRG_ST_LanceCharge:
                    DrawSliderInt(0, 50, DRG_ST_LanceChargeHPOption,
                        Generics.StopEnemyHpPercent);

                    ImGui.Indent();
                    ImGui.TextColored(ImGuiColors.DalamudYellow,
                        Generics.EnemyTypeCheck);

                    DrawHorizontalRadioButton(DRG_ST_LanceChargeBossOption,
                        Generics.NonBosses,
                        Generics.HPCheckNonBosses, 0);

                    DrawHorizontalRadioButton(DRG_ST_LanceChargeBossOption,
                        Generics.AllEnemies,
                        Generics.HPCheckAllEnemies, 1);
                    ImGui.Unindent();
                    break;

                case Preset.DRG_ST_HighJump:
                    DrawHorizontalMultiChoice(DRG_ST_JumpMovingOrInRanged,
                        Generics.NoMovement,
                        FormatAndCache(Generics.OnlyUse0WhenNotMoving, Jump.ActionName()), 2, 0);

                    DrawHorizontalMultiChoice(DRG_ST_JumpMovingOrInRanged,
                        Generics.InMeleeRange,
                        FormatAndCache(Generics.OnlyUse0WhenInMeleeRange, Jump.ActionName()), 2, 1);
                    break;

                case Preset.DRG_ST_Mirage:
                    DrawAdditionalBoolChoice(DRG_ST_DoubleMirage,
                        FormatAndCache(DRG_Config.BurstMirageDuringLoTD, MirageDive.ActionName(), Buffs.LifeOfTheDragon.StatusName()),
                        FormatAndCache(DRG_Config.AddMirageDiveUnderLOTD, MirageDive.ActionName(), Buffs.LifeOfTheDragon.StatusName()));
                    break;

                case Preset.DRG_ST_Geirskogul:
                    DrawSliderInt(0, 100, DRG_ST_GeirskogulBossOption,
                        Generics.BossOnlyHpPercent);

                    DrawSliderInt(0, 100, DRG_ST_GeirskogulBossAddsOption,
                        Generics.BossEncounterNonBossHpPercent);

                    DrawSliderInt(0, 100, DRG_ST_GeirskogulTrashOption,
                        Generics.NonBossHpPercent);
                    break;

                case Preset.DRG_ST_DragonfireDive:
                    DrawSliderInt(0, 50, DRG_ST_DragonfireDiveHPOption,
                        Generics.StopEnemyHpPercent);

                    ImGui.Indent();
                    ImGui.TextColored(ImGuiColors.DalamudYellow,
                        Generics.EnemyTypeCheck);

                    DrawHorizontalRadioButton(DRG_ST_DragonfireDiveBossOption,
                        Generics.NonBosses,
                        Generics.HPCheckNonBosses, 0);

                    DrawHorizontalRadioButton(DRG_ST_DragonfireDiveBossOption,
                        Generics.AllEnemies,
                        Generics.HPCheckAllEnemies, 1);
                    ImGui.Unindent();

                    DrawHorizontalMultiChoice(DRG_ST_DragonfireDiveMovingOrInRanged,
                        Generics.NoMovement,
                        Generics.OnlyUse0WhenNotMoving, 2, 0);

                    DrawHorizontalMultiChoice(DRG_ST_DragonfireDiveMovingOrInRanged,
                        Generics.InMeleeRange,
                        Generics.OnlyUse0WhenInMeleeRange, 2, 1);
                    break;

                case Preset.DRG_ST_Stardiver:
                    DrawHorizontalMultiChoice(DRG_ST_StardiverMovingOrInRanged,
                        Generics.NoMovement,
                        FormatAndCache(Generics.OnlyUse0WhenNotMoving, Stardiver.ActionName()), 2, 0);

                    DrawHorizontalMultiChoice(DRG_ST_StardiverMovingOrInRanged,
                        Generics.InMeleeRange,
                        FormatAndCache(Generics.OnlyUse0WhenInMeleeRange, Stardiver.ActionName()), 2, 1);
                    break;

                case Preset.DRG_TrueNorthDynamic:
                    DrawSliderInt(0, 1, DRG_ManualTN,
                        Generics.ChargePool);
                    break;

                case Preset.DRG_ST_ComboHeals:
                    DrawSliderInt(0, 100, DRG_ST_SecondWindHPThreshold,
                        FormatAndCache(Generics.HPPercentageThreshold, Role.SecondWind.ActionName()));

                    DrawSliderInt(0, 100, DRG_ST_BloodbathHPThreshold,
                        FormatAndCache(Generics.HPPercentageThreshold, Role.Bloodbath.ActionName()));
                    break;

                case Preset.DRG_AoE_BattleLitany:
                    DrawSliderInt(0, 100, DRG_AoE_BattleLitanyHPTreshold,
                        Generics.StopEnemyHpPercent);
                    break;

                case Preset.DRG_AoE_LanceCharge:
                    DrawSliderInt(0, 100, DRG_AoE_LanceChargeHPTreshold,
                        Generics.StopEnemyHpPercent);
                    break;

                case Preset.DRG_AoE_Geirskogul:
                    DrawSliderInt(0, 100, DRG_AoE_GeirskogulHPTreshold,
                        Generics.StopEnemyHpPercent);
                    break;

                case Preset.DRG_AoE_HighJump:
                    DrawHorizontalMultiChoice(DRG_AoE_JumpMovingOrInRanged,
                        Generics.NoMovement,
                        FormatAndCache(Generics.OnlyUse0WhenNotMoving, Jump.ActionName()), 2, 0);

                    DrawHorizontalMultiChoice(DRG_AoE_JumpMovingOrInRanged,
                        Generics.InMeleeRange,
                        FormatAndCache(Generics.OnlyUse0WhenInMeleeRange, Jump.ActionName()), 2, 1);
                    break;

                case Preset.DRG_AoE_DragonfireDive:
                    DrawSliderInt(0, 100, DRG_AoE_DragonfireDiveHPTreshold,
                        Generics.StopEnemyHpPercent);

                    DrawHorizontalMultiChoice(DRG_AoE_DragonfireDiveMovingOrInRanged,
                        Generics.NoMovement,
                        FormatAndCache(Generics.OnlyUse0WhenNotMoving, DragonfireDive.ActionName()), 2, 0);

                    DrawHorizontalMultiChoice(DRG_AoE_DragonfireDiveMovingOrInRanged,
                        Generics.InMeleeRange,
                        FormatAndCache(Generics.OnlyUse0WhenInMeleeRange, DragonfireDive.ActionName()), 2, 1);
                    break;

                case Preset.DRG_AoE_Stardiver:
                    DrawHorizontalMultiChoice(DRG_AoE_StardiverMovingOrInRanged,
                        Generics.NoMovement,
                        FormatAndCache(Generics.OnlyUse0WhenNotMoving, Stardiver.ActionName()), 2, 0);

                    DrawHorizontalMultiChoice(DRG_AoE_StardiverMovingOrInRanged,
                        Generics.InMeleeRange,
                        FormatAndCache(Generics.OnlyUse0WhenInMeleeRange, Stardiver.ActionName()), 2, 1);
                    break;

                case Preset.DRG_AoE_ComboHeals:
                    DrawSliderInt(0, 100, DRG_AoE_SecondWindHPThreshold,
                        FormatAndCache(Generics.HPPercentageThreshold, Role.SecondWind.ActionName()));

                    DrawSliderInt(0, 100, DRG_AoE_BloodbathHPThreshold,
                        FormatAndCache(Generics.HPPercentageThreshold, Role.Bloodbath.ActionName()));
                    break;

                case Preset.DRG_HeavensThrust:
                    DrawAdditionalBoolChoice(DRG_ChaoticCombo,
                        DRG_Config.AddChaosCombo,
                        DRG_Config.AddChaosComboWhenApplicable);
                    break;
            }
        }

        #region Variables

        public static UserInt
            DRG_SelectedOpener = new("DRG_SelectedOpener"),
            DRG_BalanceContent = new("DRG_BalanceContent", 1),
            DRG_ST_BattleLitanyHPOption = new("DRG_ST_BattleLitanyHPOption", 25),
            DRG_ST_BattleLitanyBossOption = new("DRG_ST_BattleLitanyBossOption"),
            DRG_ST_LanceChargeHPOption = new("DRG_ST_LanceChargeHPOption", 25),
            DRG_ST_LanceChargeBossOption = new("DRG_ST_LanceChargeBossOption"),
            DRG_ST_GeirskogulBossOption = new("DRG_ST_GeirskogulBossOption"),
            DRG_ST_GeirskogulBossAddsOption = new("DRG_ST_GeirskogulBossAddsOption", 10),
            DRG_ST_GeirskogulTrashOption = new("DRG_ST_GeirskogulTrashOption", 25),
            DRG_ST_DragonfireDiveHPOption = new("DRG_ST_DragonfireDiveHPOption", 25),
            DRG_ST_DragonfireDiveBossOption = new("DRG_ST_DragonfireDiveBossOption"),
            DRG_ManualTN = new("DRG_ManualTN"),
            DRG_ST_SecondWindHPThreshold = new("DRG_ST_SecondWindHPThreshold", 40),
            DRG_ST_BloodbathHPThreshold = new("DRG_ST_BloodbathHPThreshold", 30),
            DRG_AoE_BattleLitanyHPTreshold = new("DRG_AoE_BattleLitanyHPTreshold", 25),
            DRG_AoE_LanceChargeHPTreshold = new("DRG_AoE_LanceChargeHPTreshold", 25),
            DRG_AoE_GeirskogulHPTreshold = new("DRG_AoE_GeirskogulHPTreshold", 25),
            DRG_AoE_DragonfireDiveHPTreshold = new("DRG_AoE_DragonfireDiveHPTreshold", 25),
            DRG_AoE_SecondWindHPThreshold = new("DRG_AoE_SecondWindHPThreshold", 40),
            DRG_AoE_BloodbathHPThreshold = new("DRG_AoE_BloodbathHPThreshold", 30);

        public static UserBool
            DRG_ST_DoubleMirage = new("DRG_ST_DoubleMirage"),
            DRG_ChaoticCombo = new("DRG_ChaoticCombo");

        public static UserBoolArray
            DRG_ST_JumpMovingOrInRanged = new("DRG_ST_JumpMovingOrInRanged"),
            DRG_ST_DragonfireDiveMovingOrInRanged = new("DRG_ST_DragonfireDiveMovingOrInRanged"),
            DRG_ST_StardiverMovingOrInRanged = new("DRG_ST_StardiverMovingOrInRanged"),
            DRG_AoE_JumpMovingOrInRanged = new("DRG_AoE_JumpMovingOrInRanged"),
            DRG_AoE_DragonfireDiveMovingOrInRanged = new("DRG_AoE_DragonfireDiveMovingOrInRanged"),
            DRG_AoE_StardiverMovingOrInRanged = new("DRG_AoE_StardiverMovingOrInRanged");

        #endregion
    }
}
