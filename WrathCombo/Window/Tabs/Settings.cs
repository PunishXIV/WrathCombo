using System;
using System.Linq;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System.Numerics;
using System.Reflection;
using System.Text.RegularExpressions;
using Dalamud.Interface;
using Dalamud.Interface.Colors;
using ECommons.GameHelpers;
using ECommons.ImGuiMethods;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Services;
using WrathCombo.Window.Functions;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;

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

                ImGuiEx.Spacing(new Vector2(0, 10));

                #region Open to PvE

                if (ImGui.Checkbox("Open Wrath to the PvE Features tab", ref
                        Service.Configuration.OpenToPvE))
                    Service.Configuration.Save();

                ImGuiComponents.HelpMarker("When you open Wrath with `/wrath`, it will open to the PvE Features tab, instead of the last tab you were on." +
                                           "\nSame as always using the `/wrath pve` command to open Wrath.");

                #endregion

                #region Open to PvP

                if (ImGui.Checkbox("Open Wrath to the PvP Features tab in PvP areas", ref
                        Service.Configuration.OpenToPvP))
                    Service.Configuration.Save();

                ImGuiComponents.HelpMarker("Same as above, when you open Wrath with `/wrath`, it will open to the PvP Features tab, instead of the last tab you were on, when in a PvP area." +
                                           "\nSimilar to using the `/wrath pvp` command to open Wrath.");

                ImGui.SameLine();
                ImGui.TextColored(ImGuiColors.DalamudGrey, $"(Will override the option above)");

                #endregion

                #region Open to Current Job

                if (ImGui.Checkbox("Open PvE Features UI to Current Job on Opening", ref Service.Configuration.OpenToCurrentJob))
                    Service.Configuration.Save();

                ImGuiComponents.HelpMarker("When you open Wrath's UI, if your last tab was PvE, it will automatically open to the job you are currently playing.");

                #endregion

                #region Open to Current Job on Switching

                if (ImGui.Checkbox("Open PvE Features UI to Current Job on Switching Jobs", ref Service.Configuration.OpenToCurrentJobOnSwitch))
                    Service.Configuration.Save();

                ImGuiComponents.HelpMarker("When you switch jobs, it will automatically switch the PvE Features tab to the job you are currently playing.");

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

                ImGuiComponents.HelpMarker("Controls whether Actions will be Intercepted Replaced with combos from the plugin.\nIf disabled, your manual presses of abilities will no longer be affected by your Wrath settings.\n\nAuto-Rotation will work regardless of the setting.\n\nControlled by the `/wrath combo` command.");

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
                    "\nValues over 500ms are NOT recommended." +
                    "\nDefault: 50");

                #endregion

                #region Movement Check Delay

                ImGui.PushItemWidth(75);
                if (ImGui.InputFloat("###MovementLeeway", ref Service.Configuration.MovementLeeway))
                    Service.Configuration.Save();

                ImGui.SameLine();
                ImGui.Text("seconds");

                ImGui.SameLine(pos);

                ImGui.Text($"   -   Movement Check Delay");

                ImGuiComponents.HelpMarker("Many features check if you are moving to decide actions, this will allow you to set a delay on how long you need to be moving before it recognizes you as moving.\nThis allows you to not have to worry about small movements affecting your rotation, primarily for casters.\n\nIt is recommended to keep this value between 0 and 1 seconds.\nDefault: 0.0");

                #endregion

                #region Opener Failure Timeout

                if (ImGui.InputFloat("###OpenerTimeout", ref Service.Configuration.OpenerTimeout))
                    Service.Configuration.Save();

                ImGui.SameLine();
                ImGui.Text("seconds");

                ImGui.SameLine(pos);

                ImGui.Text($"   -   Opener Failure Timeout");

                ImGuiComponents.HelpMarker("During an opener if this amount of time has passed since your last action, it will fail the opener and resume with non-opener functionality.\n\nIt is recommended to keep this value below 6.\nDefault: 4.0");

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

                ImGuiComponents.HelpMarker("Offset of melee check distance.\nFor those who don't want to immediately use their ranged attack if the boss walks slightly out of range.\n\nFor example, a value of -0.5 would make you have to be 0.5 yalms closer to the target,\nor a value of 2 would disable triggering of ranged features until you are 2 yalms further from the hitbox.\n\nIt is recommended to keep this value at 0.\nDefault: 0.0");
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

                ImGuiComponents.HelpMarker("The percentage of a total cast time to wait before interrupting.\nApplies to all interrupts, in every job's combos.\n\nIt is recommend to keep this value below 50%.\nDefault: 0%");

                #endregion

                #endregion

                #region Targeting Options

                ImGuiEx.Spacing(new Vector2(0, 20));
                ImGuiEx.TextUnderlined("Targeting Options");

                var useCusHealStack = Service.Configuration.UseCustomHealStack;

                #region Retarget ST Healing Actions

                bool retargetHealingActions =
                    Service.Configuration.RetargetHealingActionsToStack;
                if (ImGui.Checkbox("Retarget (Single Target) Healing Actions", ref retargetHealingActions))
                {
                    Service.Configuration.RetargetHealingActionsToStack =
                        retargetHealingActions;
                    Service.Configuration.Save();
                }

                ImGuiComponents.HelpMarker(
                    "This will retarget all single target healing actions to the Heal Stack as shown below,\nsimilarly to how Redirect or Reaction would.\nThis ensures that the target used to check HP% threshold logic for healing actions is the same target that will receive that heal.\n\nIt is recommended to enable this if you customize the Heal Stack at all.\nDefault: Off");
                Presets.DrawPossiblyRetargetedSymbol();

                #endregion

                ImGuiEx.Spacing(new Vector2(0, 10));

                #region Current Heal Stack

                ImGui.TextUnformatted("Current Heal Stack:");

                ImGuiComponents.HelpMarker(
                    "This is the order in which Wrath will try to select a healing target.\n\n" +
                    "If the 'Retarget Healing Actions' option is disabled, that is just the target that will be used for checking the HP threshold to trigger different healing actions to show up in their rotations.\n" +
                    "If the 'Retarget Healing Actions' option is enabled, that target is also the one that healing actions will be targeted onto (even when the action does not first check the HP of that target, like the combo's Replaced Action, for example).");

                var healStackText = "";
                var nextStackItemMarker = "   >   ";
                if (useCusHealStack)
                {
                    foreach (var item in Service.Configuration.CustomHealStack
                                 .Select((value, index) => new { value, index }))
                    {
                        healStackText += TargetDisplayNameFromPropertyName(item.value);
                        if (item.index < Service.Configuration.CustomHealStack.Length - 1)
                            healStackText += nextStackItemMarker;
                    }
                }
                else
                {
                    if (Service.Configuration.UseUIMouseoverOverridesInDefaultHealStack)
                        healStackText += "UI-MouseOver Target" + nextStackItemMarker;
                    if (Service.Configuration.UseFieldMouseoverOverridesInDefaultHealStack)
                        healStackText += "Field-MouseOver Target" + nextStackItemMarker;
                    healStackText += "Soft Target" + nextStackItemMarker;
                    healStackText += "Hard Target" + nextStackItemMarker;
                    if (Service.Configuration.UseFocusTargetOverrideInDefaultHealStack)
                        healStackText += "Focus Target" + nextStackItemMarker;
                    if (Service.Configuration.UseLowestHPOverrideInDefaultHealStack)
                        healStackText += "Lowest HP% Ally" + nextStackItemMarker;
                    healStackText += "Self";
                }
                ImGuiEx.Spacing(new Vector2(10, 0));
                ImGuiEx.TextWrapped(ImGuiColors.DalamudGrey, healStackText);

                ImGuiEx.Spacing(new Vector2(0, 10));

                #endregion

                #region Heal Stack Customization Options

                var labelText = "Heal Stack Customization Options";
                // Nest the Collapse into a Child of varying size, to be able to limit its width
                var dynamicHeight = _unCollapsed
                    ? _healStackCustomizationHeight
                    : ImGui.CalcTextSize("I").Y + 5f.Scale();
                ImGui.BeginChild("##HealStackCustomization",
                    new Vector2(ImGui.CalcTextSize(labelText).X * 2.2f, dynamicHeight),
                    false,
                    ImGuiWindowFlags.NoScrollbar);

                // Collapsing Header for the Heal Stack Customization Options
                _unCollapsed = ImGui.CollapsingHeader(labelText,
                    ImGuiTreeNodeFlags.SpanAvailWidth);
                var collapsibleHeight = ImGui.GetItemRectSize().Y;
                if (_unCollapsed)
                {
                    ImGui.BeginGroup();
                    #region Default Heal Stack Include: UI MouseOver

                    if (useCusHealStack) ImGui.BeginDisabled();

                    bool useUIMouseoverOverridesInDefaultHealStack =
                        Service.Configuration.UseUIMouseoverOverridesInDefaultHealStack;
                    if (ImGui.Checkbox("Add UI MouseOver to the Default Healing Stack", ref useUIMouseoverOverridesInDefaultHealStack))
                    {
                        Service.Configuration.UseUIMouseoverOverridesInDefaultHealStack =
                            useUIMouseoverOverridesInDefaultHealStack;
                        Service.Configuration.Save();
                    }

                    if (useCusHealStack) ImGui.EndDisabled();

                    ImGuiComponents.HelpMarker("This will add any UI MouseOver targets to the top of the Default Heal Stack, overriding the rest of the stack if you are mousing over any party member UI.\n\nIt is recommended to enable this if you are a keyboard+mouse user and enable Retarget Healing Actions (or have UI MouseOver targets in your Redirect/Reaction configuration).\nDefault: Off");

                    #endregion

                    #region Default Heal Stack Include: Field MouseOver

                    if (useCusHealStack) ImGui.BeginDisabled();

                    bool useFieldMouseoverOverridesInDefaultHealStack =
                        Service.Configuration.UseFieldMouseoverOverridesInDefaultHealStack;
                    if (ImGui.Checkbox("Add Field MouseOver to the Default Healing Stack", ref useFieldMouseoverOverridesInDefaultHealStack))
                    {
                        Service.Configuration.UseFieldMouseoverOverridesInDefaultHealStack =
                            useFieldMouseoverOverridesInDefaultHealStack;
                        Service.Configuration.Save();
                    }

                    if (useCusHealStack) ImGui.EndDisabled();

                    ImGuiComponents.HelpMarker("This will add any MouseOver targets to the top of the Default Heal Stack, overriding the rest of the stack if you are mousing over any nameplate UI or character model.\n\nIt is recommended to enable this only if you regularly intentionally use field mouseover targeting already.\nDefault: Off");

                    #endregion

                    #region Default Heal Stack Include: Focus Target

                    if (useCusHealStack) ImGui.BeginDisabled();

                    bool useFocusTargetOverrideInDefaultHealStack =
                        Service.Configuration.UseFocusTargetOverrideInDefaultHealStack;
                    if (ImGui.Checkbox("Add Focus Target to the Default Healing Stack", ref useFocusTargetOverrideInDefaultHealStack))
                    {
                        Service.Configuration.UseFocusTargetOverrideInDefaultHealStack =
                            useFocusTargetOverrideInDefaultHealStack;
                        Service.Configuration.Save();
                    }

                    if (useCusHealStack) ImGui.EndDisabled();

                    ImGuiComponents.HelpMarker("This will add your focus target under your hard and soft targets in the Default Heal Stack, overriding the rest of the stack if you have a living focus target.\n\nDefault: Off");

                    #endregion

                    #region Default Heal Stack Include: Lowest HP Ally

                    if (useCusHealStack) ImGui.BeginDisabled();

                    bool useLowestHPOverrideInDefaultHealStack =
                        Service.Configuration.UseLowestHPOverrideInDefaultHealStack;
                    if (ImGui.Checkbox("Add Lowest HP% Ally to the Default Healing Stack", ref useLowestHPOverrideInDefaultHealStack))
                    {
                        Service.Configuration.UseLowestHPOverrideInDefaultHealStack =
                            useLowestHPOverrideInDefaultHealStack;
                        Service.Configuration.Save();
                    }

                    if (useCusHealStack) ImGui.EndDisabled();

                    ImGuiComponents.HelpMarker("This will add a nearby party member with the lowest HP% to bottom of the Default Heal Stack, overriding only yourself.\n\nTHIS SHOULD BE USED WITH THE 'RETARGET HEALING ACTIONS' SETTING!\n\nDefault: Off");

                    if (useCusHealStack) ImGui.BeginDisabled();
                    if (useLowestHPOverrideInDefaultHealStack)
                    {
                        ImGuiEx.Spacing(new Vector2(30, 0));
                        ImGuiEx.Text(ImGuiColors.DalamudYellow, "This should be used with the 'Retarget Healing Actions' setting above!");
                    }
                    if (useCusHealStack) ImGui.EndDisabled();

                    #endregion

                    ImGuiEx.Spacing(new Vector2(5, 5));
                    ImGui.TextUnformatted("Or");
                    ImGuiEx.Spacing(new Vector2(0, 5));

                    #region Use Custom Heal Stack

                    bool useCustomHealStack = Service.Configuration.UseCustomHealStack;
                    if (ImGui.Checkbox("Use a Custom Heal Stack Instead", ref useCustomHealStack))
                    {
                        Service.Configuration.UseCustomHealStack = useCustomHealStack;
                        Service.Configuration.Save();
                    }

                    ImGuiComponents.HelpMarker("Select this if you would rather make your own stack of target priorities for Heal Targets instead of using our default stack.\n\nIt is recommended to use this to align with your Redirect/Reaction configuration if you're not using the Retarget Healing Actions setup; otherwise it is preference.\nDefault: Off");

                    #endregion

                    #region Custom Heal Stack Manager

                    if (Service.Configuration.UseCustomHealStack)
                    {
                        ImGui.Indent();
                        DrawCustomHealStackMaker();
                        ImGui.Unindent();
                    }

                    #endregion
                    ImGui.EndGroup();

                    // Get the max height of the section above
                    _healStackCustomizationHeight =
                        ImGui.GetItemRectSize().Y + collapsibleHeight + 5f.Scale();
                }

                ImGui.EndChild();

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
                        DebugFile.MakeDebugFile(allJobs: true);
                }

                ImGuiComponents.HelpMarker("Will generate a debug file on your desktop.\nUseful to give developers to help troubleshoot issues.\nThe same as using the following command: /wrath debug");

                #endregion

                #endregion
            }
        }

        #region Custom Heal Stack Manager Methods

        private static bool _unCollapsed;
        private static float _healStackCustomizationHeight = 0;
        private static string SimpleTargetItemToAddToCustomHealStack = "default";
        private static bool _iconGroupWidthSet;
        private static float _iconGroupWidth =
            ImGui.CalcTextSize("x").X;
        private static float _longestPropertyLabel =
            ImGui.CalcTextSize("Field-MouseOver Target").X;
        private static float _propertyHeight =
            ImGui.CalcTextSize("I").Y;

