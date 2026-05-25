using ECommons.ExcelServices;
using System.Linq;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.Extensions.UIntExtensions;
using static WrathCombo.Extensions.JobExtensions;
using static WrathCombo.Extensions.UShortExtensions;
using static WrathCombo.Window.Functions.SliderIncrements;
using static WrathCombo.Window.Functions.UserConfig;
using static WrathCombo.Window.Text;
using WrathCombo.Resources.Localization.JobConfigs;
namespace WrathCombo.Combos.PvE;

internal partial class SGE
{
    public static class Config
    {
        internal static void Draw(Preset preset)
        {
            switch (preset)
            {
                #region DPS

                case Preset.SGE_ST_DPS_Opener:
                    DrawHorizontalRadioButton(SGE_SelectedOpener,
                        FormatAndCache(Generics.Action_Opener, Toxikon.ActionName()),
                        FormatAndCache(Generics.Use_0_Opener, Toxikon.ActionName()), 0);

                    DrawHorizontalRadioButton(SGE_SelectedOpener,
                        FormatAndCache(Generics.Action_Opener, Pneuma.ActionName()),
                        FormatAndCache(Generics.Use_0_Opener, Pneuma.ActionName()), 1);

                    DrawBossOnlyChoice(SGE_Balance_Content);
                    break;

                case Preset.SGE_ST_DPS:
                    DrawHorizontalRadioButton(SGE_ST_DPS_Adv,
                        FormatAndCache(Generics.On0, Dosis.ActionName()),
                        // EukrasianDosisList is not a mistake.
                        FormatAndCache(Generics.ApplyToAll0, string.Join("\r\n", EukrasianDosisList.Select(x => x.Key.ActionName()))), 0);

                    DrawHorizontalRadioButton(SGE_ST_DPS_Adv,
                        FormatAndCache(Generics.On0, Dosis2.ActionName()),
                        FormatAndCache(Generics.ApplyOnlyTo0, Dosis2.ActionName()), 1);
                    break;

                case Preset.SGE_ST_DPS_EDosis:
                    DrawSliderInt(0, 100, SGE_ST_DPS_EukrasianDosisBossOption,
                        Generics.BossOnlyHpPercent);

                    DrawSliderInt(0, 100, SGE_ST_DPS_EukrasianDosisBossAddsOption,
                        Generics.BossEncounterNonBossHpPercent);

                    DrawSliderInt(0, 100, SGE_ST_DPS_EukrasianDosisTrashOption,
                        Generics.NonBossHpPercent);

                    ImGui.Indent();
                    DrawRoundedSliderFloat(0, 4,
                        SGE_ST_DPS_EukrasianDosisUptime_Threshold,
                        Generics.DoTSecondsRemainingZeroDisable, digits: 1);
                    ImGui.Unindent();
                    DrawAdditionalBoolChoice(SGE_ST_DPS_EDosis_TwoTarget,
                        Generics.TwoTargetDotting, Generics.TwoTargetDottingDescription);
                    break;

                case Preset.SGE_ST_DPS_Lucid:
                    DrawSliderInt(4000, 9500, SGE_ST_DPS_Lucid,
                        Generics.LucidMP, 150, Hundreds);
                    break;

                case Preset.SGE_ST_DPS_Rhizo:
                    DrawSliderInt(1, 3, SGE_ST_DPS_Rhizo,
                        FormatAndCache(Generics.Action_Threshold, Traits.Addersgall.TraitName()));
                    break;

                case Preset.SGE_ST_DPS_Phlegma:
                    if (!SGE_ST_DPS_Phlegma_Burst)
                    {
                        DrawSliderInt(0, 1, SGE_ST_DPS_Phlegma,
                            Generics.ChargePool);
                    }

                    DrawAdditionalBoolChoice(SGE_ST_DPS_Phlegma_Burst,
                        Generics.BurstOption,
                        FormatAndCache(Generics.Save0ChargesForBurst, Phlegma.ActionName()));
                    break;

                case Preset.SGE_ST_DPS_AddersgallProtect:
                    DrawSliderInt(1, 3,
                        SGE_ST_DPS_AddersgallProtect,
                        FormatAndCache(Generics.Action_Threshold, Traits.Addersgall.TraitName()));
                    break;

                case Preset.SGE_ST_DPS_Movement:
                    DrawHorizontalMultiChoice(SGE_ST_DPS_Movement, Toxikon.ActionName(),
                        FormatAndCache(Generics.Use0When1ChargesAreAvailable, Toxikon.ActionName(), Traits.Addersting.TraitName()), 3, 0);

                    DrawPriorityInput(SGE_ST_DPS_Movement_Priority, 3, 0, FormatAndCache(Generics.Action_Priority, Toxikon.ActionName()));

                    DrawHorizontalMultiChoice(SGE_ST_DPS_Movement, Dyskrasia.ActionName(),
                        FormatAndCache(Generics.Use0WhenInRangeOfEnemy, Dyskrasia.ActionName()), 3, 1);

                    DrawPriorityInput(SGE_ST_DPS_Movement_Priority, 3, 1, FormatAndCache(Generics.Action_Priority, Dyskrasia.ActionName()));

                    DrawHorizontalMultiChoice(SGE_ST_DPS_Movement, Eukrasia.ActionName(),
                        FormatAndCache(Generics.Use0, Eukrasia.ActionName()), 3, 2);

                    DrawPriorityInput(SGE_ST_DPS_Movement_Priority, 3, 2, FormatAndCache(Generics.Action_Priority, Eukrasia.ActionName()));
                    break;

                case Preset.SGE_AoE_DPS_Lucid:
                    DrawSliderInt(4000, 9500, SGE_AoE_DPS_Lucid,
                        Generics.LucidMP, 150, Hundreds);
                    break;

                case Preset.SGE_AoE_DPS_Pneuma:
                    DrawHorizontalRadioButton(SGE_AoE_DPS_PneumaBossOption,
                        Generics.AllContent, Generics.AllContentDescription, 0);

                    DrawHorizontalRadioButton(SGE_AoE_DPS_PneumaBossOption,
                        Generics.BossOnlyContent, Generics.BossOnlyDescription, 1);
                    break;

                case Preset.SGE_AoE_DPS_Rhizo:
                    DrawSliderInt(1, 3, SGE_AoE_DPS_Rhizo,
                        FormatAndCache(Generics.Action_Threshold, Traits.Addersgall.TraitName()));
                    break;

                case Preset.SGE_AoE_DPS_AddersgallProtect:
                    DrawSliderInt(1, 3, SGE_AoE_DPS_AddersgallProtect,
                        FormatAndCache(Generics.Action_Threshold, Traits.Addersgall.TraitName()));
                    break;

                #endregion

                #region Heal

                case Preset.SGE_ST_Heal:
                    DrawSliderInt(0, 2, SGE_Heal_HoldAddersgall,
                        Generics.ChargePool);

                    DrawAdditionalBoolChoice(SGE_ST_Heal_IncludeShields,
                        Generics.IncludeShields, "");
                    break;

                case Preset.SGE_ST_Heal_Esuna:
                    DrawSliderInt(0, 100, SGE_ST_Heal_Esuna,
                        Generics.StopFriendlyHpPercentZero);
                    break;

                case Preset.SGE_ST_Heal_Lucid:
                    DrawSliderInt(4000, 9500, SGE_ST_Heal_LucidOption,
                        Generics.LucidMP, 150, Hundreds);
                    break;

                case Preset.SGE_ST_Heal_Soteria:
                    DrawSliderInt(0, 100, SGE_ST_Heal_Soteria, Generics.StopFriendlyHpPercent100);
                    DrawPriorityInput(SGE_ST_Heals_Priority, 12, 0, FormatAndCache(Generics.Action_Priority, Soteria.ActionName()));
                    break;

                case Preset.SGE_ST_Heal_Zoe:
                    DrawSliderInt(0, 100, SGE_ST_Heal_Zoe,
                        Generics.StopFriendlyHpPercent100);

                    DrawPriorityInput(SGE_ST_Heals_Priority, 12, 1, FormatAndCache(Generics.Action_Priority, Zoe.ActionName()));
                    break;

                case Preset.SGE_ST_Heal_Pepsis:
                    DrawSliderInt(0, 100, SGE_ST_Heal_Pepsis,
                        Generics.StopFriendlyHpPercent100);

                    DrawPriorityInput(SGE_ST_Heals_Priority, 12, 2, FormatAndCache(Generics.Action_Priority, Pepsis.ActionName()));
                    break;

                case Preset.SGE_ST_Heal_Taurochole:
                    DrawSliderInt(0, 100, SGE_ST_Heal_Taurochole,
                        Generics.StopFriendlyHpPercent100);

                    DrawAdditionalBoolChoice(SGE_ST_Heal_Taurochole_TankOnly,
                        Generics.TanksOnly,
                        Generics.WillOnlyUseOnTanks);

                    DrawPriorityInput(SGE_ST_Heals_Priority, 12, 3, FormatAndCache(Generics.Action_Priority, Taurochole.ActionName()));
                    break;

                case Preset.SGE_ST_Heal_Haima:
                    DrawSliderInt(0, 100, SGE_ST_Heal_Haima,
                        Generics.StopFriendlyHpPercent100);

                    DrawAdditionalBoolChoice(SGE_ST_Heal_HaimaBossOption,
                        Generics.NotInBossEncounters,
                        Generics.WillNotUseInBossEncounters);

                    DrawAdditionalBoolChoice(SGE_ST_Heal_Haima_TankOnly,
                        Generics.TanksOnly,
                        Generics.WillOnlyUseOnTanks);

                    DrawPriorityInput(SGE_ST_Heals_Priority, 12, 4, FormatAndCache(Generics.Action_Priority, Haima.ActionName()));
                    break;

                case Preset.SGE_ST_Heal_Krasis:
                    DrawSliderInt(0, 100, SGE_ST_Heal_Krasis,
                        Generics.StopFriendlyHpPercent100);

                    DrawAdditionalBoolChoice(SGE_ST_Heal_KrasisBossOption,
                        Generics.NotInBossEncounters,
                        Generics.WillNotUseInBossEncounters);

                    DrawAdditionalBoolChoice(SGE_ST_Heal_Krasis_TankOnly,
                        Generics.TanksOnly,
                        Generics.WillOnlyUseOnTanks);

                    DrawPriorityInput(SGE_ST_Heals_Priority, 12, 5, FormatAndCache(Generics.Action_Priority, Krasis.ActionName()));
                    break;

                case Preset.SGE_ST_Heal_Druochole:
                    DrawSliderInt(0, 100, SGE_ST_Heal_Druochole,
                        Generics.StopFriendlyHpPercent100);

                    DrawPriorityInput(SGE_ST_Heals_Priority, 12, 6, FormatAndCache(Generics.Action_Priority, Druochole.ActionName()));
                    break;

                case Preset.SGE_ST_Heal_EDiagnosis:
                    DrawSliderInt(0, 100, SGE_ST_Heal_EDiagnosisHP,
                        Generics.StopFriendlyHpPercent100);

                    DrawHorizontalMultiChoice(SGE_ST_Heal_EDiagnosisOpts,
                        FormatAndCache(Generics.Job0ShieldCheck, Job.SGE.Name()),
                        FormatAndCache(Generics.Job0ShieldCheckDesc, Job.SGE.Name()), 2, 0);

                    DrawHorizontalMultiChoice(SGE_ST_Heal_EDiagnosisOpts,
                        FormatAndCache(Generics.Job0ShieldCheck, Job.SCH.Name()),
                        FormatAndCache(Generics.Job0ShieldCheckDesc, Job.SCH.Name()), 2, 1);

                    DrawPriorityInput(SGE_ST_Heals_Priority, 12, 7, FormatAndCache(Generics.Action_Priority, EukrasianDiagnosis.ActionName()));
                    break;


                case Preset.SGE_ST_Heal_Kerachole:
                    DrawSliderInt(0, 100, SGE_ST_Heal_KeracholeHP,
                        Generics.StopFriendlyHpPercent100);

                    DrawAdditionalBoolChoice(SGE_ST_Heal_KeracholeBossOption,
                        Generics.NotInBossEncounters,
                        Generics.WillNotUseInBossEncounters);

                    DrawPriorityInput(SGE_ST_Heals_Priority, 12, 8, FormatAndCache(Generics.Action_Priority, Kerachole.ActionName()));
                    break;

                case Preset.SGE_ST_Heal_Physis:
                    DrawSliderInt(0, 100, SGE_ST_Heal_PhysisHP,
                        Generics.StopFriendlyHpPercent100);

                    DrawAdditionalBoolChoice(SGE_ST_Heal_PhysisBossOption,
                        Generics.NotInBossEncounters,
                        Generics.WillNotUseInBossEncounters);

                    DrawPriorityInput(SGE_ST_Heals_Priority, 12, 9, FormatAndCache(Generics.Action_Priority, Physis.ActionName()));
                    break;

                case Preset.SGE_ST_Heal_Panhaima:
                    DrawSliderInt(0, 100, SGE_ST_Heal_PanhaimaHP,
                        Generics.StopFriendlyHpPercent100);

                    DrawAdditionalBoolChoice(SGE_ST_Heal_PanhaimaBossOption,
                        Generics.NotInBossEncounters,
                        Generics.WillNotUseInBossEncounters);

                    DrawPriorityInput(SGE_ST_Heals_Priority, 12, 10, FormatAndCache(Generics.Action_Priority, Panhaima.ActionName()));
                    break;

                case Preset.SGE_ST_Heal_Holos:
                    DrawSliderInt(0, 100, SGE_ST_Heal_HolosHP,
                        Generics.StopFriendlyHpPercent100);

                    DrawAdditionalBoolChoice(SGE_ST_Heal_HolosBossOption,
                        Generics.NotInBossEncounters, Generics.WillNotUseInBossEncounters);

                    DrawPriorityInput(SGE_ST_Heals_Priority,
                        12, 11, FormatAndCache(Generics.Action_Priority, Holos.ActionName()));
                    break;
                case Preset.SGE_AoE_Heal:
                    DrawSliderInt(0, 2, SGE_Heal_HoldAddersgall, FormatAndCache(Generics.ChargePool, Traits.Addersgall.TraitName()));
                    break;

                case Preset.SGE_AoE_Heal_Lucid:
                    DrawSliderInt(4000, 9500, SGE_AoE_Heal_LucidOption,
                        Generics.LucidMP, 150, Hundreds);
                    break;

                case Preset.SGE_AoE_Heal_Kerachole:
                    DrawSliderInt(0, 100, SGE_AoE_Heal_KeracholeOption,
                        Generics.StartUsingWhenBelowPartyAverageHPSetTo100ToDisableThisCheck);

                    DrawAdditionalBoolChoice(SGE_AoE_Heal_KeracholeTrait,
                        FormatAndCache(SGE_Config.KerecholeTraitCheck, Traits.EnhancedKerachole.TraitName()),
                        FormatAndCache(SGE_Config.KerecholeTraitCheckDesc, Kerachole.ActionName()));

                    DrawPriorityInput(SGE_AoE_Heals_Priority, 9, 0, FormatAndCache(Generics.Action_Priority, Kerachole.ActionName()));
                    break;

                case Preset.SGE_AoE_Heal_Ixochole:
                    DrawSliderInt(0, 100, SGE_AoE_Heal_IxocholeOption
                        , Generics.StartUsingWhenBelowPartyAverageHPSetTo100ToDisableThisCheck);

                    DrawPriorityInput(SGE_AoE_Heals_Priority, 9, 1, FormatAndCache(Generics.Action_Priority, Ixochole.ActionName()));
                    break;

                case Preset.SGE_AoE_Heal_Physis:
                    DrawSliderInt(0, 100, SGE_AoE_Heal_PhysisOption,
                        Generics.StartUsingWhenBelowPartyAverageHPSetTo100ToDisableThisCheck);

                    DrawPriorityInput(SGE_AoE_Heals_Priority, 9, 2, FormatAndCache(Generics.Action_Priority, Physis.ActionName()));
                    break;

                case Preset.SGE_AoE_Heal_Holos:
                    DrawSliderInt(0, 100, SGE_AoE_Heal_HolosOption,
                        Generics.StartUsingWhenBelowPartyAverageHPSetTo100ToDisableThisCheck);

                    DrawPriorityInput(SGE_AoE_Heals_Priority, 9, 3, FormatAndCache(Generics.Action_Priority, Holos.ActionName()));
                    break;

                case Preset.SGE_AoE_Heal_Panhaima:
                    DrawSliderInt(0, 100, SGE_AoE_Heal_PanhaimaOption,
                        Generics.StartUsingWhenBelowPartyAverageHPSetTo100ToDisableThisCheck);

                    DrawHorizontalMultiChoice(SGE_ST_Heal_PanhaimaOpts,
                        FormatAndCache(SGE_Config.AnyPanhaimaCheck, Panhaima.ActionName()),
                        FormatAndCache(SGE_Config.AnyPanhaimaCheckDesc, Panhaima.ActionName()), 1, 0);

                    DrawPriorityInput(SGE_AoE_Heals_Priority, 9, 4, FormatAndCache(Generics.Action_Priority, Panhaima.ActionName()));
                    break;

                case Preset.SGE_AoE_Heal_Pepsis:
                    DrawSliderInt(0, 100, SGE_AoE_Heal_PepsisOption,
                        Generics.StartUsingWhenBelowPartyAverageHPSetTo100ToDisableThisCheck);

                    DrawPriorityInput(SGE_AoE_Heals_Priority, 9, 5, FormatAndCache(Generics.Action_Priority, Pepsis.ActionName()));
                    break;

                case Preset.SGE_AoE_Heal_Philosophia:
                    DrawSliderInt(0, 100, SGE_AoE_Heal_PhilosophiaOption,
                        Generics.StartUsingWhenBelowPartyAverageHPSetTo100ToDisableThisCheck);

                    DrawPriorityInput(SGE_AoE_Heals_Priority, 9, 6, FormatAndCache(Generics.Action_Priority, Philosophia.ActionName()));
                    break;

                case Preset.SGE_AoE_Heal_Zoe:
                    DrawSliderInt(0, 100, SGE_AoE_Heal_ZoeOption,
                        Generics.StartUsingWhenBelowPartyAverageHPSetTo100ToDisableThisCheck);

                    DrawPriorityInput(SGE_AoE_Heals_Priority, 9, 7, FormatAndCache(Generics.Action_Priority, Zoe.ActionName()));
                    break;

                case Preset.SGE_AoE_Heal_EPrognosis:
                    DrawSliderInt(0, 100, SGE_AoE_Heal_EPrognosisOption,
                        Generics.ShieldCheckPartyMemberNeedPercent);

                    DrawPriorityInput(SGE_AoE_Heals_Priority, 9, 8, FormatAndCache(Generics.Action_Priority, EukrasianPrognosis.ActionName()));
                    break;

                case Preset.SGE_Eukrasia:
                    DrawRadioButton(SGE_Eukrasia_Mode,
                        $"{EukrasianDosis.ActionName()}", "", 0);

                    DrawRadioButton(SGE_Eukrasia_Mode,
                        $"{EukrasianDiagnosis.ActionName()}", "", 1);

                    DrawRadioButton(SGE_Eukrasia_Mode,
                        $"{EukrasianPrognosis.ActionName()}", "", 2);

                    DrawRadioButton(SGE_Eukrasia_Mode,
                        $"{EukrasianDyskrasia.ActionName()}", "", 3);
                    break;

                case Preset.SGE_Mit_ST:
                    DrawHorizontalMultiChoice(SGE_Mit_ST_Options,
                        FormatAndCache(Generics.Include0, Haima.ActionName()),
                        FormatAndCache(SGE_Config.SGE_Mit_ST_Haima_Help, Haima.ActionName()), 2, 0);
                    ImGui.NewLine();
                    DrawHorizontalMultiChoice(SGE_Mit_ST_Options,
                        FormatAndCache(Generics.Include0, Taurochole.ActionName()),
                        FormatAndCache(SGE_Config.SGE_Mit_ST_Taurochole_Help, Taurochole.ActionName()), 2, 1);

                    if (SGE_Mit_ST_Options[1])
                    {
                        ImGui.Indent();
                        DrawSliderInt(1, 100, SGE_Mit_ST_TaurocholeThreshold,
                            FormatAndCache(Generics.TargetHPUse0AtOrBelow, Taurochole.ActionName()));
                        ImGui.Unindent();
                    }
                    break;

                case Preset.SGE_Mit_AoE:
                    DrawSliderInt(0, 100, SGE_Mit_AoE_PrognosisOption,
                        FormatAndCache(SGE_Config.SGE_Mit_AoE_PrognosisOption_Name, Prognosis.ActionName()), sliderIncrement: 25);

                    DrawHorizontalMultiChoice(SGE_Mit_AoE_Options,
                        FormatAndCache(Generics.Include0, Philosophia.ActionName()),
                        FormatAndCache(SGE_Config.SGE_Mit_AoE_Philosophia_Help, Philosophia.ActionName(), EukrasianPrognosis.ActionName()), 3, 0);

                    DrawHorizontalMultiChoice(SGE_Mit_AoE_Options,
                        FormatAndCache(Generics.Include0, Kerachole.ActionName()),
                        FormatAndCache(SGE_Config.SGE_Mit_AoE_Kerachole_Help, Kerachole.ActionName()), 3, 1);

                    DrawHorizontalMultiChoice(SGE_Mit_AoE_Options,
                        FormatAndCache(Generics.Include0, Panhaima.ActionName()),
                        FormatAndCache(SGE_Config.SGE_Mit_AoE_Panhaima_Help, Panhaima.ActionName()), 3, 2);
                    break;

                case Preset.SGE_Raidwide_Holos:
                    DrawSliderInt(0, 100, SGE_Raidwide_HolosOption,
                        Generics.StartUsingWhenBelowPartyAverageHPSetTo100ToDisableThisCheck);
                    break;

                    #endregion
            }
        }

