using Dalamud.Interface.Colors;
using ECommons.ImGuiMethods;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Window.Functions;
using static WrathCombo.Window.Functions.UserConfig;
namespace WrathCombo.Combos.PvE;

internal partial class OccultCrescent
{
    internal static class Config
    {
        public static UserInt
            Phantom_Freelancer_Resuscitation_Health = new("Phantom_Freelancer_Resuscitation_Health", 50),
            Phantom_Geomancer_Sunbath_Health = new("Phantom_Geomancer_Sunbath_Health", 50),
            Phantom_Knight_PhantomGuard_Health = new("Phantom_Knight_PhantomGuard_Health", 50),
            Phantom_Knight_Pray_Health = new("Phantom_Knight_Pray_Health", 50),
            Phantom_Knight_OccultHeal_Health = new("Phantom_Knight_OccultHeal_Health", 50),
            Phantom_Knight_Pledge_Health = new("Phantom_Knight_Pledge_Health", 50),
            Phantom_Bard_MightyMarch_Health = new("Phantom_Bard_MightyMarch_Health", 50),
            Phantom_Monk_OccultChakra_Health = new("Phantom_Monk_OccultChakra_Health", 29),
            Phantom_Chemist_OccultPotion_Health = new("Phantom_Chemist_OccultPotion_Health", 50),
            Phantom_Chemist_OccultEther_MP = new("Phantom_Chemist_OccultEther_MP", 50),
            Phantom_Chemist_OccultElixir_HP = new("Phantom_Chemist_OccultElixir_HP", 25),
            Phantom_Oracle_Blessing_Health = new("Phantom_Oracle_Blessing_Health", 50),
            Phantom_Oracle_Starfall_Health = new("Phantom_Oracle_Starfall_Health", 100),
            Phantom_Ranger_OccultUnicorn_Health = new("Phantom_Ranger_OccultUnicorn_Health", 50),
            Phantom_Ranger_PhantomAim_Stop = new("Phantom_Ranger_PhantomAim_Stop", 30),
            Phantom_Thief_Steal_Health = new("Phantom_Thief_Steal_Health", 10);

        public static UserBool
            Phantom_Chemist_OccultElixir_RequireParty = new("Phantom_Chemist_OccultElixir_RequireParty", true),
            Phantom_TimeMage_Comet_RequireSpeed = new("Phantom_TimeMage_Comet_RequireSpeed", true),
            Phantom_TimeMage_Comet_UseSpeed = new("Phantom_TimeMage_Comet_UseSpeed", true);

        internal static void Draw(Preset preset)
        {
            switch (preset)
            {
                case Preset.Phantom_Freelancer_OccultResuscitation:
                    DrawSliderInt(1, 100, Phantom_Freelancer_Resuscitation_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    break;

                case Preset.Phantom_Geomancer_Sunbath:
                    DrawSliderInt(1, 100, Phantom_Geomancer_Sunbath_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    break;

                case Preset.Phantom_Knight_PhantomGuard:
                    DrawSliderInt(1, 100, Phantom_Knight_PhantomGuard_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    break;
                case Preset.Phantom_Knight_Pray:
                    DrawSliderInt(1, 100, Phantom_Knight_Pray_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    break;
                case Preset.Phantom_Knight_OccultHeal:
                    DrawSliderInt(1, 100, Phantom_Knight_OccultHeal_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    break;
                case Preset.Phantom_Knight_Pledge:
                    DrawSliderInt(1, 100, Phantom_Knight_Pledge_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    break;
                case Preset.Phantom_Bard_MightyMarch:
                    DrawSliderInt(1, 100, Phantom_Bard_MightyMarch_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    break;

                case Preset.Phantom_Monk_OccultChakra:
                    DrawSliderInt(1, 100, Phantom_Monk_OccultChakra_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    break;

                case Preset.Phantom_Oracle_Blessing:
                    DrawSliderInt(1, 100, Phantom_Oracle_Blessing_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    break;

                case Preset.Phantom_Oracle_Starfall:
                    DrawSliderInt(91, 100, Phantom_Oracle_Starfall_Health,
                        "Player HP% to be \ngreater than or equal to:", 200);
                    break;

                case Preset.Phantom_Ranger_OccultUnicorn:
                    DrawSliderInt(1, 100, Phantom_Ranger_OccultUnicorn_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    break;

                case Preset.Phantom_Ranger_PhantomAim:
                    DrawSliderInt(1, 100, Phantom_Ranger_PhantomAim_Stop,
                        "Target HP% to be \ngreater than or equal to:", 200);
                    break;

                case Preset.Phantom_Thief_Steal:
                    DrawSliderInt(1, 50, Phantom_Thief_Steal_Health,
                        "Target HP% to be \nless than or equal to:", 200);
                    break;

                case Preset.Phantom_Chemist_OccultPotion:
                    DrawSliderInt(1, 100, Phantom_Chemist_OccultPotion_Health,
                        "Player HP% to be \nless than or equal to:", 200);
                    break;

                case Preset.Phantom_Chemist_OccultEther:
                    DrawSliderInt(1, 10000, Phantom_Chemist_OccultEther_MP,
                        "Player MP to be \nless than or equal to:", sliderIncrement: SliderIncrements.Hundreds);
                    break;

                case Preset.Phantom_Chemist_OccultElixir:
                    ImGui.Indent();
                    ImGuiEx.TextWrapped(ImGuiColors.DalamudRed, "This is a VERY costly Feature!");
                    ImGui.Unindent();
                    DrawSliderInt(1, 100, Phantom_Chemist_OccultElixir_HP,
                        "Avg Party HP to be \nless than or equal to:", 200);
                    DrawAdditionalBoolChoice(Phantom_Chemist_OccultElixir_RequireParty,
                        "Require at least 1 party member", "");
                    ImGui.Indent();
                    ImGuiEx.TextWrapped(ImGuiColors.DalamudYellow, "Not advisable in most situations!");
                    ImGuiEx.TextWrapped(ImGuiColors.DalamudYellow, "The slider value should be rather low, if you do use it!");
                    ImGui.Unindent();
                    break;

                case Preset.Phantom_TimeMage_OccultComet:
                    DrawAdditionalBoolChoice(Phantom_TimeMage_Comet_RequireSpeed,
                        "Require Swiftcast or Occult Quick to use Comet", "");
                    if (Phantom_TimeMage_Comet_RequireSpeed)
                    {
                        ImGui.Indent();
                        DrawAdditionalBoolChoice(
                            Phantom_TimeMage_Comet_UseSpeed,
                            "Add Swiftcast or Occult Quick prior to using Comet", "");
                        ImGui.Unindent();
                    }
                    break;
            }
        }
    }
}
