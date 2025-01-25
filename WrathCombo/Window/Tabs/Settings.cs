using Dalamud.Interface.Components;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System.Numerics;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Services;

namespace WrathCombo.Window.Tabs
{
    internal class Settings : ConfigWindow
    {
        internal static new void Draw()
        {
            PvEFeatures.HasToOpenJob = true;
            using (var child = ImRaii.Child("main", new Vector2(0, 0), true))
            {
                ImGui.Text("This tab allows you to customise your options when enabling features.");

                #region SubCombos

                bool hideChildren = Service.Configuration.HideChildren;

                if (ImGui.Checkbox("Hide SubCombo Options", ref hideChildren))
                {
                    Service.Configuration.HideChildren = hideChildren;
                    Service.Configuration.Save();
                }

                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted("Hides the sub-options of disabled features.");
                    ImGui.EndTooltip();
                }
                ImGui.NextColumn();

                #endregion

                #region Conflicting

                bool hideConflicting = Service.Configuration.HideConflictedCombos;
                if (ImGui.Checkbox("Hide Conflicted Combos", ref hideConflicting))
                {
                    Service.Configuration.HideConflictedCombos = hideConflicting;
                    Service.Configuration.Save();
                }

                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted("Hides any combos that conflict with others you have selected.");
                    ImGui.EndTooltip();
                }

                #endregion

                #region Combat Log

                bool showCombatLog = Service.Configuration.EnabledOutputLog;

                if (ImGui.Checkbox("Output Log to Chat", ref showCombatLog))
                {
                    Service.Configuration.EnabledOutputLog = showCombatLog;
                    Service.Configuration.Save();
                }

                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted("Every time you use an action, the plugin will print it to the chat.");
                    ImGui.EndTooltip();
                }
                #endregion

                #region Melee Offset
                float offset = (float)Service.Configuration.MeleeOffset;
                ImGui.PushItemWidth(75);

                bool inputChangedeth = false;
                inputChangedeth |= ImGui.InputFloat("Melee Distance Offset", ref offset);

                if (inputChangedeth)
                {
                    Service.Configuration.MeleeOffset = (double)offset;
                    Service.Configuration.Save();
                }

                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted("Offset of melee check distance for features that use it.\r\nFor those who don't want to immediately use their ranged attack if the boss walks slightly out of range.");
                    ImGui.EndTooltip();
                }
                #endregion

                #region Message of the Day

                bool motd = Service.Configuration.HideMessageOfTheDay;

                if (ImGui.Checkbox("Hide Message of the Day", ref motd))
                {
                    Service.Configuration.HideMessageOfTheDay = motd;
                    Service.Configuration.Save();
                }

                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted("Disables the Message of the Day message in your chat when you login.");
                    ImGui.EndTooltip();
                }
                ImGui.NextColumn();

                #endregion

                #region TargetHelper

                Vector4 colour = Service.Configuration.TargetHighlightColor;
                if (ImGui.ColorEdit4("Target Highlight Colour", ref colour, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.AlphaPreview | ImGuiColorEditFlags.AlphaBar))
                {
                    Service.Configuration.TargetHighlightColor = colour;
                    Service.Configuration.Save();
                }

                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted($"Used for {CustomComboFunctions.JobIDs.JobIDToName(33)} card targeting features.\r\nSet Alpha to 0 to hide the box.");
                    ImGui.EndTooltip();
                }

                #endregion

                if (ImGui.Checkbox($"Block spells if moving", ref Service.Configuration.BlockSpellOnMove))
                    Service.Configuration.Save();

                if (ImGui.Checkbox($"Output opener status to chat", ref Service.Configuration.OutputOpenerLogs))
                    Service.Configuration.Save();

                #region Shorten DTR bar text

                bool shortDTRText = Service.Configuration.ShortDTRText;

                if (ImGui.Checkbox("Shorten Server Info Bar Text", ref shortDTRText))
                {
                    Service.Configuration.ShortDTRText = shortDTRText;
                    Service.Configuration.Save();
                }

                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(
                        "By default, the Server Info Bar for Wrath Combo shows whether Auto-Rotation is on or off, " +
                        "\nthen -if on- it will show how many active Auto-Mode combos you have enabled. " +
                        "\nAnd finally, it will also show if another plugin is controlling that value." +
                        "\nThis option will make the number of active Auto-Mode combos not show.");
                    ImGui.EndTooltip();
                }
                #endregion

                if (ImGui.InputFloat("Movement Check Delay", ref Service.Configuration.MovementLeeway))
                    Service.Configuration.Save();

                ImGuiComponents.HelpMarker("Many features check if you are moving to decide actions. This will allow you to set a delay on how long you need to be moving before it recognizes you as moving.");

                if (ImGui.InputFloat("Opener Failure Timeout", ref Service.Configuration.OpenerTimeout))
                    Service.Configuration.Save();

                ImGuiComponents.HelpMarker("During an opener, if this amount of seconds has passed since your last action, it will fail the opener and resume with non-opener functionality.");

                if (ImGui.Checkbox("Performance Mode", ref Service.Configuration.PerformanceMode))
                    Service.Configuration.Save();

                ImGuiComponents.HelpMarker("This mode will disable actions being changed on your hotbar, but will still continue to work in the background as you press your buttons.");
            }
        }
    }
}
