using Dalamud.Interface.Colors;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.Extensions.UIntExtensions;
using static WrathCombo.Window.Functions.SliderIncrements;
using static WrathCombo.Window.Functions.UserConfig;
namespace WrathCombo.Combos.PvE;

internal partial class AST
{
    public static class Config
    {
        #region Options
        public static UserIntArray
            AST_ST_SimpleHeals_Priority = new("AST_ST_SimpleHeals_Priority", [12,11,10,9,8,6,7,5,4,3,2,1]),
            AST_AoE_SimpleHeals_Priority = new("AST_AoE_SimpleHeals_Priority", [3, 6, 1, 4, 7, 2, 8, 9, 5]);
        
        public static UserInt
            //HEALS
            AST_ST_SimpleHeals_Spire = new("AST_ST_SimpleHeals_Spire", 70),
            AST_ST_SimpleHeals_Ewer = new("AST_ST_SimpleHeals_Ewer", 70),
            AST_ST_SimpleHeals_Arrow = new("AST_ST_SimpleHeals_Arrow", 70),
            AST_ST_SimpleHeals_Bole = new("AST_ST_SimpleHeals_Bole", 70),
            AST_ST_SimpleHeals_CelestialIntersection = new("AST_ST_SimpleHeals_CelestialIntersection", 70),
            AST_ST_SimpleHeals_CelestialIntersectionCharges = new ("AST_ST_SimpleHeals_CelestialIntersectionCharges", 1),
            AST_ST_SimpleHeals_EssentialDignity = new("AST_ST_SimpleHeals_EssentialDignity", 70),
            AST_ST_SimpleHeals_Exaltation = new("AST_ST_SimpleHeals_Exaltation", 70),
            AST_ST_SimpleHeals_Esuna = new("AST_ST_SimpleHeals_Esuna", 40),
            AST_ST_SimpleHeals_AspectedBeneficHigh = new("AST_ST_SimpleHeals_AspectedBeneficHigh", 100),
            AST_ST_SimpleHeals_AspectedBeneficLow = new("AST_ST_SimpleHeals_AspectedBeneficLow", 40),
            AST_ST_SimpleHeals_AspectedBeneficRefresh = new("AST_ST_SimpleHeals_AspectedBeneficRefresh", 3),
            AST_ST_SimpleHeals_CollectiveUnconscious = new("AST_ST_SimpleHeals_CollectiveUnconscious", 70),
            AST_ST_SimpleHeals_CelestialOpposition = new("AST_ST_SimpleHeals_CelestialOpposition", 70),
            AST_ST_SimpleHeals_SoloLady = new("AST_ST_SimpleHeals_SoloLady", 70),
            AST_ST_SimpleHeals_EmergencyED_Threshold = new("AST_ST_SimpleHeals_EmergencyED_Threshold", 30),
            AST_AoE_SimpleHeals_AltMode = new("AST_AoE_SimpleHeals_AltMode", 1),
            AST_AoE_SimpleHeals_LazyLady = new("AST_AoE_SimpleHeals_LazyLady", 80),
            AST_AoE_SimpleHeals_Horoscope = new("AST_AoE_SimpleHeals_Horoscope", 80),
            AST_AoE_SimpleHeals_CelestialOpposition = new("AST_AoE_SimpleHeals_CelestialOpposition", 80),
            AST_AoE_SimpleHeals_CollectiveUnconscious = new("AST_AoE_SimpleHeals_CollectiveUnconscious", 80),
            AST_AoE_SimpleHeals_NeutralSect = new("AST_AoE_SimpleHeals_NeutralSect", 80),
            AST_AoE_SimpleHeals_HoroscopeHeal = new("AST_AoE_SimpleHeals_HoroscopeHeal", 80),
            AST_AoE_SimpleHeals_StellarDetonation = new("AST_AoE_SimpleHeals_StellarDetonation", 80),
            AST_AoE_SimpleHeals_Aspected = new("AST_AoE_SimpleHeals_Aspected", 80),
            AST_AoE_SimpleHeals_Helios = new("AST_AoE_SimpleHeals_Helios", 80),
            AST_Mit_ST_EssentialDignityThreshold = new("AST_Mit_ST_EssentialDignityThreshold", 80),
            
