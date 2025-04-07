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
            Aqloquilum = 29232,
            Biolysis = 29233,
            DeploymentTactics = 29234,
            Expedient = 29236,
            ChainStratagem = 29716;


        internal class Buffs
        {
            internal const ushort
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
               SCHPvP_DiabrosisThreshold = new("SCHPvP_DiabrosisThreshold");

            internal static void Draw(CustomComboPreset preset)
            {
                switch (preset)
                {
                    case CustomComboPreset.SCHPvP_Diabrosis:
                        UserConfig.DrawSliderInt(0, 100, SCHPvP_DiabrosisThreshold,
                            "Target HP% to use Diabrosis");

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

                    if (IsEnabled(CustomComboPreset.SCHPvP_Diabrosis) && PvPHealer.CanDiabrosis() && HasTarget() &&
                            GetTargetHPPercent() <= Config.SCHPvP_DiabrosisThreshold)
                        return PvPHealer.Diabrosis;

                    // Uses Expedient when available and target isn't affected with Biolysis
                    if (IsEnabled(CustomComboPreset.SCHPvP_Expedient) && IsOffCooldown(Expedient) && !HasStatusEffect(Debuffs.Biolysis, CurrentTarget))
                        return Expedient;

                    // Uses Biolysis under Recitation, or on cooldown when option active
                    if (IsEnabled(CustomComboPreset.SCHPvP_Biolysis))
                    {
                        if (IsOffCooldown(Biolysis) || (HasStatusEffect(Buffs.Recitation) && IsOffCooldown(Biolysis)))
                            return Biolysis;
                    }

                    // Uses Deployment Tactics when available
                    if (IsEnabled(CustomComboPreset.SCHPvP_DeploymentTactics) && GetRemainingCharges(DeploymentTactics) > 1)
                        return DeploymentTactics;
                }

                return actionID;
            }
        }
    }
}
