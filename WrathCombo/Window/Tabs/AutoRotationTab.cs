using Dalamud.Interface.Components;
using ECommons.ImGuiMethods;
using ImGuiNET;
using WrathCombo.Combos.PvE;
using WrathCombo.Services;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;

namespace WrathCombo.Window.Tabs
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

                if (cfg.InCombatOnly)
                {
                    changed |= ImGui.Checkbox($"Bypass Only in Combat for Quest Targets", ref cfg.BypassQuest);
                    ImGuiComponents.HelpMarker("Disables Auto-Mode outside of combat unless you're within range of a quest target.");

                    changed |= ImGui.Checkbox($"Bypass Only in Combat for FATE Targets", ref cfg.BypassFATE);
                    ImGuiComponents.HelpMarker("Disables Auto-Mode outside of combat unless you're synced to a FATE.");

                    ImGui.SetNextItemWidth(100f.Scale());
                    changed |= ImGui.InputInt("Delay to activate Auto-Rotation once combat starts (seconds)", ref cfg.CombatDelay);

                    if (cfg.CombatDelay < 0)
                        cfg.CombatDelay = 0;
                }
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
                ImGui.Spacing();
                changed |= ImGui.Checkbox("Auto-Resurrect", ref cfg.HealerSettings.AutoRez);
                ImGuiComponents.HelpMarker($"Will attempt to resurrect dead party members. Applies to {WHM.ClassID.JobAbbreviation()}, {WHM.JobID.JobAbbreviation()}, {SCH.JobID.JobAbbreviation()}, {AST.JobID.JobAbbreviation()}, {SGE.JobID.JobAbbreviation()}");
                if (cfg.HealerSettings.AutoRez)
                {
                    changed |= ImGui.Checkbox($"Apply to {SMN.JobID.JobAbbreviation()} & {RDM.JobID.JobAbbreviation()}", ref cfg.HealerSettings.AutoRezDPSJobs);
                    ImGuiComponents.HelpMarker($"When playing as {SMN.JobID.JobAbbreviation()} or {RDM.JobID.JobAbbreviation()}, also attempt to raise a dead party member. {RDM.JobID.JobAbbreviation()} will only resurrect with {All.Buffs.Swiftcast.StatusName()} or {RDM.Buffs.Dualcast.StatusName()} active.");
                }
                changed |= ImGui.Checkbox($"Auto-{All.Esuna.ActionName()}", ref cfg.HealerSettings.AutoCleanse);
                ImGuiComponents.HelpMarker($"Will {All.Esuna.ActionName()} any cleansable debuffs (Healing takes priority).");

                changed |= ImGui.Checkbox($"[{SGE.JobID.JobAbbreviation()}] Automatically Manage Kardia", ref cfg.HealerSettings.ManageKardia);
                ImGuiComponents.HelpMarker($"Switches {SGE.Kardia.ActionName()} to party members currently being targeted by enemies, prioritising tanks if multiple people are being targeted.");
                if (cfg.HealerSettings.ManageKardia)
                    changed |= ImGui.Checkbox($"Limit {SGE.Kardia.ActionName()} swapping to tanks only", ref cfg.HealerSettings.KardiaTanksOnly);

                changed |= ImGui.Checkbox($"[{WHM.JobID.JobAbbreviation()}/{AST.JobID.JobAbbreviation()}] Pre-emptively apply heal over time on focus target", ref cfg.HealerSettings.PreEmptiveHoT);
                ImGuiComponents.HelpMarker($"Applies {WHM.Regen.ActionName()}/{AST.AspectedBenefic.ActionName()} to your focus target when out of combat and they are 30y or less away from an enemy. (Bypasses \"Only in Combat\" setting)");

            }

            if (changed)
                Service.Configuration.Save();

        }
    }
}