            //DPS
            AST_ST_DPS_DivinationOption = new("AST_ST_DPS_DivinationOption"),
            AST_ST_DPS_AltMode = new("AST_ST_DPS_AltMode"),
            AST_ST_DPS_LucidDreaming = new("AST_ST_DPS_LucidDreaming", 8000),
            AST_ST_DPS_LightSpeedOption = new("AST_ST_DPS_LightSpeedOption"),
            AST_ST_DPS_CombustOption = new("AST_ST_DPS_CombustOption"),
            AST_ST_DPS_DivinationSubOption = new("AST_ST_DPS_DivinationSubOption", 0),
            AST_ST_DPS_Balance_Content = new("AST_ST_DPS_Balance_Content", 1),
            AST_ST_DPS_CombustSubOption = new("AST_ST_DPS_CombustSubOption", 0),
            AST_ST_DPS_StellarDetonation_Threshold = new("AST_ST_DPS_StellarDetonation_Threshold", 0),
            AST_ST_DPS_StellarDetonation_SubOption = new("AST_ST_DPS_StellarDetonation_SubOption", 0),
            AST_AOE_LucidDreaming = new("AST_AOE_LucidDreaming", 8000),
            AST_AOE_DivinationSubOption = new("AST_AOE_DivinationSubOption", 0),
            AST_AOE_DivinationOption = new("AST_AOE_DivinationOption"),
            AST_AOE_LightSpeedOption = new("AST_AOE_LightSpeedOption"),
            AST_AOE_DPS_StellarDetonation_Threshold = new("AST_AOE_DPS_StellarDetonation_Threshold", 0),
            AST_AOE_DPS_StellarDetonation_SubOption = new("AST_AOE_DPS_StellarDetonation_SubOption", 0),
            AST_AOE_DPS_MacroCosmos_SubOption = new("AST_AOE_DPS_MacroCosmos_SubOption", 0),
            AST_AOE_DPS_DoT_HPThreshold = new("AST_AOE_DPS_DoT_HPThreshold", 30),
            AST_AOE_DPS_DoT_MaxTargets = new("AST_AOE_DPS_DoT_MaxTargets", 4),
            AST_QuickTarget_Override = new("AST_QuickTarget_Override", 0);

        public static UserBool
            //HEALS
            AST_ST_SimpleHeals_IncludeShields = new("AST_ST_SimpleHeals_IncludeShields"),
            AST_ST_SimpleHeals_WeaveDignity = new("AST_ST_SimpleHeals_WeaveDignity"),
            AST_ST_SimpleHeals_WeaveIntersection = new("AST_ST_SimpleHeals_WeaveIntersection"),
            AST_ST_SimpleHeals_WeaveEwer = new("AST_ST_SimpleHeals_WeaveEwer"),
            AST_ST_SimpleHeals_WeaveSpire = new("AST_ST_SimpleHeals_WeaveSpire"),
            AST_ST_SimpleHeals_WeaveEmergencyED = new("AST_ST_SimpleHeals_WeaveEmergencyED"),
            AST_AoE_SimpleHeals_WeaveLady = new("AST_AoE_SimpleHeals_WeaveLady"),
            AST_AoE_SimpleHeals_WeaveOpposition = new("AST_AoE_SimpleHeals_WeaveOpposition"),
            AST_AoE_SimpleHeals_WeaveCollectiveUnconscious = new("AST_AoE_SimpleHeals_WeaveCollectiveUnconscious"),
            AST_AoE_SimpleHeals_WeaveHoroscope = new("AST_AoE_SimpleHeals_WeaveHoroscope"),
            AST_AoE_SimpleHeals_WeaveNeutralSect = new("AST_AoE_SimpleHeals_WeaveNeutralSect"),
            AST_AoE_SimpleHeals_WeaveHoroscopeHeal = new("AST_AoE_SimpleHeals_WeaveHoroscopeHeal"),
            AST_AoE_SimpleHeals_WeaveStellarDetonation = new("AST_AoE_SimpleHeals_WeaveStellarDetonation"),
            //DPS
            AST_ST_DPS_OverwriteHealCards = new("AST_ST_DPS_OverwriteHealCards"),
            AST_AOE_DPS_OverwriteHealCards = new("AST_AOE_DPS_OverwriteHealCards"),
            AST_QuickTarget_Manuals = new("AST_QuickTarget_Manuals", true);
        public static UserFloat
            AST_AOE_DPS_DoT_Reapply = new ("AST_AOE_DPS_DoT_Reapply", 2),
            AST_ST_DPS_CombustUptime_Threshold = new("AST_ST_DPS_CombustUptime_Threshold");

