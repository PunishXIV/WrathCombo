#region

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
using WrathCombo.Services.IPC_Subscriber;
using WrathCombo.API.Enum;
using WrathCombo.Resources.Localization.UI.AutoRotation;

#endregion

namespace WrathCombo.Window.Tabs;

internal class AutoRotationTab : ConfigWindow
{
    private static uint _selectedNpc = 0;
    internal new static void Draw()
    {
        ImGui.TextWrapped(AutoRotationUI.Info_Header);
        ImGui.Separator();

        var cfg = Service.Configuration.RotationConfig;
        bool changed = false;

        if (P.UIHelper.ShowIPCControlledIndicatorIfNeeded())
            changed |= P.UIHelper.ShowIPCControlledCheckboxIfNeeded(
                AutoRotationUI.Checkbox_EnableAutoRotation, ref cfg.Enabled);
        else
            changed |= ImGui.Checkbox(AutoRotationUI.Checkbox_EnableAutoRotation, ref cfg.Enabled);
        if (P.IPC.GetAutoRotationState())
        {
            var inCombatOnly = (bool)P.IPC.GetAutoRotationConfigState(
                Enum.Parse<AutoRotationConfigOption>("InCombatOnly"))!;
            ImGuiExtensions.Prefix(!inCombatOnly);
            changed |= P.UIHelper.ShowIPCControlledCheckboxIfNeeded(
                AutoRotationUI.Checkbox_OnlyInCombat, ref cfg.InCombatOnly, "InCombatOnly");

            if (inCombatOnly)
            {
                ImGuiExtensions.Prefix(false);
                changed |= ImGui.Checkbox(AutoRotationUI.Checkbox_BypassSelfUse, ref cfg.BypassBuffs);
                ImGuiComponents.HelpMarker(
                    Text.FormatAndCache(
                        AutoRotationUI.HelpText_BypassSelfUse,
                        RPR.Soulsow.ActionName(),
                        MNK.ForbiddenMeditation.ActionName())
                );

                ImGuiExtensions.Prefix(false);
                changed |= P.UIHelper.ShowIPCControlledCheckboxIfNeeded(AutoRotationUI.Checkbox_BypassQuestTargets, ref cfg.BypassQuest, "BypassQuest");
                ImGuiComponents.HelpMarker(AutoRotationUI.HelpText_BypassQuestTargets);

                ImGuiExtensions.Prefix(false);
                changed |= P.UIHelper.ShowIPCControlledCheckboxIfNeeded(AutoRotationUI.Checkbox_BypassFATETargets, ref cfg.BypassFATE, "BypassFATE");
                ImGuiComponents.HelpMarker(AutoRotationUI.HelpText_BypassFATETargets);

                ImGuiExtensions.Prefix(true);
                ImGuiEx.SetNextItemWidthScaled(100);
                changed |= ImGui.InputInt(AutoRotationUI.Input_AutoRotationDelay, ref cfg.CombatDelay);

                if (cfg.CombatDelay < 0)
                    cfg.CombatDelay = 0;
            }
        }

        changed |= ImGui.Checkbox(AutoRotationUI.Checkbox_EnableInstancedEnter, ref cfg.EnableInInstance);
        changed |= ImGui.Checkbox(AutoRotationUI.Checkbox_DisableInstanceExit, ref cfg.DisableAfterInstance);

        ImGuiEx.SetNextItemWidthScaled(100);
        changed |= ImGuiEx.SliderFloat(AutoRotationUI.Input_QueueWindow, ref cfg.QueueWindow, 0f, 0.5f, $"{cfg.QueueWindow:N1}");
        cfg.QueueWindow = (float)Math.Round(cfg.QueueWindow, 1);
        ImGuiComponents.HelpMarker(AutoRotationUI.HelpText_QueueWindow);
        if (cfg.QueueWindow > 0.5f)
            cfg.QueueWindow = 0.5f;
        if (cfg.QueueWindow < 0)
            cfg.QueueWindow = 0;

        if (ImGui.CollapsingHeader(AutoRotationUI.Label_DamageSettings))
        {
            ImGuiEx.TextUnderlined(AutoRotationUI.Label_DPSTargetingMode);

            P.UIHelper.ShowIPCControlledIndicatorIfNeeded("DPSRotationMode");
            changed |= P.UIHelper.ShowIPCControlledComboIfNeeded(
                "###DPSTargetingMode", true, ref cfg.DPSRotationMode,
                ref cfg.HealerRotationMode, "DPSRotationMode");

            ImGuiComponents.HelpMarker(AutoRotationUI.HelpText_DPSTargettingMode);
            ImGui.Spacing();

            if (cfg.DPSRotationMode is DPSRotationMode.Manual)
            {
                changed |= ImGui.Checkbox(AutoRotationUI.Checkbox_EnforceBestAoETarget, ref cfg.DPSSettings.AoEIgnoreManual);

                ImGuiComponents.HelpMarker(AutoRotationUI.HelpText_EnforceBestAoETarget);
            }


            P.UIHelper.ShowIPCControlledIndicatorIfNeeded("DPSAoETargets");
            var input = P.UIHelper.ShowIPCControlledNumberInputIfNeeded(
                AutoRotationUI.Input_AoETargetCount, ref cfg.DPSSettings.DPSAoETargets, "DPSAoETargets");
            if (input)
            {
                changed |= input;
                if (cfg.DPSSettings.DPSAoETargets < 0)
                    cfg.DPSSettings.DPSAoETargets = 0;
            }
            ImGuiComponents.HelpMarker(AutoRotationUI.HelpText_AoETargetCount);

            ImGuiEx.SetNextItemWidthScaled(100);
            changed |= ImGui.SliderFloat(AutoRotationUI.Label_DPSMaxTargetDistance, ref cfg.DPSSettings.MaxDistance, 1, 30, $"{cfg.DPSSettings.MaxDistance:0}");
            cfg.DPSSettings.MaxDistance =
                Math.Clamp(cfg.DPSSettings.MaxDistance, 1, 30);

            ImGuiComponents.HelpMarker(AutoRotationUI.HelpText_DPSMaxTargetDistance);

            P.UIHelper.ShowIPCControlledIndicatorIfNeeded("IgnoreRangeInBoss");
            changed |= P.UIHelper.ShowIPCControlledCheckboxIfNeeded(AutoRotationUI.Label_IgnoreRangeInBoss, ref cfg.DPSSettings.IgnoreRangeInBoss, "IgnoreRangeInBoss");

            ImGuiComponents.HelpMarker(AutoRotationUI.HelpText_IgnoreRangeInBoss);

            P.UIHelper.ShowIPCControlledIndicatorIfNeeded("FATEPriority");
            changed |= P.UIHelper.ShowIPCControlledCheckboxIfNeeded(
                AutoRotationUI.Checkbox_FATEPriority, ref cfg.DPSSettings.FATEPriority, "FATEPriority");
            P.UIHelper.ShowIPCControlledIndicatorIfNeeded("QuestPriority");
            changed |= P.UIHelper.ShowIPCControlledCheckboxIfNeeded(
                AutoRotationUI.Checkbox_QuestPriority, ref cfg.DPSSettings.QuestPriority, "QuestPriority");
            changed |= ImGui.Checkbox(AutoRotationUI.Checkbox_PreferNonCombat, ref cfg.DPSSettings.PreferNonCombat);

            if (cfg.DPSSettings.PreferNonCombat && changed)
                cfg.DPSSettings.OnlyAttackInCombat = false;

            changed |= P.UIHelper.ShowIPCControlledCheckboxIfNeeded(
                AutoRotationUI.Checkbox_OnlyAttackInCombat, ref cfg.DPSSettings.OnlyAttackInCombat,
                "OnlyAttackInCombat");

            if (cfg.DPSSettings.OnlyAttackInCombat && changed)
                cfg.DPSSettings.PreferNonCombat = false;

            changed |= ImGui.Checkbox(AutoRotationUI.Checkbox_UnTargetAndDisableForPenalty, ref cfg.DPSSettings.UnTargetAndDisableForPenalty);

            ImGuiComponents.HelpMarker(AutoRotationUI.HelpText_UnTargetAndDisableForPenalty);

            changed |= P.UIHelper.ShowIPCControlledCheckboxIfNeeded(AutoRotationUI.Checkbox_DPSAlwaysHardTarget, ref cfg.DPSSettings.DPSAlwaysHardTarget, "DPSAlwaysHardTarget");

            ImGuiComponents.HelpMarker(AutoRotationUI.HelpText_DPSAlwaysHardTarget);

            var npcs = Service.Configuration.IgnoredNPCs.ToList();
            var selected = npcs.FirstOrNull(x => x.Key == _selectedNpc);
            var prev = selected is null ? "" : $"{Svc.Data.Excel.GetSheet<BNpcName>().GetRow(selected.Value.Value).Singular} (ID: {selected.Value.Key})";
            ImGuiEx.TextUnderlined(AutoRotationUI.Label_IgnoredNPCs);
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
            ImGuiComponents.HelpMarker(AutoRotationUI.HelpText_IgnoredNPCs);

            if (_selectedNpc > 0)
            {
                if (ImGui.Button(AutoRotationUI.Button_DeleteFromIgnored))
                {
                    Service.Configuration.IgnoredNPCs.Remove(_selectedNpc);
                    Service.Configuration.Save();

                    _selectedNpc = 0;
                }
            }

        }
        ImGui.Spacing();
        if (ImGui.CollapsingHeader(AutoRotationUI.Header_HealingSettings))
        {
            ImGuiEx.TextUnderlined(AutoRotationUI.Label_HealingTargetingMode);
            P.UIHelper.ShowIPCControlledIndicatorIfNeeded("HealerRotationMode");
            changed |= P.UIHelper.ShowIPCControlledComboIfNeeded(
                "###HealerTargetingMode", false, ref cfg.DPSRotationMode,
                ref cfg.HealerRotationMode, "HealerRotationMode");
            ImGuiComponents.HelpMarker(AutoRotationUI.HelpText_HealerTargetingMode);

            P.UIHelper.ShowIPCControlledIndicatorIfNeeded("SingleTargetHPP");
            changed |= P.UIHelper.ShowIPCControlledSliderIfNeeded(
                AutoRotationUI.Slider_SingleTargetHPP, ref cfg.HealerSettings.SingleTargetHPP, "SingleTargetHPP");

            P.UIHelper.ShowIPCControlledIndicatorIfNeeded("SingleTargetRegenHPP");
            changed |= P.UIHelper.ShowIPCControlledSliderIfNeeded(
                AutoRotationUI.Slider_SingleTargetRegenHPP, ref cfg.HealerSettings.SingleTargetRegenHPP, "SingleTargetRegenHPP");
            ImGuiComponents.HelpMarker(AutoRotationUI.HelpText_SingleTargetRegenHPP);

            P.UIHelper.ShowIPCControlledIndicatorIfNeeded("SingleTargetExcogHPP");
            changed |= P.UIHelper.ShowIPCControlledSliderIfNeeded(
                AutoRotationUI.Slider_SingleTargetExcogHPP, ref cfg.HealerSettings.SingleTargetExcogHPP, "SingleTargetExcogHPP");
            ImGuiComponents.HelpMarker(AutoRotationUI.HelpText_SingleTargetExcogHPP);

            P.UIHelper.ShowIPCControlledIndicatorIfNeeded("AoETargetHPP");
            changed |= P.UIHelper.ShowIPCControlledSliderIfNeeded(
                AutoRotationUI.Slider_AoETargetHPP, ref cfg.HealerSettings.AoETargetHPP, "AoETargetHPP");

            var input = ImGuiEx.InputInt(100f.Scale(), AutoRotationUI.Input_AoEHealTargetCount, ref cfg.HealerSettings.AoEHealTargetCount);
            if (input)
            {
                changed |= input;
                if (cfg.HealerSettings.AoEHealTargetCount < 0)
                    cfg.HealerSettings.AoEHealTargetCount = 0;
            }
            ImGuiComponents.HelpMarker(AutoRotationUI.HelpText_AoEHealTargetCount);
            ImGuiEx.SetNextItemWidthScaled(100);
            changed |= ImGui.InputInt(AutoRotationUI.Input_HealDelay, ref cfg.HealerSettings.HealDelay);

            if (cfg.HealerSettings.HealDelay < 0)
                cfg.HealerSettings.HealDelay = 0;
            ImGuiComponents.HelpMarker(AutoRotationUI.HelpText_HealDelay);

            ImGui.Spacing();

            P.UIHelper.ShowIPCControlledIndicatorIfNeeded("AutoRez");
            changed |= P.UIHelper.ShowIPCControlledCheckboxIfNeeded(
                AutoRotationUI.Checkbox_AutoRez, ref cfg.HealerSettings.AutoRez, "AutoRez");
            ImGuiComponents.HelpMarker(
                Text.FormatAndCache(AutoRotationUI.HelpText_AutoRez,
                    Job.CNJ.Shorthand(), 
                    Job.WHM.Shorthand(),
                    Job.SCH.Shorthand(),
                    Job.AST.Shorthand(),
                    Job.SGE.Shorthand(),
                    // Occult Crescent Phantom Chemist Revive
                    OccultCrescent.ContentName, Svc.Data.GetExcelSheet<MKDSupportJob>().GetRow(10).Name, OccultCrescent.Revive.ActionName()
                )
            );
            var autoRez = (bool)P.IPC.GetAutoRotationConfigState(AutoRotationConfigOption.AutoRez)!;
            if (autoRez)
            {
                ImGuiExtensions.Prefix(false);
                P.UIHelper.ShowIPCControlledIndicatorIfNeeded("AutoRezOutOfParty");
                changed |= P.UIHelper.ShowIPCControlledCheckboxIfNeeded(
                    AutoRotationUI.Checkbox_AutoRezOutOfParty, ref cfg.HealerSettings.AutoRezOutOfParty, "AutoRezOutOfParty");

                ImGuiExtensions.Prefix(false);
                changed |= ImGui.Checkbox(
                    Text.FormatAndCache(
                                AutoRotationUI.Checkbox_AutoRezRequireSwift, 
                                RoleActions.Magic.Swiftcast.ActionName(), 
                                RDM.Buffs.Dualcast.StatusName()), 
                    ref cfg.HealerSettings.AutoRezRequireSwift);
                ImGuiComponents.HelpMarker(
                    Text.FormatAndCache(
                        AutoRotationUI.HelpText_AutoRezRequireSwift,
                        RoleActions.Magic.Swiftcast.ActionName(), Job.RDM.Shorthand(), RDM.Buffs.Dualcast.StatusName()
                    )
                );

                ImGuiExtensions.Prefix(true);
                P.UIHelper.ShowIPCControlledIndicatorIfNeeded("AutoRezDPSJobs");
                changed |= P.UIHelper.ShowIPCControlledCheckboxIfNeeded(
                    Text.FormatAndCache(
                        AutoRotationUI.Checkbox_AutoRezDPSJobs,
                        Job.SMN.Shorthand(),
                        Job.RDM.Shorthand()
                    ), ref cfg.HealerSettings.AutoRezDPSJobs, "AutoRezDPSJobs");
                ImGuiComponents.HelpMarker(
                    Text.FormatAndCache(
                        AutoRotationUI.HelpText_AutoRezDPSJobs,
                        Job.SMN.Shorthand(),
                        Job.RDM.Shorthand(),
                        Job.RDM.Shorthand(),
                        RoleActions.Magic.Buffs.Swiftcast.StatusName(),
                        RDM.Buffs.Dualcast.StatusName()
                    )
                );

                if (cfg.HealerSettings.AutoRezDPSJobs)
                {
                    ImGuiExtensions.Prefix(true);
                    P.UIHelper.ShowIPCControlledIndicatorIfNeeded("AutoRezDPSJobsHealersOnly");
                    changed |= P.UIHelper.ShowIPCControlledCheckboxIfNeeded(
                        AutoRotationUI.Checkbox_AutoRezDPSJobsHealersOnly, ref cfg.HealerSettings.AutoRezDPSJobsHealersOnly, "AutoRezDPSJobsHealersOnly");
                    ImGuiComponents.HelpMarker(
                        Text.FormatAndCache(
                            AutoRotationUI.HelpText_AutoRezDPSJobsHealersOnly,
                            Job.SMN.Shorthand(),
                            Job.RDM.Shorthand()
                        )
                    );
                }
            }

            P.UIHelper.ShowIPCControlledIndicatorIfNeeded("AutoCleanse");
            changed |= P.UIHelper.ShowIPCControlledCheckboxIfNeeded(
            Text.FormatAndCache(
                    AutoRotationUI.Checkbox_AutoCleanse,
                    RoleActions.Healer.Esuna.ActionName()), 
                ref cfg.HealerSettings.AutoCleanse, "AutoCleanse");
            ImGuiComponents.HelpMarker(
                Text.FormatAndCache(
                    AutoRotationUI.HelpText_AutoCleanse,
                    RoleActions.Healer.Esuna.ActionName())
            );

            P.UIHelper.ShowIPCControlledIndicatorIfNeeded("ManageKardia");
            changed |= P.UIHelper.ShowIPCControlledCheckboxIfNeeded(
                $"[{Job.SGE.Shorthand()}] Automatically Manage Kardia", ref cfg.HealerSettings.ManageKardia, "ManageKardia");
            ImGuiComponents.HelpMarker($"Switches {SGE.Kardia.ActionName()} to party members currently being targeted by enemies, prioritising tanks if multiple people are being targeted.");
            if (cfg.HealerSettings.ManageKardia)
            {
                ImGuiExtensions.Prefix(cfg.HealerSettings.ManageKardia);
                changed |= ImGui.Checkbox($"Limit {SGE.Kardia.ActionName()} swapping to tanks only", ref cfg.HealerSettings.KardiaTanksOnly);
            }

            changed |= ImGui.Checkbox($"[{Job.WHM.Shorthand()}/{Job.AST.Shorthand()}/{Job.SCH.Shorthand()}/{Job.SGE.Shorthand()}] Pre-emptively apply Heal Over Time/Shields on focus target", ref cfg.HealerSettings.PreEmptiveHoT);
            ImGuiComponents.HelpMarker($"Applies {WHM.Regen.ActionName()}/{AST.AspectedBenefic.ActionName()}/{SGE.EukrasianDiagnosis.ActionName()}/{SCH.Adloquium.ActionName()} to your focus target when out of combat and they are 30y or less away from an enemy. (Bypasses \"Only in Combat\" setting)");

            P.UIHelper.ShowIPCControlledIndicatorIfNeeded("IncludeNPCs");
            changed |= P.UIHelper.ShowIPCControlledCheckboxIfNeeded("Heal Friendly NPCs", ref cfg.HealerSettings.IncludeNPCs);
            ImGuiComponents.HelpMarker("Useful for healer quests where NPCs are expected to be healed but aren't added directly to your party.");

            changed |= P.UIHelper.ShowIPCControlledCheckboxIfNeeded("Always Set Hard Target###HealerHardTarget", ref cfg.HealerSettings.HealerAlwaysHardTarget, "HealerAlwaysHardTarget");

            ImGuiComponents.HelpMarker("Auto-rotation does not need to target allies to work, however with this setting enabled it will always set your hard target when it executes a heal.");

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