using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility.Raii;
using ECommons.ImGuiMethods;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using WrathCombo.Resources.Localization.JobConfigs;
using static WrathCombo.Window.Functions.UserConfig;
using static WrathCombo.Window.Text;
namespace WrathCombo.Combos.PvE;

internal partial class MNK
{
    internal static class Config
    {
        internal static void Draw(Preset preset)
        {
            switch (preset)
            {
                #region ST

                case Preset.MNK_STUseOpener:
                    DrawBossOnlyChoice(MNK_Balance_Content);
                    DrawOpenerPotionChoice(MNK_Opener_Potion);
                    ImGuiEx.TextUnderlined("Select Opener");
                    ImGui.Spacing();
                    DrawRadioButton(MNK_SelectedOpener,
                        MNK_Config.DoubleLunarOpener,
                        MNK_Config.DoubleLunarOpenerDesc, 0, descriptionAsTooltip: true);

                    DrawRadioButton(MNK_SelectedOpener,
                        MNK_Config.SolarLunarOpener,
                        MNK_Config.SolarLunarOpenerDesc, 1, descriptionAsTooltip: true);

                    DrawRadioButton(MNK_SelectedOpener,
                        MNK_Config.BrotherhoodFirstOpener,
                        MNK_Config.BrotherhoodFirstOpenerDesc, 2, descriptionAsTooltip: true);

                    ImGuiEx.TextUnderlined("Countdown Settings");
                    ImGui.Spacing();
                    DrawRadioButton(MNK_OpenerCountdown,
                        Generics.OnlyWithCountdown,
                        Generics.OnlyUseOpenerWhenCountdownActive, 0, descriptionAsTooltip: true);
                    DrawRadioButton(MNK_OpenerCountdown,
                        Generics.Always,
                        Generics.UseAlways, 1, descriptionAsTooltip: true);
                    break;

                case Preset.MNK_STUseBrotherhood:

                    DrawSliderInt(0, 50, MNK_ST_BHHPOption,
                        Generics.StopEnemyHpPercent);

                    ImGui.Indent();
                    ImGui.TextColored(ImGuiColors.DalamudYellow,
                        Generics.EnemyTypeCheck);

                    DrawHorizontalRadioButton(MNK_ST_BHHPBossOption,
                        Generics.NonBosses,
                        Generics.HPCheckNonBosses, 0);

                    DrawHorizontalRadioButton(MNK_ST_BHHPBossOption,
                        Generics.AllEnemies,
                        Generics.HPCheckAllEnemies, 1);
                    ImGui.Unindent();
                    break;

                case Preset.MNK_STUseROF:

                    DrawSliderInt(0, 50, MNK_ST_RoFHPOption,
                        Generics.StopEnemyHpPercent);

                    ImGui.Indent();
                    ImGui.TextColored(ImGuiColors.DalamudYellow,
                        Generics.EnemyTypeCheck);

                    DrawHorizontalRadioButton(MNK_ST_RoFHPBossOption,
                        Generics.NonBosses,
                        Generics.HPCheckNonBosses, 0);

                    DrawHorizontalRadioButton(MNK_ST_RoFHPBossOption,
                        Generics.AllEnemies,
                        Generics.HPCheckAllEnemies, 1);
                    ImGui.Unindent();
                    break;

                case Preset.MNK_STUseROW:

                    DrawSliderInt(0, 50, MNK_ST_RoWHPOption,
                        Generics.StopEnemyHpPercent);

                    ImGui.Indent();
                    ImGui.TextColored(ImGuiColors.DalamudYellow,
                        Generics.EnemyTypeCheck);

                    DrawHorizontalRadioButton(MNK_ST_RoWHPBossOption,
                        Generics.NonBosses,
                        Generics.HPCheckNonBosses, 0);

                    DrawHorizontalRadioButton(MNK_ST_RoWHPBossOption,
                        Generics.AllEnemies,
                        Generics.HPCheckAllEnemies, 1);
                    ImGui.Unindent();
                    break;

                case Preset.MNK_STUseTrueNorth:
                    DrawSliderInt(0, 1, MNK_ManualTN,
                        Generics.ChargePool);
                    break;

                case Preset.MNK_ST_ComboHeals:
                    DrawSliderInt(0, 100, MNK_ST_SecondWindHPThreshold,
                        FormatAndCache(Generics.HPPercentageThreshold, Role.SecondWind.ActionName()));

                    DrawSliderInt(0, 100, MNK_ST_BloodbathHPThreshold,
                        FormatAndCache(Generics.HPPercentageThreshold, Role.Bloodbath.ActionName()));
                    break;

                #endregion

                #region AoE

                case Preset.MNK_AoEUseBuffs:
                    DrawSliderInt(0, 100, MNK_AoE_BuffsHPThreshold,
                        Generics.StopUsingWhenBelowTargetHPPercent);
                    break;

                case Preset.MNK_AoEUsePerfectBalance:
                    DrawSliderInt(0, 100, MNK_AoE_PerfectBalanceHPThreshold,
                        FormatAndCache(Generics.StopUsing0WhenBelowTargetHPPercentage, PerfectBalance.ActionName()));
                    break;

                case Preset.MNK_AoE_ComboHeals:
                    DrawSliderInt(0, 100, MNK_AoE_SecondWindHPThreshold,
                        FormatAndCache(Generics.HPPercentageThreshold, Role.SecondWind.ActionName()));

                    DrawSliderInt(0, 100, MNK_AoE_BloodbathHPThreshold,
                        FormatAndCache(Generics.HPPercentageThreshold, Role.Bloodbath.ActionName()));
                    break;

                case Preset.MNK_ST_UseRoE:
                    DrawAdditionalBoolChoice(MNK_ST_EarthsReply,
                        FormatAndCache(Generics.Add0, EarthsReply.ActionName()),
                        FormatAndCache(Generics.Add0ToTheRotation, EarthsReply.ActionName()));

                    if (MNK_ST_EarthsReply)
                    {
                        DrawSliderInt(0, 100, MNK_ST_EarthsReplyHPThreshold,
                            FormatAndCache(Generics.Add0WhenAverageHpPercentofPartyIsAtOrBelow, EarthsReply.ActionName()));
                    }
                    break;

                #endregion

                #region Misc

                case Preset.MNK_Brotherhood_Riddle:
                    DrawRadioButton(MNK_BH_RoF,
                        FormatAndCache(Generics.Replaces0, Brotherhood.ActionName()),
                        FormatAndCache(MNK_Config.Repalce0With1When2IsOnCooldown, Brotherhood.ActionName(), RiddleOfFire.ActionName(), Brotherhood.ActionName()), 0);

                    DrawRadioButton(MNK_BH_RoF,
                        FormatAndCache(Generics.Replaces0, RiddleOfFire.ActionName()),
                        FormatAndCache(MNK_Config.Repalce0With1When2IsOnCooldown, RiddleOfFire.ActionName(), Brotherhood.ActionName(), RiddleOfFire.ActionName()), 1);
                    break;

                case Preset.MNK_Retarget_Thunderclap:
                    DrawAdditionalBoolChoice(MNK_Thunderclap_FieldMouseover,
                        Generics.FieldMouseover,
                        Generics.AddFieldMouseoverTargetting);
                    break;

                case Preset.MNK_ST_BasicCombo:
                    DrawAdditionalBoolChoice(MNK_BasicCombo_MasterfulBlitz,
                        FormatAndCache(Generics.Add0WhenApplicable, MasterfulBlitz.ActionName()), "");

                    DrawAdditionalBoolChoice(MNK_BasicCombo_Chakra,
                        FormatAndCache(Generics.Add0Or1WhenApplicable, SteelPeak.ActionName(), TheForbiddenChakra.ActionName()), "");
                    break;

                case Preset.MNK_Basic_BeastChakras:
                    DrawHorizontalMultiChoice(MNK_BasicCombo,
                        FormatAndCache(Generics._0Option, Buffs.OpoOpoForm.StatusName()),
                        FormatAndCache(Generics.Replace0With1Or2, DragonKick.ActionName(), Bootshine.ActionName(), LeapingOpo.ActionName()), 3, 0);

                    DrawHorizontalMultiChoice(MNK_BasicCombo,
                        FormatAndCache(Generics._0Option, Buffs.RaptorForm.StatusName()),
                        FormatAndCache(Generics.Replace0With1Or2, TwinSnakes.ActionName(), TrueStrike.ActionName(), RisingRaptor.ActionName()), 3, 1);

                    DrawHorizontalMultiChoice(MNK_BasicCombo,
                        FormatAndCache(Generics._0Option, Buffs.CoeurlForm.StatusName()),
                        FormatAndCache(Generics.Replace0With1Or2, Demolish.ActionName(), SnapPunch.ActionName(), PouncingCoeurl.ActionName()), 3, 2);
                    break;

                  #endregion
            }
        }

