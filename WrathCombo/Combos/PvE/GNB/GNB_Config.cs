using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
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
            GNB_ST_MitsOptions = new("GNB_ST_MitsOptions", 0),
            GNB_ST_Corundum_Health = new("GNB_ST_CorundumOption", 90),
            GNB_ST_Corundum_SubOption = new("GNB_ST_Corundum_Option", 0),
            GNB_ST_Aurora_Health = new("GNB_ST_Aurora_Health", 99),
            GNB_ST_Aurora_Charges = new("GNB_ST_Aurora_Charges", 0),
            GNB_ST_Aurora_SubOption = new("GNB_ST_Aurora_Option", 0),
            GNB_ST_Rampart_Health = new("GNB_ST_Rampart_Health", 80),
            GNB_ST_Rampart_SubOption = new("GNB_ST_Rampart_Option", 0),
            GNB_ST_Camouflage_Health = new("GNB_ST_Camouflage_Health", 70),
            GNB_ST_Camouflage_SubOption = new("GNB_ST_Camouflage_Option", 0),
            GNB_ST_Nebula_Health = new("GNB_ST_Nebula_Health", 60),
            GNB_ST_Nebula_SubOption = new("GNB_ST_Nebula_Option", 0),
            GNB_ST_Superbolide_Health = new("GNB_ST_Superbolide_Health", 30),
            GNB_ST_Superbolide_SubOption = new("GNB_ST_Superbolide_Option", 0),
            GNB_ST_NoMercyStop = new("GNB_ST_NoMercyStop", 5),
            GNB_AoE_MitsOptions = new("GNB_AoE_MitsOptions", 0),
            GNB_AoE_Corundum_Health = new("GNB_AoE_CorundumOption", 90),
            GNB_AoE_Corundum_SubOption = new("GNB_AoE_Corundum_Option", 0),
            GNB_AoE_Aurora_Health = new("GNB_AoE_Aurora_Health", 99),
            GNB_AoE_Aurora_Charges = new("GNB_AoE_Aurora_Charges", 0),
            GNB_AoE_Aurora_SubOption = new("GNB_AoE_Aurora_Option", 0),
            GNB_AoE_Rampart_Health = new("GNB_AoE_Rampart_Health", 80),
            GNB_AoE_Rampart_SubOption = new("GNB_AoE_Rampart_Option", 10),
            GNB_AoE_Camouflage_Health = new("GNB_AoE_Camouflage_Health", 80),
            GNB_AoE_Camouflage_SubOption = new("GNB_AoE_Camouflage_Option", 0),
            GNB_AoE_Nebula_Health = new("GNB_AoE_Nebula_Health", 60),
            GNB_AoE_Nebula_SubOption = new("GNB_AoE_Nebula_Option", 0),
            GNB_AoE_Superbolide_Health = new("GNB_AoE_Superbolide_Health", 30),
            GNB_AoE_Superbolide_SubOption = new("GNB_AoE_Superbolide_Option", 0),
            GNB_AoE_NoMercyStop = new("GNB_AoE_NoMercyStop", 5),
            GNB_Mit_Superbolide_Health = new("GNB_Mit_Superbolide_Health", 30),
            GNB_Mit_Aurora_Charges = new("GNB_Mit_Aurora_Charges", 0),
            GNB_NM_Features_Weave = new("GNB_NM_Features_Weave", 0),
            GNB_GF_Features_Choice = new("GNB_GF_Features_Choice", 0),

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


                case CustomComboPreset.GNB_ST_Simple:
                    UserConfig.DrawHorizontalRadioButton(GNB_ST_MitsOptions,
                        "Include Mitigations",
                        "Enables the use of mitigations in Simple Mode.", 0);

                    UserConfig.DrawHorizontalRadioButton(GNB_ST_MitsOptions,
                        "Exclude Mitigations",
                        "Disables the use of mitigations in Simple Mode.", 1);
                    break;

                case CustomComboPreset.GNB_AoE_Simple:
                    UserConfig.DrawHorizontalRadioButton(GNB_AoE_MitsOptions,
                        "Include Mitigations",
                        "Enables the use of mitigations in Simple Mode.", 0);

                    UserConfig.DrawHorizontalRadioButton(GNB_AoE_MitsOptions,
                        "Exclude Mitigations",
                        "Disables the use of mitigations in Simple Mode.", 1);
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
                        $"Uses {HeartOfCorundum.ActionName()} regardless of targeted enemy type.", 0);

                    UserConfig.DrawHorizontalRadioButton(GNB_ST_Corundum_SubOption,
                        "Bosses Only",
                        $"Only ses {HeartOfCorundum.ActionName()} when the targeted enemy is a boss.", 1);

                    break;

                case CustomComboPreset.GNB_AoE_Corundum:
                    UserConfig.DrawSliderInt(1, 100, GNB_AoE_Corundum_Health,
                        "Player HP% to be \nless than or equal to:", 200);

                    UserConfig.DrawHorizontalRadioButton(GNB_AoE_Corundum_SubOption,
                        "All Enemies",
                        $"Uses {HeartOfCorundum.ActionName()} regardless of targeted enemy type.", 0);

                    UserConfig.DrawHorizontalRadioButton(GNB_AoE_Corundum_SubOption,
                        "Bosses Only",
                        $"Only uses {HeartOfCorundum.ActionName()} when the targeted enemy is a boss.", 1);

                    break;

                case CustomComboPreset.GNB_ST_Aurora:
                    UserConfig.DrawSliderInt(1, 100, GNB_ST_Aurora_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    UserConfig.DrawSliderInt(0, 1, GNB_ST_Aurora_Charges,
                        "How many charges to keep ready?\n (0 = Use All)");

                    UserConfig.DrawHorizontalRadioButton(GNB_ST_Aurora_SubOption,
                        "All Enemies",
                        $"Uses {Aurora.ActionName()} regardless of targeted enemy type.", 0);

                    UserConfig.DrawHorizontalRadioButton(GNB_ST_Aurora_SubOption,
                        "Bosses Only",
                        $"Only uses {Aurora.ActionName()} when the targeted enemy is a boss.", 1);

                    break;

                case CustomComboPreset.GNB_AoE_Aurora:
                    UserConfig.DrawSliderInt(1, 100, GNB_AoE_Aurora_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    UserConfig.DrawSliderInt(0, 1, GNB_AoE_Aurora_Charges,
                        "How many charges to keep ready?\n (0 = Use All)");

                    UserConfig.DrawHorizontalRadioButton(GNB_AoE_Aurora_SubOption,
                        "All Enemies",
                        $"Uses {Aurora.ActionName()} regardless of targeted enemy type.", 0);

                    UserConfig.DrawHorizontalRadioButton(GNB_AoE_Aurora_SubOption,
                        "Bosses Only",
                        $"Only uses {Aurora.ActionName()} when the targeted enemy is a boss.", 1);

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
                        $"Uses {All.Rampart.ActionName()} regardless of targeted enemy type.", 0);

                    UserConfig.DrawHorizontalRadioButton(GNB_ST_Rampart_SubOption,
                        "Bosses Only",
                        $"Only uses {All.Rampart.ActionName()} when the targeted enemy is a boss.", 1);

                    break;

                case CustomComboPreset.GNB_AoE_Rampart:
                    UserConfig.DrawSliderInt(1, 100, GNB_AoE_Rampart_Health,
                        "Player HP% to be \nless than or equal to:", 200);

                    UserConfig.DrawHorizontalRadioButton(GNB_AoE_Rampart_SubOption,
                        "All Enemies",
                        $"Uses {All.Rampart.ActionName()} regardless of targeted enemy type.", 0);

                    UserConfig.DrawHorizontalRadioButton(GNB_AoE_Rampart_SubOption,
                        "Bosses Only",
                        $"Only uses {All.Rampart.ActionName()} when the targeted enemy is a boss.", 1);

                    break;

                case CustomComboPreset.GNB_ST_Camouflage:
                    UserConfig.DrawSliderInt(1, 100, GNB_ST_Camouflage_Health,
                        "Player HP% to be \nless than or equal to:", 200);

                    UserConfig.DrawHorizontalRadioButton(GNB_ST_Camouflage_SubOption,
                        "All Enemies",
                        $"Uses {Camouflage.ActionName()} regardless of targeted enemy type.", 0);

                    UserConfig.DrawHorizontalRadioButton(GNB_ST_Camouflage_SubOption,
                        "Bosses Only",
                        $"Only uses {Camouflage.ActionName()} when the targeted enemy is a boss.", 1);

                    break;

                case CustomComboPreset.GNB_AoE_Camouflage:
                    UserConfig.DrawSliderInt(1, 100, GNB_AoE_Camouflage_Health,
                        "Player HP% to be \nless than or equal to:", 200);

                    UserConfig.DrawHorizontalRadioButton(GNB_AoE_Camouflage_SubOption,
                        "All Enemies",
                        $"Uses {Camouflage.ActionName()} regardless of targeted enemy type.", 0);

                    UserConfig.DrawHorizontalRadioButton(GNB_AoE_Camouflage_SubOption,
                        "Bosses Only",
                        $"Only uses {Camouflage.ActionName()} when the targeted enemy is a boss.", 1);

                    break;

                case CustomComboPreset.GNB_ST_Nebula:
                    UserConfig.DrawSliderInt(1, 100, GNB_ST_Nebula_Health,
                        "Player HP% to be \nless than or equal to:", 200);

                    UserConfig.DrawHorizontalRadioButton(GNB_ST_Nebula_SubOption,
                        "All Enemies",
                        $"Uses {Nebula.ActionName()} regardless of targeted enemy type.", 0);

                    UserConfig.DrawHorizontalRadioButton(GNB_ST_Nebula_SubOption,
                        "Bosses Only",
                        $"Only uses {Nebula.ActionName()} when the targeted enemy is a boss.", 1);

                    break;

                case CustomComboPreset.GNB_AoE_Nebula:
                    UserConfig.DrawSliderInt(1, 100, GNB_AoE_Nebula_Health,
                        "Player HP% to be \nless than or equal to:", 200);

                    UserConfig.DrawHorizontalRadioButton(GNB_AoE_Nebula_SubOption,
                        "All Enemies",
                        $"Uses {Nebula.ActionName()} regardless of targeted enemy type.", 0);

                    UserConfig.DrawHorizontalRadioButton(GNB_AoE_Nebula_SubOption,
                        "Bosses Only",
                        $"Only uses {Nebula.ActionName()} when the targeted enemy is a boss.", 1);

                    break;

                case CustomComboPreset.GNB_ST_Superbolide:
                    UserConfig.DrawSliderInt(1, 100, GNB_ST_Superbolide_Health,
                        "Player HP% to be \nless than or equal to:", 200);

                    UserConfig.DrawHorizontalRadioButton(GNB_ST_Superbolide_SubOption,
                        "All Enemies",
                        $"Uses {Superbolide.ActionName()} regardless of targeted enemy type.", 0);

                    UserConfig.DrawHorizontalRadioButton(GNB_ST_Superbolide_SubOption,
                        "Bosses Only",
                        $"Only uses {Superbolide.ActionName()} when the targeted enemy is a boss.", 1);

                    break;

                case CustomComboPreset.GNB_AoE_Superbolide:
                    UserConfig.DrawSliderInt(1, 100, GNB_AoE_Superbolide_Health,
                        "Player HP% to be \nless than or equal to:", 200);

                    UserConfig.DrawHorizontalRadioButton(GNB_AoE_Superbolide_SubOption,
                        "All Enemies",
                        $"Uses {Superbolide.ActionName()} regardless of targeted enemy type.", 0);

                    UserConfig.DrawHorizontalRadioButton(GNB_AoE_Superbolide_SubOption,
                        "Bosses Only",
                        $"Only uses {Superbolide.ActionName()} when the targeted enemy is a boss.", 1);

                    break;

                case CustomComboPreset.GNB_Mit_Superbolide:
                    UserConfig.DrawSliderInt(1, 100, GNB_Mit_Superbolide_Health,
                        "Player HP% to be \nless than or equal to:", 200);

                    break;

                case CustomComboPreset.GNB_NM_Features:

                    UserConfig.DrawHorizontalRadioButton(GNB_NM_Features_Weave,
                        "Weave-Only",
                        "Uses cooldowns only when inside a weave window (excludes No Mercy)", 0);

                    UserConfig.DrawHorizontalRadioButton(GNB_NM_Features_Weave,
                        "On Cooldown",
                        "Uses cooldowns as soon as possible", 1);

                    break;

                case CustomComboPreset.GNB_GF_Features:

                    UserConfig.DrawHorizontalRadioButton(GNB_GF_Features_Choice,
                        "Replace Gnashing Fang",
                        $"Use this feature as intended on {GnashingFang.ActionName()}", 0);

                    UserConfig.DrawHorizontalRadioButton(GNB_GF_Features_Choice,
                        "Replace No Mercy",
                        $"Use this feature instead on {NoMercy.ActionName()}\nWARNING: This WILL conflict with 'No Mercy Features'!", 1);

                    break;
            }
        }
    }
}
