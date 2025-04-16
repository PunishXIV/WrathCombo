using ECommons.DalamudServices;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Window.Functions;

namespace WrathCombo.Combos.PvP
{
    internal static class DRGPvP
    {
        #region IDS

        public const byte ClassID = 4;
        public const byte JobID = 22;

        internal class Role : PvPMelee;

        public const uint
            WheelingThrustCombo = 56,
            RaidenThrust = 29486,
            FangAndClaw = 29487,
            WheelingThrust = 29488,
            ChaoticSpring = 29490,
            Geirskogul = 29491,
            HighJump = 29493,
            ElusiveJump = 29494,
            WyrmwindThrust = 29495,
            HorridRoar = 29496,
            HeavensThrust = 29489,
            Nastrond = 29492,
            Purify = 29056,
            Guard = 29054,
            Drakesbane = 41449,
            Starcross = 41450;


        public static class Buffs
        {
            public const ushort
            FirstmindsFocus = 3178,
            LifeOfTheDragon = 3177,
            Heavensent = 3176,
            StarCrossReady = 4302;


        }
        #endregion

        #region Config
        public static class Config
        {
            public static UserInt
                DRGPvP_LOTD_Duration = new("DRGPvP_LOTD_Duration"),
                DRGPvP_LOTD_HPValue = new("DRGPvP_LOTD_HPValue"),
                DRGPvP_CS_HP_Threshold = new("DRGPvP_CS_HP_Threshold"),
                DRGPvP_Distance_Threshold = new("DRGPvP_Distance_Threshold"),
                DRGPvP_SmiteThreshold = new("DRGPvP_SmiteThreshold");

            internal static void Draw(CustomComboPreset preset)
            {            
                switch (preset)
                {
                    case CustomComboPreset.DRGPvP_Nastrond:
                        UserConfig.DrawSliderInt(0, 100, DRGPvP_LOTD_HPValue, "Ends Life of the Dragon if HP falls below the set percentage");
                        UserConfig.DrawSliderInt(2, 8, DRGPvP_LOTD_Duration, "Seconds remaining of Life of the Dragon buff before using Nastrond if you are still above the set HP percentage.");
                        break;

                    case CustomComboPreset.DRGPvP_ChaoticSpringSustain:
                        UserConfig.DrawSliderInt(0, 101, DRGPvP_CS_HP_Threshold, "Chaotic Spring HP percentage threshold. Set to 100 to use on cd");
                        break;
                        

                    case CustomComboPreset.DRGPvP_WyrmwindThrust:
                        UserConfig.DrawSliderInt(0, 20, DRGPvP_Distance_Threshold, "Minimum Distance to use Wyrmwind Thrust. Maximum damage at 15 or more");                        
                        break;

                    case CustomComboPreset.DRGPvP_Smite:
                        UserConfig.DrawSliderInt(0, 100, DRGPvP_SmiteThreshold,
                            "Target HP% to smite, Max damage below 25%");                       
                        break;
                }

            }
            
        }
        #endregion      

        internal class DRGPvP_Burst : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DRGPvP_Burst;

            protected override uint Invoke(uint actionID)
            {
                if (actionID is RaidenThrust or FangAndClaw or WheelingThrust or Drakesbane)
                {
                    if (!HasStatusEffect(PvPCommon.Buffs.Guard, CurrentTarget, true))
                    {
                        if (IsEnabled(CustomComboPreset.DRGPvP_Smite) && PvPMelee.CanSmite() && GetTargetDistance() <= 10 && HasTarget() &&
                            GetTargetHPPercent() <= Config.DRGPvP_SmiteThreshold)
                            return PvPMelee.Smite;

                        if (CanWeave())
                        {
                            if (IsEnabled(CustomComboPreset.DRGPvP_HighJump) && IsOffCooldown(HighJump) && !HasStatusEffect(Buffs.StarCrossReady) && (HasStatusEffect(Buffs.LifeOfTheDragon) || GetCooldownRemainingTime(Geirskogul) > 5)) // Will high jump after Gierskogul OR if Geir will be on cd for 2 more gcds.
                                return HighJump;

                            if (IsEnabled(CustomComboPreset.DRGPvP_Nastrond)) // Nastrond Finisher logic
                            {
                                if (HasStatusEffect(Buffs.LifeOfTheDragon) && PlayerHealthPercentageHp() < Config.DRGPvP_LOTD_HPValue
                                 || HasStatusEffect(Buffs.LifeOfTheDragon) && GetStatusEffectRemainingTime(Buffs.LifeOfTheDragon) < Config.DRGPvP_LOTD_Duration)
                                    return Nastrond;
                            }

                            if (IsEnabled(CustomComboPreset.DRGPvP_HorridRoar) && IsOffCooldown(HorridRoar) && GetTargetDistance() <= 10) // HorridRoar Roar on cd
                                return HorridRoar;
                        }
                       
                        if (IsEnabled(CustomComboPreset.DRGPvP_Geirskogul) && IsOffCooldown(Geirskogul)) 
                        {
                            if (IsEnabled(CustomComboPreset.DRGPvP_BurstProtection) && WasLastAbility(ElusiveJump) && HasStatusEffect(Buffs.FirstmindsFocus))  // With evasive burst mode
                                return Geirskogul;
                            if (!IsEnabled(CustomComboPreset.DRGPvP_BurstProtection))                                                                    // Without evasive burst mode so you can still use Gier, which will let you still use high jump
                                return Geirskogul;
                        }                       
                                                   
                        if (IsEnabled(CustomComboPreset.DRGPvP_WyrmwindThrust) && HasStatusEffect(Buffs.FirstmindsFocus) && GetTargetDistance() >= Config.DRGPvP_Distance_Threshold)
                            return WyrmwindThrust;

                        if (IsEnabled(CustomComboPreset.DRGPvP_Geirskogul) && HasStatusEffect(Buffs.StarCrossReady))
                            return Starcross;
                       
                    }
                    if (IsOffCooldown(ChaoticSpring) && InMeleeRange())
                    {
                        if (IsEnabled(CustomComboPreset.DRGPvP_ChaoticSpringSustain) && PlayerHealthPercentageHp() < Config.DRGPvP_CS_HP_Threshold) // Chaotic Spring as a self heal option, it does not break combos of other skills
                            return ChaoticSpring;
                        if (IsEnabled(CustomComboPreset.DRGPvP_ChaoticSpringExecute) && EnemyHealthCurrentHp() <= 8000) // Chaotic Spring Execute
                            return ChaoticSpring;
                    }
                  
                }
                return actionID;
            }
        }
        internal class DRGPvP_BurstProtection : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DRGPvP_BurstProtection;

            protected override uint Invoke(uint actionID)
            {
                if (actionID is ElusiveJump)
                {
                    if (HasStatusEffect(Buffs.FirstmindsFocus) || IsOnCooldown(Geirskogul))
                    {
                        return 26;
                    }
                }
                return actionID;
            }
        }
    }
}