        #region Variables

        #region DPS

        public static UserBool
            SGE_ST_DPS_EDosis_TwoTarget = new("SGE_ST_DPS_EDosis_TwoTarget", true),
            SGE_ST_DPS_Phlegma_Burst = new("SGE_ST_DPS_Phlegma_Burst", true);

        public static UserBoolArray
            SGE_ST_DPS_Movement = new("SGE_ST_DPS_Movement", [true, true, true]);

        public static UserInt
            SGE_ST_DPS_Adv = new("SGE_ST_DPS_Adv"),
            SGE_Eukrasia_Mode = new("SGE_Eukrasia_Mode", 2),
            SGE_SelectedOpener = new("SGE_SelectedOpener"),
            SGE_ST_DPS_Lucid = new("SGE_ST_DPS_Lucid", 6500),
            SGE_ST_DPS_Rhizo = new("SGE_ST_DPS_Rhizo", 1),
            SGE_ST_DPS_Phlegma = new("SGE_ST_DPS_Phlegma"),
            SGE_ST_DPS_EukrasianDosisBossOption = new("SGE_ST_DPS_EukrasianDosisBossOption"),
            SGE_ST_DPS_EukrasianDosisBossAddsOption = new("SGE_ST_DPS_EukrasianDosisBossAddsOption", 100),
            SGE_ST_DPS_EukrasianDosisTrashOption = new("SGE_ST_DPS_EukrasianDosisTrashOption", 50),
            SGE_ST_DPS_AddersgallProtect = new("SGE_ST_DPS_AddersgallProtect", 3),
            SGE_AoE_DPS_Lucid = new("SGE_AoE_Phlegma_Lucid", 6500),
            SGE_AoE_DPS_Rhizo = new("SGE_AoE_DPS_Rhizo", 1),
            SGE_AoE_DPS_AddersgallProtect = new("SGE_AoE_DPS_AddersgallProtect", 3),
            SGE_AoE_DPS_PneumaBossOption = new("SGE_AoE_DPS_Pneuma_SubOption", 1),
            SGE_Balance_Content = new("SGE_Balance_Content", 1);

