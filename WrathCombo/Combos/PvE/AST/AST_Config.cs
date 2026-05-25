using Dalamud.Interface.Colors;
using System.Linq;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Resources.Localization.JobConfigs;
using WrathCombo.Window;
using static WrathCombo.Extensions.UIntExtensions;
using static WrathCombo.Window.Functions.SliderIncrements;
using static WrathCombo.Window.Functions.UserConfig;
namespace WrathCombo.Combos.PvE;

internal partial class AST
{
    public static class Config
    {
        #region Options
        public static UserIntArray
            AST_ST_SimpleHeals_Priority = new("AST_ST_SimpleHeals_Priority", [13, 12, 10, 6, 7, 8, 9, 11, 5, 4, 3, 1, 2]),
            AST_AoE_SimpleHeals_Priority = new("AST_AoE_SimpleHeals_Priority", [3, 6, 1, 4, 7, 2, 8, 9, 5]);

        public static UserInt
            //HEALS
            AST_ST_SimpleHeals_Spire = new("AST_ST_SimpleHeals_Spire", 70),
            AST_ST_SimpleHeals_Ewer = new("AST_ST_SimpleHeals_Ewer", 70),
            AST_ST_SimpleHeals_Arrow = new("AST_ST_SimpleHeals_Arrow", 70),
            AST_ST_SimpleHeals_Bole = new("AST_ST_SimpleHeals_Bole", 70),
            AST_ST_SimpleHeals_CelestialIntersection = new("AST_ST_SimpleHeals_CelestialIntersection", 70),
            AST_ST_SimpleHeals_CelestialIntersectionCharges = new("AST_ST_SimpleHeals_CelestialIntersectionCharges", 0),
            AST_ST_SimpleHeals_EssentialDignity = new("AST_ST_SimpleHeals_EssentialDignity", 70),
            AST_ST_SimpleHeals_Exaltation = new("AST_ST_SimpleHeals_Exaltation", 70),
            AST_ST_SimpleHeals_Esuna = new("AST_ST_SimpleHeals_Esuna", 40),
            AST_ST_SimpleHeals_AspectedBeneficHigh = new("AST_ST_SimpleHeals_AspectedBeneficHigh", 100),
            AST_ST_SimpleHeals_AspectedBeneficLow = new("AST_ST_SimpleHeals_AspectedBeneficLow", 40),
            AST_ST_SimpleHeals_AspectedBeneficRefresh = new("AST_ST_SimpleHeals_AspectedBeneficRefresh", 3),
            AST_ST_SimpleHeals_CollectiveUnconscious = new("AST_ST_SimpleHeals_CollectiveUnconscious", 70),
            AST_ST_SimpleHeals_CelestialOpposition = new("AST_ST_SimpleHeals_CelestialOpposition", 70),
            AST_ST_SimpleHeals_SoloLady = new("AST_ST_SimpleHeals_SoloLady", 70),
            AST_ST_SimpleHeals_EmergencyED_Threshold = new("AST_ST_SimpleHeals_EmergencyED_Threshold", 30),
            AST_ST_Heals_NeutralSect_Threshold = new("AST_ST_Heals_NeutralSect_Threshold", 70),
            AST_AoE_SimpleHeals_AltMode = new("AST_AoE_SimpleHeals_AltMode", 1),
            AST_AoE_SimpleHeals_LazyLady = new("AST_AoE_SimpleHeals_LazyLady", 80),
            AST_AoE_SimpleHeals_Horoscope = new("AST_AoE_SimpleHeals_Horoscope", 80),
            AST_AoE_SimpleHeals_CelestialOpposition = new("AST_AoE_SimpleHeals_CelestialOpposition", 80),
            AST_AoE_SimpleHeals_CollectiveUnconscious = new("AST_AoE_SimpleHeals_CollectiveUnconscious", 80),
            AST_AoE_SimpleHeals_NeutralSect = new("AST_AoE_SimpleHeals_NeutralSect", 80),
            AST_AoE_SimpleHeals_HoroscopeHeal = new("AST_AoE_SimpleHeals_HoroscopeHeal", 80),
            AST_AoE_SimpleHeals_StellarDetonation = new("AST_AoE_SimpleHeals_StellarDetonation", 80),
            AST_AoE_SimpleHeals_Aspected = new("AST_AoE_SimpleHeals_Aspected", 80),
            AST_AoE_SimpleHeals_Helios = new("AST_AoE_SimpleHeals_Helios", 80),
            AST_Mit_ST_EssentialDignityThreshold = new("AST_Mit_ST_EssentialDignityThreshold", 80),

