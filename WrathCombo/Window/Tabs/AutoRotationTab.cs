using Dalamud.Interface.Components;
using Dalamud.Interface.Utility.Raii;
using ECommons;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.ImGuiMethods;
using Lumina.Excel.Sheets;
using System;
using System.Linq;
using WrathCombo.Combos.PvE;
using WrathCombo.Extensions;
using WrathCombo.Services;
using WrathCombo.Services.IPC;
using WrathCombo.Services.IPC_Subscriber;
namespace WrathCombo.Window.Tabs;

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

        if (P.UIHelper.ShowIPCControlledIndicatorIfNeeded())
            changed |= P.UIHelper.ShowIPCControlledCheckboxIfNeeded(
                "Enable Auto-Rotation", ref cfg.Enabled);
        else
            changed |= ImGui.Checkbox($"Enable Auto-Rotation", ref cfg.Enabled);
        if (P.IPC.GetAutoRotationState())
        {
            var inCombatOnly = (bool)P.IPC.GetAutoRotationConfigState(
                Enum.Parse<AutoRotationConfigOption>("InCombatOnly"))!;
            ImGuiExtensions.Prefix(!inCombatOnly);
            changed |= P.UIHelper.ShowIPCControlledCheckboxIfNeeded(
                "Only in Combat", ref cfg.InCombatOnly, "InCombatOnly");

            if (inCombatOnly)
            {
                ImGuiExtensions.Prefix(false);
                changed |= ImGui.Checkbox($"Bypass When Combo Suggests Self-Use Action", ref cfg.BypassBuffs);
                ImGuiComponents.HelpMarker($"Many jobs have an out of combat action that can be used, for example, {RPR.Soulsow.ActionName()} or {MNK.ForbiddenMeditation.ActionName()}. This will allow these to be used without being in combnat.");

                ImGuiExtensions.Prefix(false);
                changed |= ImGui.Checkbox($"Bypass Only in Combat for Quest Targets", ref cfg.BypassQuest);
                ImGuiComponents.HelpMarker("Disables Auto-Mode outside of combat unless you're within range of a quest target.");

                ImGuiExtensions.Prefix(false);
                changed |= ImGui.Checkbox($"Bypass Only in Combat for FATE Targets", ref cfg.BypassFATE);
                ImGuiComponents.HelpMarker("Disables Auto-Mode outside of combat unless you're synced to a FATE.");

                ImGuiExtensions.Prefix(true);
                ImGuiEx.SetNextItemWidthScaled(100);
                changed |= ImGui.InputInt("Delay to activate Auto-Rotation once combat starts (seconds)", ref cfg.CombatDelay);

                if (cfg.CombatDelay < 0)
                    cfg.CombatDelay = 0;
            }
        }

        changed |= ImGui.Checkbox("Enable Automatically in Instanced Content", ref cfg.EnableInInstance);
        changed |= ImGui.Checkbox("Disable After Leaving Instanced Content", ref cfg.DisableAfterInstance);

        if (ImGui.CollapsingHeader("Damage Settings"))
        {
            ImGuiEx.TextUnderlined($"Targeting Mode");

            P.UIHelper.ShowIPCControlledIndicatorIfNeeded("DPSRotationMode");
            changed |= P.UIHelper.ShowIPCControlledComboIfNeeded(
                "###DPSTargetingMode", true, ref cfg.DPSRotationMode,
                ref cfg.HealerRotationMode, "DPSRotationMode");

            ImGuiComponents.HelpMarker("Manual - Leaves all targeting decisions to you.\n" +
                                       "Highest Max - Prioritises enemies with the highest max HP.\n" +
                                       "Lowest Max - Prioritises enemies with the lowest max HP.\n" +
                                       "Highest Current - Prioritises the enemy with the highest current HP.\n" +
                                       "Lowest Current - Prioritises the enemy with the lowest current HP.\n" +
                                       "Tank Target - Prioritises the same target as the first tank in your group.\n" +
                                       "Nearest - Prioritises the closest target to you.\n" +
                                       "Furthest - Prioritises the furthest target from you.");
            ImGui.Spacing();

            if (cfg.DPSRotationMode == AutoRotation.DPSRotationMode.Manual)
            {
                changed |= ImGui.Checkbox("Enforce Best AoE Target Selection", ref cfg.DPSSettings.AoEIgnoreManual);

                ImGuiComponents.HelpMarker("For all other targeting modes, AoE will target based on highest number of targets hit. In manual mode, it will only do this if you tick this box.");
            }

                
            P.UIHelper.ShowIPCControlledIndicatorIfNeeded("DPSAoETargets");
            var input = P.UIHelper.ShowIPCControlledNumberInputIfNeeded(
                "Targets Required for AoE Damage Features", ref cfg.DPSSettings.DPSAoETargets, "DPSAoETargets");
            if (input)
            {
                changed |= input;
                if (cfg.DPSSettings.DPSAoETargets < 0)
                    cfg.DPSSettings.DPSAoETargets = 0;
            }
            ImGuiComponents.HelpMarker($"Disabling this will turn off AoE DPS features. Otherwise will require the amount of targets required to be in range of an AoE feature's attack to use. This applies to all 3 roles, and for any features that deal AoE damage.");

            ImGuiEx.SetNextItemWidthScaled(100);
            changed |= ImGui.SliderFloat("Max Target Distance", ref cfg.DPSSettings.MaxDistance, 1, 30);
            cfg.DPSSettings.MaxDistance =
                Math.Clamp(cfg.DPSSettings.MaxDistance, 1, 30);

            ImGuiComponents.HelpMarker("Max distance all targeting modes (except Manual) will look for a target. Values from 1 to 30 only.");

            P.UIHelper.ShowIPCControlledIndicatorIfNeeded("FATEPriority");
            changed |= P.UIHelper.ShowIPCControlledCheckboxIfNeeded(
                "Prioritise FATE Targets", ref cfg.DPSSettings.FATEPriority, "FATEPriority");
            P.UIHelper.ShowIPCControlledIndicatorIfNeeded("QuestPriority");
            changed |= P.UIHelper.ShowIPCControlledCheckboxIfNeeded(
                "Prioritise Quest Targets", ref cfg.DPSSettings.QuestPriority, "QuestPriority");
            changed |= ImGui.Checkbox($"Prioritise Targets Not In Combat", ref cfg.DPSSettings.PreferNonCombat);

            if (cfg.DPSSettings.PreferNonCombat && changed)
                cfg.DPSSettings.OnlyAttackInCombat = false;

            changed |= P.UIHelper.ShowIPCControlledCheckboxIfNeeded(
                "Only Attack Targets Already In Combat", ref cfg.DPSSettings.OnlyAttackInCombat,
                "OnlyAttackInCombat");

            if (cfg.DPSSettings.OnlyAttackInCombat && changed)
                cfg.DPSSettings.PreferNonCombat = false;

            changed |= ImGui.Checkbox("Always Target Regardless of Action", ref cfg.DPSSettings.AlwaysSelectTarget);

            ImGuiComponents.HelpMarker("Normally, Auto-rotation will only target an enemy if the next action it would fire needs a target. This will change the behaviour so it will always select the target regardless of what the action can target.");

            changed |= ImGui.Checkbox("Un-Target and Stop Actions for Pyretics", ref cfg.DPSSettings.UnTargetAndDisableForPenalty);

            ImGuiComponents.HelpMarker("This will un-set any current target and disable Auto-Rotation actions if there is a current detected Pyretic (or similar, like Acceleration Bomb) mechanic affecting the player, that would harm them if they took any action.");

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
            P.UIHelper.ShowIPCControlledIndicatorIfNeeded("HealerRotationMode");
            changed |= P.UIHelper.ShowIPCControlledComboIfNeeded(
                "###HealerTargetingMode", false, ref cfg.DPSRotationMode,
                ref cfg.HealerRotationMode, "HealerRotationMode");
            ImGuiComponents.HelpMarker("Manual - Will only heal a target if you select them manually. If the target does not meet the healing threshold settings criteria below it will skip healing in favour of DPSing (if also enabled).\n" +
                                       "Highest Current - Prioritises the party member with the highest current HP%.\n" +
                                       "Lowest Current - Prioritises the party member with the lowest current HP%.");

            P.UIHelper.ShowIPCControlledIndicatorIfNeeded("SingleTargetHPP");
            changed |= P.UIHelper.ShowIPCControlledSliderIfNeeded(
                "Single Target HP% Threshold", ref cfg.HealerSettings.SingleTargetHPP, "SingleTargetHPP");

            P.UIHelper.ShowIPCControlledIndicatorIfNeeded("SingleTargetRegenHPP");
            changed |= P.UIHelper.ShowIPCControlledSliderIfNeeded(
                "Single Target HP% Threshold (target has Regen/Aspected Benefic)", ref cfg.HealerSettings.SingleTargetRegenHPP, "SingleTargetRegenHPP");
            ImGuiComponents.HelpMarker("You typically want to set this lower than the above setting.");
                
            P.UIHelper.ShowIPCControlledIndicatorIfNeeded("SingleTargetExcogHPP");
            changed |= P.UIHelper.ShowIPCControlledSliderIfNeeded(
                "Single Target HP% Threshold (target has Excogitation)", ref cfg.HealerSettings.SingleTargetExcogHPP, "SingleTargetExcogHPP");
            ImGuiComponents.HelpMarker("You typically want to set this lower than the above setting.");

            P.UIHelper.ShowIPCControlledIndicatorIfNeeded("AoETargetHPP");
            changed |= P.UIHelper.ShowIPCControlledSliderIfNeeded(
                "AoE HP% Threshold", ref cfg.HealerSettings.AoETargetHPP, "AoETargetHPP");

            var input = ImGuiEx.InputInt(100f.Scale(), "Targets Required for AoE Healing Features", ref cfg.HealerSettings.AoEHealTargetCount);
            if (input)
            {
                changed |= input;
                if (cfg.HealerSettings.AoEHealTargetCount < 0)
                    cfg.HealerSettings.AoEHealTargetCount = 0;
            }
            ImGuiComponents.HelpMarker($"Disabling this will turn off AoE Healing features. Otherwise will require the amount of targets required to be in range of an AoE feature's heal to use.");
            ImGuiEx.SetNextItemWidthScaled(100);
            changed |= ImGui.InputInt("Delay to start healing once above conditions are met (seconds)", ref cfg.HealerSettings.HealDelay);

            if (cfg.HealerSettings.HealDelay < 0)
                cfg.HealerSettings.HealDelay = 0;
            ImGuiComponents.HelpMarker("Don't set this too high! 1-2 seconds is normally comfy enough to be considered a natural reaction.");

            ImGui.Spacing();

            P.UIHelper.ShowIPCControlledIndicatorIfNeeded("AutoRez");
            changed |= P.UIHelper.ShowIPCControlledCheckboxIfNeeded(
                "Auto-Resurrect", ref cfg.HealerSettings.AutoRez, "AutoRez");
            ImGuiComponents.HelpMarker($"Will attempt to resurrect dead party members. Applies to {Job.CNJ.Shorthand()}, {Job.WHM.Shorthand()}, {Job.SCH.Shorthand()}, {Job.AST.Shorthand()}, {Job.SGE.Shorthand()} and {OccultCrescent.ContentName} {Svc.Data.GetExcelSheet<MKDSupportJob>().GetRow(10).Unknown0} {OccultCrescent.Revive.ActionName()}");
            var autoRez = (bool)P.IPC.GetAutoRotationConfigState(AutoRotationConfigOption.AutoRez)!;
            if (autoRez)
            {
                ImGuiExtensions.Prefix(false);
                P.UIHelper.ShowIPCControlledIndicatorIfNeeded("AutoRezOutOfParty");
                changed |= P.UIHelper.ShowIPCControlledCheckboxIfNeeded(
                    "Apply to Out of Party Members", ref cfg.HealerSettings.AutoRezOutOfParty, "AutoRezOutOfParty");

                ImGuiExtensions.Prefix(false);
                changed |= ImGui.Checkbox("Require Swiftcast/Dualcast", ref
                    cfg.HealerSettings.AutoRezRequireSwift);
                ImGuiComponents.HelpMarker(
                    $"Requires {RoleActions.Magic.Swiftcast.ActionName()} " +
                    $"(or {Job.RDM.Shorthand()}'s Dualcast) " +
                    $"to be available to resurrect a party member, to avoid hard-casting.");

                ImGuiExtensions.Prefix(true);
                P.UIHelper.ShowIPCControlledIndicatorIfNeeded("AutoRezDPSJobs");
                changed |= P.UIHelper.ShowIPCControlledCheckboxIfNeeded(
                    $"Apply to {Job.SMN.Shorthand()} & {Job.RDM.Shorthand()}", ref cfg.HealerSettings.AutoRezDPSJobs, "AutoRezDPSJobs");
                ImGuiComponents.HelpMarker($"When playing as {Job.SMN.Shorthand()} or {Job.RDM.Shorthand()}, also attempt to raise a dead party member. {Job.RDM.Shorthand()} will only resurrect with {RoleActions.Magic.Buffs.Swiftcast.StatusName()} or {RDM.Buffs.Dualcast.StatusName()} active.");
            }

            P.UIHelper.ShowIPCControlledIndicatorIfNeeded("AutoCleanse");
            changed |= P.UIHelper.ShowIPCControlledCheckboxIfNeeded(
                $"Auto-{RoleActions.Healer.Esuna.ActionName()}", ref cfg.HealerSettings.AutoCleanse, "AutoCleanse");
            ImGuiComponents.HelpMarker($"Will {RoleActions.Healer.Esuna.ActionName()} any cleansable debuffs (Healing takes priority).");

            P.UIHelper.ShowIPCControlledIndicatorIfNeeded("ManageKardia");
            changed |= P.UIHelper.ShowIPCControlledCheckboxIfNeeded(
                $"[{Job.SGE.Shorthand()}] Automatically Manage Kardia", ref cfg.HealerSettings.ManageKardia, "ManageKardia");
            ImGuiComponents.HelpMarker($"Switches {SGE.Kardia.ActionName()} to party members currently being targeted by enemies, prioritising tanks if multiple people are being targeted.");
            if (cfg.HealerSettings.ManageKardia)
            {
                ImGuiExtensions.Prefix(cfg.HealerSettings.ManageKardia);
                changed |= ImGui.Checkbox($"Limit {SGE.Kardia.ActionName()} swapping to tanks only", ref cfg.HealerSettings.KardiaTanksOnly);
            }

            changed |= ImGui.Checkbox($"[{Job.WHM.Shorthand()}/{Job.AST.Shorthand()}] Pre-emptively apply heal over time on focus target", ref cfg.HealerSettings.PreEmptiveHoT);
            ImGuiComponents.HelpMarker($"Applies {WHM.Regen.ActionName()}/{AST.AspectedBenefic.ActionName()} to your focus target when out of combat and they are 30y or less away from an enemy. (Bypasses \"Only in Combat\" setting)");

            P.UIHelper.ShowIPCControlledIndicatorIfNeeded("IncludeNPCs");
            changed |= P.UIHelper.ShowIPCControlledCheckboxIfNeeded("Heal Friendly NPCs", ref cfg.HealerSettings.IncludeNPCs);
            ImGuiComponents.HelpMarker("Useful for healer quests where NPCs are expected to be healed but aren't added directly to your party.");

        }

        ImGuiEx.TextUnderlined("Advanced");
        changed |= ImGui.InputInt("Throttle Delay (ms)", ref cfg.Throttler);
        ImGuiComponents.HelpMarker("Auto-Rotation has a built in throttler to only run every so many milliseconds for performance reasons. If you experience issues with frame rate, try increasing this value. Do note this may have a side-effect of introducing clipping if set too high, so experiment with the value.");

        var orbwalker = OrbwalkerIPC.IsEnabled && OrbwalkerIPC.PluginEnabled();
        using (ImRaii.Disabled(!orbwalker))
        {
            P.UIHelper.ShowIPCControlledIndicatorIfNeeded("OrbwalkerIntegration");
            changed |= P.UIHelper.ShowIPCControlledCheckboxIfNeeded(
                "Enable Orbwalker Integration", ref cfg.OrbwalkerIntegration, "OrbwalkerIntegration");

            ImGuiComponents.HelpMarker($"This will make Auto-Rotation use actions with cast times even whilst moving, as Orbwalker will lock movement during the cast. You may need to enable \"Buffer Initial Cast\" setting in Orbwalker if not already enabled. Requires an Orbwalker plugin to be installed and enabled.");
        }

        if (changed)
            Service.Configuration.Save();

    }
}