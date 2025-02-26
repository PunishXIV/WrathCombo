using Dalamud.Interface.Colors;
using ImGuiNET;
using WrathCombo.CustomComboNS.Functions;

namespace WrathCombo.Window.MessagesNS
{
    internal static class Messages
    {
        internal static bool PrintBLUMessage(string jobName)
        {
            if (jobName == CustomComboFunctions.JobIDs.JobIDToName(36)) //Blue Mage ID
            {
                ImGui.TextColored(ImGuiColors.ParsedPink, $"请注意，即使你没有激活所有需要的技能，你可能也能用以下的设置，但那些你没有激活的技能将会被自动跳过。\n所以如果有设置没有起作用，请尝试激活更多的需要的技能。");
            }

            return true;
        }
    }
}