            //DPS
            AST_ST_DPS_Opener_SkipStar = new("AST_ST_DPS_Opener_SkipStar"),
            AST_ST_DPS_DivinationOption = new("AST_ST_DPS_DivinationOption"),
            AST_ST_DPS_AltMode = new("AST_ST_DPS_AltMode"),
            AST_ST_DPS_LucidDreaming = new("AST_ST_DPS_LucidDreaming", 8000),
            AST_ST_DPS_LightSpeedOption = new("AST_ST_DPS_LightSpeedOption"),
            AST_ST_DPS_CombustBossOption = new("AST_ST_DPS_CombustBossOption", 0),
            AST_ST_DPS_CombustBossAddsOption = new("AST_ST_DPS_CombustBossAddsOption", 80),
            AST_ST_DPS_CombustTrashOption = new("AST_ST_DPS_CombustTrashOption", 50),
            AST_ST_DPS_DivinationSubOption = new("AST_ST_DPS_DivinationSubOption", 0),
            AST_ST_DPS_Balance_Content = new("AST_ST_DPS_Balance_Content", 1),
            AST_ST_DPS_EarthlyStarSubOption = new("AST_ST_DPS_EarthlyStarSubOption", 0),
            AST_ST_DPS_StellarDetonation_Threshold = new("AST_ST_DPS_StellarDetonation_Threshold", 0),
            AST_ST_DPS_StellarDetonation_SubOption = new("AST_ST_DPS_StellarDetonation_SubOption", 0),
            AST_AOE_LucidDreaming = new("AST_AOE_LucidDreaming", 8000),
            AST_AOE_DivinationSubOption = new("AST_AOE_DivinationSubOption", 0),
            AST_AOE_DivinationOption = new("AST_AOE_DivinationOption"),
            AST_AOE_LightSpeedOption = new("AST_AOE_LightSpeedOption"),
            AST_AOE_DPS_EarthlyStarSubOption = new("AST_AOE_DPS_EarthlyStarSubOption", 0),
            AST_AOE_DPS_StellarDetonation_Threshold = new("AST_AOE_DPS_StellarDetonation_Threshold", 0),
            AST_AOE_DPS_StellarDetonation_SubOption = new("AST_AOE_DPS_StellarDetonation_SubOption", 0),
            AST_AOE_DPS_MacroCosmos_SubOption = new("AST_AOE_DPS_MacroCosmos_SubOption", 0),
            AST_AOE_DPS_DoT_HPThreshold = new("AST_AOE_DPS_DoT_HPThreshold", 30),
            AST_AOE_DPS_DoT_MaxTargets = new("AST_AOE_DPS_DoT_MaxTargets", 4),
            AST_QuickTarget_Override = new("AST_QuickTarget_Override", 0);

        public static UserBool
            //HEALS
            AST_ST_SimpleHeals_IncludeShields = new("AST_ST_SimpleHeals_IncludeShields"),
            AST_ST_SimpleHeals_WeaveDignity = new("AST_ST_SimpleHeals_WeaveDignity"),
            AST_ST_SimpleHeals_WeaveIntersection = new("AST_ST_SimpleHeals_WeaveIntersection"),
            AST_ST_SimpleHeals_WeaveEwer = new("AST_ST_SimpleHeals_WeaveEwer"),
            AST_ST_SimpleHeals_WeaveSpire = new("AST_ST_SimpleHeals_WeaveSpire"),
            AST_ST_SimpleHeals_WeaveEmergencyED = new("AST_ST_SimpleHeals_WeaveEmergencyED"),
            AST_AoE_SimpleHeals_WeaveLady = new("AST_AoE_SimpleHeals_WeaveLady"),
            AST_AoE_SimpleHeals_WeaveOpposition = new("AST_AoE_SimpleHeals_WeaveOpposition"),
            AST_AoE_SimpleHeals_WeaveCollectiveUnconscious = new("AST_AoE_SimpleHeals_WeaveCollectiveUnconscious"),
            AST_AoE_SimpleHeals_WeaveHoroscope = new("AST_AoE_SimpleHeals_WeaveHoroscope"),
            AST_AoE_SimpleHeals_WeaveNeutralSect = new("AST_AoE_SimpleHeals_WeaveNeutralSect"),
            AST_AoE_SimpleHeals_WeaveHoroscopeHeal = new("AST_AoE_SimpleHeals_WeaveHoroscopeHeal"),
            AST_AoE_SimpleHeals_WeaveStellarDetonation = new("AST_AoE_SimpleHeals_WeaveStellarDetonation"),
            //DPS
            AST_ST_DPS_CombustUptime_TwoTarget = new("AST_ST_DPS_CombustUptime_TwoTarget"),
            AST_ST_DPS_OverwriteHealCards = new("AST_ST_DPS_OverwriteHealCards"),
            AST_AOE_DPS_OverwriteHealCards = new("AST_AOE_DPS_OverwriteHealCards"),
            AST_QuickTarget_Manuals = new("AST_QuickTarget_Manuals", true);
        public static UserFloat
            AST_AOE_DPS_DoT_Reapply = new("AST_AOE_DPS_DoT_Reapply", 2),
            AST_ST_DPS_CombustUptime_Threshold = new("AST_ST_DPS_CombustUptime_Threshold");

