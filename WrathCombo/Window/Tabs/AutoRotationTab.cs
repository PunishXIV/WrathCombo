using System;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility.Raii;
using ECommons;
using ECommons.DalamudServices;
using ECommons.ImGuiMethods;
using ImGuiNET;
using Lumina.Excel.Sheets;
using System.Linq;
using WrathCombo.Combos.PvE;
using WrathCombo.Extensions;
using WrathCombo.Services;
using WrathCombo.Services.IPC;

namespace WrathCombo.Window.Tabs
{
    internal class AutoRotationTab : ConfigWindow
    {
        private static uint _selectedNpc = 0;
        internal static new void Draw()
        {
            ImGui.TextWrapped($"This is where you can configure the parameters in which Auto-Rotation will operate. " +
                $"Features marked with an 'Auto-Mode' checkbox are able to be used with Auto-Rotation.");
            ImGui.Separator();

            var cfg = Service.Configuration.RotationConfig;
            bool changed = false;

            if (P.IPC.UIHelper.ShowIPCControlledIndicatorIfNeeded())
                changed |= P.IPC.UIHelper.ShowIPCControlledCheckboxIfNeeded(
                    "Enable Auto-Rotation", ref cfg.Enabled);
            else
                changed |= ImGui.Checkbox($"Enable Auto-Rotation", ref cfg.Enabled);
            if (P.IPC.GetAutoRotationState())
            {
                var inCombatOnly = (bool)P.IPC.GetAutoRotationConfigState(
                    Enum.Parse<AutoRotationConfigOption>("InCombatOnly"))!;
                if (P.IPC.UIHelper.AutoRotationConfigControlled("InCombatOnly") is not null)
                    ImGuiExtensions.Prefix(false);
                P.IPC.UIHelper.ShowIPCControlledIndicatorIfNeeded("InCombatOnly");
                ImGuiExtensions.Prefix(!inCombatOnly);
                changed |= P.IPC.UIHelper.ShowIPCControlledCheckboxIfNeeded(
                    "Only in Combat", ref cfg.InCombatOnly, "InCombatOnly");

                if (inCombatOnly)
                {
                    ImGuiExtensions.Prefix(false);
                    changed |= ImGui.Checkbox($"Bypass Only in Combat for Quest Targets", ref cfg.BypassQuest);
                    ImGuiComponents.HelpMarker("Disables Auto-Mode outside of combat unless you're within range of a quest target.");

                    ImGuiExtensions.Prefix(false);
                    changed |= ImGui.Checkbox($"Bypass Only in Combat for FATE Targets", ref cfg.BypassFATE);
                    ImGuiComponents.HelpMarker("Disables Auto-Mode outside of combat unless you're synced to a FATE.");

                    ImGuiExtensions.Prefix(true);
                    ImGui.SetNextItemWidth(100f.Scale());
                    changed |= ImGui.InputInt("Delay to activate Auto-Rotation once combat starts (seconds)", ref cfg.CombatDelay);

                    if (cfg.CombatDelay < 0)
                        cfg.CombatDelay = 0;
                }
            }

            if (ImGui.CollapsingHeader("Damage Settings"))
            {
                ImGuiEx.TextUnderlined($"Targeting Mode");

                P.IPC.UIHelper.ShowIPCControlledIndicatorIfNeeded("DPSRotationMode");
                P.IPC.UIHelper.ShowIPCControlledComboIfNeeded(
                    "###DPSTargetingMode", true, ref cfg.DPSRotationMode,
                    ref cfg.HealerRotationMode, "DPSRotationMode");

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

                ImGui.SetNextItemWidth(100f.Scale());
                changed |= ImGui.SliderFloat("Max Target Distance", ref cfg.DPSSettings.MaxDistance, 1, 30);
                cfg.DPSSettings.MaxDistance =
                    Math.Clamp(cfg.DPSSettings.MaxDistance, 1, 30);

                ImGuiComponents.HelpMarker("Max distance all targeting modes (except Manual) will look for a target. Values from 1 to 30 only.");

                P.IPC.UIHelper.ShowIPCControlledIndicatorIfNeeded("FATEPriority");
                changed |= P.IPC.UIHelper.ShowIPCControlledCheckboxIfNeeded(
                    "Prioritise FATE Targets", ref cfg.DPSSettings.FATEPriority, "FATEPriority");
                P.IPC.UIHelper.ShowIPCControlledIndicatorIfNeeded("QuestPriority");
                changed |= P.IPC.UIHelper.ShowIPCControlledCheckboxIfNeeded(
                    "Prioritise Quest Targets", ref cfg.DPSSettings.QuestPriority, "QuestPriority");
                changed |= ImGui.Checkbox($"Prioritise Targets Not In Combat", ref cfg.DPSSettings.PreferNonCombat);

                var npcs = Service.Configuration.IgnoredNPCs.ToList();
                var selected = npcs.FirstOrNull(x => x.Key == _selectedNpc);
                var prev = selected is null ? "" : $"{Svc.Data.Excel.GetSheet<BNpcName>().GetRow(selected.Value.Value).Singular} (ID: {selected.Value.Key})";
                ImGuiEx.TextUnderlined($"Ignored NPCs");
                using (var combo = ImRaii.Combo("###Ignore", prev))
                {
                    if (combo)
                    {
                        if (ImGui.Selectable(""))
                        {
                            _selectedNpc = 0;
                        }

                        foreach (var npc in npcs)
                        {
                            var npcData = Svc.Data.Excel
                                .GetSheet<BNpcName>().GetRow(npc.Value);
                            if (ImGui.Selectable($"{npcData.Singular} (ID: {npc.Key})"))
                            {
                                _selectedNpc = npc.Key;
                            }
                        }
                    }
                }
                ImGuiComponents.HelpMarker("These NPCs will be ignored by Auto-Rotation.\n" +
                                           "Every instance of this NPC will be excluded from automatic targeting (Manual will still work).\n" +
                                           "To remove an NPC from this list, select it and press the Delete button below.\n" +
                                           "To add an NPC to this list, target the NPC and use the command: /wrath ignore");

                if (_selectedNpc > 0)
                {
                    if (ImGui.Button("Delete From Ignored"))
                    {
                        Service.Configuration.IgnoredNPCs.Remove(_selectedNpc);
                        Service.Configuration.Save();

                        _selectedNpc = 0;
                    }
                }

            }
            ImGui.Spacing();
            if (ImGui.CollapsingHeader("Healing Settings"))
            {
                ImGuiEx.TextUnderlined($"Healing Targeting Mode");
                P.IPC.UIHelper.ShowIPCControlledIndicatorIfNeeded("HealerRotationMode");
                P.IPC.UIHelper.ShowIPCControlledComboIfNeeded(
                    "###HealerTargetingMode", false, ref cfg.DPSRotationMode,
                    ref cfg.HealerRotationMode, "HealerRotationMode");
                ImGuiComponents.HelpMarker("Manual - Will only heal a target if you select them manually. If the target does not meet the healing threshold settings criteria below it will skip healing in favour of DPSing (if also enabled).\n" +
                    "Highest Current - Prioritises the party member with the highest current HP%.\n" +
                    "Lowest Current - Prioritises the party member with the lowest current HP%.");

                P.IPC.UIHelper.ShowIPCControlledIndicatorIfNeeded("SingleTargetHPP");
                changed |= P.IPC.UIHelper.ShowIPCControlledSliderIfNeeded(
                    "Single Target HP% Threshold", ref cfg.HealerSettings.SingleTargetHPP, "SingleTargetHPP");

                P.IPC.UIHelper.ShowIPCControlledIndicatorIfNeeded("SingleTargetRegenHPP");
                changed |= P.IPC.UIHelper.ShowIPCControlledSliderIfNeeded(
                    "Single Target HP% Threshold (target has Regen/Aspected Benefic)", ref cfg.HealerSettings.SingleTargetRegenHPP, "SingleTargetRegenHPP");
                ImGuiComponents.HelpMarker("You typically want to set this lower than the above setting.");

                P.IPC.UIHelper.ShowIPCControlledIndicatorIfNeeded("AoETargetHPP");
                changed |= P.IPC.UIHelper.ShowIPCControlledSliderIfNeeded(
                    "AoE HP% Threshold", ref cfg.HealerSettings.AoETargetHPP, "AoETargetHPP");

                var input = ImGuiEx.InputInt(100f.Scale(), "Targets Required for AoE Healing Features", ref cfg.HealerSettings.AoEHealTargetCount);
                if (input)
                {
                    changed |= input;
                    if (cfg.HealerSettings.AoEHealTargetCount < 0)
                        cfg.HealerSettings.AoEHealTargetCount = 0;
                }
                ImGuiComponents.HelpMarker($"Disabling this will turn off AoE Healing features. Otherwise will require the amount of targets required to be in range of an AoE feature's heal to use.");
                ImGui.SetNextItemWidth(100f.Scale());
                changed |= ImGui.InputInt("Delay to start healing once above conditions are met (seconds)", ref cfg.HealerSettings.HealDelay);

                if (cfg.HealerSettings.HealDelay < 0)
                    cfg.HealerSettings.HealDelay = 0;
                ImGuiComponents.HelpMarker("Don't set this too high! 1-2 seconds is normally comfy enough to be considered a natural reaction.");

                ImGui.Spacing();

                P.IPC.UIHelper.ShowIPCControlledIndicatorIfNeeded("AutoRez");
                changed |= P.IPC.UIHelper.ShowIPCControlledCheckboxIfNeeded(
                    "Auto-Resurrect", ref cfg.HealerSettings.AutoRez, "AutoRez");
                ImGuiComponents.HelpMarker($"Will attempt to resurrect dead party members. Applies to {WHM.ClassID.JobAbbreviation()}, {WHM.JobID.JobAbbreviation()}, {SCH.JobID.JobAbbreviation()}, {AST.JobID.JobAbbreviation()}, {SGE.JobID.JobAbbreviation()}");
                var autoRez = (bool)P.IPC.GetAutoRotationConfigState(AutoRotationConfigOption.AutoRez)!;
                if (autoRez)
                {
                    ImGuiExtensions.Prefix(true);
                    P.IPC.UIHelper.ShowIPCControlledIndicatorIfNeeded("AutoRezDPSJobs");
                    changed |= P.IPC.UIHelper.ShowIPCControlledCheckboxIfNeeded(
                        $"Apply to {SMN.JobID.JobAbbreviation()} & {RDM.JobID.JobAbbreviation()}", ref cfg.HealerSettings.AutoRezDPSJobs, "AutoRezDPSJobs");
                    ImGuiComponents.HelpMarker($"When playing as {SMN.JobID.JobAbbreviation()} or {RDM.JobID.JobAbbreviation()}, also attempt to raise a dead party member. {RDM.JobID.JobAbbreviation()} will only resurrect with {All.Buffs.Swiftcast.StatusName()} or {RDM.Buffs.Dualcast.StatusName()} active.");
                }

                P.IPC.UIHelper.ShowIPCControlledIndicatorIfNeeded("AutoCleanse");
                changed |= P.IPC.UIHelper.ShowIPCControlledCheckboxIfNeeded(
                    $"Auto-{All.Esuna.ActionName()}", ref cfg.HealerSettings.AutoCleanse, "AutoCleanse");
                ImGuiComponents.HelpMarker($"Will {All.Esuna.ActionName()} any cleansable debuffs (Healing takes priority).");

                P.IPC.UIHelper.ShowIPCControlledIndicatorIfNeeded("ManageKardia");
                changed |= P.IPC.UIHelper.ShowIPCControlledCheckboxIfNeeded(
                    $"[{SGE.JobID.JobAbbreviation()}] Automatically Manage Kardia", ref cfg.HealerSettings.ManageKardia, "ManageKardia");
                ImGuiComponents.HelpMarker($"Switches {SGE.Kardia.ActionName()} to party members currently being targeted by enemies, prioritising tanks if multiple people are being targeted.");
                if (cfg.HealerSettings.ManageKardia)
                {
                    ImGuiExtensions.Prefix(cfg.HealerSettings.ManageKardia);
                    changed |= ImGui.Checkbox($"Limit {SGE.Kardia.ActionName()} swapping to tanks only", ref cfg.HealerSettings.KardiaTanksOnly);
                }

                changed |= ImGui.Checkbox($"[{WHM.JobID.JobAbbreviation()}/{AST.JobID.JobAbbreviation()}] Pre-emptively apply heal over time on focus target", ref cfg.HealerSettings.PreEmptiveHoT);
                ImGuiComponents.HelpMarker($"Applies {WHM.Regen.ActionName()}/{AST.AspectedBenefic.ActionName()} to your focus target when out of combat and they are 30y or less away from an enemy. (Bypasses \"Only in Combat\" setting)");

            }

            if (changed)
                Service.Configuration.Save();

        }
    }
}