        public static UserFloat
            SGE_ST_DPS_EukrasianDosisUptime_Threshold = new("SGE_ST_DPS_EukrasianDosisUptime_Threshold", 5.0f);

        public static UserIntArray
            SGE_ST_DPS_Movement_Priority = new("SGE_ST_Movement_Priority");

        #endregion

        #region Healing

        public static UserBool
            SGE_ST_Heal_IncludeShields = new("SGE_ST_Heal_IncludeShields", true),
            SGE_ST_Heal_KeracholeBossOption = new("SGE_ST_Heal_KeracholeBossOption", true),
            SGE_ST_Heal_PanhaimaBossOption = new("SGE_ST_Heal_PanhaimaBossOption", true),
            SGE_ST_Heal_PhysisBossOption = new("SGE_ST_Heal_PhysisBossOption", true),
            SGE_ST_Heal_HolosBossOption = new("SGE_ST_Heal_HolosBossOption", true),
            SGE_ST_Heal_HaimaBossOption = new("SGE_ST_Heal_HaimaBossOption"),
            SGE_ST_Heal_KrasisBossOption = new("SGE_ST_Heal_KrasisBossOption"),
            SGE_ST_Heal_Haima_TankOnly = new("SGE_ST_Heal_Haima_TankOnly", true),
            SGE_ST_Heal_Krasis_TankOnly = new("SGE_ST_Heal_Krasis_TankOnly", true),
            SGE_ST_Heal_Taurochole_TankOnly = new("SGE_ST_Heal_Taurochole_TankOnly", true),
            SGE_AoE_Heal_KeracholeTrait = new("SGE_AoE_Heal_KeracholeTrait", true);

