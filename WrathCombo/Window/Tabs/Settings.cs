using System;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System.Numerics;
using Dalamud.Interface.Colors;
using ECommons.GameHelpers;
using ECommons.ImGuiMethods;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Services;
using WrathCombo.Window.Functions;
using ECommons.DalamudServices;

namespace WrathCombo.Window.Tabs
{
    internal class Settings : ConfigWindow
    {
        internal new static void Draw()
        {
            using (ImRaii.Child("main", new Vector2(0, 0), true))
            {
                ImGui.Text("This tab allows you to customise your options for Wrath Combo.");

                #region UI Options

                ImGuiEx.Spacing(new Vector2(0, 20));
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

                #region Open to Current Job

                if (ImGui.Checkbox("Open PvE Features UI to Current Job on Opening", ref Service.Configuration.OpenToCurrentJob))
                    Service.Configuration.Save();

                ImGuiComponents.HelpMarker("When you open Wrath's UI, it will automatically open to the job you are currently playing.");

                if (ImGui.Checkbox("Open PvE Features UI to Current Job on Switching Jobs", ref Service.Configuration.OpenToCurrentJobOnSwitch))
                    Service.Configuration.Save();

                ImGuiComponents.HelpMarker("When you switch jobs, it will automatically open to the job you are currently playing.");

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

                ImGuiEx.Spacing(new Vector2(0, 20));
                ImGuiEx.TextUnderlined("Rotation Behavior Options");


                #region Performance Mode

                if (ImGui.Checkbox("Performance Mode", ref Service.Configuration.PerformanceMode))
                    Service.Configuration.Save();

                ImGuiComponents.HelpMarker("This mode will disable actions being changed on your hotbar, but will still continue to work in the background as you press your buttons.");

                #endregion

                #region Spells while Moving

                if (ImGui.Checkbox("Block spells if moving", ref Service.Configuration.BlockSpellOnMove))
                    Service.Configuration.Save();

                ImGuiComponents.HelpMarker("Completely blocks spells from being used if you are moving, by replacing your actions with Savage Blade.\nThis would supersede combo-specific movement options, available for most jobs.");

                #endregion

                #region Action Changing

                if (ImGui.Checkbox("Action Replacing", ref Service.Configuration.ActionChanging))
                    Service.Configuration.Save();

                ImGuiComponents.HelpMarker("Controls whether Actions will be Intercepted Replaced with combos from the plugin.\nIf disabled, your manual presses of abilities will no longer be affected by your Wrath settings.\n\nAuto-Rotation will work regardless of the setting.\n\nControlled by /wrath combo");

                #endregion

                #region Throttle

                var len = ImGui.CalcTextSize("milliseconds").X;

                ImGui.PushItemWidth(75);
                var throttle = Service.Configuration.Throttle;
                if (ImGui.InputInt("###ActionThrottle",
                        ref throttle, 0, 0))
                {
                    Service.Configuration.Throttle = Math.Clamp(throttle, 0, 1500);
                    Service.Configuration.Save();
                }

                ImGui.SameLine();
                var pos = ImGui.GetCursorPosX() + len;
                ImGui.Text($"milliseconds");
                ImGui.SameLine(pos);
                ImGui.Text($"   -   Action Updater Throttle");


                ImGuiComponents.HelpMarker(
                    "This is the restriction for how often combos will update the action on your hotbar." +
                    "\nBy default this isn't restricting the combos, so you always get an up-to-date action." +
                    "\n\nIf you have minor FPS issues, you can increase this value to make combos run less often." +
                    "\nThis makes your combos less responsive, and perhaps even clips GCDs." +
                    "\nAt high values, this can break your rotation entirely." +
                    "\nMore severe FPS issues should instead be handled with Performance Mode option above." +
                    "\n\n200ms can make a reasonable difference in your FPS." +
                    "\nValues over 500ms are NOT recommended.");

                #endregion

                #region Movement Check Delay

                ImGui.PushItemWidth(75);
                if (ImGui.InputFloat("###MovementLeeway", ref Service.Configuration.MovementLeeway))
                    Service.Configuration.Save();

                ImGui.SameLine();
                ImGui.Text("seconds");

                ImGui.SameLine(pos);

                ImGui.Text($"   -   Movement Check Delay");

                ImGuiComponents.HelpMarker("Many features check if you are moving to decide actions, this will allow you to set a delay on how long you need to be moving before it recognizes you as moving.\nThis allows you to not have to worry about small movements affecting your rotation, primarily for casters.\n\nIt is recommended to keep this value between 0 and 1 seconds.");

                #endregion

                #region Opener Failure Timeout

                if (ImGui.InputFloat("###OpenerTimeout", ref Service.Configuration.OpenerTimeout))
                    Service.Configuration.Save();

                ImGui.SameLine();
                ImGui.Text("seconds");

                ImGui.SameLine(pos);

                ImGui.Text($"   -   Opener Failure Timeout");

                ImGuiComponents.HelpMarker("During an opener, if this amount of time has passed since your last action, it will fail the opener and resume with non-opener functionality.");

                #endregion

                #region Melee Offset
                var offset = (float)Service.Configuration.MeleeOffset;

                if (ImGui.InputFloat("###MeleeOffset", ref offset))
                {
                    Service.Configuration.MeleeOffset = (double)offset;
                    Service.Configuration.Save();
                }

                ImGui.SameLine();
                ImGui.Text($"yalms");
                ImGui.SameLine(pos);

                ImGui.Text($"   -   Melee Distance Offset");

                ImGuiComponents.HelpMarker("Offset of melee check distance.\nFor those who don't want to immediately use their ranged attack if the boss walks slightly out of range.\n\nFor example, a value of -0.5 would make you have to be 0.5 yalms closer to the target,\nor a value of 2 would disable triggering of ranged features until you are 2 yalms further from the hitbox.\n\nIt is recommended to keep this value at 0.");
                #endregion

                #region Interrupt Delay

                var delay = (int)(Service.Configuration.InterruptDelay * 100d);

                if (ImGui.SliderInt("###InterruptDelay",
                    ref delay, 0, 100))
                {
                    delay = delay.RoundOff(SliderIncrements.Fives);

                    Service.Configuration.InterruptDelay = ((double)delay) / 100d;
                    Service.Configuration.Save();
                }
                ImGui.SameLine();
                ImGui.Text($"%% of cast");
                ImGui.SameLine( pos);
                ImGui.Text($"   -   Interrupt Delay");

                ImGuiComponents.HelpMarker("The percentage of a total cast time to wait before interrupting.\nApplies to all interrupts, in every job's combos.\n\nIt is recommend to keep this value below 50%.");

                #endregion

                #endregion

                #region Troubleshooting Options

                ImGuiEx.Spacing(new Vector2(0, 20));
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

                ImGuiComponents.HelpMarker("Every time your class's opener is ready, fails, or finishes as expected, it will print to the chat.");
                #endregion

                #region Debug File

                if (ImGui.Button("Create Debug File"))
                {
                    if (Player.Available)
                        DebugFile.MakeDebugFile();
                    else
                        DebugFile.MakeDebugFile(allJobs:true);
                }

                ImGuiComponents.HelpMarker("Will generate a debug file on your desktop.\nUseful to give developers to help troubleshoot issues.\nThe same as using the following command: /wrath debug");

                #endregion

                #endregion
            }
        }
    }
}
