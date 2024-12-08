using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Window.Functions;

namespace WrathCombo.Combos.PvE;

internal partial class GNB
{
    internal static class Config
    {
        public const string
            GNB_VariantCure = "GNB_VariantCure",
            GNBPvP_Corundum = "GNBPvP_Corundum";

        public static UserInt
            GNB_ST_Corundum_Health = new("GNB_ST_CorundumOption", 90),
            GNB_ST_Corundum_SubOption = new("GNB_ST_Corundum_SubOption", 1),
            GNB_ST_Aurora_Health = new("GNB_ST_Aurora_Health", 99),
            GNB_ST_Aurora_Charges = new("GNB_ST_Aurora_Charges", 1),
            GNB_ST_Aurora_SubOption = new("GNB_ST_Aurora_SubOption", 1),
            GNB_ST_Rampart_Health = new("GNB_ST_Rampart_Health", 80),
            GNB_ST_Rampart_SubOption = new("GNB_ST_Rampart_SubOption", 1),
            GNB_ST_Camouflage_Health = new("GNB_ST_Camouflage_Health", 70),
            GNB_ST_Camouflage_SubOption = new("GNB_ST_Camouflage_SubOption", 1),
            GNB_ST_Nebula_Health = new("GNB_ST_Nebula_Health", 60),
            GNB_ST_Nebula_SubOption = new("GNB_ST_Nebula_SubOption", 1),
            GNB_ST_Superbolide_Health = new("GNB_ST_Superbolide_Health", 30),
            GNB_ST_Superbolide_SubOption = new("GNB_ST_Superbolide_SubOption", 1),
            GNB_ST_NoMercyStop = new("GNB_ST_NoMercyStop", 5),
            GNB_AoE_Corundum_Health = new("GNB_AoE_CorundumOption", 90),
            GNB_AoE_Corundum_SubOption = new("GNB_AoE_Corundum_SubOption", 1),
            GNB_AoE_Aurora_Health = new("GNB_AoE_Aurora_Health", 99),
            GNB_AoE_Aurora_Charges = new("GNB_AoE_Aurora_Charges", 1),
            GNB_AoE_Aurora_SubOption = new("GNB_AoE_Aurora_SubOption", 1),
            GNB_AoE_Rampart_Health = new("GNB_AoE_Rampart_Health", 80),
            GNB_AoE_Rampart_SubOption = new("GNB_AoE_Rampart_SubOption", 1),
            GNB_AoE_Camouflage_Health = new("GNB_AoE_Camouflage_Health", 80),
            GNB_AoE_Camouflage_SubOption = new("GNB_AoE_Camouflage_SubOption", 1),
            GNB_AoE_Nebula_Health = new("GNB_AoE_Nebula_Health", 60),
            GNB_AoE_Nebula_SubOption = new("GNB_AoE_Nebula_SubOption", 1),
            GNB_AoE_Superbolide_Health = new("GNB_AoE_Superbolide_Health", 30),
            GNB_AoE_Superbolide_SubOption = new("GNB_AoE_Superbolide_SubOption", 1),
            GNB_AoE_NoMercyStop = new("GNB_AoE_NoMercyStop", 5),
            GNB_Mit_Superbolide_Health = new("GNB_Mit_Superbolide_Health", 30),
            GNB_Mit_Aurora_Charges = new("GNB_Aurora_Charges", 1),
            GNB_NM_Features_Weave = new("GNB_NM_Features_Weave", 1),

            //Bozja
            GNB_Bozja_LostCure_Health = new("GNB_Bozja_LostCure_Health", 50),
            GNB_Bozja_LostCure2_Health = new("GNB_Bozja_LostCure_Health", 50),
            GNB_Bozja_LostCure3_Health = new("GNB_Bozja_LostCure_Health", 50),
            GNB_Bozja_LostCure4_Health = new("GNB_Bozja_LostCure_Health", 50),
            GNB_Bozja_LostAethershield_Health = new("GNB_Bozja_LostAethershield_Health", 70),
            GNB_Bozja_LostReraise_Health = new("GNB_Bozja_LostReraise_Health", 10);