        public static UserInt
            SGE_Heal_HoldAddersgall = new("SGE_Heal_HoldAddersgall", 1),
            SGE_ST_Heal_LucidOption = new("SGE_ST_Heal_LucidOption", 6500),
            SGE_ST_Heal_Zoe = new("SGE_ST_Heal_Zoe", 50),
            SGE_ST_Heal_Haima = new("SGE_ST_Heal_Haima", 50),
            SGE_ST_Heal_Krasis = new("SGE_ST_Heal_Krasis", 40),
            SGE_ST_Heal_Pepsis = new("SGE_ST_Heal_Pepsis", 70),
            SGE_ST_Heal_Soteria = new("SGE_ST_Heal_Soteria", 70),
            SGE_ST_Heal_EDiagnosisHP = new("SGE_ST_Heal_EDiagnosisHP", 70),
            SGE_ST_Heal_Druochole = new("SGE_ST_Heal_Druochole", 70),
            SGE_ST_Heal_Taurochole = new("SGE_ST_Heal_Taurochole", 60),
            SGE_ST_Heal_KeracholeHP = new("SGE_ST_Heal_KeracholeHP", 70),
            SGE_ST_Heal_PhysisHP = new("SGE_ST_Heal_PhysisHP", 70),
            SGE_ST_Heal_PanhaimaHP = new("SGE_ST_Heal_PanhaimaHP", 70),
            SGE_ST_Heal_HolosHP = new("SGE_ST_Heal_HolosHP", 70),
            SGE_ST_Heal_Esuna = new("SGE_ST_Heal_Esuna", 50),
            SGE_AoE_Heal_LucidOption = new("SGE_AoE_Heal_LucidOption", 6500),
            SGE_AoE_Heal_ZoeOption = new("SGE_AoE_Heal_PneumaOption", 50),
            SGE_AoE_Heal_PhysisOption = new("SGE_AoE_Heal_PhysisOption", 60),
            SGE_AoE_Heal_PhilosophiaOption = new("SGE_AoE_Heal_PhilosophiaOption", 40),
            SGE_AoE_Heal_PepsisOption = new("SGE_AoE_Heal_PepsisOption", 70),
            SGE_AoE_Heal_PanhaimaOption = new("SGE_AoE_Heal_PanhaimaOption", 50),
            SGE_AoE_Heal_KeracholeOption = new("SGE_AoE_Heal_KeracholeOption", 70),
            SGE_AoE_Heal_IxocholeOption = new("SGE_AoE_Heal_IxocholeOption", 70),
            SGE_AoE_Heal_HolosOption = new("SGE_AoE_Heal_HolosOption", 60),
            SGE_AoE_Heal_EPrognosisOption = new("SGE_AoE_Heal_EPrognosisOption", 70),
            SGE_Raidwide_HolosOption = new("SGE_Raidwide_HolosOption", 70),
            SGE_Mit_ST_TaurocholeThreshold = new("SGE_Mit_ST_TaurocholeThreshold", 100),
            SGE_Mit_AoE_PrognosisOption = new("SGE_Mit_AoE_PrognosisOption");

        public static UserIntArray
            SGE_ST_Heals_Priority = new("SGE_ST_Heals_Priority", [5, 10, 11, 7, 6, 8, 9, 12, 1, 2, 3, 4]),
            SGE_AoE_Heals_Priority = new("SGE_AoE_Heals_Priority", [1, 3, 2, 7, 8, 4, 5, 6, 9]);

        public static UserBoolArray
            SGE_ST_Heal_EDiagnosisOpts = new("SGE_ST_Heal_EDiagnosisOpts"),
            SGE_ST_Heal_PanhaimaOpts = new("SGE_ST_Heal_PanhaimaOpts"),
            SGE_Mit_ST_Options = new("SGE_Mit_ST_Options"),
            SGE_Mit_AoE_Options = new("SGE_Mit_AoE_Options");

        #endregion

        #endregion
    }
}
