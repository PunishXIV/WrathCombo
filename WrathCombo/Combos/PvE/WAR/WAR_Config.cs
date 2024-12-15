using WrathCombo.Combos.PvP;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using WrathCombo.Window.Functions;

namespace WrathCombo.Combos.PvE;

internal partial class WAR
{
    internal static class Config
    {
        public static UserInt
            WAR_InfuriateRange = new("WAR_InfuriateRange", 40),
            WAR_SurgingRefreshRange = new("WAR_SurgingRefreshRange", 10),
            WAR_KeepOnslaughtCharges = new("WAR_KeepOnslaughtCharges", 1),
            WAR_KeepInfuriateCharges = new("WAR_KeepInfuriateCharges", 0),
            WAR_FellCleaveGauge = new("WAR_FellCleaveGauge", 90),
            WAR_DecimateGauge = new("WAR_DecimateGauge", 90),
            WAR_InfuriateSTGauge = new("WAR_InfuriateSTGauge", 40),
            WAR_InfuriateAoEGauge = new("WAR_InfuriateAoEGauge", 40),
            WAR_EyePath_Refresh = new("WAR_EyePath", 10),
            WAR_ST_Bloodwhetting_Health = new("WAR_ST_BloodwhettingOption", 90),
            WAR_ST_Bloodwhetting_SubOption = new("WAR_ST_Bloodwhetting_SubOption", 1),
            WAR_ST_Equilibrium_Health = new("WAR_ST_EquilibriumOption", 50),
            WAR_ST_Equilibrium_SubOption = new("WAR_ST_Equilibrium_SubOption", 1),
            WAR_ST_Rampart_Health = new("WAR_ST_Rampart_Health", 80),
            WAR_ST_Rampart_SubOption = new("WAR_ST_Rampart_SubOption", 1),
            WAR_ST_Thrill_Health = new("WAR_ST_Thrill_Health", 70),
            WAR_ST_Thrill_SubOption = new("WAR_ST_Thrill_SubOption", 1),
            WAR_ST_Vengeance_Health = new("WAR_ST_Vengeance_Health", 60),
            WAR_ST_Vengeance_SubOption = new("WAR_ST_Vengeance_SubOption", 1),
            WAR_ST_Holmgang_Health = new("WAR_ST_Holmgang_Health", 30),
            WAR_ST_Holmgang_SubOption = new("WAR_ST_Holmgang_SubOption", 1),
            WAR_AoE_Bloodwhetting_Health = new("WAR_AoE_BloodwhettingOption", 90),
            WAR_AoE_Bloodwhetting_SubOption = new("WAR_AoE_Bloodwhetting_SubOption", 1),
            WAR_AoE_Equilibrium_Health = new("WAR_AoE_EquilibriumOption", 50),
            WAR_AoE_Equilibrium_SubOption = new("WAR_AoE_Equilibrium_SubOption", 1),
            WAR_AoE_Rampart_Health = new("WAR_AoE_Rampart_Health", 80),
            WAR_AoE_Rampart_SubOption = new("WAR_AoE_Rampart_SubOption", 1),
            WAR_AoE_Thrill_Health = new("WAR_AoE_Thrill_Health", 80),
            WAR_AoE_Thrill_SubOption = new("WAR_AoE_Thrill_SubOption", 1),
            WAR_AoE_Vengeance_Health = new("WAR_AoE_Vengeance_Health", 60),
            WAR_AoE_Vengeance_SubOption = new("WAR_AoE_Vengeance_SubOption", 1),
            WAR_AoE_Holmgang_Health = new("WAR_AoE_Holmgang_Health", 30),
            WAR_AoE_Holmgang_SubOption = new("WAR_AoE_Holmgang_SubOption", 1),
            WAR_VariantCure = new("WAR_VariantCure"),
            WAR_Mit_Holmgang_Health = new("WAR_Mit_Holmgang_Health", 30);


