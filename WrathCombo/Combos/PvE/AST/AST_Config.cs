using Dalamud.Interface.Colors;
using ImGuiNET;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Window.Functions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using static WrathCombo.Extensions.UIntExtensions;
using static WrathCombo.Window.Functions.SliderIncrements;
using static WrathCombo.Window.Functions.UserConfig;
namespace WrathCombo.Combos.PvE;

internal partial class AST
{
    public static class Config
    {
        public static UserInt
            AST_LucidDreaming = new("ASTLucidDreamingFeature", 8000),
            AST_EssentialDignity = new("ASTCustomEssentialDignity", 50),
            AST_Spire = new("AST_Spire", 80),
            AST_Ewer = new("AST_Ewer", 80),
            AST_Arrow = new("AST_Arrow", 80),
            AST_Bole = new("AST_Bole", 80),
            AST_AoE_SimpleHeals_LazyLadyThreshold = new("AST_AoE_SimpleHeals_LazyLadyThreshold", 80),
            AST_AoE_SimpleHeals_HoroscopeThreshold = new("AST_AoE_SimpleHeals_HoroscopeThreshold", 80),
            AST_AoE_SimpleHeals_CelestialOppositionThreshold = new("AST_AoE_SimpleHeals_CelestialOppositionThreshold", 80),
            AST_AoE_SimpleHeals_NeutralSectThreshold = new("AST_AoE_SimpleHeals_NeutralSectThreshold", 80),
            AST_ST_SimpleHeals_Esuna = new("AST_ST_SimpleHeals_Esuna", 100),
            AST_DPS_AltMode = new("AST_DPS_AltMode"),
            AST_AoEHeals_AltMode = new("AST_AoEHeals_AltMode"),
            AST_DPS_DivinationOption = new("AST_DPS_DivinationOption"),
            AST_AOE_DivinationOption = new("AST_AOE_DivinationOption"),
            AST_DPS_LightSpeedOption = new("AST_DPS_LightSpeedOption"),
            AST_AOE_LightSpeedOption = new("AST_AOE_LightSpeedOption"),
            AST_DPS_CombustOption = new("AST_DPS_CombustOption"),
            AST_QuickTarget_Override = new("AST_QuickTarget_Override"),
            AST_ST_DPS_Balance_Content = new("AST_ST_DPS_Balance_Content", 1),
            AST_ST_DPS_CombustSubOption = new("AST_ST_DPS_CombustSubOption", 0),
            AST_ST_SimpleHeals_AspectedBeneficHigh = new("AST_ST_SimpleHeals_AspectedBeneficHigh", 90),
            AST_ST_SimpleHeals_AspectedBeneficLow = new("AST_ST_SimpleHeals_AspectedBeneficLow", 40),
            AST_ST_SimpleHeals_AspectedBeneficRefresh = new("AST_ST_SimpleHeals_AspectedBeneficRefresh", 3),
            AST_AOE_DPS_MacroCosmos_SubOption = new("AST_AOE_DPS_MacroCosmos_SubOption", 0);            

