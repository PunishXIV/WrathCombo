using Dalamud.Interface.Colors;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using static WrathCombo.Window.Functions.UserConfig;
namespace WrathCombo.Combos.PvE;

internal partial class NIN
{
    internal static class Config
    {
        #region Options
        internal static UserInt
            NIN_ST_AdvancedMode_BurnKazematoi = new("NIN_ST_AdvancedMode_BurnKazematoi", 10),
            NIN_ST_AdvancedMode_SuitonSetup = new("NIN_ST_AdvancedMode_SuitonSetup", 18),
            NIN_ST_AdvancedMode_SecondWindThreshold = new("NIN_ST_AdvancedMode_SecondWindThreshold", 40),
            NIN_ST_AdvancedMode_ShadeShiftThreshold = new("NIN_ST_AdvancedMode_ShadeShiftThreshold", 20),
            NIN_ST_AdvancedMode_BloodbathThreshold = new("NIN_ST_AdvancedMode_BloodbathThreshold", 40),
            NIN_ST_AdvancedMode_Mug_Threshold = new("NIN_ST_AdvancedMode_Mug_Threshold", 40),
            NIN_ST_AdvancedMode_Mug_SubOption = new("NIN_ST_AdvancedMode_Mug_SubOption", 0),
            NIN_ST_AdvancedMode_TrickAttack_Threshold = new("NIN_ST_AdvancedMode_TrickAttack_Threshold", 40),
            NIN_ST_AdvancedMode_TrickAttack_SubOption = new("NIN_ST_AdvancedMode_TrickAttack_SubOption", 0),
            NIN_AoE_AdvancedMode_SecondWindThreshold = new("NIN_AoE_AdvancedMode_SecondWindThreshold", 40),
            NIN_AoE_AdvancedMode_ShadeShiftThreshold = new("NIN_AoE_AdvancedMode_ShadeShiftThreshold", 20),
            NIN_AoE_AdvancedMode_BloodbathThreshold = new("NIN_AoE_AdvancedMode_BloodbathThreshold", 40),
            NIN_AoE_AdvancedMode_HutonSetup = new("NIN_AoE_AdvancedMode_HutonSetup", 18),
            NIN_AoE_AdvancedMode_Doton_Threshold = new("NIN_AoE_AdvancedMode_Doton_Threshold", 20),
            NIN_AoE_AdvancedMode_Mug_Threshold = new("NIN_AoE_AdvancedMode_Mug_Threshold", 40),
            NIN_AoE_AdvancedMode_Mug_SubOption = new("NIN_AoE_AdvancedMode_Mug_SubOption", 0),
            NIN_AoE_AdvancedMode_TrickAttack_Threshold = new("NIN_AoE_AdvancedMode_TrickAttack_Threshold", 40),
            NIN_AoE_AdvancedMode_TrickAttack_SubOption = new("NIN_AoE_AdvancedMode_TrickAttack_SubOption", 0),
            NIN_Adv_Opener_Selection = new("NIN_Adv_Opener_Selection", 0),
            NIN_Balance_Content = new("NIN_Balance_Content", 1),
            NIN_SimpleMudra_Choice = new("NIN_SimpleMudra_Choice", 1);

        internal static UserBool
            NIN_ST_AdvancedMode_Bhavacakra_Pooling = new("Ninki_BhavaPooling"),
            NIN_ST_AdvancedMode_TrueNorth = new("NIN_ST_AdvancedMode_TrueNorth"),
            NIN_ST_AdvancedMode_ShadeShiftRaidwide = new("NIN_ST_AdvancedMode_ShadeShiftRaidwide"),
            NIN_ST_AdvancedMode_ForkedRaiju = new("NIN_ST_AdvancedMode_ForkedRaiju"),
            NIN_AoE_AdvancedMode_HellfrogMedium_Pooling = new("Ninki_HellfrogPooling"),
            NIN_AoE_AdvancedMode_ShadeShiftRaidwide = new("NIN_AoE_AdvancedMode_ShadeShiftRaidwide"),
            NIN_HideMug_TrickAfterMug = new("NIN_HideMug_TrickAfterMug"),
            NIN_HideMug_ToggleLevelCheck = new("NIN_HideMug_ToggleLevelCheck"),
            NIN_HideMug_Toggle = new("NIN_HideMug_Toggle"),
            NIN_HideMug_Trick = new("NIN_HideMug_Trick"),
            NIN_HideMug_Mug = new("NIN_HideMug_Mug");