        public static UserBoolArray
            AST_ST_SimpleHeals_CelestialOppositionOptions = new("AST_ST_SimpleHeals_CelestialOppositionOptions"),
            AST_ST_SimpleHeals_CollectiveUnconsciousOptions = new("AST_ST_SimpleHeals_CollectiveUnconsciousOptions"),
            AST_ST_SimpleHeals_SoloLadyOptions = new("AST_ST_SimpleHeals_SoloLadyOptions"),
            AST_ST_SimpleHeals_ExaltationOptions = new("AST_ST_SimpleHeals_ExaltationOptions"),
            AST_ST_SimpleHeals_BoleOptions = new("AST_ST_SimpleHeals_BoleOptions"),
            AST_ST_SimpleHeals_ArrowOptions = new("AST_ST_SimpleHeals_ArrowOptions"),
            AST_Mit_ST_Options = new("AST_Mit_ST_Options"),
            AST_EarthlyStarOptions = new("AST_EarthlyStarOptions");

        #endregion
        internal static void Draw(Preset preset)
        {
            switch (preset)
            {
                #region DPS
                case Preset.AST_ST_DPS_Opener:
                    DrawBossOnlyChoice(AST_ST_DPS_Balance_Content);
                    break;

                case Preset.AST_ST_DPS:
                    DrawRadioButton(AST_ST_DPS_AltMode, $"On {Malefic.ActionName()}", "", 0);
                    DrawRadioButton(AST_ST_DPS_AltMode, $"On {Combust.ActionName()}", $"Alternative DPS Mode. Leaves {Malefic.ActionName()} alone for pure DPS, becomes {Malefic.ActionName()} when features are on cooldown", 1);
                    break;

                case Preset.AST_DPS_Lucid:
                    DrawSliderInt(4000, 9500, AST_ST_DPS_LucidDreaming, "Set value for your MP to be at or under for this feature to work", 150, Hundreds);
                    break;

                case Preset.AST_ST_DPS_CombustUptime:
                    DrawSliderInt(0, 50, AST_ST_DPS_CombustOption, "Stop using at Enemy HP %. Set to Zero to disable this check.");
                    ImGui.Indent();
                    ImGui.TextColored(ImGuiColors.DalamudYellow, "Select what kind of enemies the HP check should be applied to:");
                    DrawHorizontalRadioButton(AST_ST_DPS_CombustSubOption,
                        "Non-Bosses", "Only applies the HP check above to non-bosses.\nAllows you to only stop DoTing early when it's not a boss.", 0);
                    DrawHorizontalRadioButton(AST_ST_DPS_CombustSubOption,
                        "All Enemies", "Applies the HP check above to all enemies.", 1);
                    DrawRoundedSliderFloat(0, 4, AST_ST_DPS_CombustUptime_Threshold, "Seconds remaining before reapplying the DoT. Set to Zero to disable this check.", digits: 1);
                    ImGui.Unindent();
                    break;

                case Preset.AST_DPS_Divination:
                    DrawSliderInt(0, 100, AST_ST_DPS_DivinationOption, "Stop using at Enemy HP %. Set to Zero to disable this check.");
                    ImGui.Indent();
                    ImGui.TextColored(ImGuiColors.DalamudYellow, "Select what kind of enemies the HP check should be applied to:");
                    DrawHorizontalRadioButton(AST_ST_DPS_DivinationSubOption,
                        "Non-Bosses", "Only applies the HP check above to non-bosses.", 0);
                    DrawHorizontalRadioButton(AST_ST_DPS_DivinationSubOption,
                        "All Enemies", "Applies the HP check above to all enemies.", 1);
                    ImGui.Unindent();
                    break;

                case Preset.AST_DPS_LightSpeed:
                    DrawSliderInt(0, 100, AST_ST_DPS_LightSpeedOption, "Stop using at Enemy HP %. Set to Zero to disable this check.");
                    break;
                
                case Preset.AST_DPS_AutoDraw:
                    DrawAdditionalBoolChoice(AST_ST_DPS_OverwriteHealCards, "Overwrite Non-DPS Cards", "Will draw even if you have healing cards remaining.");
                    break;
                
                case Preset.AST_ST_DPS_StellarDetonation:
                    DrawHorizontalRadioButton(AST_ST_DPS_StellarDetonation_SubOption,
                        "Non-boss Encounters Only", $"Non-Boss Encounters only", 0);

                    DrawHorizontalRadioButton(AST_ST_DPS_StellarDetonation_SubOption,
                        "All Content", $"All Content", 1);
                    
                    DrawSliderInt(0, 100, AST_ST_DPS_StellarDetonation_Threshold,
                        $"Use when Target is at or below HP% (0% = Never Detonate Early, 100% = Detonate ASAP).");
                    break;
                
                case Preset.AST_AOE_Lucid:
                    DrawSliderInt(4000, 9500, AST_AOE_LucidDreaming, "Set value for your MP to be at or under for this feature to work", 150, Hundreds);
                    break;

                case Preset.AST_AOE_Divination:
                    DrawSliderInt(0, 100, AST_AOE_DivinationOption, "Stop using at Enemy HP %. Set to Zero to disable this check.");
                    ImGui.Indent();
                    ImGui.TextColored(ImGuiColors.DalamudYellow, "Select what kind of enemies the HP check should be applied to:");
                    DrawHorizontalRadioButton(AST_AOE_DivinationSubOption,
                        "Non-Bosses", "Only applies the HP check above to non-bosses.", 0);
                    DrawHorizontalRadioButton(AST_AOE_DivinationSubOption,
                        "All Enemies", "Applies the HP check above to all enemies.", 1);
                    ImGui.Unindent();
                    break;

                case Preset.AST_AOE_LightSpeed:
                    DrawSliderInt(0, 100, AST_AOE_LightSpeedOption, "Stop using at Enemy HP %. Set to Zero to disable this check.");
                    break;

                case Preset.AST_AOE_AutoDraw:
                    DrawAdditionalBoolChoice(AST_AOE_DPS_OverwriteHealCards, "Overwrite Non-DPS Cards", "Will draw even if you have healing cards remaining.");
                    break;
                
                case Preset.AST_AOE_DPS_StellarDetonation:
                    DrawHorizontalRadioButton(AST_AOE_DPS_StellarDetonation_SubOption,
                        "Non-boss Encounters Only", $"Non-Boss Encounters only", 0);

                    DrawHorizontalRadioButton(AST_AOE_DPS_StellarDetonation_SubOption,
                        "All Content", $"All Content", 1);
                    
                    DrawSliderInt(0, 100, AST_AOE_DPS_StellarDetonation_Threshold,
                        $"Use when Target is at or below HP% (0% = Never Detonate Early, 100% = Detonate ASAP).");
                    break;

                case Preset.AST_AOE_DPS_MacroCosmos:
                    DrawHorizontalRadioButton(AST_AOE_DPS_MacroCosmos_SubOption, "Non-boss Encounters Only", $"Will not use on bosses", 0);
                    DrawHorizontalRadioButton(AST_AOE_DPS_MacroCosmos_SubOption, "All Content", $"Will use in all content", 1);
                    break;
                
                case Preset.AST_AOE_DPS_DoT:
                    DrawSliderInt(0, 100, AST_AOE_DPS_DoT_HPThreshold, "Target HP% to stop using (0 = Use Always, 100 = Never)");
                    ImGui.Indent();
                    DrawRoundedSliderFloat(0, 5, AST_AOE_DPS_DoT_Reapply,  "Seconds remaining before reapplying (0 = Do not reapply early)", digits: 1);
                    ImGui.Unindent();
                    DrawSliderInt(0, 10, AST_AOE_DPS_DoT_MaxTargets, "Maximum number of targets to employ multi-dotting ");
                    break;

                #endregion
                
                #region ST Heals
                case Preset.AST_ST_Heals:
                    DrawAdditionalBoolChoice(AST_ST_SimpleHeals_IncludeShields, "Include Shields in HP Percent Sliders", "");
                    break;
                
                case Preset.AST_ST_Heals_Esuna:
                    DrawSliderInt(0, 100, AST_ST_SimpleHeals_Esuna, "Stop using when below HP %. Set to Zero to disable this check");
                    break;
                
                case Preset.AST_ST_Heals_CelestialIntersection:
                    DrawSliderInt(0, 100, AST_ST_SimpleHeals_CelestialIntersection, "Start using when below HP %. Set to 100 to disable this check");
                    DrawSliderInt(0, 1, AST_ST_SimpleHeals_CelestialIntersectionCharges, "How many charges to retain for manual use. Set to 0 to disable this check.");
                    DrawAdditionalBoolChoice(AST_ST_SimpleHeals_WeaveIntersection, "Only Weave", "");
                    DrawPriorityInput(AST_ST_SimpleHeals_Priority, 12, 1, $"{CelestialIntersection.ActionName()} Priority: ");
                    break;
                
                case Preset.AST_ST_Heals_EssentialDignity:
                    DrawSliderInt(0, 100, AST_ST_SimpleHeals_EssentialDignity, "Start using when below HP %. Set to 100 to disable this check");
                    DrawAdditionalBoolChoice(AST_ST_SimpleHeals_WeaveDignity, "Only Weave", "");
                    DrawPriorityInput(AST_ST_SimpleHeals_Priority, 12, 0, $"Standard {EssentialDignity.ActionName()} Priority: ");
                    break;
                
                case Preset.AST_ST_Heals_EssentialDignity_Emergency:
                    DrawSliderInt(0, 100, AST_ST_SimpleHeals_EmergencyED_Threshold, "Start using when below HP %. Set to 100 to disable this check");
                    DrawAdditionalBoolChoice(AST_ST_SimpleHeals_WeaveEmergencyED, "Only Weave", "");
                    DrawPriorityInput(AST_ST_SimpleHeals_Priority, 12, 11, $"Emergency {EssentialDignity.ActionName()} Priority:");
                    break;
                
                case Preset.AST_ST_Heals_Exaltation:
                    DrawSliderInt(0, 100, AST_ST_SimpleHeals_Exaltation, "Start using when below HP %. Set to 100 to disable this check");
                    DrawHorizontalMultiChoice(AST_ST_SimpleHeals_ExaltationOptions, "Only Weave", "Will only weave this action.", 3, 0);
                    DrawHorizontalMultiChoice(AST_ST_SimpleHeals_ExaltationOptions," Not On Bosses", "Will not use in Boss encounters.", 3, 1);
                    DrawHorizontalMultiChoice(AST_ST_SimpleHeals_ExaltationOptions," Tanks Only", "Will only use on Tanks", 3, 2);
                    DrawPriorityInput(AST_ST_SimpleHeals_Priority, 12, 2, $"{Exaltation.ActionName()} Priority: ");
                    break;
                
                case Preset.AST_ST_Heals_Bole:
                    DrawSliderInt(0, 100, AST_ST_SimpleHeals_Bole, "Start using when below HP %. Set to 100 to disable this check");
                    DrawHorizontalMultiChoice(AST_ST_SimpleHeals_BoleOptions, "Only Weave", "Will only weave this action.", 2, 0);
                    DrawHorizontalMultiChoice(AST_ST_SimpleHeals_BoleOptions," Tanks Only", "Will only use on Tanks", 2, 1);
                    DrawPriorityInput(AST_ST_SimpleHeals_Priority, 12, 3, $"{Bole.ActionName()} Priority: ");
                    break;

                case Preset.AST_ST_Heals_Arrow:
                    DrawSliderInt(0, 100, AST_ST_SimpleHeals_Arrow, "Start using when below HP %. Set to 100 to disable this check");
                    DrawHorizontalMultiChoice(AST_ST_SimpleHeals_ArrowOptions, "Only Weave", "Will only weave this action.", 2, 0);
                    DrawHorizontalMultiChoice(AST_ST_SimpleHeals_ArrowOptions," Tanks Only", "Will only use on Tanks", 2, 1);
                    DrawPriorityInput(AST_ST_SimpleHeals_Priority, 12, 4, $"{Arrow.ActionName()} Priority: ");
                    break;
                
                case Preset.AST_ST_Heals_Ewer:
                    DrawSliderInt(0, 100, AST_ST_SimpleHeals_Ewer, "Start using when below HP %. Set to 100 to disable this check");
                    DrawAdditionalBoolChoice(AST_ST_SimpleHeals_WeaveEwer, "Only Weave", "");
                    DrawPriorityInput(AST_ST_SimpleHeals_Priority, 12, 5, $"{Ewer.ActionName()} Priority: ");
                    break;
                
                case Preset.AST_ST_Heals_Spire:
                    DrawSliderInt(0, 100, AST_ST_SimpleHeals_Spire, "Start using when below HP %. Set to 100 to disable this check");
                    DrawAdditionalBoolChoice(AST_ST_SimpleHeals_WeaveSpire, "Only Weave", "");
                    DrawPriorityInput(AST_ST_SimpleHeals_Priority, 12, 6, $"{Spire.ActionName()} Priority: ");
                    break;
                
                case Preset.AST_ST_Heals_AspectedBenefic:
                    DrawSliderInt(0, 100, AST_ST_SimpleHeals_AspectedBeneficHigh, "Start using when below HP %. Set to 100 to disable this check");
                    DrawSliderInt(0, 100, AST_ST_SimpleHeals_AspectedBeneficLow, "Stop using when below set percentage");
                    DrawSliderInt(0, 15, AST_ST_SimpleHeals_AspectedBeneficRefresh, "Seconds remaining before reapplying (0 = Do not reapply early)");
                    DrawPriorityInput(AST_ST_SimpleHeals_Priority, 12, 7, $"{AspectedBenefic.ActionName()} Priority: ");
                    break;
                
                case Preset.AST_ST_Heals_CelestialOpposition:
                    DrawSliderInt(0, 100, AST_ST_SimpleHeals_CelestialOpposition, "Start using when below HP %. Set to 100 to disable this check");
                    DrawHorizontalMultiChoice(AST_ST_SimpleHeals_CelestialOppositionOptions,"Only Weave", "Will only weave this action.", 2, 0);
                    DrawHorizontalMultiChoice(AST_ST_SimpleHeals_CelestialOppositionOptions," Not On Bosses", "Will not use on ST in Boss encounters.", 2, 1);
                    DrawPriorityInput(AST_ST_SimpleHeals_Priority, 12, 8, $"{CelestialOpposition.ActionName()} Priority: ");
                    break;
                
                case Preset.AST_ST_Heals_CollectiveUnconscious:
                    DrawSliderInt(0, 100, AST_ST_SimpleHeals_CollectiveUnconscious, "Start using when below HP %. Set to 100 to disable this check");
                    DrawHorizontalMultiChoice(AST_ST_SimpleHeals_CollectiveUnconsciousOptions,"Only Weave", "Will only weave this action.", 2, 0);
                    DrawHorizontalMultiChoice(AST_ST_SimpleHeals_CollectiveUnconsciousOptions," Not On Bosses", "Will not use on ST in Boss encounters.", 2, 1);
                    DrawPriorityInput(AST_ST_SimpleHeals_Priority, 12, 9, $"{CollectiveUnconscious.ActionName()} Priority: ");
                    break;
                
                case Preset.AST_ST_Heals_SoloLady:
                    DrawSliderInt(0, 100, AST_ST_SimpleHeals_SoloLady, "Start using when below HP %. Set to 100 to disable this check");
                    DrawHorizontalMultiChoice(AST_ST_SimpleHeals_SoloLadyOptions,"Only Weave", "Will only weave this action.", 2, 0);
                    DrawHorizontalMultiChoice(AST_ST_SimpleHeals_SoloLadyOptions," Not On Bosses", "Will not use on ST in Boss encounters.", 2, 1);
                    DrawPriorityInput(AST_ST_SimpleHeals_Priority, 12, 10, $"{LadyOfCrown.ActionName()} Priority: ");
                    break;
                
                
                #endregion
                
                #region AOE Heals

                case Preset.AST_AoE_Heals:
                    DrawRadioButton(AST_AoE_SimpleHeals_AltMode, $"On {AspectedHelios.ActionName()}", "", 0);
                    DrawRadioButton(AST_AoE_SimpleHeals_AltMode, $"On {Helios.ActionName()}", "Alternative AOE Mode. Leaves Aspected Helios alone for manual HoTs", 1);
                    break;

                case Preset.AST_AoE_Heals_LazyLady:
                    DrawSliderInt(0, 100, AST_AoE_SimpleHeals_LazyLady, "Start using when below party average HP %. Set to 100 to disable this check");
                    DrawAdditionalBoolChoice(AST_AoE_SimpleHeals_WeaveLady, "Only Weave", "Will only weave this action.");
                    DrawPriorityInput(AST_AoE_SimpleHeals_Priority, 9, 0, $"{LadyOfCrown.ActionName()} Priority: ");
                    break;

                case Preset.AST_AoE_Heals_Horoscope:
                    DrawSliderInt(0, 100, AST_AoE_SimpleHeals_Horoscope, "Start using when below party average HP %. Set to 100 to disable this check");
                    DrawAdditionalBoolChoice(AST_AoE_SimpleHeals_WeaveHoroscope, "Only Weave", "Will only weave this action.");
                    DrawPriorityInput(AST_AoE_SimpleHeals_Priority, 9, 1, $"{Horoscope.ActionName()} Priority: ");
                    break;
                
                case Preset.AST_AoE_Heals_HoroscopeHeal:
                    DrawSliderInt(0, 100, AST_AoE_SimpleHeals_HoroscopeHeal, "Start using when below party average HP %. Set to 100 to disable this check");
                    DrawAdditionalBoolChoice(AST_AoE_SimpleHeals_WeaveHoroscopeHeal, "Only Weave", "Will only weave this action.");
                    DrawPriorityInput(AST_AoE_SimpleHeals_Priority, 9, 2, $"{HoroscopeHeal.ActionName()} Priority: ");
                    break;

                case Preset.AST_AoE_Heals_CelestialOpposition:
                    DrawSliderInt(0, 100, AST_AoE_SimpleHeals_CelestialOpposition, "Start using when below party average HP %. Set to 100 to disable this check");
                    DrawAdditionalBoolChoice(AST_AoE_SimpleHeals_WeaveOpposition, "Only Weave", "Will only weave this action.");
                    DrawPriorityInput(AST_AoE_SimpleHeals_Priority, 9, 3, $"{CelestialOpposition.ActionName()} Priority: ");
                    break;


                case Preset.AST_AoE_Heals_NeutralSect:
                    DrawSliderInt(0, 100, AST_AoE_SimpleHeals_NeutralSect, "Start using when below party average HP %. Set to 100 to disable this check");
                    DrawAdditionalBoolChoice(AST_AoE_SimpleHeals_WeaveNeutralSect, "Only Weave", "Will only weave this action.");
                    DrawPriorityInput(AST_AoE_SimpleHeals_Priority, 9, 4, $"{NeutralSect.ActionName()} Priority: ");
                    break;
                
                case Preset.AST_AoE_Heals_StellarDetonation:
                    DrawSliderInt(0, 100, AST_AoE_SimpleHeals_StellarDetonation, "Start using when below party average HP %. Set to 100 to disable this check");
                    DrawAdditionalBoolChoice(AST_AoE_SimpleHeals_WeaveStellarDetonation, "Only Weave", "Will only weave this action.");
                    DrawPriorityInput(AST_AoE_SimpleHeals_Priority, 9, 5, $"{StellarDetonation.ActionName()} Priority: ");
                    break;
                
                case Preset.AST_AoE_Heals_Aspected:
                    DrawSliderInt(0, 100, AST_AoE_SimpleHeals_Aspected, "Start using when below party average HP %. Set to 100 to disable this check");
                    DrawPriorityInput(AST_AoE_SimpleHeals_Priority, 9, 6, $"{AspectedHelios.ActionName()} Priority: ");
                    break;
                
                case Preset.AST_AoE_Heals_Helios:
                    DrawSliderInt(0, 100, AST_AoE_SimpleHeals_Helios, "Start using when below party average HP %. Set to 100 to disable this check");
                    DrawPriorityInput(AST_AoE_SimpleHeals_Priority, 9, 7, $"{Helios.ActionName()} Priority: ");
                    break;
                
                case Preset.AST_AoE_Heals_CollectiveUnconscious:
                    DrawSliderInt(0, 100, AST_AoE_SimpleHeals_CollectiveUnconscious, "Start using when below party average HP %. Set to 100 to disable this check");
                    DrawAdditionalBoolChoice(AST_AoE_SimpleHeals_WeaveCollectiveUnconscious, "Only Weave", "Will only weave this action.");
                    DrawPriorityInput(AST_AoE_SimpleHeals_Priority, 9, 8, $"{CollectiveUnconscious.ActionName()} Priority: ");
                    break;
                
                #endregion
                
                #region Standalone
                case Preset.AST_Cards_QuickTargetCards:
                    DrawAdditionalBoolChoice(AST_QuickTarget_Manuals,
                        "Also Retarget manually-used Cards",
                        "Will also automatically target Cards that you manually use, as in, those outside of your damage rotations.",
                        indentDescription: true);

                    ImGui.Indent();
                    ImGui.TextWrapped("Target Overrides:           (hover each for more info)");
                    ImGui.Unindent();
                    ImGui.NewLine();
                    DrawRadioButton(AST_QuickTarget_Override, "No Override", "Will not override the automatic party target viability checking with any manual input.\nThe cards will be targeted according to The Balance's priorities and status checking\n(like not doubling up on cards, and no damage down, etc.).", 0, descriptionAsTooltip: true);
                    DrawRadioButton(AST_QuickTarget_Override, "Hard Target Override", "Overrides selection with hard target, if you have one that is in range and does not have damage down or rez sickness.", 1, descriptionAsTooltip: true);
                    DrawRadioButton(AST_QuickTarget_Override, "UI MouseOver Override", "Overrides selection with UI MouseOver target, if you have one that is in range and does not have damage down or rez sickness.", 2, descriptionAsTooltip: true);
                    DrawRadioButton(AST_QuickTarget_Override, "Any MouseOver Override", "Overrides selection with UI or Nameplate or Model MouseOver target (in that order), if you have one that is in range and does not have damage down or rez sickness.", 3, descriptionAsTooltip: true);
                    break;
                
                case Preset.AST_Retargets_EarthlyStar:
                    ImGui.Indent();
                    ImGui.TextColored(ImGuiColors.DalamudGrey, "Options to try to Retarget Earthly Star to before Self:");
                    ImGui.Unindent();
                    DrawHorizontalMultiChoice(AST_EarthlyStarOptions,
                        "Enemy Hard Target", "Will place at hard target if enemy", 2, 0);
                    DrawHorizontalMultiChoice(AST_EarthlyStarOptions,
                        "Ally Hard Target", "Will place at hard target if ally", 2, 1);
                    break;
                
                case Preset.AST_Mit_ST:
                    DrawHorizontalMultiChoice(AST_Mit_ST_Options,
                        "Include Celestial Intersection", "Will add Celestial Intersection for more mitigation.", 2, 0);
                    ImGui.NewLine();
                    DrawHorizontalMultiChoice(AST_Mit_ST_Options,
                        "Include Essential Dignity", "Will add Essential Dignity to top off targets health.", 2, 1);
                    if (AST_Mit_ST_Options[1])
                    {
                        ImGui.Indent();
                        DrawSliderInt(1, 100, AST_Mit_ST_EssentialDignityThreshold,
                            "Target HP% to use Essential Dignity below");
                        ImGui.Unindent();
                    }
                    break;
                #endregion
            }
        }
    }
}
