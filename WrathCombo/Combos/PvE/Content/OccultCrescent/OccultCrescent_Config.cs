using ECommons.DalamudServices;
using ImGuiNET;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Data;
using WrathCombo.Extensions;
using WrathCombo.Window.Functions;
using BossAvoidance = WrathCombo.Combos.PvE.All.Enums.BossAvoidance;
using PartyRequirement = WrathCombo.Combos.PvE.All.Enums.PartyRequirement;

namespace WrathCombo.Combos.PvE;

internal partial class OccultCrescent
{
    internal static class Config
    {
        public static UserInt
            Phantom_Freelancer_Resuscitation_Health = new("Phantom_Freelancer_Resuscitation_Health", 50),
            Phantom_Geomancer_Sunbath_Health = new ("Phantom_Geomancer_Sunbath_Health", 50),
            Phantom_Knight_PhantomGuard_Health = new("Phantom_Knight_PhantomGuard_Health", 50),
            Phantom_Knight_Pray_Health = new("Phantom_Knight_Pray_Health", 50),
            Phantom_Knight_OccultHeal_Health = new("Phantom_Knight_OccultHeal_Health", 50),
            Phantom_Knight_Pledge_Health = new("Phantom_Knight_Pledge_Health", 50),
            Phantom_Bard_MightyMarch_Health = new("Phantom_Bard_MightyMarch_Health", 50),
            Phantom_Monk_OccultChakra_Health = new("Phantom_Monk_OccultChakra_Health", 29),
            Phantom_Chemist_OccultPotion_Health = new("Phantom_Chemist_OccultPotion_Health", 50),
            Phantom_Chemist_OccultEther_MP = new("Phantom_Chemist_OccultEther_MP", 50),
            Phantom_Oracle_Blessing_Health = new("Phantom_Oracle_Blessing_Health", 50),
            Phantom_Oracle_Starfall_Health = new("Phantom_Oracle_Starfall_Health", 100),
            Phantom_Ranger_OccultUnicorn_Health = new("Phantom_Ranger_OccultUnicorn_Health", 50),
            Phantom_Ranger_PhantomAim_Stop = new("Phantom_Ranger_PhantomAim_Stop", 30),
            Phantom_Thief_Steal_Health = new("Phantom_Thief_Steal_Health", 10);

        internal static void Draw(CustomComboPreset preset)
        {
            switch (preset)
            {
                case CustomComboPreset.Phantom_Freelancer_OccultResuscitation:
                    UserConfig.DrawSliderInt(1, 100, Phantom_Freelancer_Resuscitation_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    break;

                case CustomComboPreset.Phantom_Geomancer_Sunbath:
                    UserConfig.DrawSliderInt(1, 100, Phantom_Geomancer_Sunbath_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    break;

                case CustomComboPreset.Phantom_Knight_PhantomGuard:
                    UserConfig.DrawSliderInt(1, 100, Phantom_Knight_PhantomGuard_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    break;
                case CustomComboPreset.Phantom_Knight_Pray:
                    UserConfig.DrawSliderInt(1, 100, Phantom_Knight_Pray_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    break;
                case CustomComboPreset.Phantom_Knight_OccultHeal:
                    UserConfig.DrawSliderInt(1, 100, Phantom_Knight_OccultHeal_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    break;
                case CustomComboPreset.Phantom_Knight_Pledge:
                    UserConfig.DrawSliderInt(1, 100, Phantom_Knight_Pledge_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    break;
                case CustomComboPreset.Phantom_Bard_MightyMarch:
                    UserConfig.DrawSliderInt(1, 100, Phantom_Bard_MightyMarch_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    break;

                case CustomComboPreset.Phantom_Monk_OccultChakra:
                    UserConfig.DrawSliderInt(1, 100, Phantom_Monk_OccultChakra_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    break;

                case CustomComboPreset.Phantom_Oracle_Blessing:
                    UserConfig.DrawSliderInt(1, 100, Phantom_Oracle_Blessing_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    break;

                case CustomComboPreset.Phantom_Oracle_Starfall:
                    UserConfig.DrawSliderInt(91, 100, Phantom_Oracle_Starfall_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    break;

                case CustomComboPreset.Phantom_Ranger_OccultUnicorn:
                    UserConfig.DrawSliderInt(1, 100, Phantom_Ranger_OccultUnicorn_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    break;

                case CustomComboPreset.Phantom_Ranger_PhantomAim:
                    UserConfig.DrawSliderInt(1, 100, Phantom_Ranger_PhantomAim_Stop,
                        "Target HP% to be \ngreater than or equal to:", 200);
                    break;

                case CustomComboPreset.Phantom_Thief_Steal:
                    UserConfig.DrawSliderInt(1, 50, Phantom_Thief_Steal_Health,
                        "Target HP% to be \nless than or equal to:", 200);
                    break;

                case CustomComboPreset.Phantom_Chemist_OccultPotion:
                    UserConfig.DrawSliderInt(1, 100, Phantom_Chemist_OccultPotion_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    break;

                case CustomComboPreset.Phantom_Chemist_OccultEther:
                    UserConfig.DrawSliderInt(1, 10000, Phantom_Chemist_OccultEther_MP,
                        "Player MP to be \nless than or equal to:", sliderIncrement: SliderIncrements.Hundreds);
                    break;
            }
        }
    }
}