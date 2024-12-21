using ImGuiNET;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Window.Functions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;
using static WrathCombo.Extensions.UIntExtensions;
using static WrathCombo.Window.Functions.UserConfig;
using static WrathCombo.Window.Functions.SliderIncrements;

namespace WrathCombo.Combos.PvE
{
    internal static partial class AST
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
                AST_ST_SimpleHeals_Esuna = new("AST_ST_SimpleHeals_Esuna", 100),
                AST_DPS_AltMode = new("AST_DPS_AltMode"),
                AST_AoEHeals_AltMode = new("AST_AoEHeals_AltMode"),
                AST_DPS_DivinationOption = new("AST_DPS_DivinationOption"),
                AST_AOE_DivinationOption = new("AST_AOE_DivinationOption"),
                AST_DPS_LightSpeedOption = new("AST_DPS_LightSpeedOption"),
                AST_AOE_LightSpeedOption = new("AST_AOE_LightSpeedOption"),
                AST_DPS_CombustOption = new("AST_DPS_CombustOption"),
                AST_QuickTarget_Override = new("AST_QuickTarget_Override"),
                AST_ST_DPS_Play_SpeedSetting = new("AST_ST_DPS_Play_SpeedSetting"),
                AST_ST_DPS_Balance_Content = new("AST_ST_DPS_Balance_Content", 1),
                //PVP
                ASTPvP_Burst_PlayCardOption = new("ASTPvP_Burst_PlayCardOption");
            public static UserBool
                AST_QuickTarget_SkipDamageDown = new("AST_QuickTarget_SkipDamageDown"),
                AST_QuickTarget_SkipRezWeakness = new("AST_QuickTarget_SkipRezWeakness"),
                AST_ST_SimpleHeals_Adv = new("AST_ST_SimpleHeals_Adv"),
                AST_ST_SimpleHeals_UIMouseOver = new("AST_ST_SimpleHeals_UIMouseOver"),
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
                AST_ST_DPS_OverwriteCards = new("AST_ST_DPS_OverwriteCards"),
                AST_AOE_DPS_OverwriteCards = new("AST_AOE_DPS_OverwriteCards"),
                AST_ST_DPS_CombustUptime_Adv = new("AST_ST_DPS_CombustUptime_Adv");
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
                        DrawSliderInt(0, 100, AST_DPS_CombustOption, "Stop using at Enemy HP %. Set to Zero to disable this check.");

                        DrawAdditionalBoolChoice(AST_ST_DPS_CombustUptime_Adv, "Advanced Options", "", isConditionalChoice: true);
                        if (AST_ST_DPS_CombustUptime_Adv)
                        {
                            ImGui.Indent();
                            DrawRoundedSliderFloat(0, 4, AST_ST_DPS_CombustUptime_Threshold, "Seconds remaining before reapplying the DoT. Set to Zero to disable this check.", digits: 1);
                            ImGui.Unindent();
                        }

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

                    //end aoe added

                    case CustomComboPreset.AST_ST_SimpleHeals:
                        DrawAdditionalBoolChoice(AST_ST_SimpleHeals_Adv, "Advanced Options", "", isConditionalChoice: true);
                        if (AST_ST_SimpleHeals_Adv)
                        {
                            ImGui.Indent(); ImGui.Spacing();
                            DrawAdditionalBoolChoice(AST_ST_SimpleHeals_UIMouseOver,
                                "Party UI Mouseover Checking",
                                "Check party member's HP & Debuffs by using mouseover on the party list.\n" +
                                "To be used in conjunction with Redirect/Reaction/etc");
                            DrawAdditionalBoolChoice(AST_ST_SimpleHeals_IncludeShields, "Include Shields in HP Percent Sliders", "");
                            ImGui.Unindent();
                        }
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
                        DrawAdditionalBoolChoice(AST_AoE_SimpleHeals_WeaveLady, "Only Weave", "Will only weave this action.");
                        break;

                    case CustomComboPreset.AST_AoE_SimpleHeals_Horoscope:
                        DrawAdditionalBoolChoice(AST_AoE_SimpleHeals_Horoscope, "Only Weave", "Will only weave this action.");
                        break;

                    case CustomComboPreset.AST_AoE_SimpleHeals_CelestialOpposition:
                        DrawAdditionalBoolChoice(AST_AoE_SimpleHeals_Opposition, "Only Weave", "Will only weave this action.");
                        break;

                    case CustomComboPreset.AST_Cards_QuickTargetCards:
                        DrawRadioButton(AST_QuickTarget_Override, "No Override", "", 0);
                        DrawRadioButton(AST_QuickTarget_Override, "Hard Target Override", "Overrides selection with hard target if you have one", 1);
                        DrawRadioButton(AST_QuickTarget_Override, "UI Mousover Override", "Overrides selection with UI mouseover target if you have one", 2);

                        ImGui.Spacing();
                        DrawAdditionalBoolChoice(AST_QuickTarget_SkipDamageDown, $"Skip targets with a {GetStatusName(62)} debuff", "");
                        DrawAdditionalBoolChoice(AST_QuickTarget_SkipRezWeakness, $"Skip targets with a {GetStatusName(43)} or {GetStatusName(44)} debuff", "");
                        break;

                    case CustomComboPreset.AST_DPS_AutoPlay:
                        DrawRadioButton(AST_ST_DPS_Play_SpeedSetting, "Fast (1 DPS GCD minimum delay)", "", 1);
                        DrawRadioButton(AST_ST_DPS_Play_SpeedSetting, "Medium (2 DPS GCD minimum delay)", "", 2);
                        DrawRadioButton(AST_ST_DPS_Play_SpeedSetting, "Slow (3 DPS GCD minimum delay)", "", 3);

                        break;

                    case CustomComboPreset.AST_DPS_AutoDraw:
                        DrawAdditionalBoolChoice(AST_ST_DPS_OverwriteCards, "Overwrite Non-DPS Cards", "Will draw even if you have healing cards remaining.");
                        break;

                    //PVP
                    case CustomComboPreset.ASTPvP_Burst_PlayCard:
                        DrawHorizontalRadioButton(ASTPvP_Burst_PlayCardOption, "Lord and Lady card play",
                            "Uses Lord and Lady of Crowns when available.", 1);

                        DrawHorizontalRadioButton(ASTPvP_Burst_PlayCardOption, "Lord of Crowns card play",
                            "Only uses Lord of Crowns when available.", 2);

                        DrawHorizontalRadioButton(ASTPvP_Burst_PlayCardOption, "Lady of Crowns card play",
                            "Only uses Lady of Crowns when available.", 3);

                        break;


                }
            }
        }
    }
}