        public static UserBool
            AST_QuickTarget_Manuals = new("AST_QuickTarget_Manuals", true),
            AST_ST_SimpleHeals_Adv = new("AST_ST_SimpleHeals_Adv"),
            AST_ST_SimpleHeals_IncludeShields = new("AST_ST_SimpleHeals_IncludeShields"),
            AST_ST_SimpleHeals_WeaveDignity = new("AST_ST_SimpleHeals_WeaveDignity"),
            AST_ST_SimpleHeals_WeaveIntersection = new("AST_ST_SimpleHeals_WeaveIntersection"),
            AST_ST_SimpleHeals_WeaveEwer = new("AST_ST_SimpleHeals_WeaveEwer"),
            AST_ST_SimpleHeals_WeaveSpire = new("AST_ST_SimpleHeals_WeaveSpire"),
            AST_ST_SimpleHeals_WeaveArrow = new("AST_ST_SimpleHeals_WeaveArrow"),
            AST_ST_SimpleHeals_WeaveBole = new("AST_ST_SimpleHeals_WeaveBole"),
            AST_ST_SimpleHeals_WeaveExalt = new("AST_ST_SimpleHeals_WeaveExalt"),
            AST_AoE_SimpleHeals_WeaveLady = new("AST_AoE_SimpleHeals_WeaveLady"),
            AST_AoE_SimpleHeals_Opposition = new("AST_AoE_SimpleHeals_Opposition"),
            AST_AoE_SimpleHeals_Horoscope = new("AST_AoE_SimpleHeals_Horoscope"),
            AST_AoE_SimpleHeals_NeutralSectWeave = new("AST_AoE_SimpleHeals_NeutralSectWeave"),
            AST_ST_DPS_OverwriteCards = new("AST_ST_DPS_OverwriteCards"),
            AST_AOE_DPS_OverwriteCards = new("AST_AOE_DPS_OverwriteCards");
        public static UserFloat
            AST_ST_DPS_CombustUptime_Threshold = new("AST_ST_DPS_CombustUptime_Threshold");

