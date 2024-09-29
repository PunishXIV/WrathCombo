using Dalamud.Interface.Components;
using ECommons.ImGuiMethods;
using ImGuiNET;
using XIVSlothCombo.Services;

namespace XIVSlothCombo.Window.Tabs
{
    internal class AutoRotationTab : ConfigWindow
    {
        internal static new void Draw()
        {
            ImGui.TextWrapped($"This is where you can configure the parameters in which Auto-Rotation will operate. " +
                $"Features marked with an 'Auto-Mode' checkbox are able to be used with Auto-Rotation.");
            ImGui.Separator();

            var cfg = Service.Configuration.RotationConfig;
            bool changed = false;

            changed |= ImGui.Checkbox($"Enable Auto-Rotation", ref cfg.Enabled);
            if (cfg.Enabled)
            {
                changed |= ImGui.Checkbox("Only in Combat", ref cfg.InCombatOnly);
            }

            if (ImGui.CollapsingHeader("Damage Settings"))
            {
                ImGuiEx.TextUnderlined($"Targeting Mode");
                changed |= ImGuiEx.EnumCombo("###DPSTargetingMode", ref cfg.DPSRotationMode);
                ImGuiComponents.HelpMarker("Manual - Leaves all targeting decisions to you.\n" +
                    "Highest Max - Prioritises enemies with the highest max HP.\n" +
                    "Lowest Max - Prioritises enemies with the lowest max HP.\n" +
                    "Highest Current - Prioritises the enemy with the highest current HP.\n" +
                    "Lowest Current - Prioritises the enemy with the lowest current HP.\n" +
                    "Tank Target - Prioritises the same target as the first tank in your group.\n" +
                    "Nearest - Prioritises the closest target to you.\n" +
                    "Furthest - Prioritises the furthest target from you.");
                ImGui.Spacing();
                var input = ImGuiEx.InputInt(100f.Scale(), "Targets Required for AoE Damage Features", ref cfg.DPSSettings.DPSAoETargets);
                if (input)
                {
                    changed |= input;
                    if (cfg.DPSSettings.DPSAoETargets < 0)
                        cfg.DPSSettings.DPSAoETargets = 0;
                }
                ImGuiComponents.HelpMarker($"Disabling this will turn off AoE DPS features. Otherwise will require the amount of targets required to be in range of an AoE feature's attack to use. This applies to all 3 roles, and for any features that deal AoE damage.");

                changed |= ImGui.Checkbox($"Prioritise FATE Targets", ref cfg.DPSSettings.FATEPriority);
                changed |= ImGui.Checkbox($"Prioritise Quest Targets", ref cfg.DPSSettings.QuestPriority);

            }
            ImGui.Spacing();
            if (ImGui.CollapsingHeader("Healing Settings"))
            {
                ImGuiEx.TextUnderlined($"Healing Targeting Mode");
                changed |= ImGuiEx.EnumCombo("###HealerTargetingMode", ref cfg.HealerRotationMode);
                ImGuiComponents.HelpMarker("Manual - Will only heal a target if you select them manually. If the target does not meet the healing threshold settings criteria below it will skip healing in favour of DPSing (if also enabled).\n" +
                    "Highest Current - Prioritises the party member with the highest current HP%.\n" +
                    "Lowest Current - Prioritises the party member with the lowest current HP%.");

                ImGui.SetNextItemWidth(200f.Scale());
                changed |= ImGuiEx.SliderInt("Single Target HP% Threshold", ref cfg.HealerSettings.SingleTargetHPP, 1, 99, "%d%%");
                ImGui.SetNextItemWidth(200f.Scale());
                changed |= ImGuiEx.SliderInt("Single Target HP% Threshold (target has Regen/Aspected Benefic)", ref cfg.HealerSettings.SingleTargetRegenHPP, 1, 99, "%d%%");
                ImGuiComponents.HelpMarker("You typically want to set this lower than the above setting.");
                ImGui.SetNextItemWidth(200f.Scale());
                changed |= ImGuiEx.SliderInt("AoE HP% Threshold", ref cfg.HealerSettings.AoETargetHPP, 1, 99, "%d%%");
                var input = ImGuiEx.InputInt(100f.Scale(), "Targets Required for AoE Healing Features", ref cfg.HealerSettings.AoEHealTargetCount);
                if (input)
                {
                    changed |= input;
                    if (cfg.HealerSettings.AoEHealTargetCount < 0)
                        cfg.HealerSettings.AoEHealTargetCount = 0;
                }
                ImGuiComponents.HelpMarker($"Disabling this will turn off AoE Healing features. Otherwise will require the amount of targets required to be in range of an AoE feature's heal to use.");
            }

            if (changed)
                Service.Configuration.Save();

        }
    }
}