        internal static void Draw(CustomComboPreset preset)
        {
            switch (preset)
            {
                case CustomComboPreset.WAR_Variant_Cure:
                    UserConfig.DrawSliderInt(1, 100, WAR_VariantCure,
                        "Player HP% to be \nless than or equal to:", 200);

                    break;

                case CustomComboPreset.WAR_InfuriateFellCleave:
                    UserConfig.DrawSliderInt(0, 50, WAR_InfuriateRange,
                        "Use when gauge is less than or equal to:");

                    break;

                case CustomComboPreset.WAR_ST_Advanced_StormsEye:
                    UserConfig.DrawSliderInt(0, 30, WAR_SurgingRefreshRange,
                        $"Seconds remaining before refreshing {Buffs.SurgingTempest.StatusName()} buff");

                    break;

                case CustomComboPreset.WAR_EyePath:
                    UserConfig.DrawSliderInt(0, 30, WAR_EyePath_Refresh,
                        $"Seconds remaining before refreshing {Buffs.SurgingTempest.StatusName()} buff");

                    break;

                case CustomComboPreset.WAR_ST_Advanced_Onslaught:
                    UserConfig.DrawSliderInt(0, 2, WAR_KeepOnslaughtCharges,
                        "How many charges to keep ready?\n (0 = Use All)");

                    break;

                case CustomComboPreset.WAR_ST_Advanced_Infuriate:
                    UserConfig.DrawSliderInt(0, 2, WAR_KeepInfuriateCharges,
                        "How many charges to keep ready?\n (0 = Use All)");

                    UserConfig.DrawSliderInt(0, 50, WAR_InfuriateSTGauge,
                        "Use when gauge is less than or equal to:");

                    break;

                case CustomComboPreset.WAR_ST_Advanced_FellCleave:
                    UserConfig.DrawSliderInt(50, 100, WAR_FellCleaveGauge,
                        "Minimum gauge required to spend");

                    break;

                case CustomComboPreset.WAR_AoE_Advanced_Decimate:
                    UserConfig.DrawSliderInt(50, 100, WAR_DecimateGauge,
                        "Minimum gauge required to spend");

                    break;

                case CustomComboPreset.WAR_AoE_Advanced_Infuriate:
                    UserConfig.DrawSliderInt(0, 50, WAR_InfuriateAoEGauge, 
                        "Use when gauge is under or equal to");

                    break;

                case CustomComboPreset.WARPvP_BurstMode_Blota:
                    UserConfig.DrawHorizontalRadioButton(WARPvP.Config.WARPVP_BlotaTiming, $"Before {PrimalRend.ActionName()}", "", 0);
                    UserConfig.DrawHorizontalRadioButton(WARPvP.Config.WARPVP_BlotaTiming, $"After {PrimalRend.ActionName()}", "", 1);

                    break;

                case CustomComboPreset.WAR_ST_Advanced_Bloodwhetting:
                    UserConfig.DrawSliderInt(1, 100, WAR_ST_Bloodwhetting_Health,
                        "Player HP% to be \nless than or equal to:", 200);

                    UserConfig.DrawHorizontalRadioButton(WAR_ST_Bloodwhetting_SubOption, 
                        "All Enemies",
                        $"Uses {Bloodwhetting.ActionName()} regardless of targeted enemy type.", 1);

                    UserConfig.DrawHorizontalRadioButton(WAR_ST_Bloodwhetting_SubOption, 
                        "Bosses Only",
                        $"Only uses {Bloodwhetting.ActionName()} when the targeted enemy is a boss.", 2);

                    break;

                case CustomComboPreset.WAR_AoE_Advanced_Bloodwhetting:
                    UserConfig.DrawSliderInt(1, 100, WAR_AoE_Bloodwhetting_Health,
                        "Player HP% to be \nless than or equal to:", 200);

                    UserConfig.DrawHorizontalRadioButton(WAR_AoE_Bloodwhetting_SubOption, 
                        "All Enemies",
                        $"Uses {Bloodwhetting.ActionName()} regardless of targeted enemy type.", 1);

                    UserConfig.DrawHorizontalRadioButton(WAR_AoE_Bloodwhetting_SubOption, 
                        "Bosses Only",
                       $"Only uses {Bloodwhetting.ActionName()} when the targeted enemy is a boss.", 2);

                    break;

                case CustomComboPreset.WAR_ST_Advanced_Equilibrium:
                    UserConfig.DrawSliderInt(1, 100, WAR_ST_Equilibrium_Health,
                        "Player HP% to be \nless than or equal to:", 200);

                    UserConfig.DrawHorizontalRadioButton(WAR_ST_Equilibrium_SubOption,
                        "All Enemies",
                        $"Uses {Equilibrium.ActionName()} regardless of targeted enemy type.", 1);

                    UserConfig.DrawHorizontalRadioButton(WAR_ST_Equilibrium_SubOption,
                        "Bosses Only",
                        $"Only uses {Equilibrium.ActionName()} when the targeted enemy is a boss.", 2);

                    break;

                case CustomComboPreset.WAR_AoE_Advanced_Equilibrium:
                    UserConfig.DrawSliderInt(1, 100, WAR_AoE_Equilibrium_Health,
                        "Player HP% to be \nless than or equal to:", 200);

                    UserConfig.DrawHorizontalRadioButton(WAR_AoE_Equilibrium_SubOption,
                        "All Enemies",
                        $"Uses {Equilibrium.ActionName()} regardless of targeted enemy type.", 1);

                    UserConfig.DrawHorizontalRadioButton(WAR_AoE_Equilibrium_SubOption,
                        "Bosses Only",
                        $"Only uses {Equilibrium.ActionName()} when the targeted enemy is a boss.", 2);

                    break;

                case CustomComboPreset.WAR_ST_Advanced_Rampart:
                    UserConfig.DrawSliderInt(1, 100, WAR_ST_Rampart_Health,
                        "Player HP% to be \nless than or equal to:", 200);

                    UserConfig.DrawHorizontalRadioButton(WAR_ST_Rampart_SubOption, 
                        "All Enemies",
                        $"Uses {All.Rampart.ActionName()} regardless of targeted enemy type.", 1);

                    UserConfig.DrawHorizontalRadioButton(WAR_ST_Rampart_SubOption, 
                        "Bosses Only",
                        $"Only uses {All.Rampart.ActionName()} when the targeted enemy is a boss.", 2);

                    break;

                case CustomComboPreset.WAR_AoE_Advanced_Rampart:
                    UserConfig.DrawSliderInt(1, 100, WAR_AoE_Rampart_Health,
                        "Player HP% to be \nless than or equal to:", 200);

                    UserConfig.DrawHorizontalRadioButton(WAR_AoE_Rampart_SubOption, 
                        "All Enemies",
                        $"Uses {All.Rampart.ActionName()} regardless of targeted enemy type.", 1);

                    UserConfig.DrawHorizontalRadioButton(WAR_AoE_Rampart_SubOption, 
                        "Bosses Only",
                        $"Only uses {All.Rampart.ActionName()} when the targeted enemy is a boss.", 2);

                    break;

                case CustomComboPreset.WAR_ST_Advanced_Thrill:
                    UserConfig.DrawSliderInt(1, 100, WAR_ST_Thrill_Health,
                        "Player HP% to be \nless than or equal to:", 200);

                    UserConfig.DrawHorizontalRadioButton(WAR_ST_Thrill_SubOption,
                        "All Enemies",
                        $"Uses {ThrillOfBattle.ActionName()} regardless of targeted enemy type.", 1);

                    UserConfig.DrawHorizontalRadioButton(WAR_ST_Thrill_SubOption,
                        "Bosses Only",
                        $"Only uses {ThrillOfBattle.ActionName()} when the targeted enemy is a boss.", 2);

                    break;

                case CustomComboPreset.WAR_AoE_Advanced_Thrill:
                    UserConfig.DrawSliderInt(1, 100, WAR_AoE_Thrill_Health,
                        "Player HP% to be \nless than or equal to:", 200);

                    UserConfig.DrawHorizontalRadioButton(WAR_AoE_Thrill_SubOption,
                        "All Enemies",
                        $"Uses {ThrillOfBattle.ActionName()} regardless of targeted enemy type.", 1);

                    UserConfig.DrawHorizontalRadioButton(WAR_AoE_Thrill_SubOption,
                        "Bosses Only",
                        $"Only uses {ThrillOfBattle.ActionName()} when the targeted enemy is a boss.", 2);

                    break;

                case CustomComboPreset.WAR_ST_Advanced_Vengeance:
                    UserConfig.DrawSliderInt(1, 100, WAR_ST_Vengeance_Health, 
                        "Player HP% to be \nless than or equal to:", 200);

                    UserConfig.DrawHorizontalRadioButton(WAR_ST_Vengeance_SubOption, 
                        "All Enemies",
                        $"Uses {Vengeance.ActionName()} regardless of targeted enemy type.", 1);

                    UserConfig.DrawHorizontalRadioButton(WAR_ST_Vengeance_SubOption, 
                        "Bosses Only",
                        $"Only uses {Vengeance.ActionName()} when the targeted enemy is a boss.", 2);

                    break;

                case CustomComboPreset.WAR_AoE_Advanced_Vengeance:
                    UserConfig.DrawSliderInt(1, 100, WAR_AoE_Vengeance_Health,
                        "Player HP% to be \nless than or equal to:", 200);

                    UserConfig.DrawHorizontalRadioButton(WAR_AoE_Vengeance_SubOption, 
                        "All Enemies",
                        $"Uses {Vengeance.ActionName()} regardless of targeted enemy type.", 1);

                    UserConfig.DrawHorizontalRadioButton(WAR_AoE_Vengeance_SubOption, 
                        "Bosses Only",
                        $"Only uses {Vengeance.ActionName()} when the targeted enemy is a boss.", 2);

                    break;

                case CustomComboPreset.WAR_ST_Advanced_Holmgang:
                    UserConfig.DrawSliderInt(1, 100, WAR_ST_Holmgang_Health,
                        "Player HP% to be \nless than or equal to:", 200);

                    UserConfig.DrawHorizontalRadioButton(WAR_ST_Holmgang_SubOption, 
                        "All Enemies",
                        $"Uses {Holmgang.ActionName()} regardless of targeted enemy type.", 1);

                    UserConfig.DrawHorizontalRadioButton(WAR_ST_Holmgang_SubOption, 
                        "Bosses Only",
                        $"Only uses {Holmgang.ActionName()} when the targeted enemy is a boss.", 2);

                    break;

                case CustomComboPreset.WAR_AoE_Advanced_Holmgang:
                    UserConfig.DrawSliderInt(1, 100, WAR_AoE_Holmgang_Health,
                        "Player HP% to be \nless than or equal to:", 200);

                    UserConfig.DrawHorizontalRadioButton(WAR_AoE_Holmgang_SubOption, 
                        "All Enemies",
                        $"Uses {Holmgang.ActionName()} regardless of targeted enemy type.", 1);

                    UserConfig.DrawHorizontalRadioButton(WAR_AoE_Holmgang_SubOption, 
                        "Bosses Only",
                        $"Only uses {Holmgang.ActionName()} when the targeted enemy is a boss.", 2);

                    break;

                case CustomComboPreset.WAR_Mit_Holmgang:
                    UserConfig.DrawSliderInt(1, 100, WAR_Mit_Holmgang_Health,
                        "Player HP% to be \nless than or equal to:", 200);

                    break;
            }
        }
    }
}
