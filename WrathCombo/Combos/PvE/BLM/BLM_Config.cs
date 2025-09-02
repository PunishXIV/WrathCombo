using Dalamud.Interface.Colors;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using static WrathCombo.Window.Functions.UserConfig;
namespace WrathCombo.Combos.PvE;

internal partial class BLM
{
    internal static class Config
    {
        internal static void Draw(Preset preset)
        {
            switch (preset)
            {
                case Preset.BLM_ST_Opener:
                    DrawHorizontalRadioButton(BLM_SelectedOpener,
                        "Standard opener", "Uses Standard opener",
                        0);

                    DrawHorizontalRadioButton(BLM_SelectedOpener,
                        $"{Flare.ActionName()} opener", $"Uses {Flare.ActionName()} opener",
                        1);

                    DrawBossOnlyChoice(BLM_Balance_Content);
                    break;

                case Preset.BLM_ST_LeyLines:

                    DrawHorizontalRadioButton(BLM_ST_LeyLinesMovement,
                        "Stationary Only", "Uses Leylines only while stationary", 0);

                    DrawHorizontalRadioButton(BLM_ST_LeyLinesMovement,
                        "Any Movement", "Uses Leylines regardless of any movement conditions.\nNOTE: This could possibly get you killed", 1);

                    ImGui.Spacing();
                    if (BLM_ST_LeyLinesMovement == 0)
                    {
                        ImGui.SetCursorPosX(48);
                        DrawSliderFloat(0, 3, BLM_ST_LeyLinesTimeStill,
                            " Stationary Delay Check (in seconds):", decimals: 1);
                    }

                    ImGui.SetCursorPosX(48);
                    DrawSliderInt(0, 2, BLM_ST_LeyLinesCharges,
                        " How many charges to keep ready?\n (0 = Use All)");

                    DrawSliderInt(0, 50, BLM_ST_LeyLinesHPOption,
                        "Stop using at Enemy HP %. Set to Zero to disable this check.");
                    ImGui.Indent();

                    DrawHorizontalRadioButton(BLM_ST_LeyLinesBossOption,
                        "Non-Bosses", "Only applies the HP check above to non-bosses.", 0);

                    DrawHorizontalRadioButton(BLM_ST_LeyLinesBossOption,
                        "All Enemies", "Applies the HP check above to all enemies.", 1);
                    break;

                case Preset.BLM_ST_Movement:
                    DrawHorizontalMultiChoice(BLM_ST_MovementOption,
                        $"Use {Triplecast.ActionName()}", "", 4, 0);

                    DrawPriorityInput(BLM_ST_Movement_Priority,
                        4, 0, $"{Triplecast.ActionName()} Priority: ");

                    DrawHorizontalMultiChoice(BLM_ST_MovementOption,
                        $"Use {Paradox.ActionName()}", "", 4, 1);

                    DrawPriorityInput(BLM_ST_Movement_Priority,
                        4, 1, $"{Paradox.ActionName()} Priority: ");

                    DrawHorizontalMultiChoice(BLM_ST_MovementOption,
                        $"Use {Role.Swiftcast.ActionName()}", "", 4, 2);

                    DrawPriorityInput(BLM_ST_Movement_Priority,
                        4, 2, $"{Role.Swiftcast.ActionName()} Priority: ");

                    DrawHorizontalMultiChoice(BLM_ST_MovementOption,
                        $"Use {Foul.ActionName()} / {Xenoglossy.ActionName()}", "", 4, 3);

                    DrawPriorityInput(BLM_ST_Movement_Priority,
                        4, 3, $"{Xenoglossy.ActionName()} Priority: ");
                    break;

                case Preset.BLM_ST_UsePolyglot:
                    if (DrawSliderInt(0, 3, BLM_ST_Polyglot_Save,
                        "How many charges to save for manual use?"))
                        if (BLM_ST_Polyglot_Movement > 3 - BLM_ST_Polyglot_Save)
                            BLM_ST_Polyglot_Movement.Value = 3 - BLM_ST_Polyglot_Save;

                    if (DrawSliderInt(0, 3, BLM_ST_Polyglot_Movement,
                        "How many charges to save for movement?"))
                        if (BLM_ST_Polyglot_Save > 3 - BLM_ST_Polyglot_Movement)
                            BLM_ST_Polyglot_Save.Value = 3 - BLM_ST_Polyglot_Movement;
                    break;

                case Preset.BLM_ST_Triplecast:
                    DrawHorizontalRadioButton(BLM_ST_Triplecast_WhenToUse,
                        "Always", "Use always.", 0);

                    DrawHorizontalRadioButton(BLM_ST_Triplecast_WhenToUse,
                        "Not under Leylines", "Do not use while under the effect of Leylines.\nThis is the recommended behaviour.", 1);

                    if (BLM_ST_MovementOption[0])
                        DrawSliderInt(1, 2, BLM_ST_Triplecast_MovementCharges,
                            "How many charges to save for movement?");
                    break;


                case Preset.BLM_ST_Thunder:

                    DrawSliderInt(0, 50, BLM_ST_ThunderHPOption,
                        "Stop using at Enemy HP %. Set to Zero to disable this check.");

                    ImGui.Indent();

                    ImGui.TextColored(ImGuiColors.DalamudYellow,
                        "Select what kind of enemies the HP check should be applied to:");

                    DrawHorizontalRadioButton(BLM_ST_ThunderBossOption,
                        "Non-Bosses", "Only applies the HP check above to non-bosses.\nAllows you to only stop DoTing early when it's not a boss.", 0);

                    DrawHorizontalRadioButton(BLM_ST_ThunderBossOption,
                        "All Enemies", "Applies the HP check above to all enemies.", 1);

                    DrawSliderInt(0, 5, BLM_ST_ThunderRefresh,
                        "Seconds remaining before reapplying the DoT. Set to Zero to disable this check.");

                    ImGui.Unindent();
                    break;

                case Preset.BLM_ST_Manaward:
                    DrawSliderInt(0, 100, BLM_ST_Manaward_Threshold,
                        $"{Manaward.ActionName()} HP percentage threshold");
                    break;

                case Preset.BLM_AoE_LeyLines:

                    DrawHorizontalRadioButton(BLM_AoE_LeyLinesMovement,
                        "Stationary Only", "Uses Leylines only while stationary", 0);

                    DrawHorizontalRadioButton(BLM_AoE_LeyLinesMovement,
                        "Any Movement", "Uses Leylines regardless of any movement conditions.\nNOTE: This could possibly get you killed", 1);

                    ImGui.Spacing();
                    if (BLM_AoE_LeyLinesMovement == 0)
                    {
                        ImGui.SetCursorPosX(48);
                        DrawSliderFloat(0, 3, BLM_AoE_LeyLinesTimeStill,
                            " Stationary Delay Check (in seconds):", decimals: 1);
                    }

                    ImGui.SetCursorPosX(48);
                    DrawSliderInt(0, 2, BLM_AoE_LeyLinesCharges,
                        " How many charges to keep ready?\n (0 = Use All)");

                    DrawSliderInt(0, 50, BLM_AoE_LeyLinesOption,
                        "Stop using at Enemy HP %. Set to Zero to disable this check.");
                    break;

                case Preset.BLM_AoE_Triplecast:
                    DrawSliderInt(0, 1, BLM_AoE_Triplecast_HoldCharges,
                        $"How many charges of {Triplecast.ActionName()} to keep ready? (0 = Use all)");
                    break;

                case Preset.BLM_AoE_Thunder:
                    DrawSliderInt(0, 50, BLM_AoE_ThunderHP,
                        $"Stop Using {Thunder2.ActionName()} When Target HP% is at or Below (Set to 0 to Disable This Check)");
                    break;

                case Preset.BLM_Retargetting_Aetherial_Manipulation:
                    DrawAdditionalBoolChoice(BLM_AM_FieldMouseover,
                        "Add Field Mouseover", "Adds Field mouseover targetting.");
                    break;


                case Preset.BLM_Fire1and3:
                    DrawRadioButton(BLM_F1to3,
                        $"Replaces {Fire.ActionName()}", $"Replaces {Fire.ActionName()} with {Fire3.ActionName()} when out of Astral Fire III or not in combat.", 0);

                    if (BLM_F1to3 == 0)
                    {
                        DrawAdditionalBoolChoice(BLM_Fire1_Despair,
                            "Despair", "Adds Despair when in Astral Fire and below 2400 MP.");
                    }

                    DrawRadioButton(BLM_F1to3,
                        $"Replaces {Fire3.ActionName()}", $"Replaces {Fire3.ActionName()} with {Fire.ActionName()} when in Astral Fire III.", 1);
                    break;

                case Preset.BLM_Fire4:
                    DrawAdditionalBoolChoice(BLM_Fire4_FlareStar,
                        "Flarestar", "Adds Flarestar in Astral Fire when ready.");

                    DrawAdditionalBoolChoice(BLM_Fire4_Fire3,
                        "Fire I / III", "Adds Fire I / III when in Astral Fire and stack is less than 3");

                    DrawRadioButton(BLM_Fire4_FireAndIce,
                        "Use Ice in Umbral Ice", "Adds Blizzard I / III / IV in Umbral Ice depending on stacks and level", 0);

                    DrawRadioButton(BLM_Fire4_FireAndIce,
                        "Use Fire in Umbral Ice", "Leaves Fire in Umbral Ice \nWill Use Fire I / III if selected above.", 1);
                    break;

                case Preset.BLM_Flare:
                    DrawAdditionalBoolChoice(BLM_Flare_FlareStar,
                        "Flarestar", "Adds Flarestar in Astral Fire when ready.");
                    break;

                case Preset.BLM_Blizzard1and3:
                    DrawRadioButton(BLM_B1to3,
                        $"Replaces {Blizzard.ActionName()}",
                        $"Replaces {Blizzard.ActionName()} with {Blizzard3.ActionName()} when out of Umbral Ice III.", 0);

                    DrawRadioButton(BLM_B1to3,
                        $"Replaces {Blizzard3.ActionName()}",
                        $"Replaces {Blizzard3.ActionName()} with {Blizzard.ActionName()} when in Umbral Ice III.", 1);
                    if (BLM_B1to3 == 1)
                    {
                        DrawAdditionalBoolChoice(BLM_Blizzard3_Despair,
                            "Despair", "Adds Despair when in Astral Fire and above 800 MP.");
                    }
                    break;

                case Preset.BLM_AmplifierXeno:
                    DrawAdditionalBoolChoice(BLM_AmplifierXenoCD,
                        "Show Xenoglossy when Amplifier is on cooldown", "Makes it so that Xenoglossy also shows when Amplifier is on cooldown.");
                    break;
            }
        }

