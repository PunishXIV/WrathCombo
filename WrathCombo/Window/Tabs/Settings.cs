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
            using (ImRaii.Child("main", new Vector2(0, 0), true)) {
                ImGui.Text("这个标签允许你自定义Wrath Combo的选项.");

                #region UI Options


                ImGuiEx.Spacing(new Vector2(0, 20));
                ImGuiEx.TextUnderlined("主界面UI设置");

                #region SubCombos

                var hideChildren = Service.Configuration.HideChildren;

                if (ImGui.Checkbox("隐藏子连招选项", ref hideChildren)) {
                    Service.Configuration.HideChildren = hideChildren;
                    Service.Configuration.Save();
                }

                ImGuiComponents.HelpMarker("隐藏已禁用的子选项");

                #endregion

                #region Conflicting

                bool hideConflicting = Service.Configuration.HideConflictedCombos;
                if (ImGui.Checkbox("隐藏冲突的连招", ref hideConflicting)) {
                    Service.Configuration.HideConflictedCombos = hideConflicting;
                    Service.Configuration.Save();
                }

                ImGuiComponents.HelpMarker("隐藏所有与已选连招冲突的连招选项");

                #endregion

                #region Open to Current Job

                if (ImGui.Checkbox("打开PvE功能界面时自动切换到当前职业", ref Service.Configuration.OpenToCurrentJob))
                    Service.Configuration.Save();

                ImGuiComponents.HelpMarker("当你打开Wrath的用户界面时，它将自动切换到当前职业");

                if (ImGui.Checkbox("切换职业时，自动打开当前职业的PvE功能界面", ref Service.Configuration.OpenToCurrentJobOnSwitch))
                    Service.Configuration.Save();

                ImGuiComponents.HelpMarker("当你切换职业时，界面将自动打开并切换到你的当前职业");

                #endregion

                #region Shorten DTR bar text

                bool shortDTRText = Service.Configuration.ShortDTRText;

                if (ImGui.Checkbox("缩短在服务器信息栏显示的文本", ref shortDTRText)) {
                    Service.Configuration.ShortDTRText = shortDTRText;
                    Service.Configuration.Save();
                }

                ImGuiComponents.HelpMarker(
                    "默认情况下，Wrath Combo的服务器信息栏会显示自动循环是否开启" +
                    "\n如果启用，会显示启用了多少个自动循环的数量 " +
                    "\n最后，它还会显示是否有其他插件在控制该自动循环" +
                    "\n该选项将隐藏已经启用的自动循环的数量"
                );

                #endregion

                #region Message of the Day

                bool motd = Service.Configuration.HideMessageOfTheDay;

                if (ImGui.Checkbox("隐藏每日消息", ref motd)) {
                    Service.Configuration.HideMessageOfTheDay = motd;
                    Service.Configuration.Save();
                }

                ImGuiComponents.HelpMarker("隐藏登录时在聊天框中显示的每日消息");

                #endregion

                #region TargetHelper

                Vector4 colour = Service.Configuration.TargetHighlightColor;
                if (ImGui.ColorEdit4("目标高亮颜色设置", ref colour, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.AlphaPreview | ImGuiColorEditFlags.AlphaBar)) {
                    Service.Configuration.TargetHighlightColor = colour;
                    Service.Configuration.Save();
                }

                ImGuiComponents.HelpMarker("在小队列表中的该目标队员周围绘制一个框，以标明他是某些功能中所选择的目标。\n（将透明度设置为0以隐藏该框）");

                ImGui.SameLine();
                ImGui.TextColored(ImGuiColors.DalamudGrey, $"（目前只有占星会用这个）");

                #endregion

                #endregion

                #region Rotation Behavior Options


                ImGuiEx.Spacing(new Vector2(0, 20));
                ImGuiEx.TextUnderlined("自动循环选项");

                #region Performance Mode

                if (ImGui.Checkbox("性能模式", ref Service.Configuration.PerformanceMode))
                    Service.Configuration.Save();

                ImGuiComponents.HelpMarker("此模式将禁用热键栏上技能的变更，但在你按下技能时，仍会在后台继续工作。");

                #endregion

                #region Spells while Moving

                if (ImGui.Checkbox("移动时禁止读条", ref Service.Configuration.BlockSpellOnMove))
                    Service.Configuration.Save();

                ImGuiComponents.HelpMarker("当你在移动时，完全禁止使用读条技能，这是通过把读条技能变成“雪仇剑”来实现的。\n这将取代针对大多数职业所具备的特定连招的移动选项。");

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
                        ref throttle, 0, 0)) {
                    Service.Configuration.Throttle = Math.Clamp(throttle, 0, 1500);
                    Service.Configuration.Save();
                }

                ImGui.SameLine();
                var pos = ImGui.GetCursorPosX() + len;
                ImGui.Text($"毫秒");
                ImGui.SameLine(pos);
                ImGui.Text($"   -   技能刷新频率限制");


                ImGuiComponents.HelpMarker(
                    "此条限制本插件对你的热键栏里的技能的刷新频率。" +
                    "\n默认情况下，这不会限制连招，所以你始终会施放最新的该放的技能。" +
                    "\n\n如果你有轻微的帧率问题，可以增加该数值，以减少连招的刷新频率" +
                    "\n这会使你的连招反应变慢，甚至可能导致GCD出现延迟或卡顿，" +
                    "\n在较高的数值下，这可能会完全破坏你的输出循环" +
                    "\n更严重的帧率问题应通过下面的性能模式来解决" +
                    "\n\n200毫秒的延迟可能会对你的帧率产生明显的影响" +
                    "\n不建议设置超过500毫秒。");

                #endregion

                #region Movement Check Delay

                ImGui.PushItemWidth(75);
                if (ImGui.InputFloat("###MovementLeeway", ref Service.Configuration.MovementLeeway))
                    Service.Configuration.Save();

                ImGui.SameLine();
                ImGui.Text("秒");

                ImGui.SameLine(pos);

                ImGui.Text($"   -   移动检测延迟");

                ImGuiComponents.HelpMarker("许多功能检测会检测你是否在移动来决定下一步指令，这个选项允许你设置延迟数值，来决定在多长时间的移动后才会被识别为“正在移动”。\n这可以让你不必担心小幅度的移动影响自动输出循环，主要适用于需要读条的职业。\n\n建议将此数值设置在0到1秒之间。");

                #endregion

                #region Opener Failure Timeout

                if (ImGui.InputFloat("###OpenerTimeout", ref Service.Configuration.OpenerTimeout))
                    Service.Configuration.Save();

                ImGui.SameLine();
                ImGui.Text("秒");

                ImGui.SameLine(pos);

                ImGui.Text($"   -   开场爆发超时时间");

                ImGuiComponents.HelpMarker("在开场爆发中，如果在你的上一个技能之后过了这么多秒你都没打新技能，那就判定为开场爆发失败，接着会让你继续打一般情况下的技能。");

                #endregion

                #region Melee Offset
                var offset = (float)Service.Configuration.MeleeOffset;

                if (ImGui.InputFloat("###MeleeOffset", ref offset)) {
                    Service.Configuration.MeleeOffset = (double)offset;
                    Service.Configuration.Save();
                }

                ImGui.SameLine();
                ImGui.Text($"星码");
                ImGui.SameLine(pos);

                ImGui.Text($"   -   近战距离偏移");

                ImGuiComponents.HelpMarker("近战距离检查的偏移。\n适用于那些不想一出近战就立刻打远程技能的人。\n\n举个例子：如果你设置为-0.5，那么你就得更靠近目标0.5星码。\n反之如果你设置为2，那么你的近战范围检测会比原来远2星码（只是检测，实际施法距离不会增加）。\n\n推荐让此条保持为他的默认值0。");
                #endregion

                #region Interrupt Delay

                var delay = (int)(Service.Configuration.InterruptDelay * 100d);

                if (ImGui.SliderInt("###InterruptDelay",
                    ref delay, 0, 100)) {
                    delay = delay.RoundOff(SliderIncrements.Fives);

                    Service.Configuration.InterruptDelay = ((double)delay) / 100d;
                    Service.Configuration.Save();
                }
                ImGui.SameLine();
                ImGui.Text($"咏唱百分比");
                ImGui.SameLine(pos);
                ImGui.Text($"   -   打断延迟");

                ImGuiComponents.HelpMarker("等读条读了这么多百分比后再打断它。\n适用于所有职业的所有的打断技能。\n\n推荐令此值低于50%。");

                #endregion

                #endregion

                #region Troubleshooting Options


                ImGuiEx.Spacing(new Vector2(0, 20));
                ImGuiEx.TextUnderlined("找Bug/分析选项");

                #region Combat Log

                bool showCombatLog = Service.Configuration.EnabledOutputLog;

                if (ImGui.Checkbox("把日志输出到聊天框", ref showCombatLog)) {
                    Service.Configuration.EnabledOutputLog = showCombatLog;
                    Service.Configuration.Save();
                }

                ImGuiComponents.HelpMarker("每当你用了一个技能后，本插件会把这个技能输出到聊天框。");
                #endregion

                #region Opener Log

                if (ImGui.Checkbox($"把开场爆发状态输出到聊天框", ref Service.Configuration.OutputOpenerLogs))
                    Service.Configuration.Save();

                ImGuiComponents.HelpMarker("每当你当前职业的开场爆发就绪/失败/如期结束后，本插件会把结果输出到聊天框。");
                #endregion

                #region Debug File

                if (ImGui.Button("建立调试文件")) {
                    if (Player.Available)
                        DebugFile.MakeDebugFile();
                    else
                        DebugFile.MakeDebugFile(allJobs: true);
                }

                ImGuiComponents.HelpMarker("将生成一个调试文件到你的桌面。\n对开发者找Bug很有用。\n可以用下面这个命令等效之：/wrath debug");

                #endregion

                #endregion
            }
        }
    }
}
