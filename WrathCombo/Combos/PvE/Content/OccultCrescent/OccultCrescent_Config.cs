using Dalamud.Interface.Colors;
using ECommons.ImGuiMethods;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using WrathCombo.Resources.Localization.JobConfigs;
using WrathCombo.Window.Functions;
using static WrathCombo.Window.Functions.UserConfig;
using static WrathCombo.Window.Text;
namespace WrathCombo.Combos.PvE;

internal partial class OccultCrescent
{
    internal static class Config
    {
        internal static void Draw(Preset preset)
        {
            switch (preset)
            {
                case Preset.Phantom_Freelancer_OccultResuscitation:
                    DrawSliderInt(1, 100, Phantom_Freelancer_Resuscitation_Health,
                        Generics.StopFriendlyHpPercent100, 200);
                    break;

                case Preset.Phantom_Geomancer_Sunbath:
                    DrawSliderInt(1, 100, Phantom_Geomancer_Sunbath_Health,
                        Generics.StopFriendlyHpPercent100, 200);
                    break;

                case Preset.Phantom_Knight_PhantomGuard:
                    DrawSliderInt(1, 100, Phantom_Knight_PhantomGuard_Health,
                        Generics.StopFriendlyHpPercent100, 200);
                    break;
                case Preset.Phantom_Knight_Pray:
                    DrawSliderInt(1, 100, Phantom_Knight_Pray_Health,
                        Generics.StopFriendlyHpPercent100, 200);
                    DrawAdditionalBoolChoice(Phantom_Knight_Pray_KeepUp,
                        "Keep Pray up",
                        "Also use Pray whenever the buff is missing, not only below the HP threshold.");
                    break;
                case Preset.Phantom_Knight_OccultHeal:
                    DrawSliderInt(1, 100, Phantom_Knight_OccultHeal_Health,
                        Generics.StopFriendlyHpPercent100, 200);
                    break;
                case Preset.Phantom_Knight_Pledge:
                    DrawSliderInt(1, 100, Phantom_Knight_Pledge_Health,
                        Generics.StopFriendlyHpPercent100, 200);
                    DrawAdditionalBoolChoice(Phantom_Knight_Pledge_SelfOnly,
                        "Self only",
                        "When disabled, prefers the lowest-HP party member under the threshold.");
                    break;
                case Preset.Phantom_Bard_MightyMarch:
                    DrawSliderInt(1, 100, Phantom_Bard_MightyMarch_Health,
                        Generics.StopFriendlyHpPercent100, 200);
                    break;

                case Preset.Phantom_Monk_OccultChakra:
                    DrawSliderInt(1, 100, Phantom_Monk_OccultChakra_Health,
                        Generics.StopFriendlyHpPercent100, 200);
                    DrawSliderInt(0, 10000, Phantom_Monk_OccultChakra_MP,
                        Generics.MPLessOrEqual, sliderIncrement: SliderIncrements.Hundreds);
                    break;

                case Preset.Phantom_Monk_PhantomKick:
                    DrawSliderInt(1, 15, Phantom_Monk_PhantomKick_Distance,
                        "Max target distance (yalms) to use Phantom Kick", 200);
                    break;

                case Preset.Phantom_Oracle_Blessing:
                    DrawSliderInt(1, 100, Phantom_Oracle_Blessing_Health,
                        "Self or average party HP % at or below which to use Blessing", 200);
                    break;

                case Preset.Phantom_Oracle_PhantomJudgment:
                    DrawSliderInt(1, 100, Phantom_Oracle_Judgment_PartyHP,
                        "Self or average party HP % at or below which to prioritize Judgment as a heal", 200);
                    break;

                case Preset.Phantom_Oracle_Starfall:
                    DrawSliderInt(91, 100, Phantom_Oracle_Starfall_Health,
                        Generics.PlayerHPGreaterOrEqual, 200);
                    break;

                case Preset.Phantom_Oracle_PhantomRejuvenation:
                    DrawSliderInt(1, 100, Phantom_Oracle_PhantomRejuvenation_Health,
                        Generics.StopFriendlyHpPercent100, 200);
                    break;

                case Preset.Phantom_Oracle_Invulnerability:
                    DrawAdditionalBoolChoice(Phantom_Oracle_SaveInvulnForStarfall,
                        "Save for Starfall",
                        "Use Invulnerability before Starfall when Prediction of Starfall is up.");
                    if (!Phantom_Oracle_SaveInvulnForStarfall)
                    {
                        DrawSliderInt(1, 100, Phantom_Oracle_Invulnerability_Health,
                            Generics.StopFriendlyHpPercent100, 200);
                    }
                    break;

                case Preset.Phantom_Geomancer_Suspend:
                    DrawAdditionalBoolChoice(Phantom_Geomancer_Suspend_InCombat,
                        "In combat", "Use Suspend while in combat.");
                    DrawAdditionalBoolChoice(Phantom_Geomancer_Suspend_OutOfCombat,
                        "Out of combat", "Use Suspend while out of combat.");
                    break;

                case Preset.Phantom_BlackMage_OccultToad:
                    DrawAdditionalBoolChoice(Phantom_BlackMage_OccultToad_RequireAoE,
                        "Only as AoE mit",
                        "Only use Occult Toad with 2+ targets in range, or when raidwide damage is incoming.");
                    break;

                case Preset.Phantom_Ranger_OccultUnicorn:
                    DrawSliderInt(1, 100, Phantom_Ranger_OccultUnicorn_Health,
                        Generics.StopFriendlyHpPercent100, 200);
                    break;

                case Preset.Phantom_Ranger_PhantomAim:
                    DrawSliderInt(1, 100, Phantom_Ranger_PhantomAim_Stop,
                        Generics.PlayerHPGreaterOrEqual, 200);
                    break;

                case Preset.Phantom_Dragoon_OccultJump:
                    DrawHorizontalMultiChoice(Phantom_Dragoon_OccultJumpMovingOrInRanged,
                        Generics.NoMovement,
                        Generics.OnlyUse0WhenNotMoving, 2, 0);

                    DrawHorizontalMultiChoice(Phantom_Dragoon_OccultJumpMovingOrInRanged,
                        Generics.InMeleeRange,
                        Generics.OnlyUse0WhenInMeleeRange, 2, 1);
                    break;

                case Preset.Phantom_Thief_Steal:
                    DrawSliderInt(1, 50, Phantom_Thief_Steal_Health,
                        Generics.PlayerHPGreaterOrEqual, 200);
                    break;

                case Preset.Phantom_Samurai_Zeninage:
                    ImGui.Indent();
                    ImGuiEx.TextWrapped(ImGuiColors.DalamudRed, Resources
                        .Localization.Content.OccultCrescent.Costly);
                    ImGui.Unindent();
                    break;

                case Preset.Phantom_Chemist_OccultPotion:
                    ImGui.Indent();
                    ImGuiEx.TextWrapped(ImGuiColors.DalamudRed, Resources
                        .Localization.Content.OccultCrescent.Costly);
                    ImGui.Unindent();
                    DrawSliderInt(1, 100, Phantom_Chemist_OccultPotion_Health,
                        Generics.StopFriendlyHpPercent100, 200);
                    DrawAdditionalBoolChoice(Phantom_Chemist_OccultPotion_SelfOnly,
                        "Self only", "When disabled, also triggers if the lowest party member is below the HP threshold.");
                    break;

                case Preset.Phantom_Chemist_OccultEther:
                    ImGui.Indent();
                    ImGuiEx.TextWrapped(ImGuiColors.DalamudRed, Resources
                        .Localization.Content.OccultCrescent.Costly);
                    ImGui.Unindent();
                    DrawSliderInt(1, 10000, Phantom_Chemist_OccultEther_MP,
                        Generics.MPLessOrEqual, sliderIncrement: SliderIncrements.Hundreds);
                    DrawAdditionalBoolChoice(Phantom_Chemist_OccultEther_SelfOnly,
                        "Self only", "When disabled, also triggers if any party member is below the MP threshold.");
                    break;

                case Preset.Phantom_Chemist_OccultElixir:
                    ImGui.Indent();
                    ImGuiEx.TextWrapped(ImGuiColors.DalamudRed, Resources.Localization.Content.OccultCrescent.VeryCostly);
                    ImGui.Unindent();
                    DrawSliderInt(1, 100, Phantom_Chemist_OccultElixir_HP,
                        Resources.Localization.Content.OccultCrescent.AveragePartyHPLessOrEqual, 200);
                    DrawAdditionalBoolChoice(Phantom_Chemist_OccultElixir_RequireParty,
                        Resources.Localization.Content.OccultCrescent.AtLeast1PartyMember, "");
                    ImGui.Indent();
                    ImGuiEx.TextWrapped(ImGuiColors.DalamudYellow, Resources.Localization.Content.OccultCrescent.NotAdvised);
                    ImGuiEx.TextWrapped(ImGuiColors.DalamudYellow, Resources.Localization.Content.OccultCrescent.SliderShouldBeLow);
                    ImGui.Unindent();
                    break;

                case Preset.Phantom_TimeMage_OccultComet:
                    DrawAdditionalBoolChoice(Phantom_TimeMage_Comet_RequireSpeed,
                        FormatAndCache(Resources.Localization.Content.OccultCrescent.Requires0Or1ToUse2, Caster.Role.Swiftcast.ActionName(), Buffs.OccultQuick.StatusName(), OccultComet.ActionName()), "");
                    if (Phantom_TimeMage_Comet_RequireSpeed)
                    {
                        ImGui.Indent();
                        DrawAdditionalBoolChoice(
                            Phantom_TimeMage_Comet_UseSpeed,
                            FormatAndCache(Resources.Localization.Content.OccultCrescent.Add0Or1PriorUsing2, Caster.Role.Swiftcast.ActionName(), Buffs.OccultQuick.StatusName(), OccultComet.ActionName()), "");
                        ImGui.Unindent();
                    }
                    break;

                case Preset.Phantom_RestrictToBuff:
                    ImGui.Indent();
                    ImGuiEx.TextWrapped(ImGuiColors.DalamudYellow,
                        Resources.Localization.Content.OccultCrescent.BuffOnlyNotRecommended);
                    ImGuiEx.TextWrapped(ImGuiColors.DalamudRed,
                        Resources.Localization.Content.OccultCrescent.BuffOnlyDont);
                    ImGuiEx.TextWrapped(ImGuiColors.DalamudGrey,
                        Resources.Localization.Content.OccultCrescent.BuffOnlyList);
                    ImGui.Unindent();
                    break;

                case Preset.Phantom_WhiteMage_OccultCureII:
                    DrawSliderInt(1, 100, Phantom_WhiteMage_OccultCureII_Health,
                        Generics.StopFriendlyHpPercent100, 200);
                    break;
                case Preset.Phantom_WhiteMage_OccultCureIII:
                    DrawSliderInt(1, 100, Phantom_WhiteMage_OccultCureIII_Health,
                        Generics.StopFriendlyHpPercent100, 200);
                    break;
                case Preset.Phantom_BlueMage_OccultWhiteWind:
                    DrawSliderInt(1, 100, Phantom_BlueMage_OccultWhiteWind_Health,
                        Generics.StopFriendlyHpPercent100, 200);
                    break;
                case Preset.Phantom_RedMage_OccultCureII:
                    DrawSliderInt(1, 100, Phantom_RedMage_OccultCureII_Health,
                        Generics.StopFriendlyHpPercent100, 200);
                    break;
                case Preset.Phantom_RedMage_OccultCureII_Retarget:
                    DrawAdditionalBoolChoice(Phantom_RedMage_Retarget_OutOfParty, 
                        "Retarget to Out of Party Players", 
                        "Also retargets if anyone outside your party falls below this HP%");
                    break;
                case Preset.Phantom_Necromancer_DrainTouch:
                    ImGui.Indent();
                    ImGui.Text("Drain Touch usage:");
                    DrawHorizontalRadioButton(Phantom_Necromancer_DrainTouch_Mode,
                        "DPS", "Use for damage (respects Restrict to Buff).", 0);
                    DrawHorizontalRadioButton(Phantom_Necromancer_DrainTouch_Mode,
                        "Heal", "Only when your HP is at or below the heal threshold.", 1);
                    DrawHorizontalRadioButton(Phantom_Necromancer_DrainTouch_Mode,
                        "Emergency", "Only when your HP is at or below the emergency threshold.", 2);
                    ImGui.Unindent();
                    if (Phantom_Necromancer_DrainTouch_Mode == 1)
                    {
                        DrawSliderInt(1, 100, Phantom_Necromancer_DrainTouch_Health,
                            Generics.StopFriendlyHpPercent100, 200);
                    }
                    else if (Phantom_Necromancer_DrainTouch_Mode == 2)
                    {
                        DrawSliderInt(1, 100, Phantom_Necromancer_DrainTouch_EmergencyHealth,
                            Generics.StopFriendlyHpPercent100, 200);
                    }
                    break;

                case Preset.Phantom_Necromancer:
                    ImGui.Indent();
                    ImGui.Text("Necromancer spells while Drain Touch is active:");
                    DrawHorizontalRadioButton(Phantom_Necromancer_SpellDuringDrainTouch,
                        "Only when inactive",
                        "Cast Deep Freeze / Hell Wind / Chaos Drive / Doomsday only while Drain Touch is down.", 0);
                    DrawHorizontalRadioButton(Phantom_Necromancer_SpellDuringDrainTouch,
                        "Only when active",
                        "Cast those spells only during the Drain Touch buff.", 1);
                    DrawHorizontalRadioButton(Phantom_Necromancer_SpellDuringDrainTouch,
                        "Either",
                        "Ignore Drain Touch buff for spell usage.", 2);
                    ImGui.Unindent();
                    break;

                case Preset.Phantom_Gladiator_Defend:
                    DrawAdditionalBoolChoice(Phantom_Gladiator_DefendOnlyAtMaxFervor,
                        "Only at 4 Finishing Fervor",
                        "Keep Defend for when you have full Finishing Fervor stacks.");
                    break;

                case Preset.Phantom_Cannoneer_DarkCannon:
                case Preset.Phantom_Cannoneer_ShockCannon:
                    ImGui.Indent();
                    ImGui.Text("When both Blind and Paralysis can apply:");
                    DrawHorizontalRadioButton(Phantom_Cannoneer_DarkShockPrefer,
                        "Prefer Dark Cannon", "Blind", 0);
                    DrawHorizontalRadioButton(Phantom_Cannoneer_DarkShockPrefer,
                        "Prefer Shock Cannon", "Paralysis", 1);
                    ImGui.Text("When neither can apply (immune / capped):");
                    DrawHorizontalRadioButton(Phantom_Cannoneer_DarkShockImmunePrefer,
                        "Use Dark Cannon", "", 0);
                    DrawHorizontalRadioButton(Phantom_Cannoneer_DarkShockImmunePrefer,
                        "Use Shock Cannon", "", 1);
                    ImGui.Unindent();
                    break;
            }
        }
        #region Variables