#pragma warning disable SYSLIB1045
        private static void DrawCustomHealStackMaker()
        {
            ImGuiEx.Spacing(new Vector2(5f.Scale(), 0));
            ImGui.Text("Add to the Stack:");
            ImGui.SameLine();
            DrawItemAdding();

            ImGuiComponents.HelpMarker("Click this dropdown to open the list of available Target options.\nClick any entry to add it to your Custom Heal Stack, at the bottom.\nThere is a Textbox that says 'Filter...' at the top, type into this to search the list.");

            ImGuiEx.Spacing(new Vector2(0, 5));

            #region Sizing Variables

            var currentStyle = ImGui.GetStyle();
            var widthModifiers = (currentStyle.ItemSpacing.X * 2) +
                                 (currentStyle.ItemInnerSpacing.X * 2);
            var width = _longestPropertyLabel + _iconGroupWidth +
                        widthModifiers;
            var height = (_propertyHeight * 5) +
                         (currentStyle.ItemSpacing.Y * 4 / 2) +
                         (currentStyle.ItemInnerSpacing.Y * 5 / 2) +
                         (currentStyle.WindowPadding.Y * 2);
            var size = new Vector2(width, height);

            #endregion

            const ImGuiWindowFlags flags = ImGuiWindowFlags.NoMove
                                           | ImGuiWindowFlags.NoResize;

            // Display the Custom Heal Stack
            using (ImRaii.Child("###CustomHealStackList", size, true, flags))
            {
                foreach (var item in Service.Configuration.CustomHealStack)
                {
                    var text = TargetDisplayNameFromPropertyName(item);
                    #region Sizing Variables

                    var areaWidth = ImGui.GetContentRegionAvail().X;
                    var textWidth = ImGui.CalcTextSize(text).X;
                    var dummyWidth = areaWidth - textWidth - _iconGroupWidth -
                                     widthModifiers / 2;
                    #endregion

                    ImGui.TextUnformatted(text);

                    ImGui.SameLine();
                    ImGui.Dummy(new Vector2(dummyWidth, 0));
                    ImGui.SameLine();

                    DrawPropertyControlGroup(item);
                }
            }

            ImGuiComponents.HelpMarker("The priority goes from top to bottom.\nScroll down to see all of your items.\nClick the Up and Down buttons to move items in the list.\nClick the X button to remove an item from the list.\n\nIf there are fewer than 4 items, and all return nothing when checked, will fall back to Self.\nThese targets will only be considered valid if they are friendly and within 25y.\n\nDefault: Focus Target > Hard Target > Self");

            // Utility
            GetButtonGroupSize();

            return;

            void DrawItemAdding()
            {
                #region Combo Variables

                var defaultLabel = "Select a Target to Add";
                var minSize = ImGui.CalcTextSize(defaultLabel).X;

                // List of ally-related SimpleTarget properties
                var simpleTargetProperties = typeof(SimpleTarget)
                    .GetProperties(BindingFlags.Public |
                                   BindingFlags.Static)
                    .Select(x => x.Name)
                    .Where(x => !x.Contains("Enemy") && !x.Contains("Attack") && !x.Contains("Dead"))
                    .Prepend("default")
                    .ToArray();

                // Put the ordered Party Member properties at the bottom
                var nonPartyMembers = simpleTargetProperties
                    .Where(name => !name.StartsWith("PartyMember"));
                var partyMembers = simpleTargetProperties
                    .Where(name => name.StartsWith("PartyMember"));
                var simpleTargets =
                    nonPartyMembers.Concat(partyMembers).ToArray();

                // Make Property names properly spaced and readable
                var simpleTargetNames =
                    simpleTargets.ToDictionary(
                        name => name,
                        TargetDisplayNameFromPropertyName
                    );

                // Save some data about the sizing of the text
                _longestPropertyLabel = simpleTargetNames
                    .Select(x => x.Value)
                    .Max(x => ImGui.CalcTextSize(x).X);
                _propertyHeight =
                    ImGui.CalcTextSize("I").Y > _propertyHeight
                        ? ImGui.CalcTextSize("I").Y
                        : _propertyHeight;

                #endregion

                ImGui.PushItemWidth(minSize + 40f.Scale());
                if (ImGuiEx.Combo(
                        "##CustomHealTargetStack",
                        ref SimpleTargetItemToAddToCustomHealStack,
                        simpleTargets,
                        names: simpleTargetNames
                    ))
                {
                    PluginConfiguration.AddHealStackItem(
                        SimpleTargetItemToAddToCustomHealStack);
                    SimpleTargetItemToAddToCustomHealStack = "default";
                }
            }

            void DrawPropertyControlGroup(string property)
            {
                using (ImRaii.Group())
                {
                    ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 0));
                    using (ImRaii.PushFont(UiBuilder.IconFont))
                    {
                        bool disable;
                        // Move Up Button
                        disable = Service.Configuration.CustomHealStack.First() == property;
                        if (disable)
                            ImGui.BeginDisabled();
                        if (ImGuiEx.IconButtonScaled(FontAwesomeIcon.CaretUp,
                                "customStack"+property+"up"))
                            PluginConfiguration.MoveHealStackItemUp(property);
                        if (disable)
                            ImGui.EndDisabled();

                        ImGui.SameLine();

                        // Move Down Button
                        disable = Service.Configuration.CustomHealStack.Last() == property;
                        if (disable)
                            ImGui.BeginDisabled();
                        if (ImGuiEx.IconButtonScaled(FontAwesomeIcon.CaretDown,
                                "customStack"+property+"down"))
                            PluginConfiguration.MoveHealStackItemDown(property);
                        if (disable)
                            ImGui.EndDisabled();

                        ImGui.SameLine();

                        // Delete Button
                        disable = Service.Configuration.CustomHealStack.Length <= 1;
                        if (disable)
                            ImGui.BeginDisabled();
                        if (ImGuiEx.IconButtonScaled(FontAwesomeIcon.Times,
                                "customStack"+property+"del"))
                            PluginConfiguration.RemoveHealStackItem(property);
                        if (disable)
                            ImGui.EndDisabled();
                    }
                    ImGui.PopStyleVar();
                }
            }

            void GetButtonGroupSize()
            {
                if (_iconGroupWidthSet) return;

                ImGui.SameLine();
                var transparent = new Vector4(0f, 0f, 0f, 0f);
                using (ImRaii.PushColor(ImGuiCol.Text, transparent))
                    DrawPropertyControlGroup("");

                _iconGroupWidth = ImGui.GetItemRectSize().X;
                _propertyHeight = ImGui.GetItemRectSize().Y > _propertyHeight
                    ? ImGui.GetItemRectSize().Y
                    : _propertyHeight;
                _iconGroupWidthSet = true;
            }
        }

        private static string TargetDisplayNameFromPropertyName (string propertyName)
        {
            return propertyName switch
            {
                "default" => "Select a Target to Add",
                // Handle special cases
                "UIMouseOverTarget" => "UI-MouseOver Target",
                "ModelMouseOverTarget" => "Field-MouseOver Target",
                "LowestHPAlly" => "Lowest HP Ally",
                "LowestHPPAlly" => "Lowest HP% Ally",
                // Format the rest with Regex
                _ => Regex.Replace(propertyName,
                    @"(?<=[a-z])(?=[A-Z0-9])", " "),
            };
        }
#pragma warning restore SYSLIB1045

        #endregion
    }
}
