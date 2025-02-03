using Dalamud.Interface.Components;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System.Numerics;
using Dalamud.Interface.Colors;
using ECommons.ImGuiMethods;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Services;
using WrathCombo.Window.Functions;

namespace WrathCombo.Window.Tabs
{
    internal class Settings : ConfigWindow
    {
        internal new static void Draw()
        {
            PvEFeatures.HasToOpenJob = true;
            using (ImRaii.Child("main", new Vector2(0, 0), true))
            {
                ImGui.Text("This tab allows you to customise your options for Wrath Combo.");

                #region UI Options

                ImGui.Dummy(new Vector2(20f));
                ImGuiEx.TextUnderlined("Main UI Options");

                #region SubCombos

                var hideChildren = Service.Configuration.HideChildren;

                if (ImGui.Checkbox("Hide SubCombo Options", ref hideChildren))
                {
                    Service.Configuration.HideChildren = hideChildren;
                    Service.Configuration.Save();
                }

                ImGuiComponents.HelpMarker("Hides the sub-options of disabled features.");

                #endregion

                #region Conflicting

                bool hideConflicting = Service.Configuration.HideConflictedCombos;
                if (ImGui.Checkbox("Hide Conflicted Combos", ref hideConflicting))
                {
                    Service.Configuration.HideConflictedCombos = hideConflicting;
                    Service.Configuration.Save();
                }

                ImGuiComponents.HelpMarker("Hides any combos that conflict with others you have selected.");

                #endregion

                #region Shorten DTR bar text

                bool shortDTRText = Service.Configuration.ShortDTRText;

                if (ImGui.Checkbox("Shorten Server Info Bar Text", ref shortDTRText))
                {
                    Service.Configuration.ShortDTRText = shortDTRText;
                    Service.Configuration.Save();
                }

                ImGuiComponents.HelpMarker(
                    "By default, the Server Info Bar for Wrath Combo shows whether Auto-Rotation is on or off, " +
                    "\nthen -if on- it will show how many active Auto-Mode combos you have enabled. " +
                    "\nAnd finally, it will also show if another plugin is controlling that value." +
                    "\nThis option will make the number of active Auto-Mode combos not show."
                );

                #endregion

                #region Message of the Day

                bool motd = Service.Configuration.HideMessageOfTheDay;

                if (ImGui.Checkbox("Hide Message of the Day", ref motd))
                {
                    Service.Configuration.HideMessageOfTheDay = motd;
                    Service.Configuration.Save();
                }

                ImGuiComponents.HelpMarker("Disables the Message of the Day message in your chat when you login.");

                #endregion

                #region TargetHelper

                Vector4 colour = Service.Configuration.TargetHighlightColor;
                if (ImGui.ColorEdit4("Target Highlight Colour", ref colour, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.AlphaPreview | ImGuiColorEditFlags.AlphaBar))
                {
                    Service.Configuration.TargetHighlightColor = colour;
                    Service.Configuration.Save();
                }

                ImGuiComponents.HelpMarker("Draws a box around party members in the vanilla Party List, as targeted by certain features.\nSet Alpha to 0 to hide the box.");

                ImGui.SameLine();
                ImGui.TextColored(ImGuiColors.DalamudGrey, $"(Only used by {CustomComboFunctions.JobIDs.JobIDToName(33)} currently)");

                #endregion

                #endregion

                #region Rotation Behavior Options

                ImGui.Dummy(new Vector2(20f));
                ImGuiEx.TextUnderlined("Rotation Behavior Options");

                #region Performance Mode

                if (ImGui.Checkbox("Performance Mode", ref Service.Configuration.PerformanceMode))
                    Service.Configuration.Save();

                ImGuiComponents.HelpMarker("This mode will disable actions being changed on your hotbar, but will still continue to work in the background as you press your buttons.");

                #endregion

                #region Spells while Moving

                if (ImGui.Checkbox("Block spells if moving", ref Service.Configuration.BlockSpellOnMove))
                    Service.Configuration.Save();

                ImGuiComponents.HelpMarker("Completely blocks spells from being used if you are moving.\nThis would supersede combo-specific movement options, available for most jobs.");

                #endregion

                #region Movement Check Delay

                ImGui.PushItemWidth(75);
                if (ImGui.InputFloat("seconds    -    Movement Check Delay", ref Service.Configuration.MovementLeeway))
                    Service.Configuration.Save();

                ImGuiComponents.HelpMarker("Many features check if you are moving to decide actions, this will allow you to set a delay on how long you need to be moving before it recognizes you as moving.\nThis allows you to not have to worry about small movements affecting your rotation, primarily for casters.\n\nIt is recommended to keep this value between 0 and 1 seconds.");

                #endregion

                #region Opener Failure Timeout

                if (ImGui.InputFloat("seconds    -    Opener Failure Timeout", ref Service.Configuration.OpenerTimeout))
                    Service.Configuration.Save();

                ImGuiComponents.HelpMarker("During an opener, if this amount of time has passed since your last action, it will fail the opener and resume with non-opener functionality.");

                #endregion

                #region Melee Offset
                var offset = (float)Service.Configuration.MeleeOffset;

                if (ImGui.InputFloat("yalms    -    Melee Distance Offset", ref offset))
                {
                    Service.Configuration.MeleeOffset = (double)offset;
                    Service.Configuration.Save();
                }

                ImGuiComponents.HelpMarker("Offset of melee check distance.\nFor those who don't want to immediately use their ranged attack if the boss walks slightly out of range.\n\nFor example, a value of -0.5 would make you have to be 0.5 yalms closer to the target,\nor a value of 2 would disable triggering of ranged features until you are 2 yalms further from the hitbox.\n\nIt is recommended to keep this value at 0.");
                #endregion

                #region Interrupt Delay

                var delay = (int)(Service.Configuration.InterruptDelay * 100d);

                if (ImGui.SliderInt("percent of cast    -    Interrupt Delay",
                    ref delay, 0, 100))
                {
                    delay = delay.RoundOff(SliderIncrements.Fives);

                    Service.Configuration.InterruptDelay = ((double)delay) / 100d;
                    Service.Configuration.Save();
                }

                ImGuiComponents.HelpMarker("The percentage of a total cast time to wait before interrupting.\nApplies to all interrupts, in every job's combos.\n\nIt is recommend to keep this value below 50%.");

                #endregion

                #endregion

                #region Logging Options

                ImGui.Dummy(new Vector2(20f));
                ImGuiEx.TextUnderlined("Troubleshooting / Analysis Options");

                #region Combat Log

                bool showCombatLog = Service.Configuration.EnabledOutputLog;

                if (ImGui.Checkbox("Output Log to Chat", ref showCombatLog))
                {
                    Service.Configuration.EnabledOutputLog = showCombatLog;
                    Service.Configuration.Save();
                }

                ImGuiComponents.HelpMarker("Every time you use an action, the plugin will print it to the chat.");
                #endregion

                #region Opener Log

                if (ImGui.Checkbox($"Output opener status to chat", ref Service.Configuration.OutputOpenerLogs))
                    Service.Configuration.Save();

                ImGuiComponents.HelpMarker("Every time your class's opener ir ready, fails, or finishes as expected, it will print to the chat.");
                #endregion

                #endregion
            }
        }
    }
}
