using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Window.Functions;
using static WrathCombo.Combos.PvP.SCHPvP.Config;

namespace WrathCombo.Combos.PvP;

internal static class SCHPvP
{
        #region IDS

    internal class Role : PvPHealer;

    public const uint
        Broil = 29231,
        Adloquilum = 29232,
        Biolysis = 29233,
        DeploymentTactics = 29234,
        Expedient = 29236,
        ChainStratagem = 29716;


    internal class Buffs
    {
        internal const ushort
            Catalyze = 3088,
            Recitation = 3094;
    }
    internal class Debuffs
    {
        internal const ushort
            Biolysis = 3089,
            Biolytic = 3090;
    }
        #endregion

        #region Config
    public static class Config
    {
        public static UserInt
            SCHPvP_DiabrosisThreshold = new("SCHPvP_DiabrosisThreshold"),
            SCHPvP_AdloThreshold = new("SCHPvP_AdloThreshold");

        internal static void Draw(Preset preset)
        {
            switch (preset)
            {
                case Preset.SCHPvP_Diabrosis:
                    UserConfig.DrawSliderInt(0, 100, SCHPvP_DiabrosisThreshold,
                        "Target HP% to use Diabrosis");

                    break;

                case Preset.SCHPvP_Selfcare:
                    UserConfig.DrawSliderInt(1, 100, SCHPvP_AdloThreshold,
                        "Player HP% to use Adlo on self");

                    break;
            }
        }
    }

        #endregion
          
    internal class SCHPvP_Burst : CustomCombo
    {
        protected internal override Preset Preset => Preset.SCHPvP_Burst;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is Broil && InCombat())
            {
                // Uses Chain Stratagem when available
                if (IsEnabled(Preset.SCHPvP_ChainStratagem) && IsOffCooldown(ChainStratagem))
                    return ChainStratagem;                    

                // Uses Expedient when available and target isn't affected with Biolysis
                if (IsEnabled(Preset.SCHPvP_Expedient) && IsOffCooldown(Expedient) && GetCooldownRemainingTime(Biolysis) < 3)
                    return Expedient;

                // Uses Biolysis on cooldown or with Recitation when Expedient is enabled with safety for too long of an expedient cooldown. 
                if (IsEnabled(Preset.SCHPvP_Biolysis) && IsOffCooldown(Biolysis))
                {
                    if (IsEnabled(Preset.SCHPvP_Expedient))
                    {
                        if (HasStatusEffect(Buffs.Recitation) || GetCooldownRemainingTime(Expedient) > 5)
                            return Biolysis;
                    } 
                    return Biolysis;
                }

                //Uses Diabrosis when below set health
                if (IsEnabled(Preset.SCHPvP_Diabrosis) && PvPHealer.CanDiabrosis() && HasTarget() && 
                    GetTargetHPPercent() <= SCHPvP_DiabrosisThreshold)
                    return PvPHealer.Diabrosis;

                // Uses Deployment Tactics when available
                if (IsEnabled(Preset.SCHPvP_DeploymentTactics) && GetRemainingCharges(DeploymentTactics) > 1 && HasStatusEffect(Debuffs.Biolysis, CurrentTarget))
                    return DeploymentTactics;

                // Adds Adloquium when at or below threshold, will not Overwrite the 10% damage reduction buff to prevent waste
                if (IsEnabled(Preset.SCHPvP_Selfcare) && !HasStatusEffect(Buffs.Catalyze) && PlayerHealthPercentageHp() <= SCHPvP_AdloThreshold)
                    return Adloquilum;
            }

            return actionID;
        }
    }
}