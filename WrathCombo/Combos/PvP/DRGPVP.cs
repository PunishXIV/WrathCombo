using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.Window.Functions.UserConfig;
using static WrathCombo.Combos.PvP.DRGPvP.Config;

namespace WrathCombo.Combos.PvP;

internal static class DRGPvP
{
    #region IDS
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
            SkyHigh = 3180,
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

        internal static void Draw(Preset preset)
        {            
            switch (preset)
            {
                case Preset.DRGPvP_Nastrond:
                    DrawSliderInt(0, 100, DRGPvP_LOTD_HPValue, "Ends Life of the Dragon if HP falls below the set percentage");
                    DrawSliderInt(2, 8, DRGPvP_LOTD_Duration, "Seconds remaining of Life of the Dragon buff before using Nastrond if you are still above the set HP percentage.");
                    break;

                case Preset.DRGPvP_ChaoticSpringSustain:
                    DrawSliderInt(0, 101, DRGPvP_CS_HP_Threshold, "Chaotic Spring HP percentage threshold. Set to 100 to use on cd");
                    break;
                        

                case Preset.DRGPvP_WyrmwindThrust:
                    DrawSliderInt(0, 20, DRGPvP_Distance_Threshold, "Minimum Distance to use Wyrmwind Thrust. Maximum damage at 15 or more");                        
                    break;

                case Preset.DRGPvP_Smite:
                    DrawSliderInt(0, 100, DRGPvP_SmiteThreshold,
                        "Target HP% to smite, Max damage below 25%");                       
                    break;
            }
        }
    }
    #endregion      

    internal class DRGPvP_Burst : CustomCombo
    {
        protected internal override Preset Preset => Preset.DRGPvP_Burst;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (RaidenThrust or FangAndClaw or WheelingThrust or Drakesbane)) 
                return actionID;
            
            if (!HasStatusEffect(PvPCommon.Buffs.Guard, CurrentTarget, true))
            {
                if (IsEnabled(Preset.DRGPvP_Smite) && PvPMelee.CanSmite() && GetTargetDistance() <= 10 && HasTarget() &&
                    GetTargetHPPercent() <= DRGPvP_SmiteThreshold)
                    return PvPMelee.Smite;

                if (CanWeave())
                {
                    if (IsEnabled(Preset.DRGPvP_HighJump) && IsOffCooldown(HighJump) && !HasStatusEffect(Buffs.StarCrossReady) && (HasStatusEffect(Buffs.LifeOfTheDragon) || GetCooldownRemainingTime(Geirskogul) > 5)) // Will high jump after Gierskogul OR if Geir will be on cd for 2 more gcds.
                        return HighJump;

                    if (IsEnabled(Preset.DRGPvP_Nastrond)) // Nastrond Finisher logic
                    {
                        if (HasStatusEffect(Buffs.LifeOfTheDragon) && PlayerHealthPercentageHp() < DRGPvP_LOTD_HPValue
                            || HasStatusEffect(Buffs.LifeOfTheDragon) && GetStatusEffectRemainingTime(Buffs.LifeOfTheDragon) < DRGPvP_LOTD_Duration)
                            return Nastrond;
                    }
                    if (IsEnabled(Preset.DRGPvP_HorridRoar) && IsOffCooldown(HorridRoar) && GetTargetDistance() <= 10) // HorridRoar Roar on cd
                        return HorridRoar;
                }
                       
                if (IsEnabled(Preset.DRGPvP_Geirskogul) && IsOffCooldown(Geirskogul)) 
                {
                    if (IsEnabled(Preset.DRGPvP_BurstProtection) && WasLastAbility(ElusiveJump) && HasStatusEffect(Buffs.FirstmindsFocus))// With evasive burst mode
                        return Geirskogul;
                    if (!IsEnabled(Preset.DRGPvP_BurstProtection))// Without evasive burst mode so you can still use Gier, which will let you still use high jump
                        return Geirskogul;
                }                       
                                                   
                if (IsEnabled(Preset.DRGPvP_WyrmwindThrust) && HasStatusEffect(Buffs.FirstmindsFocus) && GetTargetDistance() >= DRGPvP_Distance_Threshold)
                    return WyrmwindThrust;

                if (IsEnabled(Preset.DRGPvP_Geirskogul) && HasStatusEffect(Buffs.StarCrossReady))
                    return Starcross;
                       
            }
            if (IsOffCooldown(ChaoticSpring) && InMeleeRange())
            {
                if (IsEnabled(Preset.DRGPvP_ChaoticSpringSustain) && PlayerHealthPercentageHp() < DRGPvP_CS_HP_Threshold) // Chaotic Spring as a self heal option, it does not break combos of other skills
                    return ChaoticSpring;
                if (IsEnabled(Preset.DRGPvP_ChaoticSpringExecute) && GetTargetCurrentHP() <= 8000) // Chaotic Spring Execute
                    return ChaoticSpring;
            }
            return actionID;
        }
    }
    internal class DRGPvP_BurstProtection : CustomCombo
    {
        protected internal override Preset Preset => Preset.DRGPvP_BurstProtection;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not ElusiveJump) 
                return actionID;
            
            if (HasStatusEffect(Buffs.FirstmindsFocus) || IsOnCooldown(Geirskogul))
            {
                return 26;
            }
            return actionID;
        }
    }
}