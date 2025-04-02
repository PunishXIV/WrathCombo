using ECommons.GameHelpers;
using WrathCombo.Core;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.Window.Functions.SliderIncrements;
using static WrathCombo.Window.Functions.UserConfig;

namespace WrathCombo.Combos.PvP
{
    internal static class ASTPvP
    {
        internal const byte JobID = 33;
        
        internal class Role : PvPHealer;

        internal const uint
            Malefic = 29242,
            AspectedBenefic = 29243,
            Gravity = 29244,
            DoubleCast = 29245,
            DoubleMalefic = 29246,
            NocturnalBenefic = 29247,
            DoubleGravity = 29248,
            Draw = 29249,
            Macrocosmos = 29253,
            Microcosmos = 29254,
            MinorArcana = 41503,
            Epicycle = 41506;

        internal class Buffs
        {
            internal const ushort
                    LadyOfCrowns = 4328,
                    LordOfCrowns = 4329,
                    RetrogradeReady = 4331;

        }

        public class Config
        {
            public static UserInt
                ASTPvP_Burst_PlayCardOption = new("ASTPvP_Burst_PlayCardOption");

            internal static void Draw(CustomComboPreset preset)
            {
                switch (preset)
                {
                    case CustomComboPreset.ASTPvP_Burst_PlayCard:
                        DrawHorizontalRadioButton(ASTPvP_Burst_PlayCardOption, "Lord and Lady card play",
                             "Uses Lord and Lady of Crowns when available.", 1);

                        DrawHorizontalRadioButton(ASTPvP_Burst_PlayCardOption, "Lord of Crowns card play",
                            "Only uses Lord of Crowns when available.", 2);

                        DrawHorizontalRadioButton(ASTPvP_Burst_PlayCardOption, "Lady of Crowns card play",
                            "Only uses Lady of Crowns when available.", 3);

                        break;
                }
            }
        }

        internal class ASTPvP_Burst : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ASTPvP_Burst;

            protected override uint Invoke(uint actionID)
            {
                if (actionID is Malefic)
                {
                    // Card Draw
                    if (IsEnabled(CustomComboPreset.ASTPvP_Burst_DrawCard) && IsOffCooldown(MinorArcana) && (!HasEffect(Buffs.LadyOfCrowns) && !HasEffect(Buffs.LordOfCrowns)))
                        return MinorArcana;                                      
                   
                    var cardPlayOption = PluginConfiguration.GetCustomIntValue(Config.ASTPvP_Burst_PlayCardOption);

                    if (IsEnabled(CustomComboPreset.ASTPvP_Burst_PlayCard))
                    {
                        bool hasLadyOfCrowns = HasEffect(Buffs.LadyOfCrowns);
                        bool hasLordOfCrowns = HasEffect(Buffs.LordOfCrowns);

                        // Card Playing Split so Lady can still be used if target is immune
                        if ((cardPlayOption == 1 && hasLordOfCrowns && !PvPCommon.TargetImmuneToDamage()) ||
                            (cardPlayOption == 1 && hasLadyOfCrowns) ||
                            (cardPlayOption == 2 && hasLordOfCrowns && !PvPCommon.TargetImmuneToDamage()) ||
                            (cardPlayOption == 3 && hasLadyOfCrowns))

                            return OriginalHook(MinorArcana);
                    }    
                        
                    if (!PvPCommon.TargetImmuneToDamage())
                    { 
                        // Macrocosmos only with double gravity or on coodlown when double gravity is disabled
                        if (IsEnabled(CustomComboPreset.ASTPvP_Burst_Macrocosmos) && IsOffCooldown(Macrocosmos) &&
                           (ComboAction == DoubleGravity || !IsEnabled(CustomComboPreset.ASTPvP_Burst_DoubleGravity)))
                            return Macrocosmos;

                        // Double Gravity
                        if (IsEnabled(CustomComboPreset.ASTPvP_Burst_DoubleGravity) && ComboAction == Gravity && HasCharges(DoubleCast))
                            return DoubleGravity;

                        // Gravity on cd
                        if (IsEnabled(CustomComboPreset.ASTPvP_Burst_Gravity) && IsOffCooldown(Gravity))
                            return Gravity;

                        // Double Malefic logic to not leave gravity without a charge
                        if (IsEnabled(CustomComboPreset.ASTPvP_Burst_DoubleMalefic))
                        {
                            if (ComboAction == Malefic && (GetRemainingCharges(DoubleCast) > 1 ||
                                GetCooldownRemainingTime(Gravity) > 7.5f) && CanWeave())
                                return DoubleMalefic;
                        }

                    }

                }

                return actionID;
            }

            internal class ASTPvP_Epicycle : CustomCombo
            {
                protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ASTPvP_Epicycle;

                protected override uint Invoke(uint actionID)
                {
                    if(actionID is Epicycle)
                    {
                        if (IsOffCooldown(MinorArcana))
                            return MinorArcana;

                        if (HasEffect(Buffs.RetrogradeReady))
                        {
                            if (HasEffect(Buffs.LordOfCrowns))
                                return OriginalHook(MinorArcana);
                            if (IsOffCooldown(Macrocosmos))
                                return Macrocosmos;
                        }
                    }

                    return actionID;
                }
            }

            internal class ASTPvP_Heal : CustomCombo
            {
                protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ASTPvP_Heal;

                protected override uint Invoke(uint actionID)
                {
                    if (actionID is AspectedBenefic && CanWeave() &&
                        ComboAction == AspectedBenefic &&
                        HasCharges(DoubleCast))
                        return OriginalHook(DoubleCast);

                    return actionID;
                }
            }
        }
    }
}
