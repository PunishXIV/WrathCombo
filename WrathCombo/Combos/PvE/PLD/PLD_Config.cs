using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Window.Functions;

namespace WrathCombo.Combos.PvE;

internal partial class PLD
{
    internal static class Config
    {
        public static UserInt
            PLD_ST_FoF_Trigger = new("PLD_ST_FoF_Trigger", 0),
            PLD_AoE_FoF_Trigger = new("PLD_AoE_FoF_Trigger", 0),
            PLD_ST_SheltronOption = new("PLD_ST_SheltronOption", 50),
            PLD_ST_Sheltron_SubOption = new("PLD_ST_Sheltron_SubOption", 1),
            PLD_ST_Rampart_Health = new("PLD_ST_Rampart_Health", 80),
            PLD_ST_Rampart_SubOption = new("PLD_ST_Rampart_SubOption", 1),
            PLD_ST_Sentinel_Health = new("PLD_ST_Sentinel_Health", 60),
            PLD_ST_Sentinel_SubOption = new("PLD_ST_Sentinel_SubOption", 1),
            PLD_ST_HallowedGround_Health = new("PLD_ST_HallowedGround_Health", 30),
            PLD_ST_HallowedGround_SubOption = new("PLD_ST_HallowedGround_SubOption", 1),
            PLD_AoE_SheltronOption = new("PLD_AoE_SheltronOption", 50),
            PLD_AoE_Sheltron_SubOption = new("PLD_AoE_Sheltron_SubOption", 1),
            PLD_AoE_Rampart_Health = new("PLD_AoE_Rampart_Health", 80),
            PLD_AoE_Rampart_SubOption = new("PLD_AoE_Rampart_SubOption", 1),
            PLD_AoE_Sentinel_Health = new("PLD_AoE_Sentinel_Health", 60),
            PLD_AoE_Sentinel_SubOption = new("PLD_AoE_Sentinel_SubOption", 1),
            PLD_AoE_HallowedGround_Health = new("PLD_AoE_HallowedGround_Health", 30),
            PLD_AoE_HallowedGround_SubOption = new("PLD_AoE_HallowedGround_SubOption", 1),
            PLD_Intervene_HoldCharges = new("PLD_Intervene_HoldCharges", 1),
            PLD_AoE_Intervene_HoldCharges = new("PLD_AoE_Intervene_HoldCharges", 1),
            PLD_Intervene_MeleeOnly = new("PLD_Intervene_MeleeOnly", 1),
            PLD_AoE_Intervene_MeleeOnly = new("PLD_AoE_Intervene_MeleeOnly", 1),
            PLD_ST_MP_Reserve = new("PLD_ST_MP_Reserve", 1000),
            PLD_AoE_MP_Reserve = new("PLD_AoE_MP_Reserve", 1000),
            PLD_ShieldLob_SubOption = new("PLD_ShieldLob_SubOption", 1),
            PLD_Requiescat_SubOption = new("PLD_Requiescat_SubOption", 1),
            PLD_SpiritsWithin_SubOption = new("PLD_SpiritsWithin_SubOption", 1),
            PLD_VariantCure = new("PLD_VariantCure");