        #region Variables

        public static UserInt
            BLM_SelectedOpener = new("BLM_SelectedOpener", 0),
            BLM_Balance_Content = new("BLM_Balance_Content", 1),
            BLM_ST_LeyLinesCharges = new("BLM_ST_LeyLinesCharges", 1),
            BLM_ST_LeyLinesMovement = new("BLM_ST_LeyLinesMovement", 0),
            BLM_ST_LeyLinesHPOption = new("BLM_ST_LeyLinesOption", 25),
            BLM_ST_LeyLinesBossOption = new("BLM_ST_LeyLinesSubOption", 0),
            BLM_ST_ThunderHPOption = new("BLM_ST_ThunderOption", 10),
            BLM_ST_ThunderBossOption = new("BLM_ST_Thunder_SubOption", 0),
            BLM_ST_Triplecast_WhenToUse = new("BLM_ST_Triplecast_WhenToUse", 1),
            BLM_ST_ThunderRefresh = new("BLM_ST_ThunderUptime_Threshold", 5),
            BLM_ST_Triplecast_MovementCharges = new("BLM_ST_Triplecast_MovementCharges", 1),
            BLM_ST_Polyglot_Movement = new("BLM_ST_Polyglot_Movement", 1),
            BLM_ST_Polyglot_Save = new("BLM_ST_Polyglot_Save", 0),
            BLM_ST_Manaward_Threshold = new("BLM_ST_Manaward_Threshold", 40),
            BLM_AoE_Triplecast_HoldCharges = new("BLM_AoE_Triplecast_HoldCharges", 0),
            BLM_AoE_LeyLinesCharges = new("BLM_AoE_LeyLinesCharges", 0),
            BLM_AoE_LeyLinesMovement = new("BLM_AoE_LeyLinesMovement", 0),
            BLM_AoE_LeyLinesOption = new("BLM_AoE_LeyLinesOption", 40),
            BLM_AoE_ThunderHP = new("BLM_AoE_ThunderHP", 20),
            BLM_B1to3 = new("BLM_B1to3", 0),
            BLM_B4toDespair = new("BLM_B4toDespair", 0),
            BLM_F1to3 = new("BLM_F1to3", 0),
            BLM_Fire4_FireAndIce = new("BLM_Fire4_FireAndIce", 0);

        public static UserFloat
            BLM_ST_LeyLinesTimeStill = new("BLM_ST_LeyLinesTimeStill", 2.5f),
            BLM_AoE_LeyLinesTimeStill = new("BLM_AoE_LeyLinesTimeStill", 2.5f);

        public static UserBool
            BLM_AM_FieldMouseover = new("BLM_AM_FieldMouseover"),
            BLM_AmplifierXenoCD = new("BLM_AmplifierXenoCD"),
            BLM_Fire4_FlareStar = new("BLM_Fire4_FlareStar"),
            BLM_Flare_FlareStar = new("BLM_Flare_FlareStar"),
            BLM_Fire1_Despair = new("BLM_Fire1_Despair"),
            BLM_Blizzard3_Despair = new("BLM_Blizzard3_Despair"),
            BLM_Fire4_Fire3 = new("BLM_Fire4_Fire3");

        public static UserBoolArray
            BLM_ST_MovementOption = new("BLM_ST_MovementOption");

        public static UserIntArray
            BLM_ST_Movement_Priority = new("BLM_ST_Movement_Priority");

        #endregion
    }
}