        internal static void Draw(CustomComboPreset preset)
        {
            switch (preset)
            {
                case CustomComboPreset.AST_ST_DPS_Opener:
                    DrawBossOnlyChoice(AST_ST_DPS_Balance_Content);
                    break;

                case CustomComboPreset.AST_ST_DPS:
                    DrawRadioButton(AST_DPS_AltMode, $"On {Malefic.ActionName()}", "", 0);
                    DrawRadioButton(AST_DPS_AltMode, $"On {Combust.ActionName()}", $"Alternative DPS Mode. Leaves {Malefic.ActionName()} alone for pure DPS, becomes {Malefic.ActionName()} when features are on cooldown", 1);
                    break;

                case CustomComboPreset.AST_DPS_Lucid:
                    DrawSliderInt(4000, 9500, AST_LucidDreaming, "Set value for your MP to be at or under for this feature to work", 150, Hundreds);
                    break;

                case CustomComboPreset.AST_ST_DPS_CombustUptime:
                    
                    DrawSliderInt(0, 50, AST_DPS_CombustOption, "Stop using at Enemy HP %. Set to Zero to disable this check.");

                    ImGui.Indent();

                    ImGui.TextColored(ImGuiColors.DalamudYellow, "Select what kind of enemies the HP check should be applied to:");

                    DrawHorizontalRadioButton(AST_ST_DPS_CombustSubOption,
                        "Non-Bosses", "Only applies the HP check above to non-bosses.\nAllows you to only stop DoTing early when it's not a boss.", 0);
                    
                    DrawHorizontalRadioButton(AST_ST_DPS_CombustSubOption,
                        "All Enemies", "Applies the HP check above to all enemies.", 1);

                    
                    

                    DrawRoundedSliderFloat(0, 4, AST_ST_DPS_CombustUptime_Threshold, "Seconds remaining before reapplying the DoT. Set to Zero to disable this check.", digits: 1);

                    ImGui.Unindent();

                    break;

                case CustomComboPreset.AST_DPS_Divination:
                    DrawSliderInt(0, 100, AST_DPS_DivinationOption, "Stop using at Enemy HP %. Set to Zero to disable this check.");
                    break;

                case CustomComboPreset.AST_DPS_LightSpeed:
                    DrawSliderInt(0, 100, AST_DPS_LightSpeedOption, "Stop using at Enemy HP %. Set to Zero to disable this check.");
                    break;

                //AOE added
                case CustomComboPreset.AST_AOE_Lucid:
                    DrawSliderInt(4000, 9500, AST_LucidDreaming, "Set value for your MP to be at or under for this feature to work", 150, Hundreds);
                    break;

                case CustomComboPreset.AST_AOE_Divination:
                    DrawSliderInt(0, 100, AST_AOE_DivinationOption, "Stop using at Enemy HP %. Set to Zero to disable this check.");
                    break;

                case CustomComboPreset.AST_AOE_LightSpeed:
                    DrawSliderInt(0, 100, AST_AOE_LightSpeedOption, "Stop using at Enemy HP %. Set to Zero to disable this check.");
                    break;

                case CustomComboPreset.AST_AOE_AutoDraw:
                    DrawAdditionalBoolChoice(AST_AOE_DPS_OverwriteCards, "Overwrite Non-DPS Cards", "Will draw even if you have healing cards remaining.");
                    break;

                case CustomComboPreset.AST_AOE_DPS_MacroCosmos:

                    DrawHorizontalRadioButton(AST_AOE_DPS_MacroCosmos_SubOption,
                        "Non-boss Encounters Only", $"Will not use on bosses", 0);
                    DrawHorizontalRadioButton(AST_AOE_DPS_MacroCosmos_SubOption,
                        "All Content", $"Will use in all content", 1);

                    ImGui.Unindent();

                    break;

                //end aoe added

                case CustomComboPreset.AST_ST_SimpleHeals:
                    DrawAdditionalBoolChoice(AST_ST_SimpleHeals_Adv, "Advanced Options", "", isConditionalChoice: true);
                    if (AST_ST_SimpleHeals_Adv)
                    {
                        ImGui.Indent();
                        ImGui.Spacing();
                        DrawAdditionalBoolChoice(AST_ST_SimpleHeals_IncludeShields, "Include Shields in HP Percent Sliders", "");
                        ImGui.Unindent();
                    }
                    break;

                case CustomComboPreset.AST_ST_SimpleHeals_AspectedBenefic:
                    DrawSliderInt(0, 100, AST_ST_SimpleHeals_AspectedBeneficHigh, "Start using when below set percentage");
                    DrawSliderInt(0, 100, AST_ST_SimpleHeals_AspectedBeneficLow, "Stop using when below set percentage");
                    DrawSliderInt(0, 15, AST_ST_SimpleHeals_AspectedBeneficRefresh, "Seconds remaining before reapplying (0 = Do not reapply early)");
                    
                    break;

                case CustomComboPreset.AST_ST_SimpleHeals_EssentialDignity:
                    DrawSliderInt(0, 100, AST_EssentialDignity, "Set percentage value");
                    DrawAdditionalBoolChoice(AST_ST_SimpleHeals_WeaveDignity, "Only Weave", "Will only weave this action.");
                    break;

                case CustomComboPreset.AST_ST_SimpleHeals_CelestialIntersection:
                    DrawAdditionalBoolChoice(AST_ST_SimpleHeals_WeaveIntersection, "Only Weave", "Will only weave this action.");
                    break;

                case CustomComboPreset.AST_ST_SimpleHeals_Exaltation:
                    DrawAdditionalBoolChoice(AST_ST_SimpleHeals_WeaveExalt, "Only Weave", "Will only weave this action.");
                    break;

                case CustomComboPreset.AST_ST_SimpleHeals_Spire:
                    DrawSliderInt(0, 100, AST_Spire, "Set percentage value");
                    DrawAdditionalBoolChoice(AST_ST_SimpleHeals_WeaveSpire, "Only Weave", "Will only weave this action.");
                    break;

                case CustomComboPreset.AST_ST_SimpleHeals_Ewer:
                    DrawSliderInt(0, 100, AST_Ewer, "Set percentage value");
                    DrawAdditionalBoolChoice(AST_ST_SimpleHeals_WeaveEwer, "Only Weave", "Will only weave this action.");
                    break;

                case CustomComboPreset.AST_ST_SimpleHeals_Bole:
                    DrawSliderInt(0, 100, AST_Bole, "Set percentage value");
                    DrawAdditionalBoolChoice(AST_ST_SimpleHeals_WeaveBole, "Only Weave", "Will only weave this action.");
                    break;

                case CustomComboPreset.AST_ST_SimpleHeals_Arrow:
                    DrawSliderInt(0, 100, AST_Arrow, "Set percentage value");
                    DrawAdditionalBoolChoice(AST_ST_SimpleHeals_WeaveArrow, "Only Weave", "Will only weave this action.");
                    break;

                case CustomComboPreset.AST_ST_SimpleHeals_Esuna:
                    DrawSliderInt(0, 100, AST_ST_SimpleHeals_Esuna, "Stop using when below HP %. Set to Zero to disable this check");
                    break;

                case CustomComboPreset.AST_AoE_SimpleHeals_AspectedHelios:
                    DrawRadioButton(AST_AoEHeals_AltMode, $"On {AspectedHelios.ActionName()}", "", 0);
                    DrawRadioButton(AST_AoEHeals_AltMode, $"On {Helios.ActionName()}", "Alternative AOE Mode. Leaves Aspected Helios alone for manual HoTs", 1);
                    break;

                case CustomComboPreset.AST_AoE_SimpleHeals_LazyLady:
                    DrawSliderInt(0, 100, AST_AoE_SimpleHeals_LazyLadyThreshold, "Start using when below party average HP %. Set to 100 to disable this check");
                    DrawAdditionalBoolChoice(AST_AoE_SimpleHeals_WeaveLady, "Only Weave", "Will only weave this action.");
                    break;

                case CustomComboPreset.AST_AoE_SimpleHeals_Horoscope:
                    DrawSliderInt(0, 100, AST_AoE_SimpleHeals_HoroscopeThreshold, "Start using when below party average HP %. Set to 100 to disable this check");
                    DrawAdditionalBoolChoice(AST_AoE_SimpleHeals_Horoscope, "Only Weave", "Will only weave this action.");
                    break;

                case CustomComboPreset.AST_AoE_SimpleHeals_CelestialOpposition:
                    DrawSliderInt(0, 100, AST_AoE_SimpleHeals_CelestialOppositionThreshold, "Start using when below party average HP %. Set to 100 to disable this check");
                    DrawAdditionalBoolChoice(AST_AoE_SimpleHeals_Opposition, "Only Weave", "Will only weave this action.");
                    break;


                case CustomComboPreset.AST_AoE_SimpleHeals_NeutralSect:
                    DrawSliderInt(0, 100, AST_AoE_SimpleHeals_NeutralSectThreshold, "Start using when below party average HP %. Set to 100 to disable this check");
                    DrawAdditionalBoolChoice(AST_AoE_SimpleHeals_NeutralSectWeave, "Only Weave", "Will only weave this action.");
                    break;

                case CustomComboPreset.AST_Cards_QuickTargetCards:
                    DrawAdditionalBoolChoice(AST_QuickTarget_Manuals,
                        "Also Retarget manually-used Cards",
                        "Will also automatically target Cards that you manually use, as in, those outside of your damage rotations.",
                        indentDescription: true);

                    ImGui.Indent();
                    ImGui.TextWrapped("Target Overrides:           (hover each for more info)");
                    ImGui.Unindent();
                    ImGui.NewLine();
                    DrawRadioButton(AST_QuickTarget_Override, "No Override", "Will not override the automatic party target viability checking with any manual input.\nThe cards will be targeted according to The Balance's priorities and status checking\n(like not doubling up on cards, and no damage down, etc.).", 0, descriptionAsTooltip: true);
                    DrawRadioButton(AST_QuickTarget_Override, "Hard Target Override", "Overrides selection with hard target, if you have one that is in range and does not have damage down or rez sickness.", 1, descriptionAsTooltip: true);
                    DrawRadioButton(AST_QuickTarget_Override, "UI MouseOver Override", "Overrides selection with UI MouseOver target, if you have one that is in range and does not have damage down or rez sickness.", 2, descriptionAsTooltip: true);
                    DrawRadioButton(AST_QuickTarget_Override, "Any MouseOver Override", "Overrides selection with UI or Nameplate or Model MouseOver target (in that order), if you have one that is in range and does not have damage down or rez sickness.", 3, descriptionAsTooltip: true);
                    break;

                case CustomComboPreset.AST_DPS_AutoDraw:
                    DrawAdditionalBoolChoice(AST_ST_DPS_OverwriteCards, "Overwrite Non-DPS Cards", "Will draw even if you have healing cards remaining.");
                    break;

               
                
            }
        }
    }
}
