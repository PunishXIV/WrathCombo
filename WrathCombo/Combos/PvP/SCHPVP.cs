using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Window.Functions;

namespace WrathCombo.Combos.PvP
{
    internal static class SCHPvP
    {
        #region IDS

        public const byte ClassID = 26;
        public const byte JobID = 28;       

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

            internal static void Draw(CustomComboPreset preset)
            {
                switch (preset)
                {
                    case CustomComboPreset.SCHPvP_Diabrosis:
                        UserConfig.DrawSliderInt(0, 100, SCHPvP_DiabrosisThreshold,
                            "Target HP% to use Diabrosis");

                        break;

                    case CustomComboPreset.SCHPvP_Selfcare:
                        UserConfig.DrawSliderInt(1, 100, SCHPvP_AdloThreshold,
                            "Player HP% to use Adlo on self");

                        break;
                }
            }
        }

        #endregion

       

        internal class SCHPvP_Burst : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SCHPvP_Burst;

            protected override uint Invoke(uint actionID)
            {
                if (actionID is Broil && InCombat())
                {
                    // Uses Chain Stratagem when available
                    if (IsEnabled(CustomComboPreset.SCHPvP_ChainStratagem) && IsOffCooldown(ChainStratagem))
                        return ChainStratagem;                    

                    // Uses Expedient when available and target isn't affected with Biolysis
                    if (IsEnabled(CustomComboPreset.SCHPvP_Expedient) && IsOffCooldown(Expedient) && GetCooldownRemainingTime(Biolysis) < 3)
                        return Expedient;

                    // Uses Biolysis on cooldown or with Recitation when Expedient is enabled with safetey for too long of an expedient cooldown. 
                    if (IsEnabled(CustomComboPreset.SCHPvP_Biolysis) && IsOffCooldown(Biolysis))
                    {
                        if (IsEnabled(CustomComboPreset.SCHPvP_Expedient))
                        {
                            if (HasEffect(Buffs.Recitation) || GetCooldownRemainingTime(Expedient) > 5)
                                return Biolysis;
                        } 
                        return Biolysis;
                    }

                    //Uses Diabrosis when below set health
                    if (IsEnabled(CustomComboPreset.SCHPvP_Diabrosis) && PvPHealer.CanDiabrosis() && HasTarget() && 
                            GetTargetHPPercent() <= Config.SCHPvP_DiabrosisThreshold)
                        return PvPHealer.Diabrosis;

                    // Uses Deployment Tactics when available
                    if (IsEnabled(CustomComboPreset.SCHPvP_DeploymentTactics) && GetRemainingCharges(DeploymentTactics) > 1 && TargetHasEffect(Debuffs.Biolysis))
                        return DeploymentTactics;

                    // Adds Adloquium when at or below threshold, will not Overwrite the 10% damage reduction buff to prevent waste
                    if (IsEnabled(CustomComboPreset.SCHPvP_Selfcare) && !HasEffect(Buffs.Catalyze) && PlayerHealthPercentageHp() <= Config.SCHPvP_AdloThreshold)
                        return Adloquilum;
                }

                return actionID;
            }
        }
    }
}