        public static UserInt
            Phantom_Freelancer_Resuscitation_Health = new("Phantom_Freelancer_Resuscitation_Health", 50),
            Phantom_Geomancer_Sunbath_Health = new("Phantom_Geomancer_Sunbath_Health", 50),
            Phantom_Knight_PhantomGuard_Health = new("Phantom_Knight_PhantomGuard_Health", 50),
            Phantom_Knight_Pray_Health = new("Phantom_Knight_Pray_Health", 50),
            Phantom_Knight_OccultHeal_Health = new("Phantom_Knight_OccultHeal_Health", 50),
            Phantom_Knight_Pledge_Health = new("Phantom_Knight_Pledge_Health", 50),
            Phantom_Bard_MightyMarch_Health = new("Phantom_Bard_MightyMarch_Health", 50),
            Phantom_Monk_OccultChakra_Health = new("Phantom_Monk_OccultChakra_Health", 29),
            Phantom_Monk_OccultChakra_MP = new("Phantom_Monk_OccultChakra_MP", 3000),
            Phantom_Monk_PhantomKick_Distance = new("Phantom_Monk_PhantomKick_Distance", 5),
            Phantom_Chemist_OccultPotion_Health = new("Phantom_Chemist_OccultPotion_Health", 50),
            Phantom_Chemist_OccultEther_MP = new("Phantom_Chemist_OccultEther_MP", 2000),
            Phantom_Chemist_OccultElixir_HP = new("Phantom_Chemist_OccultElixir_HP", 25),
            Phantom_Oracle_Blessing_Health = new("Phantom_Oracle_Blessing_Health", 50),
            Phantom_Oracle_Judgment_PartyHP = new("Phantom_Oracle_Judgment_PartyHP", 70),
            Phantom_Oracle_Starfall_Health = new("Phantom_Oracle_Starfall_Health", 100),
            Phantom_Oracle_PhantomRejuvenation_Health = new("Phantom_Oracle_PhantomRejuvenation_Health", 50),
            Phantom_Oracle_Invulnerability_Health = new("Phantom_Oracle_Invulnerability_Health", 30),
            Phantom_Ranger_OccultUnicorn_Health = new("Phantom_Ranger_OccultUnicorn_Health", 50),
            Phantom_Ranger_PhantomAim_Stop = new("Phantom_Ranger_PhantomAim_Stop", 30),
            Phantom_Thief_Steal_Health = new("Phantom_Thief_Steal_Health", 10),
            Phantom_WhiteMage_OccultCureII_Health = new("Phantom_WhiteMage_OccultCureII_Health", 50),
            Phantom_WhiteMage_OccultCureIII_Health = new("Phantom_WhiteMage_OccultCureIII_Health", 40),
            Phantom_BlueMage_OccultWhiteWind_Health = new("Phantom_BlueMage_OccultWhiteWind_Health", 50),
            Phantom_RedMage_OccultCureII_Health = new("Phantom_RedMage_OccultCureII_Health", 50),
            Phantom_Necromancer_DrainTouch_Health = new("Phantom_Necromancer_DrainTouch_Health", 50),
            Phantom_Necromancer_DrainTouch_EmergencyHealth = new("Phantom_Necromancer_DrainTouch_EmergencyHealth", 25),
            Phantom_Necromancer_DrainTouch_Mode = new("Phantom_Necromancer_DrainTouch_Mode", 0),
            Phantom_Necromancer_SpellDuringDrainTouch = new("Phantom_Necromancer_SpellDuringDrainTouch", 0),
            Phantom_Cannoneer_DarkShockPrefer = new("Phantom_Cannoneer_DarkShockPrefer", 0),
            Phantom_Cannoneer_DarkShockImmunePrefer = new("Phantom_Cannoneer_DarkShockImmunePrefer", 0);