        internal static void Draw(CustomComboPreset preset)
        {
            switch (preset)
            {
                // Fight or Flight
                case CustomComboPreset.PLD_ST_AdvancedMode_FoF:
                    UserConfig.DrawSliderInt(0, 50, PLD_ST_FoF_Trigger, "Target HP%", 200);

                    break;

                case CustomComboPreset.PLD_AoE_AdvancedMode_FoF:
                    UserConfig.DrawSliderInt(0, 50, PLD_AoE_FoF_Trigger, "Target HP%", 200);

                    break;

                // Sheltron
                case CustomComboPreset.PLD_ST_AdvancedMode_Sheltron:
                    UserConfig.DrawSliderInt(50, 100, PLD_ST_SheltronOption, "Oath Gauge", 200, 5);

                    UserConfig.DrawHorizontalRadioButton(PLD_ST_Sheltron_SubOption, "All Enemies",
                        "Uses Sheltron regardless of targeted enemy type.", 1);

                    UserConfig.DrawHorizontalRadioButton(PLD_ST_Sheltron_SubOption, "Bosses Only",
                        "Only uses Sheltron when the targeted enemy is a boss.", 2);

                    break;

                case CustomComboPreset.PLD_AoE_AdvancedMode_Sheltron:
                    UserConfig.DrawSliderInt(50, 100, PLD_AoE_SheltronOption, "Oath Gauge", 200, 5);

                    UserConfig.DrawHorizontalRadioButton(PLD_AoE_Sheltron_SubOption, "All Enemies",
                        "Uses Sheltron regardless of targeted enemy type.", 1);

                    UserConfig.DrawHorizontalRadioButton(PLD_AoE_Sheltron_SubOption, "Bosses Only",
                        "Only uses Sheltron when the targeted enemy is a boss.", 2);

                    break;

                // Rampart
                case CustomComboPreset.PLD_ST_AdvancedMode_Rampart:
                    UserConfig.DrawSliderInt(1, 100, PLD_ST_Rampart_Health, "Player HP%", 200);

                    UserConfig.DrawHorizontalRadioButton(PLD_ST_Rampart_SubOption, "All Enemies",
                        "Uses Rampart regardless of targeted enemy type.", 1);

                    UserConfig.DrawHorizontalRadioButton(PLD_ST_Rampart_SubOption, "Bosses Only",
                        "Only uses Rampart when the targeted enemy is a boss.", 2);

                    break;

                case CustomComboPreset.PLD_AoE_AdvancedMode_Rampart:
                    UserConfig.DrawSliderInt(1, 100, PLD_AoE_Rampart_Health, "Player HP%", 200);

                    UserConfig.DrawHorizontalRadioButton(PLD_AoE_Rampart_SubOption, "All Enemies",
                        "Uses Rampart regardless of targeted enemy type.", 1);

                    UserConfig.DrawHorizontalRadioButton(PLD_AoE_Rampart_SubOption, "Bosses Only",
                        "Only uses Rampart when the targeted enemy is a boss.", 2);

                    break;

                // Sentinel / Guardian
                case CustomComboPreset.PLD_ST_AdvancedMode_Sentinel:
                    UserConfig.DrawSliderInt(1, 100, PLD_ST_Sentinel_Health, "Player HP%", 200);

                    UserConfig.DrawHorizontalRadioButton(PLD_ST_Sentinel_SubOption, "All Enemies",
                        "Uses Sentinel regardless of targeted enemy type.", 1);

                    UserConfig.DrawHorizontalRadioButton(PLD_ST_Sentinel_SubOption, "Bosses Only",
                        "Only uses Sentinel when the targeted enemy is a boss.", 2);

                    break;

                case CustomComboPreset.PLD_AoE_AdvancedMode_Sentinel:
                    UserConfig.DrawSliderInt(1, 100, PLD_AoE_Sentinel_Health, "Player HP%", 200);

                    UserConfig.DrawHorizontalRadioButton(PLD_AoE_Sentinel_SubOption, "All Enemies",
                        "Uses Sentinel regardless of targeted enemy type.", 1);

                    UserConfig.DrawHorizontalRadioButton(PLD_AoE_Sentinel_SubOption, "Bosses Only",
                        "Only uses Sentinel when the targeted enemy is a boss.", 2);

                    break;

                // Hallowed Ground
                case CustomComboPreset.PLD_ST_AdvancedMode_HallowedGround:
                    UserConfig.DrawSliderInt(1, 100, PLD_ST_HallowedGround_Health, "Player HP%", 200);

                    UserConfig.DrawHorizontalRadioButton(PLD_ST_HallowedGround_SubOption, "All Enemies",
                        "Uses Hallowed Ground regardless of targeted enemy type.", 1);

                    UserConfig.DrawHorizontalRadioButton(PLD_ST_HallowedGround_SubOption, "Bosses Only",
                        "Only uses Hallowed Ground when the targeted enemy is a boss.", 2);

                    break;

                case CustomComboPreset.PLD_AoE_AdvancedMode_HallowedGround:
                    UserConfig.DrawSliderInt(1, 100, PLD_AoE_HallowedGround_Health, "Player HP%", 200);

                    UserConfig.DrawHorizontalRadioButton(PLD_AoE_HallowedGround_SubOption, "All Enemies",
                        "Uses Hallowed Ground regardless of targeted enemy type.", 1);

                    UserConfig.DrawHorizontalRadioButton(PLD_AoE_HallowedGround_SubOption, "Bosses Only",
                        "Only uses Hallowed Ground when the targeted enemy is a boss.", 2);

                    break;

                // Intervene
                case CustomComboPreset.PLD_ST_AdvancedMode_Intervene:
                    UserConfig.DrawSliderInt(0, 1, PLD_Intervene_HoldCharges, "Charges", 200);

                    UserConfig.DrawHorizontalRadioButton(PLD_Intervene_MeleeOnly, "Melee Range",
                        "Uses Intervene while within melee range.\n- May result in minor movement.", 1);

                    UserConfig.DrawHorizontalRadioButton(PLD_Intervene_MeleeOnly, "No Movement",
                        "Only uses Intervene when it would not result in movement.\n- Requires target to be within zero distance.", 2);

                    break;

                case CustomComboPreset.PLD_AoE_AdvancedMode_Intervene:
                    UserConfig.DrawSliderInt(0, 1, PLD_AoE_Intervene_HoldCharges, "Charges", 200);

                    UserConfig.DrawHorizontalRadioButton(PLD_AoE_Intervene_MeleeOnly, "Melee Range",
                        "Uses Intervene while within melee range.\n- May result in minor movement.", 1);

                    UserConfig.DrawHorizontalRadioButton(PLD_AoE_Intervene_MeleeOnly, "No Movement",
                        "Only uses Intervene when it would not result in movement.\n- Requires target to be within zero distance.", 2);

                    break;

                // Shield Lob
                case CustomComboPreset.PLD_ST_AdvancedMode_ShieldLob:
                    UserConfig.DrawHorizontalRadioButton(PLD_ShieldLob_SubOption, "Shield Lob Only",
                        "", 1);

                    UserConfig.DrawHorizontalRadioButton(PLD_ShieldLob_SubOption, "Add Holy Spirit",
                        "Attempts to hardcast Holy Spirit when not moving.\n- Requires sufficient MP to cast.", 2);

                    break;

                // MP Reservation
                case CustomComboPreset.PLD_ST_AdvancedMode_MP_Reserve:
                    UserConfig.DrawSliderInt(1000, 5000, PLD_ST_MP_Reserve, "Minimum MP", sliderIncrement: 100);

                    break;

                case CustomComboPreset.PLD_AoE_AdvancedMode_MP_Reserve:
                    UserConfig.DrawSliderInt(1000, 5000, PLD_AoE_MP_Reserve, "Minimum MP", sliderIncrement: 100);

                    break;

                // Requiescat Spender Feature
                case CustomComboPreset.PLD_Requiescat_Options:
                    UserConfig.DrawHorizontalRadioButton(PLD_Requiescat_SubOption, "Normal Behavior",
                        "", 1);

                    UserConfig.DrawHorizontalRadioButton(PLD_Requiescat_SubOption, "Add Fight or Flight",
                        "Adds Fight or Flight to the normal logic.\n- Requires Resquiescat to be ready.", 2);

                    break;

                // Spirits Within / Circle of Scorn Feature
                case CustomComboPreset.PLD_SpiritsWithin:
                    UserConfig.DrawHorizontalRadioButton(PLD_SpiritsWithin_SubOption, "Normal Behavior",
                        "", 1);

                    UserConfig.DrawHorizontalRadioButton(PLD_SpiritsWithin_SubOption, "Add Drift Prevention",
                        "Prevents Spirits Within and Circle of Scorn from drifting.\n- Actions must be used within 5 seconds of each other.", 2);

                    break;

                // Variant Cure Feature
                case CustomComboPreset.PLD_Variant_Cure:
                    UserConfig.DrawSliderInt(1, 100, PLD_VariantCure, "Player HP%", 200);

                    break;
            }
        }
    }
}