        public static UserBoolArray
            AST_ST_SimpleHeals_CelestialOppositionOptions = new("AST_ST_SimpleHeals_CelestialOppositionOptions", [false, true]),
            AST_ST_SimpleHeals_CollectiveUnconsciousOptions = new("AST_ST_SimpleHeals_CollectiveUnconsciousOptions", [false, true]),
            AST_ST_SimpleHeals_SoloLadyOptions = new("AST_ST_SimpleHeals_SoloLadyOptions", [false, true]),
            AST_ST_Heals_NeutralSectOptions = new("AST_ST_Heals_NeutralSectOptions", [false, true]),
            AST_ST_SimpleHeals_ExaltationOptions = new("AST_ST_SimpleHeals_ExaltationOptions", [false, true, true]),
            AST_ST_SimpleHeals_BoleOptions = new("AST_ST_SimpleHeals_BoleOptions", [false, true]),
            AST_ST_SimpleHeals_ArrowOptions = new("AST_ST_SimpleHeals_ArrowOptions", [false, true]),
            AST_Mit_ST_Options = new("AST_Mit_ST_Options"),
            AST_EarthlyStarOptions = new("AST_EarthlyStarOptions");

        #endregion
        internal static void Draw(Preset preset)
        {
            switch (preset)
            {
                #region DPS
                case Preset.AST_ST_DPS_Opener:
                    DrawBossOnlyChoice(AST_ST_DPS_Balance_Content);
                    ImGui.NewLine();
                    DrawHorizontalRadioButton(AST_ST_DPS_Opener_SkipStar, Text.FormatAndCache(Generics.Use0, EarthlyStar.ActionName()), Text.FormatAndCache(AST_Config.PlacesEarthlyStarInTheOpener, EarthlyStar.ActionName()), 0);
                    DrawHorizontalRadioButton(AST_ST_DPS_Opener_SkipStar, Text.FormatAndCache(Generics.DontUse0, EarthlyStar.ActionName()), Text.FormatAndCache(AST_Config.DoesNotUseEarthlyStarInTheOpener, EarthlyStar.ActionName()), 1);
                    break;

                case Preset.AST_ST_DPS:
                    DrawHorizontalRadioButton(AST_ST_DPS_AltMode, Text.FormatAndCache(Generics.On0, Malefic.ActionName()), Text.FormatAndCache(Generics.ApplyToAll0, string.Join("\r\n", MaleficList.Select(x => x.ActionName()))), 0);
                    DrawHorizontalRadioButton(AST_ST_DPS_AltMode, Text.FormatAndCache(Generics.On0, Combust.ActionName()), Text.FormatAndCache(Generics.ApplyToAll0, string.Join("\r\n", CombustList.Select(x => x.Key.ActionName()))), 1);
                    DrawHorizontalRadioButton(AST_ST_DPS_AltMode, Text.FormatAndCache(Generics.On0, Malefic2.ActionName()), Text.FormatAndCache(Generics.ApplyOnlyTo0, Malefic2.ActionName()), 2);
                    break;

                case Preset.AST_DPS_Lucid:
                    DrawSliderInt(4000, 9500, Text.FormatAndCache(AST_ST_DPS_LucidDreaming), Text.FormatAndCache(Generics.LucidMP), 150, Hundreds);
                    break;

                case Preset.AST_ST_DPS_CombustUptime:
                    DrawSliderInt(0, 100, AST_ST_DPS_CombustBossOption, Generics.BossOnlyHpPercent);
                    DrawSliderInt(0, 100, AST_ST_DPS_CombustBossAddsOption, Generics.BossEncounterNonBossHpPercent);
                    DrawSliderInt(0, 100, AST_ST_DPS_CombustTrashOption, Generics.NonBossHpPercent);
                    ImGui.Indent();
                    DrawRoundedSliderFloat(0, 4, AST_ST_DPS_CombustUptime_Threshold, Generics.DoTSecondsRemainingZeroDisable, digits: 1);
                    ImGui.Unindent();
                    DrawAdditionalBoolChoice(AST_ST_DPS_CombustUptime_TwoTarget, Generics.TwoTargetDotting, Generics.TwoTargetDottingDescription);
                    break;

                case Preset.AST_DPS_Divination:
                    DrawSliderInt(0, 100, AST_ST_DPS_DivinationOption, Generics.StopEnemyHpPercent);
                    ImGui.Indent();
                    ImGui.TextColored(ImGuiColors.DalamudYellow, Generics.EnemyTypeCheck);
                    DrawHorizontalRadioButton(AST_ST_DPS_DivinationSubOption,
                        Generics.NonBosses, Generics.HPCheckNonBosses, 0);
                    DrawHorizontalRadioButton(AST_ST_DPS_DivinationSubOption,
                        Generics.AllEnemies, Generics.HPCheckAllEnemies, 1);
                    ImGui.Unindent();
                    break;

                case Preset.AST_DPS_LightSpeed:
                    DrawSliderInt(0, 100, AST_ST_DPS_LightSpeedOption, Generics.StopEnemyHpPercent);
                    break;

                case Preset.AST_DPS_AutoDraw:
                    DrawAdditionalBoolChoice(AST_ST_DPS_OverwriteHealCards, AST_Config.OverwriteNonDPSCards, AST_Config.WillDrawEvenIfYouHaveHealingCardsRemaining);
                    break;

                case Preset.AST_ST_DPS_EarthlyStar:
                    DrawHorizontalRadioButton(AST_ST_DPS_EarthlyStarSubOption,
                        Generics.NormalTargeting, AST_Config.FollowsNormalTargetingPlan, 0);
                    DrawHorizontalRadioButton(AST_ST_DPS_EarthlyStarSubOption,
                        Generics.SelfOnly, AST_Config.PlacesAtOwnFeetOnly, 1);
                    break;

                case Preset.AST_ST_DPS_StellarDetonation:
                    DrawHorizontalRadioButton(AST_ST_DPS_StellarDetonation_SubOption,
                        Generics.NonBossEncountersOnly, Generics.NonBossEncountersOnly, 0);

                    DrawHorizontalRadioButton(AST_ST_DPS_StellarDetonation_SubOption,
                        Generics.AllContent, Generics.AllContent, 1);

                    DrawSliderInt(0, 100, AST_ST_DPS_StellarDetonation_Threshold,
                        AST_Config.UseWhenTargetIsAtOrBelowHP0NeverDetonateEarly100DetonateASAP);
                    break;

                case Preset.AST_AOE_Lucid:
                    DrawSliderInt(4000, 9500, AST_AOE_LucidDreaming, Generics.LucidMP, 150, Hundreds);
                    break;

                case Preset.AST_AOE_Divination:
                    DrawSliderInt(0, 100, AST_AOE_DivinationOption, Generics.StopEnemyHpPercent);
                    ImGui.Indent();
                    ImGui.TextColored(ImGuiColors.DalamudYellow, Generics.EnemyTypeCheck);
                    DrawHorizontalRadioButton(AST_AOE_DivinationSubOption,
                        Generics.NonBosses, Generics.HPCheckNonBosses, 0);
                    DrawHorizontalRadioButton(AST_AOE_DivinationSubOption,
                        Generics.AllEnemies, Generics.HPCheckAllEnemies, 1);
                    ImGui.Unindent();
                    break;

                case Preset.AST_AOE_LightSpeed:
                    DrawSliderInt(0, 100, AST_AOE_LightSpeedOption, Generics.StopEnemyHpPercent);
                    break;

                case Preset.AST_AOE_AutoDraw:
                    DrawAdditionalBoolChoice(AST_AOE_DPS_OverwriteHealCards, AST_Config.OverwriteNonDPSCards, AST_Config.WillDrawEvenIfYouHaveHealingCardsRemaining);
                    break;

                case Preset.AST_AOE_DPS_EarthlyStar:
                    DrawHorizontalRadioButton(AST_AOE_DPS_EarthlyStarSubOption,
                        Generics.NormalTargeting, AST_Config.FollowsNormalTargetingPlan, 0);
                    DrawHorizontalRadioButton(AST_AOE_DPS_EarthlyStarSubOption,
                        Generics.SelfOnly, AST_Config.PlacesAtOwnFeetOnly, 1);
                    break;

                case Preset.AST_AOE_DPS_StellarDetonation:
                    DrawHorizontalRadioButton(AST_AOE_DPS_StellarDetonation_SubOption,
                        Generics.NonBossEncountersOnly, Generics.NonBossEncountersOnly, 0);

                    DrawHorizontalRadioButton(AST_AOE_DPS_StellarDetonation_SubOption,
                        Generics.AllContent, Generics.AllContent, 1);

                    DrawSliderInt(0, 100, AST_AOE_DPS_StellarDetonation_Threshold,
                        AST_Config.UseWhenTargetIsAtOrBelowHP0NeverDetonateEarly100DetonateASAP);
                    break;

                case Preset.AST_AOE_DPS_MacroCosmos:
                    DrawHorizontalRadioButton(AST_AOE_DPS_MacroCosmos_SubOption, Generics.NonBossEncountersOnly, Generics.NonBossEncountersOnly, 0);
                    DrawHorizontalRadioButton(AST_AOE_DPS_MacroCosmos_SubOption, Generics.AllContent, Generics.AllContent, 1);
                    break;

                case Preset.AST_AOE_DPS_DoT:
                    DrawSliderInt(0, 100, AST_AOE_DPS_DoT_HPThreshold, Generics.StopEnemyHpPercent);
                    ImGui.Indent();
                    DrawRoundedSliderFloat(0, 5, AST_AOE_DPS_DoT_Reapply, Generics.StopSeconds, digits: 1);
                    ImGui.Unindent();
                    DrawSliderInt(0, 10, AST_AOE_DPS_DoT_MaxTargets, Generics.MaxTargetsMultiDot);
                    break;

                #endregion

                #region ST Heals
                case Preset.AST_ST_Heals:
                    DrawAdditionalBoolChoice(AST_ST_SimpleHeals_IncludeShields, Generics.IncludeShields, "");
                    break;

                case Preset.AST_ST_Heals_Esuna:
                    DrawSliderInt(0, 100, AST_ST_SimpleHeals_Esuna, Generics.StopFriendlyHpPercentZero);
                    break;

                case Preset.AST_ST_Heals_CelestialIntersection:
                    DrawSliderInt(0, 100, AST_ST_SimpleHeals_CelestialIntersection, Generics.StopFriendlyHpPercent100);
                    DrawSliderInt(0, 1, AST_ST_SimpleHeals_CelestialIntersectionCharges, Generics.ChargePool);
                    DrawAdditionalBoolChoice(AST_ST_SimpleHeals_WeaveIntersection, Generics.OnlyWeave, "");
                    DrawPriorityInput(AST_ST_SimpleHeals_Priority, 13, 1, Text.FormatAndCache(Generics.Action_Priority, CelestialIntersection.ActionName()));
                    break;

                case Preset.AST_ST_Heals_EssentialDignity:
                    DrawSliderInt(0, 100, AST_ST_SimpleHeals_EssentialDignity, Generics.StopFriendlyHpPercent100);
                    DrawAdditionalBoolChoice(AST_ST_SimpleHeals_WeaveDignity, Generics.OnlyWeave, "");
                    DrawPriorityInput(AST_ST_SimpleHeals_Priority, 13, 0, Text.FormatAndCache(AST_Config.Standard0Priority, EssentialDignity.ActionName()));
                    break;

                case Preset.AST_ST_Heals_EssentialDignity_Emergency:
                    DrawSliderInt(0, 100, AST_ST_SimpleHeals_EmergencyED_Threshold, Generics.StopFriendlyHpPercent100);
                    DrawAdditionalBoolChoice(AST_ST_SimpleHeals_WeaveEmergencyED, Generics.OnlyWeave, "");
                    DrawPriorityInput(AST_ST_SimpleHeals_Priority, 13, 11, Text.FormatAndCache(AST_Config.Emergency0Priority, EssentialDignity.ActionName()));
                    break;

                case Preset.AST_ST_Heals_Exaltation:
                    DrawSliderInt(0, 100, AST_ST_SimpleHeals_Exaltation, Generics.StopFriendlyHpPercent100);
                    DrawHorizontalMultiChoice(AST_ST_SimpleHeals_ExaltationOptions, Generics.OnlyWeave, Generics.WillOnlyWeaveThisAction, 3, 0);
                    DrawHorizontalMultiChoice(AST_ST_SimpleHeals_ExaltationOptions, Generics.NotInBossEncounters, Generics.WillNotUseInBossEncounters, 3, 1);
                    DrawHorizontalMultiChoice(AST_ST_SimpleHeals_ExaltationOptions, Generics.TanksOnly, Generics.WillOnlyUseOnTanks, 3, 2);
                    DrawPriorityInput(AST_ST_SimpleHeals_Priority, 13, 2, Text.FormatAndCache(Generics.Action_Priority, Exaltation.ActionName()));
                    break;

                case Preset.AST_ST_Heals_Bole:
                    DrawSliderInt(0, 100, AST_ST_SimpleHeals_Bole, Generics.StopFriendlyHpPercent100);
                    DrawHorizontalMultiChoice(AST_ST_SimpleHeals_BoleOptions, Generics.OnlyWeave, Generics.WillOnlyWeaveThisAction, 2, 0);
                    DrawHorizontalMultiChoice(AST_ST_SimpleHeals_BoleOptions, Generics.TanksOnly, Generics.WillOnlyUseOnTanks, 2, 1);
                    DrawPriorityInput(AST_ST_SimpleHeals_Priority, 13, 3, Text.FormatAndCache(Generics.Action_Priority, Bole.ActionName()));
                    break;

                case Preset.AST_ST_Heals_Arrow:
                    DrawSliderInt(0, 100, AST_ST_SimpleHeals_Arrow, Generics.StopFriendlyHpPercent100);
                    DrawHorizontalMultiChoice(AST_ST_SimpleHeals_ArrowOptions, Generics.OnlyWeave, Generics.WillOnlyWeaveThisAction, 2, 0);
                    DrawHorizontalMultiChoice(AST_ST_SimpleHeals_ArrowOptions, Generics.TanksOnly, Generics.WillOnlyUseOnTanks, 2, 1);
                    DrawPriorityInput(AST_ST_SimpleHeals_Priority, 13, 4, Text.FormatAndCache(Generics.Action_Priority, Arrow.ActionName()));
                    break;

                case Preset.AST_ST_Heals_Ewer:
                    DrawSliderInt(0, 100, AST_ST_SimpleHeals_Ewer, Generics.StopFriendlyHpPercent100);
                    DrawAdditionalBoolChoice(AST_ST_SimpleHeals_WeaveEwer, Generics.OnlyWeave, Generics.WillOnlyWeaveThisAction);
                    DrawPriorityInput(AST_ST_SimpleHeals_Priority, 13, 5, Text.FormatAndCache(Generics.Action_Priority, Ewer.ActionName()));
                    break;

                case Preset.AST_ST_Heals_Spire:
                    DrawSliderInt(0, 100, AST_ST_SimpleHeals_Spire, Generics.StopFriendlyHpPercent100);
                    DrawAdditionalBoolChoice(AST_ST_SimpleHeals_WeaveSpire, Generics.OnlyWeave, Generics.WillOnlyWeaveThisAction);
                    DrawPriorityInput(AST_ST_SimpleHeals_Priority, 13, 6, Text.FormatAndCache(Generics.Action_Priority, Spire.ActionName()));
                    break;

                case Preset.AST_ST_Heals_AspectedBenefic:
                    DrawSliderInt(0, 100, AST_ST_SimpleHeals_AspectedBeneficHigh, Generics.StopFriendlyHpPercent100);
                    DrawSliderInt(0, 100, AST_ST_SimpleHeals_AspectedBeneficLow, Generics.StopUsingWhenBelowSetPercentage);
                    DrawSliderInt(0, 15, AST_ST_SimpleHeals_AspectedBeneficRefresh, Generics.StopSeconds);
                    DrawPriorityInput(AST_ST_SimpleHeals_Priority, 13, 7, Text.FormatAndCache(Generics.Action_Priority, AspectedBenefic.ActionName()));
                    break;

                case Preset.AST_ST_Heals_CelestialOpposition:
                    DrawSliderInt(0, 100, AST_ST_SimpleHeals_CelestialOpposition, Generics.StopFriendlyHpPercent100);
                    DrawHorizontalMultiChoice(AST_ST_SimpleHeals_CelestialOppositionOptions, Generics.OnlyWeave, Generics.WillOnlyWeaveThisAction, 2, 0);
                    DrawHorizontalMultiChoice(AST_ST_SimpleHeals_CelestialOppositionOptions, Generics.NotInBossEncounters, Generics.WillNotUseInBossEncounters, 2, 1);
                    DrawPriorityInput(AST_ST_SimpleHeals_Priority, 13, 8, Text.FormatAndCache(Generics.Action_Priority, CelestialOpposition.ActionName()));
                    break;

                case Preset.AST_ST_Heals_CollectiveUnconscious:
                    DrawSliderInt(0, 100, AST_ST_SimpleHeals_CollectiveUnconscious, Generics.StopFriendlyHpPercent100);
                    DrawHorizontalMultiChoice(AST_ST_SimpleHeals_CollectiveUnconsciousOptions, Generics.OnlyWeave, Generics.WillOnlyWeaveThisAction, 2, 0);
                    DrawHorizontalMultiChoice(AST_ST_SimpleHeals_CollectiveUnconsciousOptions, Generics.NotInBossEncounters, Generics.WillNotUseInBossEncounters, 2, 1);
                    DrawPriorityInput(AST_ST_SimpleHeals_Priority, 13, 9, Text.FormatAndCache(Generics.Action_Priority, CollectiveUnconscious.ActionName()));
                    break;

                case Preset.AST_ST_Heals_SoloLady:
                    DrawSliderInt(0, 100, AST_ST_SimpleHeals_SoloLady, Generics.StopFriendlyHpPercent100);
                    DrawHorizontalMultiChoice(AST_ST_SimpleHeals_SoloLadyOptions, Generics.OnlyWeave, Generics.WillOnlyWeaveThisAction, 2, 0);
                    DrawHorizontalMultiChoice(AST_ST_SimpleHeals_SoloLadyOptions, Generics.NotInBossEncounters, Generics.WillNotUseInBossEncounters, 2, 1);
                    DrawPriorityInput(AST_ST_SimpleHeals_Priority, 13, 10, Text.FormatAndCache(Generics.Action_Priority, LadyOfCrown.ActionName()));
                    break;

                case Preset.AST_ST_Heals_NeutralSect:
                    DrawSliderInt(0, 100, AST_ST_Heals_NeutralSect_Threshold, Generics.StopFriendlyHpPercent100);
                    DrawHorizontalMultiChoice(AST_ST_Heals_NeutralSectOptions, Generics.OnlyWeave, Generics.WillOnlyWeaveThisAction, 2, 0);
                    DrawHorizontalMultiChoice(AST_ST_Heals_NeutralSectOptions, Generics.NotInBossEncounters, Generics.WillNotUseInBossEncounters, 2, 1);
                    DrawPriorityInput(AST_ST_SimpleHeals_Priority, 13, 12, Text.FormatAndCache(Generics.Action_Priority, NeutralSect.ActionName()));
                    break;


                #endregion

                #region AOE Heals

                case Preset.AST_AoE_Heals:
                    DrawRadioButton(AST_AoE_SimpleHeals_AltMode, Text.FormatAndCache(Generics.On0, AspectedHelios.ActionName()), "", 0);
                    DrawRadioButton(AST_AoE_SimpleHeals_AltMode, Text.FormatAndCache(Generics.On0, Helios.ActionName()), Text.FormatAndCache(AST_Config.AlternativeAoEModeLeaves0AloneForManualHoTs, AspectedHelios.ActionName()), 1);
                    break;

                case Preset.AST_AoE_Heals_LazyLady:
                    DrawSliderInt(0, 100, AST_AoE_SimpleHeals_LazyLady, Generics.StartUsingWhenBelowPartyAverageHPSetTo100ToDisableThisCheck);
                    DrawAdditionalBoolChoice(AST_AoE_SimpleHeals_WeaveLady, Generics.OnlyWeave, Generics.WillOnlyWeaveThisAction);
                    DrawPriorityInput(AST_AoE_SimpleHeals_Priority, 9, 0, Text.FormatAndCache(Generics.Action_Priority, LadyOfCrown.ActionName()));
                    break;

                case Preset.AST_AoE_Heals_Horoscope:
                    DrawSliderInt(0, 100, AST_AoE_SimpleHeals_Horoscope, Generics.StartUsingWhenBelowPartyAverageHPSetTo100ToDisableThisCheck);
                    DrawAdditionalBoolChoice(AST_AoE_SimpleHeals_WeaveHoroscope, Generics.OnlyWeave, Generics.WillOnlyWeaveThisAction);
                    DrawPriorityInput(AST_AoE_SimpleHeals_Priority, 9, 1, Text.FormatAndCache(Generics.Action_Priority, Horoscope.ActionName()));
                    break;

                case Preset.AST_AoE_Heals_HoroscopeHeal:
                    DrawSliderInt(0, 100, AST_AoE_SimpleHeals_HoroscopeHeal, Generics.StartUsingWhenBelowPartyAverageHPSetTo100ToDisableThisCheck);
                    DrawAdditionalBoolChoice(AST_AoE_SimpleHeals_WeaveHoroscopeHeal, Generics.OnlyWeave, Generics.WillOnlyWeaveThisAction);
                    DrawPriorityInput(AST_AoE_SimpleHeals_Priority, 9, 2, Text.FormatAndCache(Generics.Action_Priority, HoroscopeHeal.ActionName()));
                    break;

                case Preset.AST_AoE_Heals_CelestialOpposition:
                    DrawSliderInt(0, 100, AST_AoE_SimpleHeals_CelestialOpposition, Generics.StartUsingWhenBelowPartyAverageHPSetTo100ToDisableThisCheck);
                    DrawAdditionalBoolChoice(AST_AoE_SimpleHeals_WeaveOpposition, Generics.OnlyWeave, Generics.WillOnlyWeaveThisAction);
                    DrawPriorityInput(AST_AoE_SimpleHeals_Priority, 9, 3, Text.FormatAndCache(Generics.Action_Priority, CelestialOpposition.ActionName()));
                    break;


                case Preset.AST_AoE_Heals_NeutralSect:
                    DrawSliderInt(0, 100, AST_AoE_SimpleHeals_NeutralSect, Generics.StartUsingWhenBelowPartyAverageHPSetTo100ToDisableThisCheck);
                    DrawAdditionalBoolChoice(AST_AoE_SimpleHeals_WeaveNeutralSect, Generics.OnlyWeave, Generics.WillOnlyWeaveThisAction);
                    DrawPriorityInput(AST_AoE_SimpleHeals_Priority, 9, 4, Text.FormatAndCache(Generics.Action_Priority, NeutralSect.ActionName()));
                    break;

                case Preset.AST_AoE_Heals_StellarDetonation:
                    DrawSliderInt(0, 100, AST_AoE_SimpleHeals_StellarDetonation, Generics.StartUsingWhenBelowPartyAverageHPSetTo100ToDisableThisCheck);
                    DrawAdditionalBoolChoice(AST_AoE_SimpleHeals_WeaveStellarDetonation, Generics.OnlyWeave, Generics.WillOnlyWeaveThisAction);
                    DrawPriorityInput(AST_AoE_SimpleHeals_Priority, 9, 5, Text.FormatAndCache(Generics.Action_Priority, StellarDetonation.ActionName()));
                    break;

                case Preset.AST_AoE_Heals_Aspected:
                    DrawSliderInt(0, 100, AST_AoE_SimpleHeals_Aspected, Generics.StartUsingWhenBelowPartyAverageHPSetTo100ToDisableThisCheck);
                    DrawPriorityInput(AST_AoE_SimpleHeals_Priority, 9, 6, Text.FormatAndCache(Generics.Action_Priority, AspectedHelios.ActionName()));
                    break;

                case Preset.AST_AoE_Heals_Helios:
                    DrawSliderInt(0, 100, AST_AoE_SimpleHeals_Helios, Generics.StartUsingWhenBelowPartyAverageHPSetTo100ToDisableThisCheck);
                    DrawPriorityInput(AST_AoE_SimpleHeals_Priority, 9, 7, Text.FormatAndCache(Generics.Action_Priority, Helios.ActionName()));
                    break;

                case Preset.AST_AoE_Heals_CollectiveUnconscious:
                    DrawSliderInt(0, 100, AST_AoE_SimpleHeals_CollectiveUnconscious, Generics.StartUsingWhenBelowPartyAverageHPSetTo100ToDisableThisCheck);
                    DrawAdditionalBoolChoice(AST_AoE_SimpleHeals_WeaveCollectiveUnconscious, Generics.OnlyWeave, Generics.WillOnlyWeaveThisAction);
                    DrawPriorityInput(AST_AoE_SimpleHeals_Priority, 9, 8, Text.FormatAndCache(Generics.Action_Priority, CollectiveUnconscious.ActionName()));
                    break;

                #endregion

                #region Standalone
                case Preset.AST_Cards_QuickTargetCards:
                    DrawAdditionalBoolChoice(AST_QuickTarget_Manuals,
                        AST_Config.AlsoRetargetManuallyUsedCards,
                        AST_Config.WillAlsoAutomaticallyTargetCardsThatYouManuallyUseAsInThoseOutsideOfYourDamageRotations,
                        indentDescription: true);

                    ImGui.Indent();
                    ImGui.TextWrapped(AST_Config.TargetOverrides);
                    ImGui.Unindent();
                    ImGui.NewLine();
                    DrawRadioButton(AST_QuickTarget_Override, AST_Config.NoOverride, AST_Config.NoOverrideDesc, 0, descriptionAsTooltip: true);
                    DrawRadioButton(AST_QuickTarget_Override, AST_Config.HardTargetOverride, AST_Config.HardTargetOverrideDesc, 1, descriptionAsTooltip: true);
                    DrawRadioButton(AST_QuickTarget_Override, AST_Config.UIMouseOverOverride, AST_Config.UIMouseOverOverrideDesc, 2, descriptionAsTooltip: true);
                    DrawRadioButton(AST_QuickTarget_Override, AST_Config.AnyMouseoverOverride, AST_Config.AnyMouseoverOverrideDesc, 3, descriptionAsTooltip: true);
                    DrawRadioButton(AST_QuickTarget_Override, AST_Config.FocusTargetOverrideWhenCorrectRole, AST_Config.FocusTargetOverrideWhenCorrectRoleDesc, 4, descriptionAsTooltip: true);
                    break;

                case Preset.AST_Retargets_EarthlyStar:
                    ImGui.Indent();
                    ImGui.TextColored(ImGuiColors.DalamudGrey, Text.FormatAndCache(AST_Config.OptionsToTryToRetarget0ToBeforeSelf, EarthlyStar.ActionName()));
                    ImGui.Unindent();
                    DrawHorizontalMultiChoice(AST_EarthlyStarOptions,
                        Generics.EnemyHardTarget, AST_Config.WillPlaceAtHardTargetIfEnemy, 2, 0);
                    DrawHorizontalMultiChoice(AST_EarthlyStarOptions,
                        Generics.AllyHardTarget, AST_Config.WillPlaceAtHardTargetIfAlly, 2, 1);
                    break;

                case Preset.AST_Mit_ST:
                    DrawHorizontalMultiChoice(AST_Mit_ST_Options,
                        Text.FormatAndCache(Generics.Include0, CelestialIntersection.ActionName()), Text.FormatAndCache(AST_Config.WillAdd0ForMoreMitigation, CelestialIntersection.ActionName()), 2, 0);
                    ImGui.NewLine();
                    DrawHorizontalMultiChoice(AST_Mit_ST_Options,
                        Text.FormatAndCache(Generics.Include0, EssentialDignity.ActionName()), Text.FormatAndCache(AST_Config.WillAdd0ToTopOffTargetsHealth, EssentialDignity.ActionName()), 2, 1);
                    if (AST_Mit_ST_Options[1])
                    {
                        ImGui.Indent();
                        DrawSliderInt(1, 100, AST_Mit_ST_EssentialDignityThreshold,
                            Text.FormatAndCache(AST_Config.TargetHPToUse0Below, EssentialDignity.ActionName()));
                        ImGui.Unindent();
                    }
                    break;
                #endregion
            }
        }
    }
}