        public static UserBool
            Phantom_Chemist_OccultElixir_RequireParty = new("Phantom_Chemist_OccultElixir_RequireParty", true),
            Phantom_Chemist_OccultPotion_SelfOnly = new("Phantom_Chemist_OccultPotion_SelfOnly", true),
            Phantom_Chemist_OccultEther_SelfOnly = new("Phantom_Chemist_OccultEther_SelfOnly", true),
            Phantom_TimeMage_Comet_RequireSpeed = new("Phantom_TimeMage_Comet_RequireSpeed", true),
            Phantom_TimeMage_Comet_UseSpeed = new("Phantom_TimeMage_Comet_UseSpeed", true),
            Phantom_Oracle_SaveInvulnForStarfall = new("Phantom_Oracle_SaveInvulnForStarfall", true),
            Phantom_Gladiator_DefendOnlyAtMaxFervor = new("Phantom_Gladiator_DefendOnlyAtMaxFervor", false),
            Phantom_Knight_Pray_KeepUp = new("Phantom_Knight_Pray_KeepUp", true),
            Phantom_Knight_Pledge_SelfOnly = new("Phantom_Knight_Pledge_SelfOnly", false),
            Phantom_Geomancer_Suspend_InCombat = new("Phantom_Geomancer_Suspend_InCombat", false),
            Phantom_Geomancer_Suspend_OutOfCombat = new("Phantom_Geomancer_Suspend_OutOfCombat", false),
            Phantom_RedMage_Retarget_OutOfParty = new("Phantom_RedMage_Retarget_OutOfParty", false),
            Phantom_BlackMage_OccultToad_RequireAoE = new("Phantom_BlackMage_OccultToad_RequireAoE", true);

        public static UserBoolArray
            Phantom_Dragoon_OccultJumpMovingOrInRanged = new("Phantom_Dragoon_OccultJumpMovingOrInRanged", [true, true]);

        #endregion
    }
}