        internal static UserBoolArray
            NIN_ST_AdvancedMode_Ninjitsus_Options = new("NIN_ST_AdvancedMode_Ninjitsus_Options"),
            NIN_ST_AdvancedMode_Raiton_Options = new("NIN_ST_AdvancedMode_Raiton_Options"),
            NIN_ST_AdvancedMode_TenChiJin_Options = new("NIN_ST_AdvancedMode_TenChiJin_Options"),
            NIN_AoE_AdvancedMode_Ninjitsus_Options = new("NIN_AoE_AdvancedMode_Ninjitsus_Options"),
            NIN_AoE_AdvancedMode_Katon_Options = new("NIN_AoE_AdvancedMode_Katon_Options"),
            NIN_AoE_AdvancedMode_TenChiJin_Options = new("NIN_AoE_AdvancedMode_TenChiJin_Options"),
            NIN_MudraProtection_Options = new("NIN_MudraProtection_Options");
        
        internal static UserFloat
            NIN_AoE_AdvancedMode_Doton_TimeStill = new("NIN_AoE_AdvancedMode_Doton_TimeStill");
        #endregion

        internal static void Draw(Preset preset)
        {
            switch (preset)
            {
                #region ST
                case Preset.NIN_ST_AdvancedMode:
                    DrawAdditionalBoolChoice(NIN_ST_AdvancedMode_TrueNorth, "Dynamic True North",
                        "Dynamic choice of combo finisher based on position and available charges.\nGo to Flank to build charges, Rear to spend them. \nPrevents overcap or waste and will use true north as needed.");
                    DrawSliderInt(0, 10, NIN_ST_AdvancedMode_BurnKazematoi, $"Focus on using {AeolianEdge.ActionName()} to use Kazematoi when target is below Health %");
                    break;

                case Preset.NIN_ST_AdvancedMode_BalanceOpener:
                    DrawRadioButton(NIN_Adv_Opener_Selection, $"Standard Opener - 4th GCD {KunaisBane.ActionName()}", "", 0);
                    DrawRadioButton(NIN_Adv_Opener_Selection, $"Standard Opener - 3rd GCD {Dokumori.ActionName()}", "", 1);
                    DrawRadioButton(NIN_Adv_Opener_Selection, $"Standard Opener - 3rd GCD {KunaisBane.ActionName()}", "", 2);

                    DrawBossOnlyChoice(NIN_Balance_Content);
                    break;

                case Preset.NIN_ST_AdvancedMode_Mug:
                    DrawSliderInt(0, 100, NIN_ST_AdvancedMode_Mug_Threshold,
                        $"Stop using on targets below this HP % (0% = always use, 100% = never use).");
                    ImGui.Indent();
                    ImGui.TextColored(ImGuiColors.DalamudYellow, "Select what kind of enemies the HP check should be applied to:");
                    DrawHorizontalRadioButton(NIN_ST_AdvancedMode_Mug_SubOption,
                        "Non-boss Encounters Only", $"Applies HP check to Non-Boss Encounters only", 0);
                    DrawHorizontalRadioButton(NIN_ST_AdvancedMode_Mug_SubOption,
                        "All Content", $"Applies HP Check to All Content", 1);
                    ImGui.Unindent();
                    break;

                case Preset.NIN_ST_AdvancedMode_TrickAttack:
                    DrawSliderInt(0, 100, NIN_ST_AdvancedMode_TrickAttack_Threshold,
                        $"Stop using on targets below this HP % (0% = always use, 100% = never use).");
                    ImGui.Indent();
                    ImGui.TextColored(ImGuiColors.DalamudYellow, "Select what kind of enemies the HP check should be applied to:");
                    DrawHorizontalRadioButton(NIN_ST_AdvancedMode_TrickAttack_SubOption,
                        "Non-boss Encounters Only", $"Applies HP check to Non-Boss Encounters only", 0);
                    DrawHorizontalRadioButton(NIN_ST_AdvancedMode_TrickAttack_SubOption,
                        "All Content", $"Applies HP Check to All Content", 1);
                    ImGui.Unindent();
                    break;


                case Preset.NIN_ST_AdvancedMode_Ninjitsus:
                    DrawHorizontalMultiChoice(NIN_ST_AdvancedMode_Ninjitsus_Options, "Raiton/Fuma Shuriken",
                        "Adds Raiton to the rotation, will use Fuma Shuriken if below level", 3, 0);
                    DrawHorizontalMultiChoice(NIN_ST_AdvancedMode_Ninjitsus_Options, "Suiton",
                        "Adds Suiton to the rotation when trick attack cooldown will be ready.", 3, 1);
                    DrawHorizontalMultiChoice(NIN_ST_AdvancedMode_Ninjitsus_Options, "Hyosho Ranryu",
                        "Adds Hyosho Ranryu to the rotation when you have Kassatsu", 3, 2);
                    if (NIN_ST_AdvancedMode_Ninjitsus_Options[0])
                    {
                        DrawHorizontalMultiChoice(NIN_ST_AdvancedMode_Raiton_Options, "Raiton Pooling",
                            "Will Pool the charges, saving them for Trick Window", 2, 0);
                        DrawHorizontalMultiChoice(NIN_ST_AdvancedMode_Raiton_Options, "Raiton Uptime",
                            "Will Use Raiton when out of Melee range of the target, " +
                            "\nThis can negatively affect your burst windows", 2, 1);
                    }
                    if (NIN_ST_AdvancedMode_Ninjitsus_Options[1])
                    {
                        DrawSliderInt(0, 21, NIN_ST_AdvancedMode_SuitonSetup,
                            "Set the amount of time remaining on Trick Attack cooldown before trying to set up with Suiton.");
                    }
                    break;

                case Preset.NIN_ST_AdvancedMode_TenChiJin:
                    DrawHorizontalMultiChoice(NIN_ST_AdvancedMode_TenChiJin_Options, "Auto TCJ Option",
                        "Will automatically Fuma Shuriken then Raiton then Suiton", 2, 0);
                    DrawHorizontalMultiChoice(NIN_ST_AdvancedMode_TenChiJin_Options, "TenriJindo Option",
                        "Will weave followup OGCD, TenriJindo, when available", 2, 1);
                    break;

                case Preset.NIN_ST_AdvancedMode_Bhavacakra:
                    DrawAdditionalBoolChoice(NIN_ST_AdvancedMode_Bhavacakra_Pooling, "Bhavacakra Pooling", "Will pool Ninki for the buff windows, while preventing overcap");
                    break;

                case Preset.NIN_ST_AdvancedMode_Raiju:
                    DrawAdditionalBoolChoice(NIN_ST_AdvancedMode_ForkedRaiju, "Forked Raiju", "Allows the Use of forked Raiju instead of Fleeting if out of melee range. (Gap Closer)");
                    break;

                case Preset.NIN_ST_AdvancedMode_SecondWind:
                    DrawSliderInt(0, 100, NIN_ST_AdvancedMode_SecondWindThreshold,
                        "Set a HP% threshold for when Second Wind will be used.");
                    break;

                case Preset.NIN_ST_AdvancedMode_ShadeShift:
                    DrawSliderInt(0, 100, NIN_ST_AdvancedMode_ShadeShiftThreshold,
                        "Set a HP% threshold for when Shade Shift will be used.");
                    DrawAdditionalBoolChoice(NIN_ST_AdvancedMode_ShadeShiftRaidwide, "Raidwide Option", "Use Shade Shift when Raidwide casting is detected regardless of health");
                    break;

                case Preset.NIN_ST_AdvancedMode_Bloodbath:
                    DrawSliderInt(0, 100, NIN_ST_AdvancedMode_BloodbathThreshold,
                        "Set a HP% threshold for when Bloodbath will be used.");
                    break;
                #endregion

                #region AoE
                case Preset.NIN_AoE_AdvancedMode_Ninjitsus:
                    DrawHorizontalMultiChoice(NIN_AoE_AdvancedMode_Ninjitsus_Options, "Katon/Fuma Shuriken",
                        "Adds Katon to the rotation, will use Fuma Shuriken if below level", 4, 0);
                    DrawHorizontalMultiChoice(NIN_AoE_AdvancedMode_Ninjitsus_Options, "Huton",
                        "Adds Huton to the rotation when trick attack cooldown will be ready.", 4, 1);
                    DrawHorizontalMultiChoice(NIN_AoE_AdvancedMode_Ninjitsus_Options, "Doton",
                        "Adds Doton to the rotation when you dont current have one down.", 4, 2);
                    DrawHorizontalMultiChoice(NIN_AoE_AdvancedMode_Ninjitsus_Options, "Goka Mekkyaku",
                        "Adds Goka Mekkyaku to the rotation when you have Kassatsu", 4, 3);
                    if (NIN_ST_AdvancedMode_Ninjitsus_Options[0])
                    {
                        DrawHorizontalMultiChoice(NIN_AoE_AdvancedMode_Katon_Options, "Katon Pooling",
                            "Will Pool the charges, saving them for Trick Window", 2, 0);
                        DrawHorizontalMultiChoice(NIN_AoE_AdvancedMode_Katon_Options, "Katon Uptime",
                            "Will Use Katon when out of Melee range of the target, " +
                            "\nThis can negatively affect your burst windows", 2, 1);
                    }
                    if (NIN_ST_AdvancedMode_Ninjitsus_Options[1])
                    {
                        DrawSliderInt(0, 21, NIN_AoE_AdvancedMode_HutonSetup,
                            "Set the amount of time remaining on Trick Attack cooldown before trying to set up with Huton.");
                    }
                    if (NIN_AoE_AdvancedMode_Ninjitsus_Options[2])
                    {
                        DrawSliderInt(0, 100, NIN_AoE_AdvancedMode_Doton_Threshold,
                            "Sets the max remaining HP percentage of the current target to cast Doton.");
                        ImGui.Indent(); DrawSliderFloat(0, 3, NIN_AoE_AdvancedMode_Doton_TimeStill,"How Long Standing still before using Doton (in seconds):", decimals: 1);
                        
                        ImGui.Unindent();
                    }
                    break;

                case Preset.NIN_AoE_AdvancedMode_Mug:
                    DrawSliderInt(0, 100, NIN_AoE_AdvancedMode_Mug_Threshold,
                        $"Stop using on targets below this HP % (0% = always use, 100% = never use).");
                    ImGui.Indent();
                    ImGui.TextColored(ImGuiColors.DalamudYellow, "Select what kind of enemies the HP check should be applied to:");
                    DrawHorizontalRadioButton(NIN_AoE_AdvancedMode_Mug_SubOption,
                        "Non-boss Encounters Only", $"Applies HP check to Non-Boss Encounters only", 0);
                    DrawHorizontalRadioButton(NIN_AoE_AdvancedMode_Mug_SubOption,
                        "All Content", $"Applies HP Check to All Content", 1);
                    ImGui.Unindent();
                    break;

                case Preset.NIN_AoE_AdvancedMode_TrickAttack:
                    DrawSliderInt(0, 100, NIN_AoE_AdvancedMode_TrickAttack_Threshold,
                        $"Stop using on targets below this HP % (0% = always use, 100% = never use).");
                    ImGui.Indent();
                    ImGui.TextColored(ImGuiColors.DalamudYellow, "Select what kind of enemies the HP check should be applied to:");
                    DrawHorizontalRadioButton(NIN_AoE_AdvancedMode_TrickAttack_SubOption,
                        "Non-boss Encounters Only", $"Applies HP check to Non-Boss Encounters only", 0);
                    DrawHorizontalRadioButton(NIN_AoE_AdvancedMode_TrickAttack_SubOption,
                        "All Content", $"Applies HP Check to All Content", 1);
                    ImGui.Unindent();
                    break;


                case Preset.NIN_AoE_AdvancedMode_TenChiJin:
                    DrawHorizontalMultiChoice(NIN_AoE_AdvancedMode_TenChiJin_Options, "Auto TCJ Option",
                        "Will automatically Fuma Shuriken then Raiton then Suiton", 2, 0);
                    DrawHorizontalMultiChoice(NIN_AoE_AdvancedMode_TenChiJin_Options, "TenriJindo Option",
                        "Will weave followup OGCD, TenriJindo, when available", 2, 1);
                    break;

                case Preset.NIN_AoE_AdvancedMode_SecondWind:
                    DrawSliderInt(0, 100, NIN_AoE_AdvancedMode_SecondWindThreshold, "Set a HP% threshold for when Second Wind will be used.");
                    break;

                case Preset.NIN_AoE_AdvancedMode_ShadeShift:
                    DrawSliderInt(0, 100, NIN_AoE_AdvancedMode_ShadeShiftThreshold, "Set a HP% threshold for when Shade Shift will be used.");
                    DrawAdditionalBoolChoice(NIN_AoE_AdvancedMode_ShadeShiftRaidwide, "Raidwide Option", "Use Shade Shift when Raidwide casting is detected regardless of health");
                    break;

                case Preset.NIN_AoE_AdvancedMode_Bloodbath:
                    DrawSliderInt(0, 100, NIN_AoE_AdvancedMode_BloodbathThreshold, "Set a HP% threshold for when Bloodbath will be used.");
                    break;

                case Preset.NIN_AoE_AdvancedMode_HellfrogMedium:
                    DrawAdditionalBoolChoice(NIN_AoE_AdvancedMode_HellfrogMedium_Pooling, "Hellfrog Pooling", "Will pool Ninki for the buff windows, while preventing overcap");
                    break;
                #endregion

                #region Standalone
                case Preset.NIN_Simple_Mudras:
                    DrawRadioButton(NIN_SimpleMudra_Choice, "Mudra Path Set 1",
                        $"1. {Ten.ActionName()} Mudras -> {FumaShuriken.ActionName()}, {Raiton.ActionName()}/{HyoshoRanryu.ActionName()}, {Suiton.ActionName()} " +
                        $"({Doton.ActionName()} under {Kassatsu.ActionName()}).\n{Chi.ActionName()} Mudras -> {FumaShuriken.ActionName()}, {Hyoton.ActionName()}, " +
                        $"{Huton.ActionName()}.\n{Jin.ActionName()} Mudras -> {FumaShuriken.ActionName()}, {Katon.ActionName()}/{GokaMekkyaku.ActionName()}, {Doton.ActionName()}",
                        1);
                    DrawRadioButton(NIN_SimpleMudra_Choice, "Mudra Path Set 2",
                        $"2. {Ten.ActionName()} Mudras -> {FumaShuriken.ActionName()}, {Hyoton.ActionName()}/{HyoshoRanryu.ActionName()}, {Doton.ActionName()}.\n{Chi.ActionName()} " +
                        $"Mudras -> {FumaShuriken.ActionName()}, {Katon.ActionName()}, {Suiton.ActionName()}.\n{Jin.ActionName()} Mudras -> {FumaShuriken.ActionName()}, " +
                        $"{Raiton.ActionName()}/{GokaMekkyaku.ActionName()}, {Huton.ActionName()} ({Doton.ActionName()} under {Kassatsu.ActionName()}).",
                        2);
                    break;


                case Preset.NIN_HideMug:
                    DrawAdditionalBoolChoice(NIN_HideMug_Mug, "Mug", "Adds Mug when in Combat");
                    DrawAdditionalBoolChoice(NIN_HideMug_Toggle, "Hide Quick Toggle", "Instantly toggles off hidden so you can use it to reset mudra cooldown.");
                    ImGui.Indent();
                    if (NIN_HideMug_Toggle)
                    {
                        DrawAdditionalBoolChoice(NIN_HideMug_ToggleLevelCheck, "Level Check", "Will only toggle hidden if you above level 45 and can Suiton for Shadowwalker.");
                    }
                    ImGui.Unindent();
                    DrawAdditionalBoolChoice(NIN_HideMug_Trick, "Add Trick Attack", "Adds Trick Attack when hidden or has Shadowwalker buff.");
                    ImGui.Indent();
                    if (NIN_HideMug_Trick && NIN_HideMug_Mug)
                    {
                        DrawAdditionalBoolChoice(NIN_HideMug_TrickAfterMug, "Mug First", "Will only show Trick Attack if Mug is on cooldown.");
                    }
                    ImGui.Unindent();
                    break;

                
                case Preset.NIN_MudraProtection:
                    DrawHorizontalMultiChoice(NIN_MudraProtection_Options, "Shade Shift", "Replaces with Savage Blade while in Mudra.", 6, 0);
                    DrawHorizontalMultiChoice(NIN_MudraProtection_Options, "Shukuchi", "Replaces with Savage Blade while in Mudra.", 6, 1);
                    DrawHorizontalMultiChoice(NIN_MudraProtection_Options, "Feint", "Replaces with Savage Blade while in Mudra or Current Target already has Feint", 6, 2);
                    DrawHorizontalMultiChoice(NIN_MudraProtection_Options, "Bloodbath", "Replaces with Savage Blade while in Mudra.", 6, 3);
                    DrawHorizontalMultiChoice(NIN_MudraProtection_Options, "Second Wind", "Replaces with Savage Blade while in Mudra.", 6, 4);
                    DrawHorizontalMultiChoice(NIN_MudraProtection_Options, "Leg Sweep", "Replaces with Savage Blade while in Mudra.", 6, 5);
                    break;
                    
                #endregion

            }
        }
    }
}
