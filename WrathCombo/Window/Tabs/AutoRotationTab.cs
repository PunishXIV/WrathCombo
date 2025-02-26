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
using WrathCombo.Services.IPC_Subscriber;

namespace WrathCombo.Window.Tabs
{
    internal class AutoRotationTab : ConfigWindow
    {
        private static uint _selectedNpc = 0;
        internal static new void Draw()
        {
            ImGui.TextWrapped($"在这里，你可以配置自动循环的参数。带有“加入自动循环”的功能可用于自动循环");
            ImGui.Separator();

            var cfg = Service.Configuration.RotationConfig;
            bool changed = false;

            if (P.UIHelper.ShowIPCControlledIndicatorIfNeeded())
                changed |= P.UIHelper.ShowIPCControlledCheckboxIfNeeded(
                    "启用自动循环", ref cfg.Enabled);
            else
                changed |= ImGui.Checkbox($"启用自动循环", ref cfg.Enabled);
            if (P.IPC.GetAutoRotationState()) {
                var inCombatOnly = (bool)P.IPC.GetAutoRotationConfigState(
                    Enum.Parse<AutoRotationConfigOption>("InCombatOnly"))!;
                if (P.UIHelper.AutoRotationConfigControlled("InCombatOnly") is not null)
                    ImGuiExtensions.Prefix(false);
                P.UIHelper.ShowIPCControlledIndicatorIfNeeded("InCombatOnly");
                ImGuiExtensions.Prefix(!inCombatOnly);
                changed |= P.UIHelper.ShowIPCControlledCheckboxIfNeeded(
                    "只在战斗中启用", ref cfg.InCombatOnly, "InCombatOnly");

                if (inCombatOnly) {
                    ImGuiExtensions.Prefix(false);
                    changed |= ImGui.Checkbox($"在任务圈中始终启用自动循环", ref cfg.BypassQuest);
                    ImGuiComponents.HelpMarker("在任务圈外，脱战后会禁用自动模式");

                    ImGuiExtensions.Prefix(false);
                    changed |= ImGui.Checkbox($"在FATE圈中始终启用自动循环", ref cfg.BypassFATE);
                    ImGuiComponents.HelpMarker("没有和FATE同步等级时，脱战后会禁用自动模式");

                    ImGuiExtensions.Prefix(true);
                    ImGui.SetNextItemWidth(100f.Scale());
                    changed |= ImGui.InputInt("战斗开始后延迟激活自动循环的时间（秒）", ref cfg.CombatDelay);

                    if (cfg.CombatDelay < 0)
                        cfg.CombatDelay = 0;
                }
            }

            changed |= ImGui.Checkbox("进入副本后自动启用", ref cfg.EnableInInstance);
            changed |= ImGui.Checkbox("离开副本后自动关闭", ref cfg.DisableAfterInstance);

            if (ImGui.CollapsingHeader("伤害设置")) {
                ImGuiEx.TextUnderlined($"目标选择模式");

                P.UIHelper.ShowIPCControlledIndicatorIfNeeded("DPSRotationMode");
                changed |= P.UIHelper.ShowIPCControlledComboIfNeeded(
                    "###DPSTargetingMode", true, ref cfg.DPSRotationMode,
                    ref cfg.HealerRotationMode, "DPSRotationMode");

                ImGuiComponents.HelpMarker("手动模式 - 手动选择战斗目标\n" +
                    "最大HP最高 - 优先选择最高最大生命值的敌人\n" +
                    "最大HP最低 - 优先选择最低最大生命值的敌人\n" +
                    "当前HP最高 - 优先选择当前生命值最高的敌人\n" +
                    "当前HP最低 - 优先选择当前生命值最低的敌人\n" +
                    "坦克目标 - 优先选择你队里的T的目标\n" +
                    "最近优先 - 优先选择离你最近的目标\n" +
                    "最远优先 - 优先选择离你最远的目标");
                ImGui.Spacing();

                if (cfg.DPSRotationMode == AutoRotation.DPSRotationMode.手动模式) {
                    changed |= ImGui.Checkbox("强制选择最佳的丢AoE的目标", ref cfg.DPSSettings.AoEIgnoreManual);

                    ImGuiComponents.HelpMarker("对于所有其他目标选择模式，会自动选择能让AoE砸到最多人的目标。在手动模式下，只有勾选此框时才会执行此操作。");
                }

                var input = ImGuiEx.InputInt(100f.Scale(), "改打AoE所需的目标数量", ref cfg.DPSSettings.DPSAoETargets);
                if (input) {
                    changed |= input;
                    if (cfg.DPSSettings.DPSAoETargets < 0)
                        cfg.DPSSettings.DPSAoETargets = 0;
                }
                ImGuiComponents.HelpMarker($"禁用此选项将关闭AOE功能。\n若启用之，则只有当敌人数达到要求时才会使用AoE。此设置适用于所有职业，并适用于任何造成范围伤害的技能。");

                ImGui.SetNextItemWidth(100f.Scale());
                changed |= ImGui.SliderFloat("最远目标选择距离", ref cfg.DPSSettings.MaxDistance, 1, 30);
                cfg.DPSSettings.MaxDistance =
                    Math.Clamp(cfg.DPSSettings.MaxDistance, 1, 30);

                ImGuiComponents.HelpMarker("在所有目标选择模式中（手动模式除外），选取目标的最大距离。仅允许设置1到30之间的值。");

                P.UIHelper.ShowIPCControlledIndicatorIfNeeded("FATEPriority");
                changed |= P.UIHelper.ShowIPCControlledCheckboxIfNeeded(
                    "优先选择FATE目标", ref cfg.DPSSettings.FATEPriority, "FATEPriority");
                P.UIHelper.ShowIPCControlledIndicatorIfNeeded("QuestPriority");
                changed |= P.UIHelper.ShowIPCControlledCheckboxIfNeeded(
                    "优先选择任务目标", ref cfg.DPSSettings.QuestPriority, "QuestPriority");
                changed |= ImGui.Checkbox($"优先选择未处于战斗中的目标", ref cfg.DPSSettings.PreferNonCombat);

                if (cfg.DPSSettings.PreferNonCombat && changed)
                    cfg.DPSSettings.OnlyAttackInCombat = false;

                changed |= P.UIHelper.ShowIPCControlledCheckboxIfNeeded(
                    "只攻击已处于战斗中的目标", ref cfg.DPSSettings.OnlyAttackInCombat,
                    "OnlyAttackInCombat");

                if (cfg.DPSSettings.OnlyAttackInCombat && changed)
                    cfg.DPSSettings.PreferNonCombat = false;

                changed |= ImGui.Checkbox("不管你的下一个技能是什么，始终自动选取目标", ref cfg.DPSSettings.AlwaysSelectTarget);

                ImGuiComponents.HelpMarker("通常情况下，自动循环仅在下一次执行的技能需要目标时才会自动选择目标。勾选此选项后，自动循环将始终自动选择目标，无论技能需要什么目标。");


                var npcs = Service.Configuration.IgnoredNPCs.ToList();
                var selected = npcs.FirstOrNull(x => x.Key == _selectedNpc);
                var prev = selected is null ? "" : $"{Svc.Data.Excel.GetSheet<BNpcName>().GetRow(selected.Value.Value).Singular} (ID: {selected.Value.Key})";
                ImGuiEx.TextUnderlined($"被忽略的NPC");
                using (var combo = ImRaii.Combo("###Ignore", prev)) {
                    if (combo) {
                        if (ImGui.Selectable("")) {
                            _selectedNpc = 0;
                        }

                        foreach (var npc in npcs) {
                            var npcData = Svc.Data.Excel
                                .GetSheet<BNpcName>().GetRow(npc.Value);
                            if (ImGui.Selectable($"{npcData.Singular} (ID: {npc.Key})")) {
                                _selectedNpc = npc.Key;
                            }
                        }
                    }
                }
                ImGuiComponents.HelpMarker("这些NPC会被自动循环忽略\n" +
                                           "每一个此NPC的实例都会被从自动选择目标中排除（手动模式仍然有效）\n" +
                                           "要从此列表中移除一个NPC，请选择该NPC并点击下面的删除按钮\n" +
                                           "要将NPC添加到此列表中，请点击选择该NPC并使用命令：/wrath ignore");

                if (_selectedNpc > 0) {
                    if (ImGui.Button("从忽略列表中删除")) {
                        Service.Configuration.IgnoredNPCs.Remove(_selectedNpc);
                        Service.Configuration.Save();

                        _selectedNpc = 0;
                    }
                }

            }
            ImGui.Spacing();
            if (ImGui.CollapsingHeader("治疗选项设置")) {
                ImGuiEx.TextUnderlined($"治疗目标选择模式");
                P.UIHelper.ShowIPCControlledIndicatorIfNeeded("HealerRotationMode");
                changed |= P.UIHelper.ShowIPCControlledComboIfNeeded(
                    "###HealerTargetingMode", false, ref cfg.DPSRotationMode,
                    ref cfg.HealerRotationMode, "HealerRotationMode");
                ImGuiComponents.HelpMarker("手动模式 - 仅在手动选择目标时才会进行治疗。如果目标不符合下面的治疗阈值设置，将会跳过治疗，优先进行输出（如果输出功能已启用）\n" +
                    "当前HP最高 - 优先治疗当前生命值百分比最高的队友\n" +
                    "当前HP最低 - 优先治疗当前生命值百分比最低的队友");

                P.UIHelper.ShowIPCControlledIndicatorIfNeeded("SingleTargetHPP");
                changed |= P.UIHelper.ShowIPCControlledSliderIfNeeded(
                    "单奶HP％阈值", ref cfg.HealerSettings.SingleTargetHPP, "SingleTargetHPP");

                P.UIHelper.ShowIPCControlledIndicatorIfNeeded("SingleTargetRegenHPP");
                changed |= P.UIHelper.ShowIPCControlledSliderIfNeeded(
                    "单奶HP％阈值（目标拥有再生/阳星相位）", ref cfg.HealerSettings.SingleTargetRegenHPP, "SingleTargetRegenHPP");
                ImGuiComponents.HelpMarker("理论上你这条应该比上面那条设置得更低");

                P.UIHelper.ShowIPCControlledIndicatorIfNeeded("AoETargetHPP");
                changed |= P.UIHelper.ShowIPCControlledSliderIfNeeded(
                    "群奶HP％阈值", ref cfg.HealerSettings.AoETargetHPP, "AoETargetHPP");

                var input = ImGuiEx.InputInt(100f.Scale(), "群奶所需的目标数", ref cfg.HealerSettings.AoEHealTargetCount);
                if (input) {
                    changed |= input;
                    if (cfg.HealerSettings.AoEHealTargetCount < 0)
                        cfg.HealerSettings.AoEHealTargetCount = 0;
                }
                ImGuiComponents.HelpMarker($"禁用此选项将关闭群体治疗功能。只有达到群体治疗功所需的目标数量要求时才用。");
                ImGui.SetNextItemWidth(100f.Scale());
                changed |= ImGui.InputInt("在满足上述条件后开始治疗的延迟时间（秒）", ref cfg.HealerSettings.HealDelay);

                if (cfg.HealerSettings.HealDelay < 0)
                    cfg.HealerSettings.HealDelay = 0;
                ImGuiComponents.HelpMarker("不要将此数值设置的太高！通常设置成1-2秒，算是一个自然反应的延迟时间。");

                ImGui.Spacing();

                P.UIHelper.ShowIPCControlledIndicatorIfNeeded("AutoRez");
                changed |= P.UIHelper.ShowIPCControlledCheckboxIfNeeded(
                    "自动复活", ref cfg.HealerSettings.AutoRez, "AutoRez");
                ImGuiComponents.HelpMarker($"将尝试复活已死亡的队友。适用于 {WHM.ClassID.JobAbbreviation()}, {WHM.JobID.JobAbbreviation()}, {SCH.JobID.JobAbbreviation()}, {AST.JobID.JobAbbreviation()}, {SGE.JobID.JobAbbreviation()}");
                var autoRez = (bool)P.IPC.GetAutoRotationConfigState(AutoRotationConfigOption.AutoRez)!;
                if (autoRez) {
                    ImGuiExtensions.Prefix(false);
                    changed |= ImGui.Checkbox("需要即刻咏唱/连续咏唱", ref
                        cfg.HealerSettings.AutoRezRequireSwift);
                    ImGuiComponents.HelpMarker("为了避免读条复活，必须拥有即刻咏唱/连续咏唱才会进行复活队友");

                    ImGuiExtensions.Prefix(true);
                    P.UIHelper.ShowIPCControlledIndicatorIfNeeded("AutoRezDPSJobs");
                    changed |= P.UIHelper.ShowIPCControlledCheckboxIfNeeded(
                        $"应用于召唤和赤魔", ref cfg.HealerSettings.AutoRezDPSJobs, "AutoRezDPSJobs");
                    ImGuiComponents.HelpMarker($"召唤和赤魔当拥有即刻咏唱/连续咏唱时，才会去复活队友");
                }

                P.UIHelper.ShowIPCControlledIndicatorIfNeeded("AutoCleanse");
                changed |= P.UIHelper.ShowIPCControlledCheckboxIfNeeded(
                    $"自动康复", ref cfg.HealerSettings.AutoCleanse, "AutoCleanse");
                ImGuiComponents.HelpMarker($"将使用“康复”清除负面状态（治疗优先级更高）");

                P.UIHelper.ShowIPCControlledIndicatorIfNeeded("ManageKardia");
                changed |= P.UIHelper.ShowIPCControlledCheckboxIfNeeded(
                    $"【贤者】自动管理心关", ref cfg.HealerSettings.ManageKardia, "ManageKardia");
                ImGuiComponents.HelpMarker($"贤者自动心关正在被攻击的队友，在多个队友被攻击时，坦克优先");
                if (cfg.HealerSettings.ManageKardia) {
                    ImGuiExtensions.Prefix(cfg.HealerSettings.ManageKardia);
                    changed |= ImGui.Checkbox($"仅在坦克之间切换心关", ref cfg.HealerSettings.KardiaTanksOnly);
                }

                changed |= ImGui.Checkbox($"【白魔/占星】提前给焦点目标上HoT", ref cfg.HealerSettings.PreEmptiveHoT);
                ImGuiComponents.HelpMarker($"当焦点目标30码内有敌人且不在战斗中时，你将对其施放HoT技能。（比“只在战斗中启用”优先级高）");

                P.UIHelper.ShowIPCControlledIndicatorIfNeeded("IncludeNPCs");
                changed |= P.UIHelper.ShowIPCControlledCheckboxIfNeeded("治疗友方NPC", ref cfg.HealerSettings.IncludeNPCs);
                ImGuiComponents.HelpMarker("对于治疗的职业任务很有用，这些任务中需要治疗NPC，但NPC不会直接加入你的队伍。");

            }

            ImGuiEx.TextUnderlined("高级设置");
            changed |= ImGui.InputInt("延迟设置（毫秒）", ref cfg.Throttler);
            ImGuiComponents.HelpMarker("自动循环内置了一个限制器，以每隔多少毫秒运行一次，以提高性能。如果你遇到帧率问题，可以尝试增加这个数值。请注意，如果数值设置的过高，可能会导致技能延迟或卡顿，因此建议尝试不同的数值。");

            using (ImRaii.Disabled(!OrbwalkerIPC.IsEnabled)) {
                changed |= ImGui.Checkbox($"启用Orbwalker兼容", ref cfg.OrbwalkerIntegration);

                ImGuiComponents.HelpMarker($"开启后，自动循环将在移动时使用读条技能，因为Orbwalk将会在读条期间锁定角色移动。");
            }

            if (changed)
                Service.Configuration.Save();

        }
    }
}