        #region Variables

        public static UserInt

            //ST
            MNK_SelectedOpener = new("MNK_SelectedOpener"),
            MNK_OpenerCountdown = new("MNK_OpenerCountdown"),
            MNK_Balance_Content = new("MNK_Balance_Content", 1),
            MNK_ST_BHHPBossOption = new("MNK_ST_BHHPBossOption"),
            MNK_ST_BHHPOption = new("MNK_ST_BHHPOption", 25),
            MNK_ST_RoFHPBossOption = new("MNK_ST_RoFHPBossOption"),
            MNK_ST_RoFHPOption = new("MNK_ST_RoFHPOption", 25),
            MNK_ST_RoWHPBossOption = new("MNK_ST_RoWHPBossOption"),
            MNK_ST_RoWHPOption = new("MNK_ST_RoWHPOption", 25),
            MNK_ManualTN = new("MNK_ManualTN"),
            MNK_ST_EarthsReplyHPThreshold = new("MNK_ST_EarthsReplyHPThreshold", 25),
            MNK_ST_SecondWindHPThreshold = new("MNK_ST_SecondWindHPThreshold", 40),
            MNK_ST_BloodbathHPThreshold = new("MNK_ST_BloodbathHPThreshold", 30),

            //AoE
            MNK_AoE_BuffsHPThreshold = new("MNK_AoE_BuffsHPThreshold", 25),
            MNK_AoE_PerfectBalanceHPThreshold = new("MNK_AoE_PerfectBalanceHPThreshold", 25),
            MNK_AoE_SecondWindHPThreshold = new("MNK_AoE_SecondWindHPThreshold", 40),
            MNK_AoE_BloodbathHPThreshold = new("MNK_AoE_BloodbathHPThreshold", 30),

            //Misc
            MNK_BH_RoF = new("MNK_BH_RoF");

        public static UserBool
            MNK_Opener_Potion = new("MNK_Opener_Potion"),
            MNK_Thunderclap_FieldMouseover = new("MNK_Thunderclap_FieldMouseover"),
            MNK_BasicCombo_MasterfulBlitz = new("MNK_BasicCombo_MasterfulBlitz"),
            MNK_BasicCombo_Chakra = new("MNK_BasicCombo_Chakra"),
            MNK_ST_EarthsReply = new("MNK_ST_EarthsReply");

        public static UserBoolArray
            MNK_BasicCombo = new("MNK_BasicCombo");

        #endregion
    }
}