        internal static void Draw(CustomComboPreset preset)
        {
            switch (preset)
            {
                case CustomComboPreset.GNB_Bozja_LostCure:
                    UserConfig.DrawSliderInt(1, 100, GNB_Bozja_LostCure_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    break;

                case CustomComboPreset.GNB_Bozja_LostCure2:
                    UserConfig.DrawSliderInt(1, 100, GNB_Bozja_LostCure2_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    break;

                case CustomComboPreset.GNB_Bozja_LostCure3:
                    UserConfig.DrawSliderInt(1, 100, GNB_Bozja_LostCure3_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    break;

                case CustomComboPreset.GNB_Bozja_LostCure4:
                    UserConfig.DrawSliderInt(1, 100, GNB_Bozja_LostCure4_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    break;

                case CustomComboPreset.GNB_Bozja_LostAethershield:
                    UserConfig.DrawSliderInt(1, 100, GNB_Bozja_LostAethershield_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    break;

                case CustomComboPreset.GNB_Bozja_LostReraise:
                    UserConfig.DrawSliderInt(1, 100, GNB_Bozja_LostReraise_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    break;

                case CustomComboPreset.GNB_Variant_Cure:
                    UserConfig.DrawSliderInt(1, 100, GNB_VariantCure,
                        "Player HP% to be \nless than or equal to:", 200);

                    break;

                case CustomComboPreset.GNB_ST_NoMercy:
                    UserConfig.DrawSliderInt(0, 25, GNB_ST_NoMercyStop,
                        "Stop Usage if Target HP% is below set value.\nTo Disable this option, set to 0.");

                    break;

                case CustomComboPreset.GNB_AoE_NoMercy:
                    UserConfig.DrawSliderInt(0, 25, GNB_AoE_NoMercyStop,
                        "Stop Usage if Target HP% is below set value.\nTo Disable this option, set to 0.");

                    break;

                case CustomComboPreset.GNB_ST_Corundum:
                    UserConfig.DrawSliderInt(1, 100, GNB_ST_Corundum_Health,
                        "Player HP% to be \nless than or equal to:", 200);

                    UserConfig.DrawHorizontalRadioButton(GNB_ST_Corundum_SubOption,
                        "All Enemies",
                        "Uses Corundum regardless of targeted enemy type.", 1);

                    UserConfig.DrawHorizontalRadioButton(GNB_ST_Corundum_SubOption,
                        "Bosses Only",
                        "Only uses Corundum when the targeted enemy is a boss.", 2);

                    break;

                case CustomComboPreset.GNB_AoE_Corundum:
                    UserConfig.DrawSliderInt(1, 100, GNB_AoE_Corundum_Health,
                        "Player HP% to be \nless than or equal to:", 200);

                    UserConfig.DrawHorizontalRadioButton(GNB_AoE_Corundum_SubOption,
                        "All Enemies",
                        "Uses Corundum regardless of targeted enemy type.", 1);

                    UserConfig.DrawHorizontalRadioButton(GNB_AoE_Corundum_SubOption,
                        "Bosses Only",
                        "Only uses Corundum when the targeted enemy is a boss.", 2);

                    break;

                case CustomComboPreset.GNB_ST_Aurora:
                    UserConfig.DrawSliderInt(1, 100, GNB_ST_Aurora_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    UserConfig.DrawSliderInt(0, 1, GNB_ST_Aurora_Charges,
                        "How many charges to keep ready?\n (0 = Use All)");

                    UserConfig.DrawHorizontalRadioButton(GNB_ST_Aurora_SubOption,
                        "All Enemies",
                        "Uses Aurora regardless of targeted enemy type.", 1);

                    UserConfig.DrawHorizontalRadioButton(GNB_ST_Aurora_SubOption,
                        "Bosses Only",
                        "Only uses Aurora when the targeted enemy is a boss.", 2);

                    break;

                case CustomComboPreset.GNB_AoE_Aurora:
                    UserConfig.DrawSliderInt(1, 100, GNB_AoE_Aurora_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    UserConfig.DrawSliderInt(0, 1, GNB_AoE_Aurora_Charges,
                        "How many charges to keep ready?\n (0 = Use All)");

                    UserConfig.DrawHorizontalRadioButton(GNB_AoE_Aurora_SubOption,
                        "All Enemies",
                        "Uses Aurora regardless of targeted enemy type.", 1);

                    UserConfig.DrawHorizontalRadioButton(GNB_AoE_Aurora_SubOption,
                        "Bosses Only",
                        "Only uses Aurora when the targeted enemy is a boss.", 2);

                    break;

                case CustomComboPreset.GNB_Mit_Aurora:
                    UserConfig.DrawSliderInt(0, 1, GNB_Mit_Aurora_Charges,
                        "How many charges to keep ready?\n (0 = Use All)");
                    break;

                case CustomComboPreset.GNB_ST_Rampart:
                    UserConfig.DrawSliderInt(1, 100, GNB_ST_Rampart_Health,
                        "Player HP% to be \nless than or equal to:", 200);

                    UserConfig.DrawHorizontalRadioButton(GNB_ST_Rampart_SubOption,
                        "All Enemies",
                        "Uses Rampart regardless of targeted enemy type.", 1);

                    UserConfig.DrawHorizontalRadioButton(GNB_ST_Rampart_SubOption,
                        "Bosses Only",
                        "Only uses Rampart when the targeted enemy is a boss.", 2);

                    break;

                case CustomComboPreset.GNB_AoE_Rampart:
                    UserConfig.DrawSliderInt(1, 100, GNB_AoE_Rampart_Health,
                        "Player HP% to be \nless than or equal to:", 200);

                    UserConfig.DrawHorizontalRadioButton(GNB_AoE_Rampart_SubOption,
                        "All Enemies",
                        "Uses Rampart regardless of targeted enemy type.", 1);

                    UserConfig.DrawHorizontalRadioButton(GNB_AoE_Rampart_SubOption,
                        "Bosses Only",
                        "Only uses Rampart when the targeted enemy is a boss.", 2);

                    break;

                case CustomComboPreset.GNB_ST_Camouflage:
                    UserConfig.DrawSliderInt(1, 100, GNB_ST_Camouflage_Health,
                        "Player HP% to be \nless than or equal to:", 200);

                    UserConfig.DrawHorizontalRadioButton(GNB_ST_Camouflage_SubOption,
                        "All Enemies",
                        "Uses Camo regardless of targeted enemy type.", 1);

                    UserConfig.DrawHorizontalRadioButton(GNB_ST_Camouflage_SubOption,
                        "Bosses Only",
                        "Only uses Camo when the targeted enemy is a boss.", 2);

                    break;

                case CustomComboPreset.GNB_AoE_Camouflage:
                    UserConfig.DrawSliderInt(1, 100, GNB_AoE_Camouflage_Health,
                        "Player HP% to be \nless than or equal to:", 200);

                    UserConfig.DrawHorizontalRadioButton(GNB_AoE_Camouflage_SubOption,
                        "All Enemies",
                        "Uses Camo regardless of targeted enemy type.", 1);

                    UserConfig.DrawHorizontalRadioButton(GNB_AoE_Camouflage_SubOption,
                        "Bosses Only",
                        "Only uses Camo when the targeted enemy is a boss.", 2);

                    break;

                case CustomComboPreset.GNB_ST_Nebula:
                    UserConfig.DrawSliderInt(1, 100, GNB_ST_Nebula_Health,
                        "Player HP% to be \nless than or equal to:", 200);

                    UserConfig.DrawHorizontalRadioButton(GNB_ST_Nebula_SubOption,
                        "All Enemies",
                        "Uses Nebula regardless of targeted enemy type.", 1);

                    UserConfig.DrawHorizontalRadioButton(GNB_ST_Nebula_SubOption,
                        "Bosses Only",
                        "Only uses Nebula when the targeted enemy is a boss.", 2);

                    break;

                case CustomComboPreset.GNB_AoE_Nebula:
                    UserConfig.DrawSliderInt(1, 100, GNB_AoE_Nebula_Health,
                        "Player HP% to be \nless than or equal to:", 200);

                    UserConfig.DrawHorizontalRadioButton(GNB_AoE_Nebula_SubOption,
                        "All Enemies",
                        "Uses Nebula regardless of targeted enemy type.", 1);

                    UserConfig.DrawHorizontalRadioButton(GNB_AoE_Nebula_SubOption,
                        "Bosses Only",
                        "Only uses Nebula when the targeted enemy is a boss.", 2);

                    break;

                case CustomComboPreset.GNB_ST_Superbolide:
                    UserConfig.DrawSliderInt(1, 100, GNB_ST_Superbolide_Health,
                        "Player HP% to be \nless than or equal to:", 200);

                    UserConfig.DrawHorizontalRadioButton(GNB_ST_Superbolide_SubOption,
                        "All Enemies",
                        "Uses Hallowed Ground regardless of targeted enemy type.", 1);

                    UserConfig.DrawHorizontalRadioButton(GNB_ST_Superbolide_SubOption,
                        "Bosses Only",
                        "Only uses Hallowed Ground when the targeted enemy is a boss.", 2);

                    break;

                case CustomComboPreset.GNB_AoE_Superbolide:
                    UserConfig.DrawSliderInt(1, 100, GNB_AoE_Superbolide_Health,
                        "Player HP% to be \nless than or equal to:", 200);

                    UserConfig.DrawHorizontalRadioButton(GNB_AoE_Superbolide_SubOption,
                        "All Enemies",
                        "Uses Hallowed Ground regardless of targeted enemy type.", 1);

                    UserConfig.DrawHorizontalRadioButton(GNB_AoE_Superbolide_SubOption,
                        "Bosses Only",
                        "Only uses Hallowed Ground when the targeted enemy is a boss.", 2);

                    break;

                case CustomComboPreset.GNB_Mit_Superbolide:
                    UserConfig.DrawSliderInt(1, 100, GNB_Mit_Superbolide_Health,
                        "Player HP% to be \nless than or equal to:", 200);

                    break;

                case CustomComboPreset.GNB_NM_Features:

                    UserConfig.DrawHorizontalRadioButton(GNB_NM_Features_Weave,
                        "Weave-Only",
                        "Uses cooldowns only when inside a weave window (excludes No Mercy)", 1);

                    UserConfig.DrawHorizontalRadioButton(GNB_NM_Features_Weave,
                        "On Cooldown",
                        "Uses cooldowns as soon as possible", 2);

                    break;
            }
        }
    }
}
